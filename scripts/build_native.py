#!/usr/bin/env python3
"""Build the recovered plugin for the macOS editor and Android ARM64."""
from pathlib import Path
import subprocess
import os

root=Path(__file__).resolve().parents[1]
ndk=Path(os.environ.get('SLINGSHOT_NDK','/Applications/Unity/Hub/Editor/6000.3.25f1/PlaybackEngines/AndroidPlayer/NDK'))
compiler=ndk/'toolchains/llvm/prebuilt/darwin-x86_64/bin/aarch64-linux-android26-clang++'
targets=[(root/'unity/Assets/Plugins/macOS/libGrappleRacerNative.dylib',
          ['clang++','-dynamiclib'], 'Editor', 'Editor', 'CPU: ARM64\n        OS: OSX'),
         (root/'unity/Assets/Plugins/Android/arm64-v8a/libGrappleRacerNative.so',
          [str(compiler),'-shared','-fPIC','-static-libstdc++','-Wl,-z,max-page-size=16384','-Wl,-z,common-page-size=16384'],
          'Android','Android','CPU: ARM64')]
for path,command,platform,target,settings in targets:
    path.parent.mkdir(parents=True,exist_ok=True)
    subprocess.run(command+['-std=c++17','-O2','-ffp-contract=off',str(root/'native/GrappleRacerNative.cpp'),'-o',str(path)],check=True)
    import hashlib
    guid=hashlib.sha256(str(path.relative_to(root)).encode()).hexdigest()[:32]
    Path(str(path)+'.meta').write_text(f'''fileFormatVersion: 2
guid: {guid}
PluginImporter:
  serializedVersion: 2
  isPreloaded: 0
  isOverridable: 0
  platformData:
  - first:
      Any:
    second:
      enabled: 0
      settings: {{}}
  - first:
      {platform}: {target}
    second:
      enabled: 1
      settings:
        {settings}
''')
    print(path)
