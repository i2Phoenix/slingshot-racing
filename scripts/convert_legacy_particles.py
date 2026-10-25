#!/usr/bin/env python3
"""Convert removed Unity 3 components without changing serialized object IDs."""
from pathlib import Path
import hashlib
import re
import yaml

ROOT=Path(__file__).resolve().parents[1]
ASSETS=ROOT/'unity/Assets'
CLASSES={12:'ParticleAnimator',15:'ParticleEmitter',26:'ParticleRenderer'}
guids={}
for kind,name in CLASSES.items():
    script=ASSETS/'Scripts/Compatibility'/f'{name}.cs'
    meta=Path(str(script)+'.meta')
    if meta.exists():guid=re.search(r'guid: (\w+)',meta.read_text())[1]
    else:
        guid=hashlib.sha256(('slingshot-modern/'+name).encode()).hexdigest()[:32]
        meta.write_text(f'fileFormatVersion: 2\nguid: {guid}\nMonoImporter:\n  serializedVersion: 2\n  defaultReferences: []\n  executionOrder: 0\n')
    guids[kind]=guid

count=0
def convert(match):
    global count
    kind=int(match[1]);fid=match[2];body=match[3]
    if kind not in CLASSES:return match[0]
    data=next(iter(yaml.safe_load(body).values()))
    result={k:data[k] for k in ('m_ObjectHideFlags','m_PrefabParentObject','m_PrefabInternal','m_GameObject') if k in data}
    result.update(m_Enabled=data.get('m_Enabled',1),m_EditorHideFlags=0,
        m_Script={'fileID':11500000,'guid':guids[kind],'type':3},m_Name='',m_EditorClassIdentifier='')
    if kind==15:
        for k in ('minSize','maxSize','minEnergy','maxEnergy','minEmission','maxEmission','worldVelocity','localVelocity','rndVelocity','emitterVelocityScale','tangentVelocity','angularVelocity','rndAngularVelocity','rndRotation'):
            if k in data:result[k]=data[k]
        for old,new in [('m_Emit','emit'),('Simulate in Worldspace?','useWorldSpace'),('m_OneShot','oneShot'),('m_Ellipsoid','ellipsoid'),('m_MinEmitterRange','minEmitterRange')]:
            if old in data:result[new]=data[old]
    elif kind==12:
        for k in ('worldRotationAxis','localRotationAxis','sizeGrow','rndForce','force','damping','stopSimulation','autodestruct'):
            if k in data:result[k]=data[k]
        result['doesAnimateColor']=data['Does Animate Color?']
        result['colorAnimation']=[{k:((data[f'colorAnimation[{i}]']['rgba']>>(j*8))&255)/255.0 for j,k in enumerate('rgba')} for i in range(5)]
    else:
        result['sharedMaterial']=data['m_Materials'][0] if data.get('m_Materials') else {'fileID':0}
        for old,new in [('m_CameraVelocityScale','cameraVelocityScale'),('m_StretchParticles','particleRenderMode'),('m_LengthScale','lengthScale'),('m_VelocityScale','velocityScale'),('m_MaxParticleSize','maxParticleSize')]:
            if old in data:result[new]=data[old]
        for old,new in [('x Tile','uvAnimationXTile'),('y Tile','uvAnimationYTile'),('cycles','uvAnimationCycles')]:result[new]=data.get('UV Animation',{}).get(old,1)
    count+=1
    return f'--- !u!114 &{fid}\n'+yaml.safe_dump({'MonoBehaviour':result},sort_keys=False,default_flow_style=None,width=120)

for path in ASSETS.rglob('*'):
    if path.suffix not in ('.unity','.prefab','.asset'):continue
    text=path.read_text()
    if not re.search(r'^--- !u!(12|15|26) ',text,re.M):continue
    text=re.sub(r'^--- !u!(\d+) &([^\n]+)\n(.*?)(?=^--- !u!|\Z)',convert,text,flags=re.M|re.S)
    text=re.sub(r'^(  - )(12|15|26)(: \{fileID:)',r'\g<1>114\3',text,flags=re.M)
    path.write_text(text)
print(f'Converted {count} legacy particle components; component fileIDs preserved.')
