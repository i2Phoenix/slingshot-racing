using System.Runtime.InteropServices;
using StreamerStructs;
using UnityEngine;

[AddComponentMenu("Graphics/Streamers")]
public class Streamers : MonoBehaviour
{
	private Camera m_streamerCamera;

	private Camera m_camera;

	private BypassRenderer m_renderer;

	private BypassMaterial m_material;

	private BypassMesh m_mesh;

	private uint m_numIndices;

	private Streamer[] m_streamers;

	private StreamerParticle[] m_particles;

	private StreamerParticle[] m_particlesOld;

	private Vector3[] m_vertices;

	private Vector3[] m_normals;

	private float m_fixedTimeStep = 1f / 30f;

	private float m_timeLeft;

	private float m_renderLerp;

	public int m_numStreamers = 16;

	public int m_numParticlesPerStreamer = 16;

	public float m_segmentDistance = 2f;

	public float m_streamerWidth = 2f;

	public float m_updateRate = 30f;

	public Vector3 m_velocityRandomEllipse = Vector3.zero;

	public float m_velocityYAdd = 20f;

	public bool m_absVelZ;

	public float m_particleVelocityDistributionPower = 2f;

	public float m_spinRate = 360f;

	public float m_spinRandom;

	public float m_constraintScale = 1f;

	public bool m_incompressibleConstraint;

	public float m_normalLerp = 0.15f;

	public float m_gravity = -30f;

	public float m_tailGravityScale = 1f;

	public Vector3 m_wind = Vector3.zero;

	public float m_renderNormalBend = 0.5f;

	public Color[] m_streamerColours;

	public bool m_renderInGUI = true;

	private void Start()
	{
		GameObject gameObject = GameObject.Find("Streamer Camera");
		if (gameObject == null)
		{
			gameObject = new GameObject("Streamer Camera");
			gameObject.AddComponent<Camera>();
		}
		m_streamerCamera = gameObject.GetComponent<Camera>();
		Renderer component = GetComponent<Renderer>();
		if (component != null)
		{
			Material sharedMaterial = component.sharedMaterial;
			m_material = ((!sharedMaterial) ? null : new BypassMaterial(sharedMaterial));
		}
		Vector3 position = base.transform.position;
		base.transform.position = Vector3.zero;
		base.transform.rotation = Quaternion.identity;
		m_fixedTimeStep = 1f / m_updateRate;
		m_streamers = new Streamer[m_numStreamers];
		m_particles = new StreamerParticle[m_numStreamers * m_numParticlesPerStreamer];
		m_particlesOld = new StreamerParticle[m_numStreamers * m_numParticlesPerStreamer];
		for (int i = 0; i < m_streamers.Length; i++)
		{
			bool flag = false;
			if (m_streamerColours != null && m_streamerColours.Length > 0)
			{
				m_streamers[i].m_colour = m_streamerColours[Random.Range(0, m_streamerColours.Length)];
				flag = true;
			}
			if (!flag)
			{
				float num = 2f;
				float value = Random.value;
				num -= value;
				float num2 = Random.value * num;
				num -= num2;
				float b = num;
				m_streamers[i].m_colour = new Color(value, num2, b, 1f);
			}
			m_streamers[i].m_spinRate = m_spinRate + (Random.value - 0.5f) * m_spinRandom;
			Vector3 zero = Vector3.zero;
			float num3 = (float)i / (float)m_streamers.Length;
			float num4 = (float)(i + 1) / (float)m_streamers.Length;
			zero.x = Random.value * (num4 - num3) + num3;
			zero.x = zero.x * 2f - 1f;
			bool flag2 = false;
			int num5 = 6;
			do
			{
				float num6 = Mathf.Sqrt(Mathf.Max(1f - zero.x * zero.x, 1E-06f));
				Vector2 vector = Random.insideUnitCircle * num6;
				zero.y = vector.x;
				zero.z = vector.y;
				flag2 = new Vector2(zero.x, zero.z).magnitude > 0.05f;
				num5--;
			}
			while (!flag2 && num5 > 0);
			if (m_absVelZ)
			{
				zero.z = 0f - Mathf.Abs(zero.z);
			}
			zero.x *= m_velocityRandomEllipse.x;
			zero.y *= m_velocityRandomEllipse.y;
			zero.z *= m_velocityRandomEllipse.z;
			zero.y += m_velocityYAdd;
			Vector3 normalized = Vector3.Cross(zero.normalized, Vector3.right).normalized;
			Vector3 normalized2 = Vector3.Cross(normalized, zero.normalized).normalized;
			Vector2 normalized3 = Random.insideUnitCircle.normalized;
			Vector3 normal = normalized * normalized3.x + normalized2 * normalized3.y;
			int num7 = i * m_numParticlesPerStreamer;
			int num8 = 0;
			while (num8 != m_numParticlesPerStreamer)
			{
				float num9 = (float)num8 / (float)(m_numParticlesPerStreamer - 1);
				m_particles[num7].m_pos = position;
				m_particles[num7].m_vel = zero * Mathf.Pow(1f - num9, m_particleVelocityDistributionPower);
				m_particles[num7].m_normal = normal;
				m_particles[num7].m_tangent = Vector3.zero;
				m_particlesOld[num7] = m_particles[num7];
				num8++;
				num7++;
			}
		}
		uint num10 = (uint)(m_numParticlesPerStreamer - 1);
		uint num11 = (uint)(m_numStreamers * m_numParticlesPerStreamer * 2);
		uint num12 = (uint)m_numStreamers * num10;
		m_numIndices = num12 * 6;
		m_mesh = new BypassMesh();
		m_vertices = new Vector3[num11];
		m_normals = new Vector3[num11];
		Vector2[] array = new Vector2[num11];
		uint num13 = 0u;
		for (int j = 0; j < m_streamers.Length; j++)
		{
			for (int k = 0; k < m_numParticlesPerStreamer; k++)
			{
				float y = (float)k / (float)(m_numParticlesPerStreamer - 1);
				array[num13].x = 0f;
				array[num13].y = y;
				num13++;
				array[num13].x = 1f;
				array[num13].y = y;
				num13++;
			}
		}
		m_mesh.uv = array;
		ColourRGBA32[] array2 = new ColourRGBA32[num11];
		uint num14 = 0u;
		for (int l = 0; l < m_streamers.Length; l++)
		{
			ColourRGBA32 colourRGBA = m_streamers[l].m_colour;
			for (int m = 0; m < m_numParticlesPerStreamer; m++)
			{
				array2[num14] = colourRGBA;
				array2[num14 + 1] = colourRGBA;
				num14 += 2;
			}
		}
		m_mesh.colors32 = array2;
		ushort[] array3 = new ushort[m_numIndices];
		int num15 = 0;
		int num16 = 0;
		for (int n = 0; n < m_numStreamers; n++)
		{
			for (uint num17 = 0u; num17 != num10; num17++)
			{
				array3[num15] = (ushort)num16;
				num15++;
				array3[num15] = (ushort)(num16 + 2);
				num15++;
				array3[num15] = (ushort)(num16 + 1);
				num15++;
				array3[num15] = (ushort)(num16 + 1);
				num15++;
				array3[num15] = (ushort)(num16 + 2);
				num15++;
				array3[num15] = (ushort)(num16 + 3);
				num15++;
				num16 += 2;
			}
			num16 += 2;
		}
		m_mesh.triangles = array3;
	}

	private void LateUpdate()
	{
		Camera camera = null;
		if (m_renderInGUI)
		{
			if (UIManager.Exists() && UIManager.instance.uiCameras.Length > 0 && (bool)m_streamerCamera)
			{
				m_streamerCamera.CopyFrom(UIManager.instance.uiCameras[0].camera);
				m_streamerCamera.cullingMask = 0;
				m_streamerCamera.depth--;
				camera = m_streamerCamera;
			}
		}
		else
		{
			camera = Camera.main;
			if (camera == null && Camera.allCameras != null)
			{
				camera = ((Camera.allCameras.Length <= 0) ? null : Camera.allCameras[0]);
			}
		}
		if (camera != m_camera)
		{
			m_camera = camera;
			m_renderer = BypassRendererBase.Get(m_camera);
		}
		if (m_material != null && m_camera != null)
		{
			float deltaTime = Time.deltaTime;
			m_timeLeft += deltaTime;
			int num = 2;
			while (m_timeLeft >= m_fixedTimeStep && num > 0)
			{
				LateUpdate_UpdateParticles();
				m_timeLeft -= m_fixedTimeStep;
				num--;
			}
			if (m_timeLeft >= m_fixedTimeStep)
			{
				int num2 = Mathf.FloorToInt(m_timeLeft / m_fixedTimeStep);
				m_timeLeft -= (float)num2 * m_fixedTimeStep;
			}
			m_renderLerp = m_timeLeft / m_fixedTimeStep;
			if (m_streamers.Length > 0)
			{
				LateUpdate_BuildVertices();
				m_mesh.vertices = m_vertices;
				m_mesh.normals = m_normals;
				m_renderer.DrawRange(m_mesh, m_material, 0u, m_numIndices);
			}
		}
	}

	private void LateUpdate_UpdateParticles()
	{
		UpdateParticlesParams p = default(UpdateParticlesParams);
		p.m_numStreamers = (uint)m_numStreamers;
		p.m_numParticlesPerStreamer = (uint)m_numParticlesPerStreamer;
		p.m_accel = new Vector3(0f, m_gravity * m_fixedTimeStep, 0f);
		p.m_tailAccel = p.m_accel * m_tailGravityScale;
		p.m_wind = m_wind * m_fixedTimeStep;
		p.m_timeStep = m_fixedTimeStep;
		p.m_normalLerp = m_normalLerp;
		p.m_segmentDistance = m_segmentDistance;
		p.m_segmentDistanceTest = ((!m_incompressibleConstraint) ? m_segmentDistance : 0f);
		p.m_constraintScale = m_constraintScale;
		Streamers_UpdateParticles(out m_particlesOld[0], ref m_particles[0], ref m_streamers[0], ref p);
		StreamerParticle[] particlesOld = m_particlesOld;
		m_particlesOld = m_particles;
		m_particles = particlesOld;
	}

	private void LateUpdate_BuildVertices()
	{
		BuildVerticesParams p = default(BuildVerticesParams);
		p.m_numParticles = (uint)(m_numStreamers * m_numParticlesPerStreamer);
		p.m_renderLerp = m_renderLerp;
		p.m_halfWidth = m_streamerWidth * 0.5f;
		p.m_normalBlend = m_renderNormalBend;
		Streamers_BuildVertices(out m_vertices[0], out m_normals[0], ref m_particles[0], ref m_particlesOld[0], ref p);
	}

	[DllImport("GrappleRacerNative")]
	private static extern void Streamers_UpdateParticles(out StreamerParticle o_particles, ref StreamerParticle particles, ref Streamer streamers, ref UpdateParticlesParams p);

	[DllImport("GrappleRacerNative")]
	private static extern void Streamers_BuildVertices(out Vector3 o_vertices, out Vector3 o_normals, ref StreamerParticle particles, ref StreamerParticle particlesOld, ref BuildVerticesParams p);
}
