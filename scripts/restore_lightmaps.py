#!/usr/bin/env python3
"""Keep original scene lightmap tables/bindings that Unity 6's importer discards."""
from pathlib import Path
import re
import yaml

ROOT = Path(__file__).resolve().parents[1]
PROJECT = ROOT / 'unity'
GUID = '5a93fa588b9e4a43871dadbfce118a20'
track_guid = re.search(r'guid: (\w+)', (PROJECT / 'Assets/Scripts/Assembly-CSharp/TrackLighting.cs.meta').read_text())[1]
count = renderers = maps = 0
for source in (ROOT / 'recovered/ExportedProject/Assets/Scenes').rglob('*.unity'):
    path = PROJECT / source.relative_to(ROOT / 'recovered/ExportedProject')
    text = path.read_text()
    # Idempotent: replace only our generated root at the end of the scene.
    text = text.split('--- !u!1 &990000001\n')[0]
    objects = {int(key): (int(kind), next(iter(yaml.safe_load(body).values())))
        for kind, key, body in re.findall(r'^--- !u!(\d+) &(\d+)\n(.*?)(?=^---|\Z)', source.read_text(), re.M | re.S)}
    settings = next((body for kind, body in objects.values() if kind == 157), {})
    textures = settings.get('m_Lightmaps', [])
    if not textures:
        continue
    bindings = [(key, body) for key, (kind, body) in objects.items()
        if kind in (23, 137) and 0 <= body.get('m_LightmapIndex', 255) < len(textures)]
    tracks = [key for key, (kind, body) in objects.items()
        if kind == 114 and body.get('m_Script', {}).get('guid') == track_guid]
    generated = '''--- !u!1 &990000001
GameObject:
  serializedVersion: 6
  m_ObjectHideFlags: 0
  m_Component:
  - component: {fileID: 990000002}
  - component: {fileID: 990000003}
  m_Layer: 0
  m_Name: Legacy baked lighting
  m_TagString: Untagged
  m_IsActive: 1
--- !u!4 &990000002
Transform:
  m_ObjectHideFlags: 0
  m_GameObject: {fileID: 990000001}
  serializedVersion: 2
  m_LocalRotation: {x: 0, y: 0, z: 0, w: 1}
  m_LocalPosition: {x: 0, y: 0, z: 0}
  m_LocalScale: {x: 1, y: 1, z: 1}
  m_Children: []
  m_Father: {fileID: 0}
--- !u!114 &990000003
MonoBehaviour:
  m_ObjectHideFlags: 0
  m_GameObject: {fileID: 990000001}
  m_Enabled: 1
  m_Script: {fileID: 11500000, guid: GUID, type: 3}
  m_Name:
  textures:
'''.replace('GUID', GUID)
    for entry in textures:
        ref = entry['m_Lightmap']
        generated += f"  - {{fileID: {ref['fileID']}, guid: {ref['guid']}, type: 3}}\n"
    generated += '  bindings:' + ('\n' if bindings else ' []\n')
    for key, body in bindings:
        uv = body['m_LightmapTilingOffset']
        generated += f"  - renderer: {{fileID: {key}}}\n    index: {body['m_LightmapIndex']}\n    scaleOffset: {{x: {uv['x']}, y: {uv['y']}, z: {uv['z']}, w: {uv['w']}}}\n"
    generated += '  trackLighting:' + ('\n' if tracks else ' []\n')
    for key in tracks:
        generated += f'  - {{fileID: {key}}}\n'
    path.write_text(text + generated)
    count += 1
    renderers += len(bindings)
    maps += len(textures)
print(f'Restored {maps} baked lightmaps / {renderers} renderer bindings in {count} scenes')
