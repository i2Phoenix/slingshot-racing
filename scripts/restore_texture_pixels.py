#!/usr/bin/env python3
"""Decode Unity 3 Android GL_UNSIGNED_SHORT_4_4_4_4 pixels from the APK.

Format id 2 in this APK predates the separate RGBA4444 enum. Treating it as
modern ARGB4444 rotates RGBA channels, making coloured trails purple and
turning alpha gradients into blue. Preserve source bytes and asset GUIDs.
"""
import json
from pathlib import Path
import numpy as np
from PIL import Image
import UnityPy

ROOT = Path(__file__).resolve().parents[1]
source = ROOT / 'work/apk/assets/bin/Data'
textures = ROOT / 'unity/Assets/Texture2D'
report = []
for path in sorted(source.iterdir()):
    if path.is_dir() or path.name.endswith('.resS') or path.name == 'unity default resources':
        continue
    try:
        objects = UnityPy.load(str(path)).objects
    except Exception:
        continue
    for obj in objects:
        if obj.type.name != 'Texture2D':
            continue
        texture = obj.read()
        target = textures / (texture.m_Name + '.png')
        if texture.m_TextureFormat != 2 or not target.is_file():
            continue
        w, h = texture.m_Width, texture.m_Height
        packed = np.frombuffer(texture.image_data, dtype='<u2', count=w*h).reshape(h, w)
        pixels = np.stack([((packed >> shift) & 15) * 17 for shift in (12, 8, 4, 0)], axis=2).astype('uint8')
        Image.fromarray(pixels[::-1]).save(target)
        report.append({'source': path.name, 'pathID': obj.path_id, 'name': texture.m_Name, 'size': [w,h]})
report_path = ROOT / 'work/qa/restored-texture-pixels.json'
report_path.write_text(json.dumps(report, indent=2) + '\n')
print(f'Restored RGBA4444 pixels for {len(report)} textures')
