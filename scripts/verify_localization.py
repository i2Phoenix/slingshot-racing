#!/usr/bin/env python3
"""Check Russian strings, substitutions, font coverage and preserved original glyphs."""
import hashlib
import json
from pathlib import Path
import re
import xml.etree.ElementTree as ET

from PIL import Image

ROOT = Path(__file__).resolve().parents[1]
ASSETS = ROOT / "unity/Assets"
ORIGINAL = ROOT / "recovered/ExportedProject/Assets"


def entries(path):
    nodes = ET.parse(path).getroot().findall("entry")
    result = {node.attrib["name"]: node.text or "" for node in nodes}
    assert len(result) == len(nodes), f"Duplicate keys: {path}"
    return result


english = entries(ASSETS / "Resources/languages/EN_Sheet1.txt")
russian = entries(ASSETS / "Resources/languages/RU_Sheet1.txt")
assert russian.keys() == english.keys(), "Russian/English keys differ"
for key, value in russian.items():
    assert value.strip(), f"Empty translation: {key}"
    assert sorted(re.findall(r"\{[^{}]+\}", value)) == sorted(re.findall(r"\{[^{}]+\}", english[key])), key


def glyphs(path):
    text = path.read_text()
    records = {}
    for line in text.splitlines():
        if line.startswith("char "):
            record = dict((k, int(v)) for k, v in re.findall(r"(\w+)=(-?\d+)", line))
            records[record["id"]] = record
    assert len(records) == int(re.search(r"chars count=(\d+)", text)[1])
    return records


atlas = Image.open(ASSETS / "Texture2D/PipeGUI_Font.png").convert("RGBA")
original = Image.open(ORIGINAL / "Texture2D/PipeGUI_Font.png").convert("RGBA")
assert atlas.crop((0, 0, *original.size)).tobytes() == original.tobytes()
for suffix in ("", "_Drop", "_Emboss"):
    name = f"TextAsset/PipeGUI_Font{suffix}.txt"
    current = glyphs(ASSETS / name)
    old = glyphs(ORIGINAL / name)
    assert all(current[key] == value for key, value in old.items()), f"Original metrics changed: {name}"
    missing = sorted({c for value in russian.values() for c in value if not c.isspace() and ord(c) not in current})
    assert not missing, (name, missing)
    for glyph in current.values():
        assert glyph["x"] + glyph["width"] <= atlas.width
        assert glyph["y"] + glyph["height"] <= atlas.height

selector = (ASSETS / "Scenes/GUI/OptionsScreen.unity").read_text()
assert "m_languages: 19000000620000002200000035000000160000001b000000" in selector
report = {"entries": len(russian), "placeholders_match": True, "missing_glyphs": [],
          "font_variants_checked": 3, "original_pixels_and_metrics_unchanged": True,
          "language_cycle": ["EN", "RU", "FR", "IT", "DE", "ES"],
          "russian_sha256": hashlib.sha256((ASSETS / "Resources/languages/RU_Sheet1.txt").read_bytes()).hexdigest()}
output = ROOT / "work/qa/russian-localization.json"
output.parent.mkdir(parents=True, exist_ok=True)
output.write_text(json.dumps(report, indent=2) + "\n")
print(json.dumps(report, indent=2))
