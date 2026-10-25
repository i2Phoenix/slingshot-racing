using UnityEngine;

public class Shadow : MonoBehaviour
{
	private struct ShadowData
	{
		public float m_lightAngleY;

		public Vector3 m_lightDir;

		public Vector3 m_offset0;

		public Vector3 m_offset1;

		public Vector3 m_offset2;

		public Vector3 m_offset3;

		public float m_lightDirComp;

		public float m_lightDirCompRecip;

		public float m_trackHeight;

		public float m_height0;

		public float m_height1;

		public float m_height2;

		public float m_height3;

		public Color m_colour;
	}

	public static bool s_enableNightShadows = false;

	public static float s_lightVerticalAngle = 27.5f;

	public static float s_casterHalfWidth = 2.5f;

	public static float s_casterYMin = -1.25f;

	public static float s_casterYMax = 2f;

	public static int s_numAngleFrames = 64;

	public static int s_numAngleFramesPerRow = 16;

	public static int s_numObjects = 4;

	public static int s_numAngleFrameRows = Mathf.CeilToInt((float)s_numAngleFrames / (float)s_numAngleFramesPerRow);

	private static float s_numAngleFramesF = s_numAngleFrames;

	private static float s_numAngleFramesF360 = s_numAngleFramesF / 360f;

	private static float s_numAngleFramesPerRowF = s_numAngleFramesPerRow;

	private static float s_numAngleFramesPerRowFRecip = 1f / s_numAngleFramesPerRowF;

	private static int s_numRows = s_numAngleFrameRows * s_numObjects;

	private static float s_numRowsF = s_numRows;

	private static float s_numRowsFRecip = 1f / s_numRowsF;

	private static int s_numNightShadows = 2;

	private Transform m_transform;

	private ShadowManager m_manager;

	private TrackSpline m_trackSpline;

	private float m_trackT;

	private TrackHub m_trackHub;

	private int m_trackHubSector;

	private int m_index;

	private int m_distributor;

	private Color m_shadowBlend = Color.white;

	private bool m_renderEnable = true;

	private ShadowData[] m_shadowData;

	public int Index
	{
		set
		{
			m_index = value;
			m_distributor = value;
		}
	}

	public Renderer Renderer { get; set; }

	public Color Colour
	{
		get
		{
			return m_shadowData[0].m_colour;
		}
	}

	public bool RenderEnable
	{
		get
		{
			return m_renderEnable;
		}
		set
		{
			m_renderEnable = value;
		}
	}

	private void Awake()
	{
		m_manager = ShadowManager.Register();
		if (SceneSettings.RenderSettings.m_night && s_enableNightShadows)
		{
			for (int i = 1; i < s_numNightShadows; i++)
			{
				m_manager = ShadowManager.Register();
			}
		}
		m_transform = base.transform;
	}

	private void OnDestroy()
	{
		for (int i = 0; i < m_shadowData.Length; i++)
		{
			m_manager.Unregister();
		}
		m_manager = null;
		m_shadowData = null;
	}

	private void Start()
	{
		if (SceneSettings.RenderSettings.m_night && s_enableNightShadows)
		{
			m_shadowData = new ShadowData[s_numNightShadows];
		}
		else
		{
			m_shadowData = new ShadowData[1];
		}
		float lightAngleY = 0f;
		GameObject gameObject = GameObject.FindGameObjectWithTag("MainLight");
		if (gameObject != null)
		{
			lightAngleY = gameObject.transform.rotation.eulerAngles.y;
		}
		SetLightAngleY(lightAngleY, 0);
		SetBlend(m_shadowBlend);
	}

	private void SetLightAngleY(float _lightAngleY, int _index)
	{
		float x = s_lightVerticalAngle;
		m_shadowData[_index].m_lightAngleY = _lightAngleY;
		float z = 0f;
		m_shadowData[_index].m_lightDir = Quaternion.Euler(x, m_shadowData[_index].m_lightAngleY, z) * Vector3.forward;
		Vector3 normalized = Vector3.Cross(Vector3.up, m_shadowData[_index].m_lightDir).normalized;
		Vector3 normalized2 = Vector3.Cross(m_shadowData[_index].m_lightDir, normalized).normalized;
		Vector3 vector = normalized * s_casterHalfWidth;
		Vector3 vector2 = normalized2 * s_casterYMin;
		Vector3 vector3 = normalized2 * s_casterYMax;
		m_shadowData[_index].m_offset0 = -vector + vector3;
		m_shadowData[_index].m_offset1 = vector + vector3;
		m_shadowData[_index].m_offset2 = -vector + vector2;
		m_shadowData[_index].m_offset3 = vector + vector2;
		Vector3 vector4 = m_shadowData[_index].m_lightDir * s_casterHalfWidth;
		m_shadowData[_index].m_offset0 -= vector4;
		m_shadowData[_index].m_offset1 -= vector4;
		m_shadowData[_index].m_offset2 -= vector4;
		m_shadowData[_index].m_offset3 -= vector4;
		m_shadowData[_index].m_lightDirComp = m_shadowData[_index].m_lightDir.y;
		m_shadowData[_index].m_lightDirCompRecip = 1f / m_shadowData[_index].m_lightDirComp;
	}

	private void SetLightShadowColour(Color shadowColour, int _index)
	{
		m_shadowData[_index].m_colour = shadowColour;
	}

	public void SetBlend(Color shadowBlend)
	{
		m_shadowBlend = shadowBlend;
		if (m_shadowData != null)
		{
			SetLightShadowColour(shadowBlend, 0);
		}
	}

	public void SetTrackData(TrackSpline _trackSpline, float _trackT, TrackHub _trackHub, int _trackHubSector)
	{
		m_trackSpline = _trackSpline;
		m_trackT = _trackT;
		m_trackHub = _trackHub;
		m_trackHubSector = _trackHubSector;
		if (Renderer != null && !Renderer.isVisible)
		{
			m_distributor = -1;
			return;
		}
		m_distributor++;
		if (m_renderEnable)
		{
			for (int i = 0; i < m_shadowData.Length; i++)
			{
				AddShadow(ref m_shadowData[i]);
			}
		}
	}

	private void AddShadow(ref ShadowData shadowData)
	{
		Vector3[] o_vertices;
		Vector2[] o_uvs;
		Vector2[] o_uv;
		Color32[] o_colours;
		int num = m_manager.AddShadow(out o_vertices, out o_uvs, out o_uv, out o_colours);
		Vector3 position = m_transform.position;
		Vector3 vector = position + shadowData.m_offset0;
		Vector3 vector2 = position + shadowData.m_offset1;
		Vector3 vector3 = position + shadowData.m_offset2;
		Vector3 vector4 = position + shadowData.m_offset3;
		if (m_trackSpline != null || m_trackHub != null)
		{
			Vector3 o_point;
			Vector3 o_tangent;
			Vector3 o_normal;
			float o_width;
			if (m_trackSpline != null)
			{
				m_trackSpline.InfoAtT(m_trackT, out o_point, out o_tangent, out o_normal, out o_width);
			}
			else
			{
				m_trackHub.InfoAt(m_trackT, m_trackHubSector, out o_point, out o_tangent, out o_normal, out o_width);
			}
			float num2 = Vector3.Dot(o_point, o_normal);
			float num3 = Vector3.Dot(position, o_normal);
			float num4 = num3 - num2;
			float num5 = 1f / o_width;
			float widthProp = num4 * num5;
			if (m_trackSpline != null)
			{
				shadowData.m_trackHeight = m_trackSpline.HeightAt(m_trackT, widthProp);
			}
			else
			{
				shadowData.m_trackHeight = m_trackHub.HeightAt(widthProp);
			}
			Vector3 normalized = Vector3.Cross(o_tangent, o_normal).normalized;
			Vector3 vector5 = default(Vector3);
			vector5.x = o_point.x;
			vector5.y = o_point.y + shadowData.m_trackHeight;
			vector5.z = o_point.z;
			float num6 = Vector3.Dot(shadowData.m_lightDir, normalized);
			float num7 = 1f / num6;
			float num8 = Vector3.Dot(vector - vector5, normalized);
			vector -= shadowData.m_lightDir * (num8 * num7);
			float num9 = Vector3.Dot(vector2 - vector5, normalized);
			vector2 -= shadowData.m_lightDir * (num9 * num7);
			float num10 = Vector3.Dot(vector3 - vector5, normalized);
			vector3 -= shadowData.m_lightDir * (num10 * num7);
			float num11 = Vector3.Dot(vector4 - vector5, normalized);
			vector4 -= shadowData.m_lightDir * (num11 * num7);
			if ((m_distributor & 3) == 0)
			{
				float value = (Vector3.Dot(vector, o_normal) - num2) * num5;
				value = Mathf.Clamp(value, -1f, 1f);
				if (m_trackSpline != null)
				{
					shadowData.m_height0 = m_trackSpline.HeightAt(m_trackT, value);
				}
				else
				{
					shadowData.m_height0 = m_trackHub.HeightAt(value);
				}
				float value2 = (Vector3.Dot(vector2, o_normal) - num2) * num5;
				value2 = Mathf.Clamp(value2, -1f, 1f);
				if (m_trackSpline != null)
				{
					shadowData.m_height1 = m_trackSpline.HeightAt(m_trackT, value2);
				}
				else
				{
					shadowData.m_height1 = m_trackHub.HeightAt(value2);
				}
				float value3 = (Vector3.Dot(vector3, o_normal) - num2) * num5;
				value3 = Mathf.Clamp(value3, -1f, 1f);
				if (m_trackSpline != null)
				{
					shadowData.m_height2 = m_trackSpline.HeightAt(m_trackT, value3);
				}
				else
				{
					shadowData.m_height2 = m_trackHub.HeightAt(value3);
				}
				float value4 = (Vector3.Dot(vector4, o_normal) - num2) * num5;
				value4 = Mathf.Clamp(value4, -1f, 1f);
				if (m_trackSpline != null)
				{
					shadowData.m_height3 = m_trackSpline.HeightAt(m_trackT, value4);
				}
				else
				{
					shadowData.m_height3 = m_trackHub.HeightAt(value4);
				}
			}
			float num12 = shadowData.m_trackHeight - 0.1f;
			vector.y += shadowData.m_height0 - num12;
			vector2.y += shadowData.m_height1 - num12;
			vector3.y += shadowData.m_height2 - num12;
			vector4.y += shadowData.m_height3 - num12;
			o_vertices[num] = vector;
			o_vertices[num + 1] = vector2;
			o_vertices[num + 2] = vector3;
			o_vertices[num + 3] = vector4;
		}
		else
		{
			float y = position.y;
			o_vertices[num] += shadowData.m_lightDir * ((vector.y - y) * (0f - shadowData.m_lightDirCompRecip));
			o_vertices[num + 1] += shadowData.m_lightDir * ((vector2.y - y) * (0f - shadowData.m_lightDirCompRecip));
			o_vertices[num + 2] += shadowData.m_lightDir * ((vector3.y - y) * (0f - shadowData.m_lightDirCompRecip));
			o_vertices[num + 3] += shadowData.m_lightDir * ((vector4.y - y) * (0f - shadowData.m_lightDirCompRecip));
		}
		float num13 = m_transform.rotation.eulerAngles.y - shadowData.m_lightAngleY;
		if (num13 < 0f)
		{
			num13 += 360f;
		}
		num13 = num13 * s_numAngleFramesF360 + 0.5f;
		float num14 = Mathf.Floor(num13);
		float num15 = num14 + 1f;
		float a = num13 - num14;
		num14 %= s_numAngleFramesF;
		num15 %= s_numAngleFramesF;
		int num16 = m_index % s_numObjects;
		float f = num14 * s_numAngleFramesPerRowFRecip;
		f = Mathf.Floor(f) + (float)(num16 * s_numAngleFrameRows);
		num14 %= (float)s_numAngleFramesPerRow;
		float f2 = num15 * s_numAngleFramesPerRowFRecip;
		f2 = Mathf.Floor(f2) + (float)(num16 * s_numAngleFrameRows);
		num15 %= (float)s_numAngleFramesPerRow;
		float num17 = num14 * s_numAngleFramesPerRowFRecip;
		float num18 = num15 * s_numAngleFramesPerRowFRecip;
		float num19 = f * s_numRowsFRecip;
		float num20 = f2 * s_numRowsFRecip;
		float num21 = num17;
		float x = num21 + s_numAngleFramesPerRowFRecip;
		float num22 = 1f - num19;
		float y2 = num22 - s_numRowsFRecip;
		o_uvs[num].x = num21;
		o_uvs[num].y = num22;
		o_uvs[num + 1].x = x;
		o_uvs[num + 1].y = num22;
		o_uvs[num + 2].x = num21;
		o_uvs[num + 2].y = y2;
		o_uvs[num + 3].x = x;
		o_uvs[num + 3].y = y2;
		num21 = num18;
		x = num21 + s_numAngleFramesPerRowFRecip;
		num22 = 1f - num20;
		y2 = num22 - s_numRowsFRecip;
		o_uv[num].x = num21;
		o_uv[num].y = num22;
		o_uv[num + 1].x = x;
		o_uv[num + 1].y = num22;
		o_uv[num + 2].x = num21;
		o_uv[num + 2].y = y2;
		o_uv[num + 3].x = x;
		o_uv[num + 3].y = y2;
		Color colour = shadowData.m_colour;
		colour.a = a;
		o_colours[num] = colour;
		o_colours[num + 1] = colour;
		o_colours[num + 2] = colour;
		o_colours[num + 3] = colour;
	}
}
