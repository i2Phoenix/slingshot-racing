using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

[AddComponentMenu("Graphics/Particle System")]
public class ParticleSys : MonoBehaviour
{
	public enum OverrideRenderMode
	{
		NoOverride = 0,
		SolidStretch = 100
	}

	private BypassRenderer m_renderer;

	private BypassMaterial m_material;

	private BypassMesh m_mesh;

	private int m_maxParticlesCheck;

	private ParticleSysParticle[] m_particles;

	private int m_numParticles;

	private bool m_maintainIds;

	private Dictionary<int, int> m_idToIndexMap;

	private int m_nextId;

	private ParticleEmitter m_particleEmitter;

	private ParticleAnimator m_particleAnimator;

	private ParticleRenderer m_particleRenderer;

	private Rigidbody m_rigidbody;

	private bool m_animateColour;

	private Color[] m_colourAnimation;

	private int m_numFramesX = 1;

	private int m_numFrames = 1;

	private float m_frameScale = 1f;

	private float m_numFramesXRecip = 1f;

	private float m_numFramesYRecip = 1f;

	private float m_emitTime;

	private Camera m_camera;

	private bool m_coloursUpdated;

	private Vector3[] m_vertices;

	private Vector2[] m_texCoords;

	private Color[] m_colours;

	public GameObject m_unityParticlePrefab;

	public int m_maxParticles;

	public bool m_combineColours;

	public OverrideRenderMode m_overrideRenderMode;

	public float m_angularVelocity;

	public Vector3 m_ellipsoid;

	private void Start()
	{
		if (m_unityParticlePrefab != null)
		{
			m_particleEmitter = m_unityParticlePrefab.GetComponent<ParticleEmitter>();
			m_particleAnimator = m_unityParticlePrefab.GetComponent<ParticleAnimator>();
			m_particleRenderer = m_unityParticlePrefab.GetComponent<ParticleRenderer>();
		}
		else
		{
			m_particleEmitter = GetComponent<ParticleEmitter>();
			m_particleAnimator = GetComponent<ParticleAnimator>();
			m_particleRenderer = GetComponent<ParticleRenderer>();
			if (m_particleEmitter != null)
			{
				m_particleEmitter.enabled = false;
			}
			if (m_particleRenderer != null)
			{
				m_particleRenderer.enabled = false;
			}
		}
		Transform parent = base.transform;
		m_rigidbody = parent.rigidbody;
		while (m_rigidbody == null && parent.parent != null)
		{
			m_rigidbody = parent.parent.rigidbody;
			parent = parent.parent;
		}
		if (m_particleAnimator != null && m_particleAnimator.doesAnimateColor)
		{
			m_animateColour = true;
			m_colourAnimation = m_particleAnimator.colorAnimation;
		}
		if (m_particleRenderer != null)
		{
			m_numFramesX = m_particleRenderer.uvAnimationXTile;
			m_numFrames = m_particleRenderer.uvAnimationXTile * m_particleRenderer.uvAnimationYTile;
			m_frameScale = (float)m_numFrames * m_particleRenderer.uvAnimationCycles;
			m_numFramesXRecip = 1f / (float)m_numFramesX;
			m_numFramesYRecip = 1f / (float)m_particleRenderer.uvAnimationYTile;
		}
		if (m_particleRenderer != null)
		{
			Material sharedMaterial = m_particleRenderer.sharedMaterial;
			m_material = ((!sharedMaterial) ? null : new BypassMaterial(sharedMaterial));
		}
		if (m_maxParticles <= 0 && m_particleEmitter != null)
		{
			m_maxParticles = Mathf.CeilToInt(m_particleEmitter.maxEnergy * m_particleEmitter.maxEmission) + 1;
		}
		m_particles = new ParticleSysParticle[m_maxParticles];
		if (m_maintainIds)
		{
			m_idToIndexMap = new Dictionary<int, int>();
		}
		uint maxParticles = (uint)m_maxParticles;
		uint num = maxParticles * 4;
		m_mesh = new BypassMesh();
		m_vertices = new Vector3[num];
		m_texCoords = new Vector2[num];
		m_colours = new Color[num];
		if (m_numFrames == 1)
		{
			uint num2 = 0u;
			for (int i = 0; i < m_maxParticles; i++)
			{
				m_texCoords[num2].x = 0f;
				m_texCoords[num2].y = 1f;
				num2++;
				m_texCoords[num2].x = 1f;
				m_texCoords[num2].y = 1f;
				num2++;
				m_texCoords[num2].x = 0f;
				m_texCoords[num2].y = 0f;
				num2++;
				m_texCoords[num2].x = 1f;
				m_texCoords[num2].y = 0f;
				num2++;
			}
			m_mesh.uv = m_texCoords;
		}
		ushort[] array = new ushort[maxParticles * 6];
		for (uint num3 = 0u; num3 != maxParticles; num3++)
		{
			array[num3 * 6] = (ushort)(num3 * 4);
			array[num3 * 6 + 1] = (ushort)(num3 * 4 + 1);
			array[num3 * 6 + 2] = (ushort)(num3 * 4 + 2);
			array[num3 * 6 + 3] = (ushort)(num3 * 4 + 2);
			array[num3 * 6 + 4] = (ushort)(num3 * 4 + 1);
			array[num3 * 6 + 5] = (ushort)(num3 * 4 + 3);
		}
		m_mesh.triangles = array;
		m_maxParticlesCheck = m_maxParticles;
	}

	public int Emit(Vector3 _pos, Vector3 _vel, float _size, float _energy, Color _colour, float _rotation, float _angularVelocity)
	{
		if (m_numParticles >= m_maxParticlesCheck)
		{
			return -1;
		}
		int numParticles = m_numParticles;
		m_particles[numParticles].m_pos = _pos;
		m_particles[numParticles].m_vel = _vel;
		m_particles[numParticles].m_energy = _energy;
		m_particles[numParticles].m_rotation = _rotation * ((float)Math.PI / 180f);
		m_particles[numParticles].m_angularVelocity = _angularVelocity * ((float)Math.PI / 180f);
		m_particles[numParticles].m_colour = ((!m_combineColours) ? Color.white : _colour);
		m_particles[numParticles].m_startEnergyRecip = 1f / _energy;
		m_particles[numParticles].m_halfSize = _size * 0.5f;
		m_particles[numParticles].m_rightX = Mathf.Cos(m_particles[numParticles].m_rotation);
		m_particles[numParticles].m_rightY = 0f - Mathf.Sin(m_particles[numParticles].m_rotation);
		if (!m_animateColour)
		{
			uint num = (uint)(numParticles * 4);
			m_colours[num] = _colour;
			num++;
			m_colours[num] = _colour;
			num++;
			m_colours[num] = _colour;
			num++;
			m_colours[num] = _colour;
			m_coloursUpdated = true;
		}
		m_numParticles++;
		if (m_maintainIds)
		{
			int nextId = m_nextId;
			m_idToIndexMap.Add(nextId, numParticles);
			m_nextId++;
			return nextId;
		}
		return -1;
	}

	public bool SetParticleData(int _id, Vector3 _pos)
	{
		int value = -1;
		if (m_idToIndexMap.TryGetValue(_id, out value) && value != -1)
		{
			m_particles[value].m_pos = _pos;
			return true;
		}
		return false;
	}

	private void LateUpdate()
	{
		Camera camera = MainCamera.Get;
		if (camera == null)
		{
			Camera[] allCameras = Camera.allCameras;
			if (allCameras.Length > 0)
			{
				camera = allCameras[0];
			}
		}
		if (camera != m_camera)
		{
			m_camera = camera;
			m_renderer = BypassRendererBase.Get(m_camera);
		}
		if (m_material == null || !(m_camera != null))
		{
			return;
		}
		float deltaTime = Time.deltaTime;
		LateUpdate_EmitParticles(deltaTime);
		LateUpdate_UpdateParticles(deltaTime);
		LateUpdate_BuildVertices();
		if (m_numFrames != 1)
		{
			LateUpdate_BuildUVs();
		}
		if (m_animateColour)
		{
			LateUpdate_BuildColours();
			m_coloursUpdated = true;
		}
		if (m_numParticles > 0)
		{
			m_mesh.vertices = m_vertices;
			if (m_numFrames != 1)
			{
				m_mesh.uv = m_texCoords;
			}
			if (m_coloursUpdated)
			{
				m_mesh.colors = m_colours;
				m_coloursUpdated = false;
			}
			m_renderer.DrawRange(m_mesh, m_material, 0u, (uint)(m_numParticles * 6));
		}
	}

	private void LateUpdate_EmitParticles(float timeStep)
	{
		if (!(m_particleEmitter != null) || !m_particleEmitter.emit)
		{
			return;
		}
		m_emitTime -= timeStep;
		while (m_emitTime <= 0f)
		{
			Vector3 position = base.transform.position;
			Vector3 insideUnitSphere = UnityEngine.Random.insideUnitSphere;
			insideUnitSphere.x *= m_ellipsoid.x;
			insideUnitSphere.y *= m_ellipsoid.y;
			insideUnitSphere.z *= m_ellipsoid.z;
			position += insideUnitSphere;
			Vector3 worldVelocity = m_particleEmitter.worldVelocity;
			worldVelocity += base.transform.TransformDirection(m_particleEmitter.localVelocity);
			Vector3 insideUnitSphere2 = UnityEngine.Random.insideUnitSphere;
			insideUnitSphere2.x *= m_particleEmitter.rndVelocity.x;
			insideUnitSphere2.y *= m_particleEmitter.rndVelocity.y;
			insideUnitSphere2.z *= m_particleEmitter.rndVelocity.z;
			worldVelocity += insideUnitSphere2;
			if (m_rigidbody != null)
			{
				position += m_rigidbody.velocity * (0f - m_emitTime);
				worldVelocity += m_rigidbody.velocity * m_particleEmitter.emitterVelocityScale;
			}
			float size = UnityEngine.Random.Range(m_particleEmitter.minSize, m_particleEmitter.maxSize);
			float energy = UnityEngine.Random.Range(m_particleEmitter.minEnergy, m_particleEmitter.maxEnergy);
			float rotation = 0f;
			if (m_particleEmitter.rndRotation)
			{
				rotation = UnityEngine.Random.Range(0f, 360f);
			}
			float angularVelocity = m_angularVelocity;
			angularVelocity += UnityEngine.Random.Range(0f - m_particleEmitter.rndAngularVelocity, m_particleEmitter.rndAngularVelocity);
			Emit(position, worldVelocity, size, energy, Color.white, rotation, angularVelocity);
			m_emitTime += 1f / ((m_particleEmitter.minEmission + m_particleEmitter.maxEmission) * 0.5f);
		}
	}

	private void LateUpdate_UpdateParticles(float timeStep)
	{
		if (m_numParticles == 0)
		{
			return;
		}
		float num = 1f;
		float num2 = 1f;
		Vector3 force = Vector3.zero;
		if (m_particleAnimator != null)
		{
			num = Mathf.Pow(m_particleAnimator.damping, timeStep);
			num2 = Mathf.Pow(1f + m_particleAnimator.sizeGrow, timeStep);
			force = m_particleAnimator.force;
		}
		if (!m_maintainIds)
		{
			m_numParticles = (int)ParticleSys_UpdateParticles(ref m_particles[0], (uint)m_numParticles, timeStep, num, num2, ref force);
			return;
		}
		Vector3 vector = force * timeStep;
		for (int i = 0; i < m_numParticles; i++)
		{
			m_particles[i].m_energy -= timeStep;
			if (m_particles[i].m_energy <= 0f)
			{
				if (m_maintainIds)
				{
					if (i < m_numParticles - 1)
					{
						int num3 = m_numParticles - 1;
						m_particles[i] = m_particles[num3];
						foreach (KeyValuePair<int, int> item in m_idToIndexMap)
						{
							if (item.Value == i)
							{
								int key = item.Key;
								m_idToIndexMap.Remove(key);
								break;
							}
						}
						foreach (KeyValuePair<int, int> item2 in m_idToIndexMap)
						{
							if (item2.Value == num3)
							{
								int key2 = item2.Key;
								m_idToIndexMap.Remove(key2);
								m_idToIndexMap.Add(key2, i);
								break;
							}
						}
						i--;
					}
					else
					{
						foreach (KeyValuePair<int, int> item3 in m_idToIndexMap)
						{
							if (item3.Value == i)
							{
								int key3 = item3.Key;
								m_idToIndexMap.Remove(key3);
								break;
							}
						}
					}
					m_numParticles--;
				}
				else
				{
					if (i < m_numParticles - 1)
					{
						m_particles[i] = m_particles[m_numParticles - 1];
						i--;
					}
					m_numParticles--;
				}
			}
			else
			{
				m_particles[i].m_vel += vector;
				m_particles[i].m_vel *= num;
				m_particles[i].m_pos += m_particles[i].m_vel * timeStep;
				m_particles[i].m_halfSize *= num2;
				if (m_particles[i].m_angularVelocity != 0f)
				{
					m_particles[i].m_rotation += m_particles[i].m_angularVelocity * timeStep;
					m_particles[i].m_rightX = Mathf.Cos(m_particles[i].m_rotation);
					m_particles[i].m_rightY = 0f - Mathf.Sin(m_particles[i].m_rotation);
				}
			}
		}
	}

	private void LateUpdate_BuildVertices()
	{
		int num = 0;
		if (m_overrideRenderMode != OverrideRenderMode.NoOverride)
		{
			num = (int)m_overrideRenderMode;
		}
		else if (m_particleRenderer != null && (m_particleRenderer.particleRenderMode == ParticleRenderMode.Stretch || m_particleRenderer.particleRenderMode == ParticleRenderMode.HorizontalBillboard))
		{
			num = (int)m_particleRenderer.particleRenderMode;
		}
		switch (num)
		{
		case 0:
		case 4:
		{
			bool flag = num == 4;
			Vector3 v = Vector3.right;
			Vector3 v2 = Vector3.up;
			if (!flag)
			{
				Quaternion q = m_camera.transform.rotation;
				QuaternionUtils.QuaternionMul(ref v, ref q);
				QuaternionUtils.QuaternionMul(ref v2, ref q);
			}
			ParticleSys_BuildVertices_Billboard(out m_vertices[0], ref m_particles[0], (uint)m_numParticles, ref v, ref v2);
			break;
		}
		case 3:
		{
			float velocityScale = m_particleRenderer.velocityScale * 0.5f;
			float lengthScale2 = m_particleRenderer.lengthScale;
			Vector3 v3 = Vector3.forward;
			Quaternion q2 = m_camera.transform.rotation;
			QuaternionUtils.QuaternionMul(ref v3, ref q2);
			ParticleSys_BuildVertices_Stretch(out m_vertices[0], ref m_particles[0], (uint)m_numParticles, ref v3, ref velocityScale, ref lengthScale2);
			break;
		}
		case 100:
		{
			float num2 = m_particleRenderer.velocityScale * 0.5f;
			float lengthScale = m_particleRenderer.lengthScale;
			Vector3 position = m_camera.transform.position;
			uint num3 = 0u;
			for (int i = 0; i < m_numParticles; i++)
			{
				Vector3 vector = Vector3.Normalize(m_particles[i].m_pos - position);
				Vector3 vel = m_particles[i].m_vel;
				float magnitude = vel.magnitude;
				vel *= 1f / magnitude;
				float num4 = magnitude * num2 + lengthScale * m_particles[i].m_halfSize;
				Vector3 vector2 = Vector3.Normalize(Vector3.Cross(vel, vector));
				vel *= num4;
				Vector3 vector3 = Vector3.Cross(vector, vector2);
				float num5 = Vector3.Dot(vel, vector3);
				if (num5 < m_particles[i].m_halfSize)
				{
					vel += vector3 * (m_particles[i].m_halfSize - num5);
				}
				vector2 *= m_particles[i].m_halfSize;
				m_vertices[num3] = m_particles[i].m_pos + vector2 + vel;
				m_vertices[num3 + 1] = m_particles[i].m_pos + vector2 - vel;
				m_vertices[num3 + 2] = m_particles[i].m_pos - vector2 + vel;
				m_vertices[num3 + 3] = m_particles[i].m_pos - vector2 - vel;
				num3 += 4;
			}
			break;
		}
		}
	}

	private void LateUpdate_BuildUVs()
	{
		uint num = 0u;
		for (int i = 0; i < m_numParticles; i++)
		{
			float num2 = 1f - m_particles[i].m_energy * m_particles[i].m_startEnergyRecip;
			int num3 = Mathf.FloorToInt(num2 * m_frameScale);
			num3 %= m_numFrames;
			int num4 = num3 % m_numFramesX;
			int num5 = num3 / m_numFramesX;
			float num6 = (float)num4 * m_numFramesXRecip;
			float num7 = 1f - (float)num5 * m_numFramesYRecip;
			float x = num6 + m_numFramesXRecip;
			float y = num7 - m_numFramesYRecip;
			m_texCoords[num].x = num6;
			m_texCoords[num].y = num7;
			m_texCoords[num + 1].x = x;
			m_texCoords[num + 1].y = num7;
			m_texCoords[num + 2].x = num6;
			m_texCoords[num + 2].y = y;
			m_texCoords[num + 3].x = x;
			m_texCoords[num + 3].y = y;
			num += 4;
		}
	}

	private void LateUpdate_BuildColours()
	{
		if (m_numParticles > 0)
		{
			ParticleSys_BuildColours(out m_colours[0], ref m_particles[0], (uint)m_numParticles, ref m_colourAnimation[0]);
		}
	}

	public void MaintainIds()
	{
		m_maintainIds = true;
	}

	[DllImport("GrappleRacerNative")]
	private static extern void ParticleSys_BuildVertices_Billboard(out Vector3 o_vertices, ref ParticleSysParticle particles, uint numParticles, ref Vector3 cameraRight, ref Vector3 cameraUp);

	[DllImport("GrappleRacerNative")]
	private static extern void ParticleSys_BuildVertices_Stretch(out Vector3 o_vertices, ref ParticleSysParticle particles, uint numParticles, ref Vector3 cameraForward, ref float velocityScale, ref float lengthScale);

	[DllImport("GrappleRacerNative")]
	private static extern void ParticleSys_BuildColours(out Color o_colours, ref ParticleSysParticle _particles, uint _numParticles, ref Color _colourAnimation);

	[DllImport("GrappleRacerNative")]
	private static extern uint ParticleSys_UpdateParticles(ref ParticleSysParticle _particles, uint numParticles, float timeStep, float velScale, float sizeScale, ref Vector3 force);
}
