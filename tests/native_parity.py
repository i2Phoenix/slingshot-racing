#!/usr/bin/env python3
"""Run original ARMv7 instructions in Unicorn and compare the portable plugin.

This checks numerical routines, not Android or gameplay acceptance. The original
APK and the extracted library remain read-only. No Android services are invoked.
"""
import ctypes as C
import json
import math
from pathlib import Path
import random
import struct
import sys
import numpy as np
from elftools.elf.elffile import ELFFile
from unicorn import Uc, UC_ARCH_ARM, UC_MODE_ARM, UC_HOOK_CODE
from unicorn.arm_const import *

ROOT = Path(__file__).resolve().parents[1]
T = {'float': C.c_float, 'int': C.c_int32, 'uint': C.c_uint32,
     'ushort': C.c_uint16, 'byte': C.c_uint8, 'bool': C.c_bool, 'void': None}
for line in (ROOT/'tests/fixtures/native-structs.tsv').read_text().splitlines():
    name, *fields = line.split('\t')
    T[name] = type(name, (C.Structure,), {'_fields_': [(n,T[t]) for t,n in (x.split(':') for x in fields)]})
globals().update({k:v for k,v in T.items() if k not in {'float','int','bool','void','byte','uint','ushort'}})
PROTOTYPES = {}
for line in (ROOT/'tests/fixtures/native-prototypes.tsv').read_text().splitlines()[5:]:
    ret, name, *args = line.split('\t')
    PROTOTYPES[name] = (ret, [x.split(':')[0] for x in args])

def fbits(f): return struct.unpack('<I', struct.pack('<f', f))[0]
def bitsf(i): return struct.unpack('<f', struct.pack('<I', i))[0]

class ArmReference:
    BASE, HEAP, STACK, IMPORTS, STOP = 0x10000, 0x100000, 0x800000, 0x900000, 0x910000
    def __init__(self):
        self.uc = uc = Uc(UC_ARCH_ARM, UC_MODE_ARM)
        uc.ctl_set_cpu_model(UC_CPU_ARM_CORTEX_A15)
        uc.reg_write(UC_ARM_REG_C1_C0_2, 0xf << 20)
        uc.reg_write(UC_ARM_REG_FPEXC, 0x40000000)
        uc.mem_map(self.BASE, 0x10000)
        uc.mem_map(self.HEAP, 0x400000)
        uc.mem_map(self.STACK, 0x100000)
        uc.mem_map(self.IMPORTS, 0x20000)
        self.symbols, self.imports = {}, {}
        with (ROOT/'work/apk/lib/armeabi-v7a/libGrappleRacerNative.so').open('rb') as f:
            elf = ELFFile(f)
            for seg in elf.iter_segments():
                if seg['p_type'] == 'PT_LOAD': uc.mem_write(self.BASE+seg['p_vaddr'],seg.data())
            symbols=elf.get_section_by_name('.dynsym')
            for symbol in symbols.iter_symbols():
                if symbol['st_value']: self.symbols[symbol.name]=self.BASE+symbol['st_value']
            def imported(name):
                for addr,n in self.imports.items():
                    if n==name:return addr
                addr=self.IMPORTS+len(self.imports)*16
                self.imports[addr]=name
                uc.mem_write(addr, bytes.fromhex('1eff2fe1')) # bx lr
                return addr
            for section in elf.iter_sections():
                if section['sh_type']!='SHT_REL':continue
                for relocation in section.iter_relocations():
                    addr=self.BASE+relocation['r_offset'];kind=relocation['r_info_type']
                    if kind==23:value=struct.unpack('<I',uc.mem_read(addr,4))[0]+self.BASE
                    elif kind in (21,22):
                        symbol=symbols.get_symbol(relocation['r_info_sym']);value=self.symbols.get(symbol.name) or imported(symbol.name)
                    else:raise ValueError(kind)
                    uc.mem_write(addr,struct.pack('<I',value))
        uc.hook_add(UC_HOOK_CODE,self.import_call,begin=self.IMPORTS,end=self.IMPORTS+0xffff)
    def import_call(self,uc,address,size,user):
        name=self.imports.get(address)
        if name is None:raise RuntimeError(hex(address))
        a=uc.reg_read(UC_ARM_REG_R0);b=uc.reg_read(UC_ARM_REG_R1)
        if name in ('sinf','cosf','acosf','fminf','fmaxf'):
            x,y=bitsf(a),bitsf(b)
            fn={'sinf':math.sin,'cosf':math.cos,'acosf':math.acos,'fminf':min,'fmaxf':max}[name]
            try:value=fn(x,y) if name in ('fminf','fmaxf') else fn(x)
            except ValueError:value=math.nan
            uc.reg_write(UC_ARM_REG_R0,fbits(value))
        elif name in ('memcpy','__aeabi_memcpy'):
            uc.mem_write(a,bytes(uc.mem_read(b,uc.reg_read(UC_ARM_REG_R2))))
        else:raise RuntimeError('Unexpected external call: '+name)
        uc.reg_write(UC_ARM_REG_PC,uc.reg_read(UC_ARM_REG_LR))
    def call(self,name,args):
        ret,types=PROTOTYPES[name];words=[];buffers=[];offset=0
        for t,arg in zip(types,args):
            if t.endswith('*'):
                addr=self.HEAP+offset;data=C.string_at(C.addressof(arg),C.sizeof(arg));self.uc.mem_write(addr,data)
                words.append(addr);buffers.append((addr,len(data)));offset+=(len(data)+15)&~15
            else:words.append(fbits(arg) if t=='float' else arg);buffers.append(None)
        sp=self.STACK+0x80000
        for i,value in enumerate(words):
            if i<4:self.uc.reg_write(UC_ARM_REG_R0+i,value)
            else:self.uc.mem_write(sp+(i-4)*4,struct.pack('<I',value))
        self.uc.reg_write(UC_ARM_REG_SP,sp);self.uc.reg_write(UC_ARM_REG_LR,self.STOP)
        self.uc.emu_start(self.symbols[name],self.STOP,count=2000000)
        if self.uc.reg_read(UC_ARM_REG_PC)!=self.STOP:raise RuntimeError('Instruction limit: '+name)
        r=self.uc.reg_read(UC_ARM_REG_R0)
        if ret=='float':r=bitsf(r)
        if ret=='void':r=None
        return r,[bytes(self.uc.mem_read(*b)) if b else None for b in buffers]

def clone(arg):return type(arg).from_buffer_copy(C.string_at(C.addressof(arg),C.sizeof(arg)))
def vector(scale=10):return Vector3(*(rng.uniform(-scale,scale) for _ in range(3)))
def unit():
    v=np.array([rng.uniform(-1,1) for _ in range(3)]);v/=np.linalg.norm(v);return Vector3(*v)
def new_particle():
    p=ParticleSysParticle();p.m_pos=vector();p.m_vel=vector();p.m_energy=rng.uniform(.01,2);p.m_rotation=rng.uniform(-3,3);p.m_angularVelocity=rng.uniform(-5,5);p.m_colour=Color(*(rng.random() for _ in range(4)));p.m_startEnergyRecip=.5;p.m_halfSize=rng.uniform(.1,5);p.m_rightX=math.cos(p.m_rotation);p.m_rightY=-math.sin(p.m_rotation);return p
def cases(name):
    v=[vector() for _ in range(4)];t=rng.uniform(0,1);up0,up1=unit(),unit()
    if name=='QuaternionMul':
        q=np.array([rng.uniform(-1,1) for _ in range(4)]);q/=np.linalg.norm(q);return [vector(),Quaternion(*q)]
    if name=='CubicHermite1D':return [C.c_float(),*[rng.uniform(-10,10) for _ in range(4)],t]
    if name.startswith('CubicHermite'):return [Vector3(),*v,t]
    if name in ('TrackUtils_SplinePoint','TrackUtils_SplineTangent','TrackUtils_SplineTangentDT'):return [Vector3(),*v,C.c_float(t)]
    if name=='TrackUtils_SplineUp':return [Vector3(),up0,up1,C.c_float(t)]
    if name=='TrackUtils_SplineNormal2':return [Vector3(),v[0],up0]
    if name in ('TrackUtils_SplineWidth','TrackUtils_SplineRoofHeight'):return [C.c_float(),C.c_float(3),C.c_float(9),C.c_float(t)]
    if name=='TrackUtils_SplineDsDt':return [C.c_float(),*v,C.c_float(t)]
    if name in ('TrackUtils_SplineNormal','TrackUtils_SplineCurve'):return [Vector3() if name.endswith('Normal') else C.c_float(),v[0],v[1],up0,v[2],v[3],up1,C.c_float(t)]
    if name.startswith('TrackUtils_ReParam'):
        n=(ReParamNode*4)(*[ReParamNode(i*10,i,10,.1) for i in range(4)]);return [n,t*(30 if name.endswith('StoT') else 3)]
    if name.startswith('TrackUtils_'):
        n=(TrackNodeCache*4)(*[TrackNodeCache(vector(),vector(),unit(),5+i) for i in range(4)])
        t=rng.uniform(-.2,3.2)
        if name=='TrackUtils_InfoAtT':return [n,4,t,Vector3(),Vector3(),Vector3(),C.c_float()]
        if name=='TrackUtils_ClosestTToPoint':return [n,4,vector(),clamp(t,0,3),.5,0.,3.]
        return [C.c_float() if name in ('TrackUtils_DsDtAtT','TrackUtils_CurveAtT') else Vector3(),n,4,t]
    if name.startswith('ParticleSys_'):
        p=(ParticleSysParticle*5)(*[new_particle() for _ in range(5)])
        if name.endswith('UpdateParticles'):return [p,5,.1,.98,1.01,vector(1)]
        if name.endswith('Billboard'):return [(Vector3*20)(),p,5,unit(),unit()]
        if name.endswith('Stretch'):return [(Vector3*20)(),p,5,unit(),C.c_float(.3),C.c_float(2)]
        return [(Color*20)(),p,5,(Color*5)(*[Color(*(rng.random() for _ in range(4))) for i in range(5)])]
    if name.startswith('Rope'):
        n=12;p=(Vector2*n)(*[Vector2(i,rng.uniform(-.2,.2)) for i in range(n)]);old=(Vector2*n)(*[Vector2(x.x+.01,x.y-.01) for x in p]);
        if name=='RopeUpdate':return [p,old,RopeUpdateParams(n,1.1,rng.choice([0,.4,1]),.3,Vector2(0,0),Vector2(n-1,.4))]
        return [(Vector3*(2*n))(),(Color32*(2*n))(),p,old,RopeUpdateMeshParams(n,.3,1,4,rng.random(),Vector2(0,0),Color(.1,.4,.8,.7))]
    if name.startswith('Streamers_'):
        n=8;old=(StreamerParticle*n)(*[StreamerParticle(Vector3(i%4,1+i//4,0),Vector3(0,1,0),Vector3(0,0,1),Vector3(1,0,.1)) for i in range(n)])
        if name.endswith('BuildVertices'):return [(Vector3*(2*n))(),(Vector3*(2*n))(),old,clone(old),BuildVerticesParams(n,.2,.1,.3)]
        return [(StreamerParticle*n)(),old,(Streamer*2)(*[Streamer(Color(1,1,1,1),30) for i in range(2)]),UpdateParticlesParams(2,4,Vector3(.02,-.01,0),Vector3(.03,-.02,0),Vector3(.02,.02,.01),.02,.5,.8,.8,.9)]
    raise KeyError(name)
def clamp(x,a,b):return max(a,min(b,x))

def main():
    global rng
    rng=random.Random(630025);reference=ArmReference();lib=C.CDLL(str(ROOT/'unity/Assets/Plugins/macOS/libGrappleRacerNative.dylib'));report={}
    for name,(ret,types) in PROTOTYPES.items():
        if name.startswith('Platform_') or name=='BypassMesh_DrawRange':continue
        fn=getattr(lib,name);fn.restype=T[ret];fn.argtypes=[C.c_void_p if t.endswith('*') else T[t] for t in types]
        failures=[];maximum=0.;runs=100
        for iteration in range(runs):
            args=cases(name);result,buffers=reference.call(name,args);copies=[clone(a) if t.endswith('*') else a for t,a in zip(types,args)]
            actual=fn(*[C.byref(a) if t.endswith('*') else a for t,a in zip(types,copies)])
            mismatch=False
            if result is not None and not np.isclose(result,actual,rtol=1e-4,atol=3e-5,equal_nan=True):mismatch=True
            for t,a,expected in zip(types,copies,buffers):
                if expected is None:continue
                got=C.string_at(C.addressof(a),C.sizeof(a))
                if got==expected:continue
                if t in ('Color32*','ColourRGBA32*'):
                    x=np.frombuffer(got,dtype=np.uint8).astype(int);y=np.frombuffer(expected,dtype=np.uint8).astype(int)
                    mismatch|=bool(np.max(np.abs(x-y))>1)
                else:
                    x=np.frombuffer(got,dtype='<f4');y=np.frombuffer(expected,dtype='<f4');diff=np.abs(x-y)
                    if np.any(np.isfinite(diff)):maximum=max(maximum,float(np.nanmax(diff)))
                    mismatch|=not np.allclose(x,y,rtol=1e-4,atol=3e-5,equal_nan=True)
            if mismatch and len(failures)<3:failures.append({'iteration':iteration,'return_original':result,'return_port':actual,'inputs':[str(a) for a in args]})
        report[name]={'cases':runs,'max_absolute_difference':maximum,'failures':failures};print(name,'PASS' if not failures else 'FAIL',f'{maximum:.5g}',flush=True)
    (ROOT/'work/native-parity.json').write_text(json.dumps(report,indent=2));return int(any(v['failures'] for v in report.values()))
if __name__=='__main__':sys.exit(main())
