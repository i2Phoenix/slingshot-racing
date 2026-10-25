#pragma once
#include <cstdint>
using uint=uint32_t; using byte=uint8_t; using ushort=uint16_t;
struct Vector2 { float x; float y; };
struct Vector3 { float x; float y; float z; };
struct Color { float r; float g; float b; float a; };
struct Quaternion { float x; float y; float z; float w; };
struct Color32 { byte r; byte g; byte b; byte a; };
struct ColourRGBA32 { byte r; byte g; byte b; byte a; };
struct TrackNodeCache { Vector3 m_position; Vector3 m_tangent; Vector3 m_up; float m_width; };
struct ReParamNode { float s; float t; float dsdt; float dtds; };
struct RopeUpdateParams { uint m_numParticles; float m_restLength; float m_straighten; float m_radius1; Vector2 m_pos0; Vector2 m_pos1; };
struct RopeUpdateMeshParams { uint m_numParticles; float m_frameLerp; float m_y0; float m_y1; float m_lengthProp; Vector2 m_renderPos0; Color m_colour; };
struct ParticleSysParticle { Vector3 m_pos; Vector3 m_vel; float m_energy; float m_rotation; float m_angularVelocity; Color m_colour; float m_startEnergyRecip; float m_halfSize; float m_rightX; float m_rightY; };
struct UpdateParticlesParams { uint m_numStreamers; uint m_numParticlesPerStreamer; Vector3 m_accel; Vector3 m_tailAccel; Vector3 m_wind; float m_timeStep; float m_normalLerp; float m_segmentDistance; float m_segmentDistanceTest; float m_constraintScale; };
struct Streamer { Color m_colour; float m_spinRate; };
struct StreamerParticle { Vector3 m_pos; Vector3 m_normal; Vector3 m_tangent; Vector3 m_vel; };
struct BuildVerticesParams { uint m_numParticles; float m_renderLerp; float m_halfWidth; float m_normalBlend; };

extern "C" {
void TrackUtils_SplinePoint(Vector3* o_ret, Vector3* p0, Vector3* m0, Vector3* p1, Vector3* m1, float* unitT);
void TrackUtils_SplineTangent(Vector3* o_ret, Vector3* p0, Vector3* m0, Vector3* p1, Vector3* m1, float* unitT);
void TrackUtils_SplineTangentDT(Vector3* o_ret, Vector3* p0, Vector3* m0, Vector3* p1, Vector3* m1, float* unitT);
void TrackUtils_SplineUp(Vector3* o_ret, Vector3* u0, Vector3* u1, float* unitT);
void TrackUtils_SplineNormal(Vector3* o_ret, Vector3* p0, Vector3* m0, Vector3* u0, Vector3* p1, Vector3* m1, Vector3* u1, float* unitT);
void TrackUtils_SplineNormal2(Vector3* o_ret, Vector3* tangent, Vector3* up);
void TrackUtils_SplineWidth(float* o_ret, float* w0, float* w1, float* unitT);
void TrackUtils_SplineRoofHeight(float* o_ret, float* h0, float* h1, float* unitT);
void TrackUtils_SplineDsDt(float* o_ret, Vector3* p0, Vector3* m0, Vector3* p1, Vector3* m1, float* unitT);
void TrackUtils_SplineCurve(float* o_ret, Vector3* p0, Vector3* m0, Vector3* u0, Vector3* p1, Vector3* m1, Vector3* u1, float* unitT);
void TrackUtils_PointAtT(Vector3* ret, TrackNodeCache* nodes, int numNodes, float t);
void TrackUtils_TangentAtT(Vector3* ret, TrackNodeCache* nodes, int numNodes, float t);
void TrackUtils_TangentDTAtT(Vector3* ret, TrackNodeCache* nodes, int numNodes, float t);
void TrackUtils_UpAtT(Vector3* ret, TrackNodeCache* nodes, int numNodes, float t);
void TrackUtils_NormalAtT(Vector3* ret, TrackNodeCache* nodes, int numNodes, float t);
void TrackUtils_DsDtAtT(float* ret, TrackNodeCache* nodes, int numNodes, float t);
void TrackUtils_CurveAtT(float* ret, TrackNodeCache* nodes, int numNodes, float t);
void TrackUtils_InfoAtT(TrackNodeCache* _nodes, int _numNodes, float _t, Vector3* o_point, Vector3* o_tangent, Vector3* o_normal, float* o_width);
float TrackUtils_ClosestTToPoint(TrackNodeCache* nodes, int numNodes, Vector3* point, float tHint, float tHintSpread, float tLowerBound, float tUpperBound);
float TrackUtils_ReParamStoT(ReParamNode* nodes, float clampedS);
float TrackUtils_ReParamTtoS(ReParamNode* nodes, float clampedT);
void QuaternionMul(float* v, Quaternion* q);
void ParticleSys_BuildVertices_Billboard(Vector3* o_vertices, ParticleSysParticle* particles, uint numParticles, Vector3* cameraRight, Vector3* cameraUp);
void ParticleSys_BuildVertices_Stretch(Vector3* o_vertices, ParticleSysParticle* particles, uint numParticles, Vector3* cameraForward, float* velocityScale, float* lengthScale);
void ParticleSys_BuildColours(Color* o_colours, ParticleSysParticle* _particles, uint _numParticles, Color* _colourAnimation);
uint ParticleSys_UpdateParticles(ParticleSysParticle* _particles, uint numParticles, float timeStep, float velScale, float sizeScale, Vector3* force);
bool Platform_IsSimdSupported();
void Platform_DisableSimd(bool disable);
void Streamers_UpdateParticles(StreamerParticle* o_particles, StreamerParticle* particles, Streamer* streamers, UpdateParticlesParams* p);
void Streamers_BuildVertices(Vector3* o_vertices, Vector3* o_normals, StreamerParticle* particles, StreamerParticle* particlesOld, BuildVerticesParams* p);
void RopeUpdate(Vector2* particles, Vector2* oldParticles, RopeUpdateParams* _params);
void RopeUpdateMesh2(Vector3* vertices, Color32* colours, Vector2* positions, Vector2* oldPositions, RopeUpdateMeshParams* job);
void CubicHermite1D(float* ret, float p0, float m0, float p1, float m1, float t);
void CubicHermite(Vector3* ret, Vector3* p0, Vector3* m0, Vector3* p1, Vector3* m1, float t);
void CubicHermiteDT(Vector3* ret, Vector3* p0, Vector3* m0, Vector3* p1, Vector3* m1, float t);
void CubicHermiteDT2(Vector3* ret, Vector3* p0, Vector3* m0, Vector3* p1, Vector3* m1, float t);
}
