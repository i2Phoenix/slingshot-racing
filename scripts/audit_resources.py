#!/usr/bin/env python3
"""Check restored YAML references and map source files to the APK export."""
import collections
import json
import re
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
ASSETS = ROOT / 'unity/Assets'
BASELINE = ROOT / 'recovered/ExportedProject/Assets'
OUT = ROOT / 'work/qa'
OUT.mkdir(parents=True, exist_ok=True)
guids = collections.defaultdict(list)
for path in ASSETS.rglob('*.meta'):
    match = re.search(r'^guid: (\w+)', path.read_text(), re.M)
    if match: guids[match[1]].append(str(path.relative_to(ASSETS)))
missing = collections.defaultdict(list)
broken = []
counts = collections.Counter()
for path in ASSETS.rglob('*'):
    if path.suffix not in {'.asset', '.unity', '.anim', '.prefab', '.mat'}: continue
    data = path.read_text(errors='replace')
    if not data.startswith('%YAML'): continue
    name = str(path.relative_to(ASSETS)); counts[path.suffix] += 1
    ids = set(re.findall(r'^--- !u!\d+ &(-?\d+)', data, re.M))
    for ref in re.findall(r'\{fileID: ([^}]+)\}', data):
        guid = re.search(r'guid: ([0-9a-f]+)', ref)
        if guid:
            value = guid[1]
            if value not in guids and not value.startswith('0000000000000000'):
                missing[value].append(name)
        else:
            value = ref.strip()
            if value != '0' and value not in ids: broken.append([name, value])
references = dict(yaml_files=counts, guid_collisions={g:p for g,p in guids.items() if len(p)>1},
                  missing_guids=missing, broken_local_refs=broken)
(OUT / 'reference-audit.json').write_text(json.dumps(references, indent=2)+'\n')

source = dict(unchanged=[], changed=[], relocated=[], excluded_metadata=[], missing=[])
for original in sorted(BASELINE.rglob('*.cs')):
    rel = original.relative_to(BASELINE)
    target = ASSETS / rel
    if not target.exists() and str(rel).startswith('Scripts/P31RestKit/'):
        target = ASSETS / str(rel).replace('Scripts/P31RestKit/', 'Plugins/P31RestKit/', 1)
        source['relocated'].append([str(rel), str(target.relative_to(ASSETS))])
    if not target.exists():
        category = 'excluded_metadata' if str(rel) in {
            'Scripts/System.Runtime.CompilerServices.Unsafe/Properties/AssemblyInfo.cs',
            'Scripts/System.Runtime.InteropServices/Properties/AssemblyInfo.cs'} else 'missing'
        source[category].append(str(rel)); continue
    source['unchanged' if original.read_bytes()==target.read_bytes() else 'changed'].append(str(rel))
(OUT / 'source-differences.json').write_text(json.dumps(source, indent=2)+'\n')
shader_interfaces = []
shader_count = 0
for path in sorted(ASSETS.rglob('*.shader')):
    shader_count += 1
    data = path.read_text()
    vertex = re.findall(r'struct SRVertexOutput\s*\{(.*?)\};', data, re.S)
    fragment = re.findall(r'struct SRFragmentInput\s*\{(.*?)\};', data, re.S)
    for variant, (v, f) in enumerate(zip(vertex, fragment)):
        fields = lambda text: {name:(kind, semantic) for kind,name,semantic in
                               re.findall(r'(\w+)\s+(\w+)\s*:\s*(\w+)\s*;', text)}
        outputs = fields(v)
        for name, binding in fields(f).items():
            if outputs.get(name) != binding:
                shader_interfaces.append(dict(shader=str(path.relative_to(ASSETS)), variant=variant,
                                              varying=name, vertex=outputs.get(name), fragment=binding))
shader_report = dict(shader_count=shader_count, interface_mismatches=shader_interfaces)
(OUT / 'shader-interface-audit.json').write_text(json.dumps(shader_report, indent=2)+'\n')
summary = {'references': references, 'source_counts': {k:len(v) for k,v in source.items()}, 'shaders': shader_report}
print(json.dumps(summary, indent=2))
raise SystemExit(bool(references['guid_collisions'] or missing or broken or source['missing'] or shader_interfaces))
