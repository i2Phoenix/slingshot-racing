#!/usr/bin/env python3
"""Verify a local release APK, including its ELF and Android 16 KB alignment."""
import argparse
import hashlib
import io
import json
import os
from pathlib import Path
import re
import subprocess
import zipfile

from elftools.elf.elffile import ELFFile

ROOT = Path(__file__).resolve().parents[1]
parser = argparse.ArgumentParser(description=__doc__)
parser.add_argument("apk", nargs="?", type=Path,
                    default=ROOT / "builds/SlingshotRacing-modern-arm64.apk")
parser.add_argument("--build-tools", type=Path,
                    default=Path.home() / "Library/Android/sdk/build-tools/36.0.0")
parser.add_argument("--java-home", type=Path,
                    default=Path("/Applications/Unity/Hub/Editor/6000.3.25f1/PlaybackEngines/AndroidPlayer/OpenJDK"))
parser.add_argument("--output", type=Path, default=ROOT / "work/qa/release-apk.json")
args = parser.parse_args()
env = os.environ.copy()
env["JAVA_HOME"] = str(args.java_home)


def run(tool, *arguments):
    return subprocess.check_output([str(args.build_tools / tool), *map(str, arguments)],
                                   text=True, stderr=subprocess.STDOUT, env=env)


badging = run("aapt", "dump", "badging", args.apk)
manifest = run("aapt", "dump", "xmltree", args.apk, "AndroidManifest.xml")
assert "native-code: 'arm64-v8a'" in badging, badging
assert "sdkVersion:'26'" in badging and "targetSdkVersion:'36'" in badging
assert "application-debuggable" not in badging, "Development APK is not a release"
assert not re.search(r"android:debuggable[^\n]*=\(type 0x12\)0xffffffff", manifest)
signature = run("apksigner", "verify", "--verbose", "--print-certs", args.apk)
alignment = run("zipalign", "-c", "-P", "16", "-v", "4", args.apk)

native = []
with zipfile.ZipFile(args.apk) as apk:
    for name in apk.namelist():
        if not (name.startswith("lib/") and name.endswith(".so")):
            continue
        elf = ELFFile(io.BytesIO(apk.read(name)))
        segments = [s for s in elf.iter_segments() if s["p_type"] == "PT_LOAD"]
        aligned = all(s["p_align"] >= 16384 and
                      (s["p_vaddr"] - s["p_offset"]) % 16384 == 0 for s in segments)
        assert elf["e_machine"] == "EM_AARCH64" and aligned, name
        native.append({"path": name, "machine": elf["e_machine"],
                       "alignments": [s["p_align"] for s in segments], "passes_16kb": aligned})
    metadata = apk.read("assets/bin/Data/Managed/Metadata/global-metadata.dat")
    diagnostics = [name for name in (b"ModernDiagnostics", b"ModernCatalogAudit", b"QA_INPUT_HINT", b"QA_GRADE", b"QA_FINAL_STATS")
                   if name in metadata]
    assert not diagnostics, diagnostics
    boot = apk.read("assets/bin/Data/boot.config").decode()

report = {"apk": str(args.apk.resolve()), "sha256": hashlib.sha256(args.apk.read_bytes()).hexdigest(),
          "size": args.apk.stat().st_size, "native": native, "debuggable": False,
          "diagnostic_types_present": [], "boot_config": boot,
          "badging": badging, "signature": signature, "zipalign_16kb": alignment.strip().splitlines()[-1]}
args.output.parent.mkdir(parents=True, exist_ok=True)
args.output.write_text(json.dumps(report, indent=2) + "\n")
args.output.with_suffix(".manifest.txt").write_text(manifest)
print(json.dumps({k: report[k] for k in ("sha256", "size", "debuggable", "diagnostic_types_present", "zipalign_16kb")}, indent=2))
print(args.output)
