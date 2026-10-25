#include "GrappleRacerNative.h"
#include <algorithm>
#include <cmath>

// Recovered scalar algorithms. Keep contraction disabled: the ARMv7 original
// rounds each multiply/add, which matters when projecting vehicles onto tracks.
static_assert(sizeof(TrackNodeCache)==40 && sizeof(ParticleSysParticle)==68);
static_assert(sizeof(RopeUpdateParams)==32 && sizeof(RopeUpdateMeshParams)==44);
static_assert(sizeof(StreamerParticle)==48 && sizeof(UpdateParticlesParams)==64);
static Vector3 operator+(Vector3 a,Vector3 b){return {a.x+b.x,a.y+b.y,a.z+b.z};}
static Vector3 operator-(Vector3 a,Vector3 b){return {a.x-b.x,a.y-b.y,a.z-b.z};}
static Vector3 operator*(Vector3 a,float b){return {a.x*b,a.y*b,a.z*b};}
static Vector2 operator+(Vector2 a,Vector2 b){return {a.x+b.x,a.y+b.y};}
static Vector2 operator-(Vector2 a,Vector2 b){return {a.x-b.x,a.y-b.y};}
static Vector2 operator*(Vector2 a,float b){return {a.x*b,a.y*b};}
static float dot(Vector3 a,Vector3 b){return a.x*b.x+a.y*b.y+a.z*b.z;}
static float dot(Vector2 a,Vector2 b){return a.x*b.x+a.y*b.y;}
static Vector3 cross(Vector3 a,Vector3 b){return {a.y*b.z-a.z*b.y,a.z*b.x-a.x*b.z,a.x*b.y-a.y*b.x};}
static float length(Vector3 a){return std::sqrt(dot(a,a));}
static Vector3 normalized(Vector3 a){return a*(1.f/length(a));}
static float nanZero(float x){return std::isnan(x)?0.f:x;}
static Vector3 nanZero(Vector3 v){return {nanZero(v.x),nanZero(v.y),nanZero(v.z)};}
static float clamp(float x,float lo,float hi){return x<lo?lo:(x>hi?hi:x);}
static Vector3 lerp(Vector3 a,Vector3 b,float t){return a+(b-a)*t;}
static Vector2 lerp(Vector2 a,Vector2 b,float t){return a+(b-a)*t;}
static float smooth(float t){return t*t*t*(t*(t*6.f-15.f)+10.f);}
static float hermite(float p0,float m0,float p1,float m1,float t){
    float t2=t*t,t3=t2*t,h=t3+t3+t2*-3.f;
    return (t3-t2)*m1+(((t3+t2*-2.f+t)*m0+(h+1.f)*p0)-p1*h);
}
static Vector3 hermite(Vector3 p0,Vector3 m0,Vector3 p1,Vector3 m1,float t,int derivative=0){
    float a,b,c;
    if(derivative==0){float t2=t*t,t3=t2*t; a=t3+t3+t2*-3.f;b=t3+t2*-2.f+t;c=t3-t2;}
    else if(derivative==1){a=t*t*6.f-t*6.f;b=t*t*3.f+t*-4.f+1.f;c=t*t*3.f+t*-2.f;}
    else {a=t*12.f-6.f;b=t*6.f-4.f;c=t*6.f-2.f;}
    float d=a+(derivative==0?1.f:0.f);
    return m1*c+((m0*b+p0*d)-p1*a);
}
static Vector3 up(Vector3 a,Vector3 b,float t){
    float d=std::fmax(std::fmin(dot(a,b),1.f),-1.f);
    Vector3 axis=nanZero(normalized(b-a*d));float angle=std::acos(d)*smooth(t);
    return a*std::cos(angle)+axis*std::sin(angle);
}
static int node(TrackNodeCache* nodes,int n,float &t){int i=std::max(0,std::min(int(t),n-2));t=std::fmax(std::fmin(t-float(i),1.f),0.f);return i;}
static Vector3 at(TrackNodeCache* nodes,int n,float t,int d=0){int i=node(nodes,n,t);auto&a=nodes[i];auto&b=nodes[i+1];return hermite(a.m_position,a.m_tangent,b.m_position,b.m_tangent,t,d);}
static Vector3 normalAt(TrackNodeCache* nodes,int n,float t){int i=node(nodes,n,t);auto&a=nodes[i];auto&b=nodes[i+1];return normalized(cross(up(a.m_up,b.m_up,t),hermite(a.m_position,a.m_tangent,b.m_position,b.m_tangent,t,1)));}

extern "C" {
bool Platform_IsSimdSupported(){return true;}
void Platform_DisableSimd(bool){}
void CubicHermite1D(float*r,float p,float m,float q,float n,float t){*r=hermite(p,m,q,n,t);}
void CubicHermite(Vector3*r,Vector3*p,Vector3*m,Vector3*q,Vector3*n,float t){*r=hermite(*p,*m,*q,*n,t);}
void CubicHermiteDT(Vector3*r,Vector3*p,Vector3*m,Vector3*q,Vector3*n,float t){*r=hermite(*p,*m,*q,*n,t,1);}
void CubicHermiteDT2(Vector3*r,Vector3*p,Vector3*m,Vector3*q,Vector3*n,float t){*r=hermite(*p,*m,*q,*n,t,2);}
void TrackUtils_SplinePoint(Vector3*r,Vector3*p,Vector3*m,Vector3*q,Vector3*n,float*t){CubicHermite(r,p,m,q,n,*t);}
void TrackUtils_SplineTangent(Vector3*r,Vector3*p,Vector3*m,Vector3*q,Vector3*n,float*t){CubicHermiteDT(r,p,m,q,n,*t);}
void TrackUtils_SplineTangentDT(Vector3*r,Vector3*p,Vector3*m,Vector3*q,Vector3*n,float*t){CubicHermiteDT2(r,p,m,q,n,*t);}
void TrackUtils_SplineUp(Vector3*r,Vector3*a,Vector3*b,float*t){*r=up(*a,*b,*t);}
void TrackUtils_SplineNormal2(Vector3*r,Vector3*t,Vector3*u){*r=normalized(cross(*u,*t));}
void TrackUtils_SplineNormal(Vector3*r,Vector3*p,Vector3*m,Vector3*u,Vector3*q,Vector3*n,Vector3*v,float*t){*r=normalized(cross(up(*u,*v,*t),hermite(*p,*m,*q,*n,*t,1)));}
void TrackUtils_SplineWidth(float*r,float*a,float*b,float*t){*r=*a+(*b-*a)**t;}
void TrackUtils_SplineRoofHeight(float*r,float*a,float*b,float*t){TrackUtils_SplineWidth(r,a,b,t);}
void TrackUtils_SplineDsDt(float*r,Vector3*p,Vector3*m,Vector3*q,Vector3*n,float*t){*r=length(hermite(*p,*m,*q,*n,*t,1));}
void TrackUtils_SplineCurve(float*r,Vector3*p,Vector3*m,Vector3*u,Vector3*q,Vector3*n,Vector3*v,float*t){
    Vector3 tangent=hermite(*p,*m,*q,*n,*t,1),normal=normalized(cross(up(*u,*v,*t),tangent));float len=length(tangent);
    *r=len>0.0001f?dot(hermite(*p,*m,*q,*n,*t,2),normal)/len:0.f;
}
void TrackUtils_PointAtT(Vector3*r,TrackNodeCache*n,int c,float t){*r=at(n,c,t);}
void TrackUtils_TangentAtT(Vector3*r,TrackNodeCache*n,int c,float t){*r=at(n,c,t,1);}
void TrackUtils_TangentDTAtT(Vector3*r,TrackNodeCache*n,int c,float t){*r=at(n,c,t,2);}
void TrackUtils_UpAtT(Vector3*r,TrackNodeCache*n,int c,float t){int i=node(n,c,t);*r=up(n[i].m_up,n[i+1].m_up,t);}
void TrackUtils_NormalAtT(Vector3*r,TrackNodeCache*n,int c,float t){*r=normalAt(n,c,t);}
void TrackUtils_DsDtAtT(float*r,TrackNodeCache*n,int c,float t){*r=length(at(n,c,t,1));}
void TrackUtils_CurveAtT(float*r,TrackNodeCache*n,int c,float t){float len=length(at(n,c,t,1));*r=len>0.0001f?dot(at(n,c,t,2),normalAt(n,c,t))/len:0.f;}
void TrackUtils_InfoAtT(TrackNodeCache*n,int c,float t,Vector3*p,Vector3*tangent,Vector3*normal,float*w){
    *p=at(n,c,t);*tangent=at(n,c,t,1);*normal=normalAt(n,c,t);int i=node(n,c,t);*w=n[i].m_width+(n[i+1].m_width-n[i].m_width)*t;
}
float TrackUtils_ReParamStoT(ReParamNode*n,float s){while(n[1].s<s)++n;float delta=n[1].s-n[0].s;return hermite(n[0].t,n[0].dtds*delta,n[1].t,n[1].dtds*delta,(s-n[0].s)/delta);}
float TrackUtils_ReParamTtoS(ReParamNode*n,float t){while(n[1].t<t)++n;float delta=n[1].t-n[0].t;return hermite(n[0].s,n[0].dsdt*delta,n[1].s,n[1].dsdt*delta,(t-n[0].t)/delta);}
float TrackUtils_ClosestTToPoint(TrackNodeCache*n,int count,Vector3*p,float hint,float spread,float lower,float upper){
    struct CP{float t,d;};
    auto distance=[&](float t){Vector3 q=t<0?n[0].m_position+n[0].m_tangent*t:(t>count-1?n[count-1].m_position+n[count-1].m_tangent*(t-float(count-1)):at(n,count,t));q=q-*p;return dot(q,q);};
    auto make=[&](float t){return CP{t,distance(t)};};
    CP cp[3]={make(clamp(hint-spread,lower,upper)),make(hint),make(clamp(hint+spread,lower,upper))};
    auto sort=[&](){if(cp[1].d<cp[0].d)std::swap(cp[0],cp[1]);if(cp[2].d<cp[1].d)std::swap(cp[1],cp[2]);if(cp[1].d<cp[0].d)std::swap(cp[0],cp[1]);};sort();
    int steps=0;
    while(steps<16){++steps;auto a=cp[0],b=cp[1],c=cp[2];float denominator=(b.t-c.t)*a.d+(c.t-a.t)*b.d+(a.t-b.t)*c.d;if(denominator<=1e-9f)break;
        float t=(((b.t*b.t-c.t*c.t)*a.d+(c.t*c.t-a.t*a.t)*b.d+(a.t*a.t-b.t*b.t)*c.d)/denominator)*0.5f;
        CP candidate=make(clamp(t,lower,upper));if(candidate.d>=cp[2].d-0.01f)break;cp[2]=candidate;sort();}
    while(steps<16){++steps;Vector3 diff=at(n,count,cp[0].t)-*p,tangent=at(n,count,cp[0].t,1),dt=at(n,count,cp[0].t,2);
        float denominator=tangent.x*tangent.x+diff.x*dt.x+tangent.y*tangent.y+diff.y*dt.y+tangent.z*tangent.z+diff.z*dt.z;
        CP candidate=make(clamp(cp[0].t-dot(diff,tangent)/denominator,lower,upper));if(candidate.d>=cp[0].d-0.01f)break;cp[0]=candidate;}
    float step=dot(at(n,count,cp[0].t)-*p,at(n,count,cp[0].t,1))>0?-0.01f:0.01f;
    while(steps<16){++steps;CP candidate=make(clamp(cp[0].t+step,lower,upper));if(candidate.d>=cp[0].d-0.01f)break;cp[0]=candidate;}return cp[0].t;
}
void QuaternionMul(float*v,Quaternion*q){
    Vector3 a{v[0],v[1],v[2]},b{q->x,q->y,q->z};float d=dot(a,b),ww=q->w*q->w+q->w*q->w;
    Vector3 r=(a*ww-a)+(b+b)*d+cross(b,a)*(q->w+q->w);v[0]=r.x;v[1]=r.y;v[2]=r.z;
}
uint ParticleSys_UpdateParticles(ParticleSysParticle*p,uint count,float dt,float damping,float scale,Vector3*force){
    Vector3 acceleration=*force*(dt*scale);
    for(uint i=0;i<count;++i){auto&a=p[i];a.m_energy-=dt;
        if(a.m_energy<=0){if(i!=count-1){a=p[count-1];--i;--count;}continue;}
        a.m_vel=acceleration+a.m_vel*damping;a.m_pos=a.m_pos+a.m_vel*dt;a.m_halfSize*=scale;
        if(a.m_angularVelocity!=0){a.m_rotation+=a.m_angularVelocity*dt;a.m_rightX=std::cos(a.m_rotation);a.m_rightY=-std::sin(a.m_rotation);}}
    return count;
}
void ParticleSys_BuildVertices_Billboard(Vector3*v,ParticleSysParticle*p,uint count,Vector3*right,Vector3*up){
    for(uint i=0;i<count;++i){auto&a=p[i];float x=a.m_halfSize*(a.m_rightX+a.m_rightY),y=a.m_halfSize*(a.m_rightX-a.m_rightY);
        Vector3 b=*right*x-*up*y,c=*right*y+*up*x;v[4*i]=a.m_pos-b;v[4*i+1]=a.m_pos+c;v[4*i+2]=a.m_pos-c;v[4*i+3]=a.m_pos+b;}
}
void ParticleSys_BuildVertices_Stretch(Vector3*v,ParticleSysParticle*p,uint count,Vector3*forward,float*velocityScale,float*lengthScale){
    for(uint i=0;i<count;++i){auto&a=p[i];Vector3 side=nanZero(normalized(cross(a.m_vel,*forward))*a.m_halfSize);
        Vector3 along=nanZero(normalized(a.m_vel)*(length(a.m_vel)**velocityScale+*lengthScale*a.m_halfSize));
        v[4*i]=a.m_pos+side+along;v[4*i+1]=a.m_pos+side-along;v[4*i+2]=a.m_pos-side+along;v[4*i+3]=a.m_pos-side-along;}
}
void ParticleSys_BuildColours(Color*c,ParticleSysParticle*p,uint count,Color*animation){
    for(uint i=0;i<count;++i){float time=4.f-p[i].m_energy*4.f*p[i].m_startEnergyRecip;int index=std::max(0,std::min(int(time),3));float t=time-float(index);
        float*o=&c[i*4].r,*a=&animation[index].r,*b=&animation[index+1].r,*colour=&p[i].m_colour.r;
        for(int j=0;j<4;++j)o[j]=(a[j]+(b[j]-a[j])*t)*colour[j];c[4*i+1]=c[4*i+2]=c[4*i+3]=c[4*i];}
}
void RopeUpdate(Vector2*positions,Vector2*old,RopeUpdateParams*p){
    uint count=p->m_numParticles;if(count<2)return;
    for(uint i=0;i<count;++i)old[i]=(positions[i]+positions[i])-old[i];float weight=0.5f;
    if(p->m_straighten>0){float reciprocal=1.f/float(count-1);Vector2 step=(p->m_pos1-p->m_pos0)*reciprocal;
        float target=std::sqrt(dot(step,step))*0.8f;if(p->m_restLength>target)p->m_restLength+=(target-p->m_restLength)*p->m_straighten;
        float t=std::fmax(std::fmin(p->m_straighten,1.f),0.f);weight=0.5f-p->m_straighten*0.5f;
        Vector2 shift=p->m_pos0-positions[0],shiftStep=shift*reciprocal,targetPosition=p->m_pos0;
        for(uint i=0;i<count;++i){old[i]=lerp(old[i],targetPosition,t);positions[i]=lerp(positions[i],old[i]-shift,t);shift=shift-shiftStep;targetPosition=targetPosition+step;}}
    for(int iteration=0;iteration<6;++iteration){Vector2 tail=old[0];for(uint i=1;i<count;++i){Vector2 d=old[i]-tail;float scale=weight+weight*p->m_restLength*(-1.f/std::sqrt(dot(d,d)));d=d*scale;old[i-1]=tail+d;tail=old[i]-d;}
        if(p->m_straighten>0){Vector2 delta=tail-p->m_pos1;Vector2 fixed=p->m_pos1+delta*((1.f/std::sqrt(dot(delta,delta)))*p->m_radius1);tail={std::isnan(fixed.x)?delta.x:fixed.x,std::isnan(fixed.y)?delta.y:fixed.y};}
        else tail=positions[count-1];old[0]=p->m_pos0;old[count-1]=tail;}
}
void RopeUpdateMesh2(Vector3*v,Color32*c,Vector2*p,Vector2*old,RopeUpdateMeshParams*j){
    uint count=j->m_numParticles;if(count<2)return;uint end=std::min(uint(j->m_lengthProp*float(count)),count-1);
    Vector2 direction=p[count-1]-j->m_renderPos0;float s=(1.f/std::sqrt(dot(direction,direction)))*0.15f;Vector2 side{direction.y*s,-direction.x*s};
    float recip=1.f/float(count-1),factor=0.f,y=j->m_y0;Vector2 position=j->m_renderPos0,previous=position,shift=position-lerp(old[0],p[0],j->m_frameLerp);Color colour=j->m_colour,last=colour;float lastY=y;
    auto write=[&](uint i,Vector2 pos,float height,Color col){v[i*2]={pos.x-side.x,height,pos.y-side.y};v[i*2+1]={pos.x+side.x,height,pos.y+side.y};auto b=[](float f)->byte{return byte(clamp(f,0,1)*255.f);};c[i*2]=c[i*2+1]={b(col.r),b(col.g),b(col.b),b(col.a)};};
    for(uint i=0;i<end;++i){write(i,position,y,colour);previous=position;lastY=y;last=colour;factor+=recip;position=lerp(old[i+1],p[i+1],j->m_frameLerp)+(shift-shift*factor);y+=(j->m_y1-j->m_y0)*recip;
        colour.r+=recip*(1.f-j->m_colour.r);colour.g+=recip*(1.f-j->m_colour.g);colour.b+=recip*(1.f-j->m_colour.b);colour.a+=recip*(1.f-j->m_colour.a);}
    float t=j->m_lengthProp*float(count)-float(end);position=lerp(previous,position,t);y=lastY+(y-lastY)*t;
    colour={last.r+(colour.r-last.r)*t,last.g+(colour.g-last.g)*t,last.b+(colour.b-last.b)*t,last.a+(colour.a-last.a)*t};
    for(uint i=end;i<count;++i)write(i,position,y,colour);
}
void Streamers_BuildVertices(Vector3*v,Vector3*n,StreamerParticle*p,StreamerParticle*old,BuildVerticesParams*args){
    for(uint i=0;i<args->m_numParticles;++i){Vector3 pos=lerp(old[i].m_pos,p[i].m_pos,args->m_renderLerp),tangent=lerp(old[i].m_tangent,p[i].m_tangent,args->m_renderLerp),normal=lerp(old[i].m_normal,p[i].m_normal,args->m_renderLerp);
        v[2*i]=pos+tangent*args->m_halfWidth;v[2*i+1]=pos-tangent*args->m_halfWidth;n[2*i]=normal+tangent*args->m_normalBlend;n[2*i+1]=normal-tangent*args->m_normalBlend;}
}
void Streamers_UpdateParticles(StreamerParticle*out,StreamerParticle*old,Streamer*streamers,UpdateParticlesParams*p){
    Vector3 wind=p->m_wind*(1.f/std::sqrt(std::sqrt(dot(p->m_wind,p->m_wind))));
    for(uint s=0;s<p->m_numStreamers;++s){uint start=s*p->m_numParticlesPerStreamer;Vector3 previous{},previousNormal{};
        for(uint i=0;i<p->m_numParticlesPerStreamer;++i){auto&a=old[start+i];auto&b=out[start+i];
            Vector3 velocity=a.m_vel+wind*std::fabs(dot(a.m_normal,wind))+(i==0?p->m_accel:p->m_tailAccel);Vector3 pos=a.m_pos+velocity*p->m_timeStep,normal=a.m_normal;
            if(i==0){Vector3 axis=normalized(velocity);float angle=streamers[s].m_spinRate*(p->m_timeStep*0.01745329238474369f)*0.5f;float sine=std::sin(angle);Quaternion q{axis.x*sine,axis.y*sine,axis.z*sine,std::cos(angle)};QuaternionMul(&normal.x,&q);}
            else {Vector3 delta=previous-pos;float d2=dot(delta,delta);bool constrained=d2>=p->m_segmentDistanceTest*p->m_segmentDistanceTest;
                float scale=constrained?nanZero(p->m_constraintScale-p->m_constraintScale*p->m_segmentDistance*(1.f/std::sqrt(d2))):0.f;
                pos=pos+delta*scale;if(constrained)velocity=(delta*scale)*(1.f/p->m_timeStep);normal=lerp(normal,previousNormal,p->m_normalLerp);}
            Vector3 axis=normalized(velocity);normal=normalized(normal-axis*dot(normal,axis));b={pos,normal,cross(normal,axis),velocity};previous=pos;previousNormal=normal;}}
}
}
