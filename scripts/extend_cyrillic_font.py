#!/usr/bin/env python3
"""Append Russian glyphs to the original EZ GUI atlas, preserving Latin pixels/metrics.

Requires Pillow. Source font and its Apache 2.0 license live in third_party/roboto-slab.
Original assets are read from the untouched recovered export, so regeneration is idempotent.
"""
from pathlib import Path
import hashlib
import json
import re

from PIL import Image, ImageDraw, ImageFont

ROOT = Path(__file__).resolve().parents[1]
SOURCE = ROOT / "recovered/ExportedProject/Assets"
OUTPUT = ROOT / "unity/Assets"
FONT_FILE = ROOT / "third_party/roboto-slab/RobotoSlab-wght.ttf"
font = ImageFont.truetype(str(FONT_FILE), 34)
font.set_variation_by_axes([600])
original = Image.open(SOURCE / "Texture2D/PipeGUI_Font.png").convert("RGBA")
atlas = Image.new("RGBA", (512, 512))
atlas.paste(original, (0, 0))
base_definition = (SOURCE / "TextAsset/PipeGUI_Font.txt").read_text()
existing = set(map(int, re.findall(r"char id=(\d+)", base_definition)))
characters = sorted(set(map(chr, range(0x410, 0x450))) | set("Ёё«»№—–…"))
characters = [c for c in characters if ord(c) not in existing]
glyphs = []
x, y, row_height = 7, 263, 0
for character in characters:
    left, top, right, bottom = font.getbbox(character, anchor="ls")
    mask = Image.new("L", (right - left, bottom - top))
    ImageDraw.Draw(mask).text((-left, -top), character, font=font, fill=255, anchor="ls")
    # Match the original 24-pixel cap height and compact semi-bold letter widths.
    width = max(1, round(mask.width * 0.85))
    mask = mask.resize((width, mask.height), Image.Resampling.LANCZOS)
    if x + width + 7 > atlas.width:
        x, y, row_height = 7, y + row_height + 7, 0
    if y + mask.height + 7 > atlas.height:
        raise ValueError("Cyrillic glyphs exceed the atlas")
    rgba = Image.new("RGBA", mask.size)
    rgba.putalpha(mask)
    atlas.paste(rgba, (x, y))
    glyphs.append(dict(id=ord(character), x=x, y=y, width=width, height=mask.height,
                       xoffset=round(left * 0.85), yoffset=29 + top,
                       xadvance=round(font.getlength(character) * 0.85)))
    x += width + 7
    row_height = max(row_height, mask.height)

atlas.save(OUTPUT / "Texture2D/PipeGUI_Font.png")
for suffix, extra in [("", 0), ("_Drop", 2), ("_Emboss", 4)]:
    filename = f"PipeGUI_Font{suffix}.txt"
    definition = (SOURCE / "TextAsset" / filename).read_text()
    definition = definition.replace("unicode=0", "unicode=1").replace("scaleH=256", "scaleH=512")
    definition = definition.replace("chars count=133", f"chars count={133 + len(glyphs)}")
    records = []
    for glyph in glyphs:
        fields = dict(glyph, width=glyph["width"] + extra, height=glyph["height"] + extra)
        records.append("char " + " ".join(f"{key}={value}" for key, value in fields.items()) + " page=0 chnl=0")
    if "kernings count=" not in definition:
        definition = definition.rstrip() + "\nkernings count=0\n"
    insertion = definition.index("kernings count=")
    definition = definition[:insertion] + "\n".join(records) + "\n" + definition[insertion:]
    (OUTPUT / "TextAsset" / filename).write_text(definition)

# Atlas height doubled. Keep the existing shadow/emboss displacement in pixels.
for filename in ["PipeGUI_Font_DropColour.mat", "PipeGUI_FontPlateEmbossBronze.mat", "PipeGUI_FontPlateEmbossSilver.mat"]:
    material = (SOURCE / "Material" / filename).read_text()
    material = re.sub(r"(name: _DropOffset\s+second: \{r: [^,]+, g: )([^,]+)",
                      lambda m: m[1] + str(float(m[2]) * 0.5), material)
    (OUTPUT / "Material" / filename).write_text(material)

assert atlas.crop((0, 0, 512, 256)).tobytes() == original.tobytes()
report = {"source_font": str(FONT_FILE.relative_to(ROOT)),
          "source_font_sha256": hashlib.sha256(FONT_FILE.read_bytes()).hexdigest(),
          "size": list(atlas.size), "original_glyphs": 133, "added_glyphs": len(glyphs),
          "original_pixels_unchanged": True, "glyphs": glyphs}
(ROOT / "work/qa/cyrillic-font.json").write_text(json.dumps(report, indent=2) + "\n")
print(f"Appended {len(glyphs)} glyphs; original Latin atlas and metrics preserved")
