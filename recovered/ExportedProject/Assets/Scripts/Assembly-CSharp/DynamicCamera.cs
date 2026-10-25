using System;
using UnityEngine;

[AddComponentMenu("Camera/Dynamic Camera")]
public class DynamicCamera : MainCamera
{
	public interface Vehicle
	{
		Vector3 Position { get; }

		float TrackForwardSpeed { get; }

		TrackSpline CurrentTrackSpline { get; }

		TrackHub CurrentTrackHub { get; }

		float CurrentT { get; }
	}

	private struct VehicleInfo
	{
		public Vehicle m_vehicle;

		public bool m_isPlayable;
	}

	private class VehicleInfoFind
	{
		private Vehicle m_vehicle;

		public VehicleInfoFind(Vehicle _vehicle)
		{
			m_vehicle = _vehicle;
		}

		public bool Predicate(VehicleInfo _vehicleInfo)
		{
			return _vehicleInfo.m_vehicle == m_vehicle;
		}
	}

	private struct PathPoint
	{
		public Vector3 position;

		public bool inner;
	}

	private const int c_maxClipPlanes = 6;

	private const int c_maxClipVertices = 4;

	private const int c_maxVehicles = 8;

	private const int c_initialPathPoints = 128;

	public new static DynamicCamera Get;

	private TouchInputManager m_touchInputManager;

	private Transform m_transform;

	private Camera m_camera;

	private PlayableArea m_playableArea;

	private PlayableArea[] m_playableAreas = new PlayableArea[2];

	private Vector3[] m_cameraPlanes = new Vector3[4];

	private Vector3[] m_clipPlanes = new Vector3[4];

	private float[] m_planeForceScale = new float[4];

	private float[] m_distHeightScale = new float[2];

	private VehicleInfo[] m_vehicles = new VehicleInfo[8];

	private float m_1pTilt;

	private float m_tanOuterFov;

	private float m_touchBlend;

	private float m_touchLeft = 1f;

	private float m_touchRight = 1f;

	private float m_touchLeftUse = 1f;

	private float m_touchRightUse = 1f;

	private float m_innerDist;

	private float m_vehicleSpeed;

	private int m_numVehicles;

	private int m_numPlayableVehicles;

	private Vector3 m_oldPosition2;

	private Vector3 m_oldPosition1;

	private Vector3 m_position;

	private Vector2 m_velocity;

	private float m_velocityY;

	private float m_desiredHeight;

	private int m_cachedNumVehicles;

	private int m_cachedNumPlayableVehicles;

	private float m_maxHeight;

	private float m_clampedMaxHeight;

	private float m_heightScale;

	private Vector3 m_centrePoint;

	private Vector3[] m_centrePoints = new Vector3[2];

	private float m_cachedAspect;

	private float m_cachedFov;

	private float m_verticalBorder;

	private float m_leftBorder;

	private float m_rightBorder;

	private float m_minLeftBorder;

	private float m_minRightBorder;

	private bool m_started;

	private bool m_raceLaunched = true;

	private Quaternion m_cachedRotation;

	private Vector3[] m_boundingCorners;

	private Vector3[][] m_boundingCorners2 = new Vector3[2][];

	private int m_currTraverseStamp;

	private PathPoint[] m_pathPoints = new PathPoint[128];

	private int m_numPathPoints;

	public float m_startLerpRate = 0.02f;

	public float m_quickStartLerpRate = 0.05f;

	public float m_minHeight = 25f;

	public float m_1pWideScreenHeightScale = 0.875f;

	public float m_zoomInSmoothing = 0.95f;

	public float m_zoomOutSmoothing = 0.9f;

	public float m_zoomSpring = 8f;

	public float m_zoomDamping = 0.1f;

	public float m_1pMaxHeight = 40f;

	public float m_1pSpeedAtMinLookAhead = 20f;

	public float m_1pSpeedAtMaxLookAhead = 40f;

	public float m_1pSpeedAtMinHeight = 20f;

	public float m_1pSpeedAtMaxHeight = 80f;

	public float m_globalDamping = 0.05f;

	public float m_boundsSpring = 10f;

	public float m_boundsDamping = 0.2f;

	public float m_1pPathSpring = 3f;

	public float m_pathSpring = 4f;

	public float m_pathExtraWidth = 1f;

	public float m_1pPathAheadDist = 60f;

	public float m_pathAheadDist = 45f;

	public float m_replayPathAheadDist = 45f;

	public float m_1pPathAroundDist = 10f;

	public float m_pathAroundDist = 10f;

	public float m_pathAheadStep = 10f;

	public float m_pathDamping = 0.2f;

	public float m_1pVerticalBorder = 0.16667f;

	public float m_mpVerticalBorder = 0.05f;

	public TouchInputManager TouchInputManager
	{
		get
		{
			return m_touchInputManager;
		}
		set
		{
			m_touchInputManager = value;
			m_minLeftBorder = 0f;
			m_minRightBorder = 0f;
			m_cachedFov = 0f;
			int numHumans = m_touchInputManager.NumHumans;
			if (numHumans > 1)
			{
				TouchInputManager.CornerMask cornerMask = TouchInputManager.CornerMask.None;
				for (int i = 0; i != numHumans; i++)
				{
					cornerMask |= m_touchInputManager.GetCornerMask(i);
				}
				if ((cornerMask & (TouchInputManager.CornerMask)5) != TouchInputManager.CornerMask.None)
				{
					m_minLeftBorder = 1f;
				}
				if ((cornerMask & (TouchInputManager.CornerMask)0xA) != TouchInputManager.CornerMask.None)
				{
					m_minRightBorder = 1f;
				}
			}
			float num = m_touchInputManager.m_visibleButtonSize * (2f / m_camera.aspect);
			m_minLeftBorder *= num;
			m_minRightBorder *= num;
		}
	}

	private new void Awake()
	{
		base.Awake();
		TimeManager.Init();
		if (Get == null)
		{
			Get = this;
		}
		m_transform = base.transform;
		m_camera = base.camera;
		m_1pSpeedAtMinHeight = Mathf.Max(1f, m_1pSpeedAtMinHeight);
		m_1pSpeedAtMaxHeight = Mathf.Max(m_1pSpeedAtMaxHeight, m_1pSpeedAtMinHeight);
		GameObject[] array = GameObject.FindGameObjectsWithTag("PlayableArea");
		GameObject[] array2 = array;
		foreach (GameObject gameObject in array2)
		{
			int num = ((gameObject.name == "PlayableAreaMP") ? 1 : 0);
			PlayableArea component = gameObject.GetComponent<PlayableArea>();
			m_playableAreas[num] = component;
			Vector3[] array3 = new Vector3[4]
			{
				new Vector3(-0.5f, 0f, 0.5f),
				new Vector3(-0.5f, 0f, -0.5f),
				new Vector3(0.5f, 0f, -0.5f),
				new Vector3(0.5f, 0f, 0.5f)
			};
			for (int j = 0; j != 4; j++)
			{
				array3[j] = component.transform.TransformPoint(array3[j]);
			}
			m_boundingCorners2[num] = array3;
		}
		if (m_playableAreas[1] == null)
		{
			m_playableAreas[1] = m_playableAreas[0];
			m_boundingCorners2[1] = m_boundingCorners2[0];
		}
		m_playableArea = m_playableAreas[0];
		m_boundingCorners = m_boundingCorners2[0];
		m_1pPathAheadDist *= m_playableArea.m_lookAheadScale;
		m_1pTilt = Mathf.Clamp(m_playableArea.m_1pCameraTilt, 0f, 30f);
		float p = Time.fixedDeltaTime * 50f;
		m_globalDamping = 1f - Mathf.Pow(1f - m_globalDamping, p);
		m_zoomDamping = 1f - Mathf.Pow(1f - m_zoomDamping, p);
		m_pathDamping = 1f - Mathf.Pow(1f - m_pathDamping, p);
		m_boundsDamping = 1f - Mathf.Pow(1f - m_boundsDamping, p);
		m_zoomInSmoothing = Mathf.Pow(m_zoomInSmoothing, p);
		m_zoomOutSmoothing = Mathf.Pow(m_zoomOutSmoothing, p);
		m_touchBlend = 1f - Mathf.Pow(0.9f, p);
		m_transform.position = Vector3.zero;
	}

	private new void OnDestroy()
	{
		base.OnDestroy();
		if (Get == this)
		{
			Get = null;
		}
	}

	private void Start()
	{
		UpdateCachedState();
		m_position = m_centrePoint;
		m_oldPosition1 = m_position;
		m_oldPosition2 = m_position;
		m_transform.position = m_position;
		Vector3 position = m_position;
		do
		{
			FixedUpdate();
		}
		while (m_velocity.magnitude > 0.5f || Mathf.Abs(m_velocityY) > 0.5f);
		m_transform.position = position;
		m_velocityY = 0f;
		m_velocity = Vector2.zero;
		m_started = true;
		UpdateCachedState();
	}

	public void PreRaceLaunch()
	{
		m_raceLaunched = false;
	}

	public void OnRaceLaunch()
	{
		m_raceLaunched = true;
	}

	public bool IsSphereInClipFrustum(Vector3 centre, float radius)
	{
		centre -= m_transform.position;
		for (int i = 0; i != 4; i++)
		{
			float num = Vector3.Dot(centre, m_clipPlanes[i]);
			if (num < 0f - radius)
			{
				return false;
			}
		}
		return true;
	}

	private void Update()
	{
		if (m_raceLaunched)
		{
			float frameProp = TimeManager.FrameProp;
			if (frameProp < 0f)
			{
				m_transform.position = Vector3Utils.LerpNoClamp(m_oldPosition2, m_oldPosition1, frameProp + 1f);
			}
			else
			{
				m_transform.position = Vector3Utils.LerpNoClamp(m_oldPosition1, m_position, frameProp);
			}
			if (m_numPlayableVehicles == 1 && m_touchInputManager != null)
			{
				m_touchLeft = m_touchInputManager.TouchLeft;
				m_touchRight = m_touchInputManager.TouchRight;
			}
		}
		else
		{
			m_transform.position = Vector3Utils.LerpNoClamp(m_transform.position, m_position, 1f - Mathf.Pow(1f - m_startLerpRate, TimeManager.DeltaTime * 50f));
		}
	}

	private void FixedUpdate()
	{
		if (m_numVehicles != 0 && (!m_started || m_raceLaunched))
		{
			if (m_numVehicles != m_cachedNumVehicles || m_numPlayableVehicles != m_cachedNumPlayableVehicles || m_camera.aspect != m_cachedAspect || m_camera.fov != m_cachedFov || m_transform.rotation != m_cachedRotation)
			{
				UpdateCachedState();
			}
			if (m_numPlayableVehicles == 1)
			{
				UpdateCameraPlanes();
			}
			m_velocity *= 1f - m_globalDamping;
			Vector2 zero = Vector2.zero;
			float o_desiredHeight;
			bool o_disableZoom;
			zero += ConstrainByVehiclePaths(out o_desiredHeight, out o_disableZoom);
			zero = ConstrainByPlayableArea(zero);
			if (m_numVehicles <= 1 && m_vehicles[0].m_vehicle != null)
			{
				float num = Mathf.Max(0f, m_vehicles[0].m_vehicle.TrackForwardSpeed);
				float value = m_minHeight + (m_1pMaxHeight - m_minHeight) * (num - m_1pSpeedAtMinHeight) / (m_1pSpeedAtMaxHeight - m_1pSpeedAtMinHeight);
				o_desiredHeight = m_vehicles[0].m_vehicle.Position.y + Mathf.Clamp(value, m_minHeight, m_clampedMaxHeight) * m_heightScale;
				m_vehicleSpeed = Mathf.Lerp(num, m_vehicleSpeed, m_zoomInSmoothing);
			}
			m_velocity += zero;
			Vector3 up = m_transform.up;
			Vector3 right = m_transform.right;
			up.y = 0f;
			right.y = 0f;
			up.Normalize();
			right.Normalize();
			if (o_disableZoom)
			{
				o_desiredHeight = Mathf.Max(o_desiredHeight, m_position.y);
			}
			float t = ((!(o_desiredHeight < m_desiredHeight)) ? m_zoomOutSmoothing : m_zoomInSmoothing);
			m_desiredHeight = Mathf.Lerp(o_desiredHeight, m_desiredHeight, t);
			m_velocityY *= 1f - m_zoomDamping;
			float num2 = m_desiredHeight - m_position.y;
			m_velocityY += m_zoomSpring * Time.fixedDeltaTime * num2;
			m_oldPosition2 = m_oldPosition1;
			m_oldPosition1 = m_position;
			m_position += right * m_velocity.x * Time.fixedDeltaTime;
			m_position += up * m_velocity.y * Time.fixedDeltaTime;
			m_position.y += m_velocityY * Time.fixedDeltaTime;
		}
	}

	private void UpdateCachedState()
	{
		float a = ((m_numPlayableVehicles > 1) ? 0f : m_1pTilt);
		a = Mathf.Min(a, 80f - m_camera.fov * 0.5f);
		a = Mathf.Min(a, m_camera.fov * 0.5f);
		m_transform.rotation = Quaternion.Euler(90f - a, m_playableArea.transform.eulerAngles.y, 0f);
		m_heightScale = ((m_numVehicles > 1 || !(m_camera.aspect >= 1.4999f)) ? 1f : m_1pWideScreenHeightScale);
		m_verticalBorder = ((m_numVehicles <= 1) ? m_1pVerticalBorder : m_mpVerticalBorder);
		m_verticalBorder = ((!m_started || m_numPlayableVehicles <= 0) ? 0f : m_verticalBorder);
		m_leftBorder = Mathf.Max(m_minLeftBorder, m_verticalBorder / m_camera.aspect);
		m_rightBorder = Mathf.Max(m_minRightBorder, m_verticalBorder / m_camera.aspect);
		float num = Mathf.Tan(m_camera.fov * ((float)Math.PI / 180f) * 0.5f);
		m_tanOuterFov = num * m_camera.aspect;
		float num2 = m_camera.fov * ((float)Math.PI / 180f) * 0.5f;
		float num3 = a * ((float)Math.PI / 180f);
		float num4 = Mathf.Tan(num2 + num3);
		float num5 = Mathf.Tan(num2 - num3);
		float num6 = (num4 + num5) * 0.5f * m_verticalBorder;
		num4 -= num6;
		num5 = Mathf.Max(0f, num5 - num6);
		float num7 = m_tanOuterFov - m_tanOuterFov * m_verticalBorder / m_camera.aspect;
		Vector3 up = m_transform.up;
		up.y = 0f;
		up.Normalize();
		Vector3 right = m_transform.right;
		float num8 = float.MaxValue;
		float num9 = float.MinValue;
		float num10 = float.MaxValue;
		float num11 = float.MinValue;
		for (int i = 0; i != 4; i++)
		{
			float b = Vector3.Dot(m_boundingCorners[i], up);
			float b2 = Vector3.Dot(m_boundingCorners[i], right);
			num8 = Mathf.Min(num8, b);
			num9 = Mathf.Max(num9, b);
			num10 = Mathf.Min(num10, b2);
			num11 = Mathf.Max(num11, b2);
		}
		m_centrePoint = new Vector3(0f, (num9 - num8) / (num4 + num5), 0f);
		m_centrePoint += (num10 + num11) * 0.5f * right;
		m_centrePoint += (num8 + m_centrePoint.y * num5) * up;
		m_centrePoints[1] = m_centrePoint;
		Vector3 forward = m_transform.forward;
		m_centrePoint += forward * (m_centrePoint.y / (0f - forward.y) * 0.33f);
		for (int j = 0; j != 4; j++)
		{
			Vector3 lhs = m_boundingCorners[j] - m_centrePoint;
			float num12 = Vector3.Dot(lhs, forward);
			float num13 = Vector3.Dot(lhs, right);
			if (num13 > num7 * num12)
			{
				m_centrePoint += forward * (num12 - num13 / num7);
			}
		}
		m_centrePoints[0] = m_centrePoint;
		m_centrePoint = ((!(m_centrePoints[0].y > m_centrePoints[1].y)) ? m_centrePoints[1] : m_centrePoints[0]);
		m_maxHeight = m_centrePoint.y;
		m_clampedMaxHeight = ((m_numVehicles > 1) ? m_maxHeight : Mathf.Min(m_maxHeight, m_1pMaxHeight));
		float num14 = Mathf.Atan(num4);
		float num15 = Mathf.Atan(num5);
		m_planeForceScale[2] = 1f / Mathf.Cos(num14);
		m_planeForceScale[3] = 1f / Mathf.Cos(num15);
		m_cameraPlanes[2] = Quaternion.AngleAxis((num3 + num15) * 57.29578f, m_transform.right) * m_transform.up;
		m_cameraPlanes[3] = Quaternion.AngleAxis((num3 - num14) * 57.29578f, m_transform.right) * -m_transform.up;
		m_distHeightScale[1] = Mathf.Tan(num14) + Mathf.Tan(num15);
		float num16 = m_camera.fov * 0.5f;
		float num17 = num16 * m_camera.aspect;
		m_clipPlanes[0] = Quaternion.AngleAxis(0f - num17, m_transform.up) * m_transform.right;
		m_clipPlanes[1] = Quaternion.AngleAxis(num17, m_transform.up) * -m_transform.right;
		m_clipPlanes[2] = Quaternion.AngleAxis(num16, m_transform.right) * m_transform.up;
		m_clipPlanes[3] = Quaternion.AngleAxis(0f - num16, m_transform.right) * -m_transform.up;
		if (m_numPlayableVehicles != m_cachedNumPlayableVehicles)
		{
			m_touchLeft = 1f;
			m_touchRight = 1f;
		}
		UpdateCameraPlanes();
		m_cachedAspect = m_camera.aspect;
		m_cachedFov = m_camera.fov;
		m_cachedRotation = m_transform.rotation;
		m_cachedNumVehicles = m_numVehicles;
		m_cachedNumPlayableVehicles = m_numPlayableVehicles;
	}

	private void UpdateCameraPlanes()
	{
		m_touchLeftUse = Mathf.Lerp(m_touchLeftUse, m_touchLeft, m_touchBlend);
		m_touchRightUse = Mathf.Lerp(m_touchRightUse, m_touchRight, m_touchBlend);
		float num = m_tanOuterFov * (m_touchLeftUse - m_leftBorder);
		float num2 = m_tanOuterFov * (m_touchRightUse - m_rightBorder);
		float num3 = Mathf.Atan(num);
		float num4 = Mathf.Atan(num2);
		m_planeForceScale[0] = 1f / Mathf.Cos(num3);
		m_planeForceScale[1] = 1f / Mathf.Cos(num4);
		m_cameraPlanes[0] = Quaternion.AngleAxis((0f - num3) * 57.29578f, m_transform.up) * m_transform.right;
		m_cameraPlanes[1] = Quaternion.AngleAxis(num4 * 57.29578f, m_transform.up) * -m_transform.right;
		m_distHeightScale[0] = num + num2;
	}

	private Vector2 ConstrainByVehiclePaths(out float o_desiredHeight, out bool o_disableZoom)
	{
		float spread = 0f;
		float pathAheadDist = m_pathAheadDist;
		float pathAroundDist = m_pathAroundDist;
		Vehicle vehicle = m_vehicles[0].m_vehicle;
		bool flag = false;
		if (m_numVehicles > 1)
		{
			float num = float.MaxValue;
			for (int i = 0; i != m_numVehicles; i++)
			{
				Vehicle vehicle2 = m_vehicles[i].m_vehicle;
				float num2 = float.MinValue;
				for (int j = 0; j != m_numVehicles; j++)
				{
					if (j != i)
					{
						float distanceAhead = GetDistanceAhead(m_vehicles[j].m_vehicle, vehicle2);
						if (distanceAhead > num2)
						{
							num2 = distanceAhead;
						}
					}
				}
				if (num2 < num)
				{
					num = num2;
					vehicle = vehicle2;
					flag = m_vehicles[i].m_isPlayable;
				}
			}
			spread = num;
			if (m_numPlayableVehicles > 0)
			{
				if (!flag)
				{
					pathAroundDist = 0f;
				}
			}
			else
			{
				pathAheadDist = m_replayPathAheadDist;
			}
		}
		else
		{
			pathAheadDist = m_1pPathAheadDist * Mathf.Max(m_vehicleSpeed, m_1pSpeedAtMinLookAhead) / m_1pSpeedAtMaxLookAhead;
			pathAroundDist = m_1pPathAroundDist;
		}
		return ConstrainByVehiclePath(out o_desiredHeight, out o_disableZoom, vehicle, pathAheadDist, pathAroundDist, spread);
	}

	private Vector2 ConstrainByVehiclePath(out float o_desiredHeight, out bool o_disableZoom, Vehicle _vehicle, float _pathAheadDist, float _pathAroundDist, float _spread)
	{
		o_disableZoom = false;
		o_desiredHeight = 0f;
		m_innerDist = _pathAroundDist + m_pathAheadStep + _spread;
		ProjectVehiclePath(_vehicle, _pathAroundDist, _pathAheadDist + _spread, m_pathAheadStep);
		if (m_numPathPoints == 0)
		{
			return Vector2.zero;
		}
		Vector2 zero = Vector2.zero;
		float y = m_position.y;
		for (int i = 0; i != 2; i++)
		{
			float num = float.MaxValue;
			float num2 = float.MaxValue;
			float a = float.MaxValue;
			float a2 = float.MaxValue;
			for (int j = 0; j != m_numPathPoints; j++)
			{
				float b = Vector3.Dot(m_cameraPlanes[i * 2], m_pathPoints[j].position);
				float b2 = Vector3.Dot(m_cameraPlanes[i * 2 + 1], m_pathPoints[j].position);
				if (m_pathPoints[j].inner)
				{
					num = Mathf.Min(num, b);
					num2 = Mathf.Min(num2, b2);
				}
				else
				{
					a = Mathf.Min(a, b);
					a2 = Mathf.Min(a2, b2);
				}
			}
			a = Mathf.Min(a, num);
			a2 = Mathf.Min(a2, num2);
			a *= m_planeForceScale[i * 2];
			a2 *= m_planeForceScale[i * 2 + 1];
			float num3 = ((m_numVehicles < 1) ? m_1pPathSpring : m_pathSpring);
			float num4 = num3 * (a - a2) * Time.fixedDeltaTime;
			if (num < num2)
			{
				if (num < 0f)
				{
					if (m_numVehicles <= 1)
					{
						if (m_velocity[i] > 0f)
						{
							int index2;
							int index = (index2 = i);
							float num5 = m_velocity[index2];
							m_velocity[index] = num5 * (1f - m_pathDamping);
						}
						num4 = Mathf.Min(0f, num4);
					}
					o_disableZoom = true;
				}
			}
			else if (num2 < 0f)
			{
				if (m_numVehicles <= 1)
				{
					if (m_velocity[i] < 0f)
					{
						int index2;
						int index3 = (index2 = i);
						float num5 = m_velocity[index2];
						m_velocity[index3] = num5 * (1f - m_pathDamping);
					}
					num4 = Mathf.Max(0f, num4);
				}
				o_disableZoom = true;
			}
			zero[i] = num4;
			float num6 = y * m_distHeightScale[i];
			o_desiredHeight = Mathf.Clamp(y * (num6 - (a + a2)) / num6, o_desiredHeight, m_maxHeight);
		}
		return zero;
	}

	private Vector2 ConstrainByPlayableArea(Vector2 deltaVelocity)
	{
		float num = Time.fixedDeltaTime * m_boundsSpring;
		float num2 = 1f - m_boundsDamping;
		for (int i = 0; i != 2; i++)
		{
			Vector3 lhs = m_position - m_centrePoints[i];
			float num3 = (0f - Vector3.Dot(lhs, m_cameraPlanes[i * 2])) * m_planeForceScale[i * 2];
			float num4 = (0f - Vector3.Dot(lhs, m_cameraPlanes[i * 2 + 1])) * m_planeForceScale[i * 2 + 1];
			if (!(num3 > 0f) && !(num4 > 0f))
			{
				continue;
			}
			num3 *= m_planeForceScale[i * 2];
			num4 *= m_planeForceScale[i * 2 + 1];
			float num5 = deltaVelocity[i];
			float num6 = m_velocity[i];
			num3 = Mathf.Max(num3, 0f);
			num4 = Mathf.Max(num4, 0f);
			float num7 = num4 - num3;
			if (num7 >= 0f)
			{
				if (num5 > 0f)
				{
					num5 -= num5 * Mathf.Min(1f, num7 / 5f);
				}
				if (num6 > 0f)
				{
					num6 *= num2;
				}
			}
			else
			{
				if (num5 < 0f)
				{
					num5 -= num5 * Mathf.Min(1f, (0f - num7) / 5f);
				}
				if (num6 < 0f)
				{
					num6 *= num2;
				}
			}
			num5 -= num7 * num;
			deltaVelocity[i] = num5;
			m_velocity[i] = num6;
		}
		return deltaVelocity;
	}

	private float GetDistanceAheadHub(TrackHub _hub, Vehicle _to)
	{
		if (_to.CurrentTrackHub == _hub || _hub.TraverseStamp == m_currTraverseStamp)
		{
			return 0f;
		}
		_hub.TraverseStamp = m_currTraverseStamp;
		float num = float.MaxValue;
		for (int i = 0; i != _hub.NumSectors; i++)
		{
			if (_hub.IsSectorOutgoing(i))
			{
				TrackSpline spline = _hub.SplineAtSector(i);
				float distanceAheadSpline = GetDistanceAheadSpline(spline, 0f, _to);
				num = Mathf.Min(num, distanceAheadSpline);
			}
		}
		return _hub.Radius * 2f + num;
	}

	private float GetDistanceAheadSpline(TrackSpline _spline, float _startT, Vehicle _to)
	{
		if (_to.CurrentTrackSpline == _spline && _startT <= _to.CurrentT)
		{
			return _spline.ReParamTtoS(_to.CurrentT) - _spline.ReParamTtoS(_startT);
		}
		float num = ((!(_spline.HubAtTail != null)) ? GetDistanceAheadSpline(_spline, 0f, _to) : GetDistanceAheadHub(_spline.HubAtTail, _to));
		return _spline.Length - _spline.ReParamTtoS(_startT) + num;
	}

	private float GetDistanceAhead(Vehicle _to, Vehicle _from)
	{
		m_currTraverseStamp++;
		TrackSpline currentTrackSpline = _from.CurrentTrackSpline;
		if (currentTrackSpline != null)
		{
			return GetDistanceAheadSpline(currentTrackSpline, _from.CurrentT, _to);
		}
		TrackHub currentTrackHub = _from.CurrentTrackHub;
		if (currentTrackHub != null)
		{
			return GetDistanceAheadHub(currentTrackHub, _to);
		}
		return 0f;
	}

	private float GetRelativeDistance(Vehicle _vehicle, Vehicle _relativeTo)
	{
		float distanceAhead = GetDistanceAhead(_vehicle, _relativeTo);
		float distanceAhead2 = GetDistanceAhead(_relativeTo, _vehicle);
		return (!(distanceAhead < distanceAhead2)) ? (0f - distanceAhead2) : distanceAhead;
	}

	private void ProjectVehiclePath(Vehicle _vehicle, float _lookBehindDist, float _lookAheadDist, float _step)
	{
		TrackSpline currentTrackSpline = _vehicle.CurrentTrackSpline;
		TrackHub currentTrackHub = _vehicle.CurrentTrackHub;
		_step = Mathf.Max(1f, Mathf.Floor(_step));
		m_numPathPoints = 0;
		if (currentTrackSpline != null)
		{
			float num = currentTrackSpline.ReParamTtoS(_vehicle.CurrentT);
			float num2 = Mathf.Min(_lookBehindDist, num);
			HandleSpline(currentTrackSpline, _lookBehindDist > 0f, num - num2, _step, 0f - num2, _lookAheadDist + num2);
		}
		if (currentTrackHub != null)
		{
			float currentT = _vehicle.CurrentT;
			float num3 = Mathf.Min(_lookBehindDist, currentT);
			HandleHub(currentTrackHub, currentT - num3, _step, 0f - num3, _lookAheadDist + num3);
		}
	}

	private void HandleSpline(TrackSpline _spline, bool noQuantise, float splineDistance, float increment, float currDistance, float remainingDistance)
	{
		while (true)
		{
			if (splineDistance > _spline.Length)
			{
				float num = splineDistance - _spline.Length;
				if (_spline.HubAtTail != null)
				{
					HandleHub(_spline.HubAtTail, num, increment, currDistance, remainingDistance);
				}
				else
				{
					HandleSpline(_spline, noQuantise, num, increment, currDistance, remainingDistance);
				}
				break;
			}
			if (remainingDistance > increment && !noQuantise)
			{
				float num2 = Mathf.Floor(splineDistance / increment) * increment;
				remainingDistance += splineDistance - num2;
				splineDistance = num2;
			}
			noQuantise = false;
			float t = _spline.ReParamStoT(splineDistance);
			Vector3 o_point;
			Vector3 o_tangent;
			Vector3 o_normal;
			float o_width;
			_spline.InfoAtT(t, out o_point, out o_tangent, out o_normal, out o_width);
			Vector3 vector = o_normal * (o_width + m_pathExtraWidth);
			AddPathPoint(o_point + vector, currDistance);
			AddPathPoint(o_point - vector, currDistance);
			if (remainingDistance <= 0f)
			{
				break;
			}
			increment = Mathf.Min(increment, remainingDistance);
			splineDistance += increment;
			currDistance += increment;
			remainingDistance -= increment;
		}
	}

	private void HandleHub(TrackHub _hub, float _hubDistance, float _increment, float _currDistance, float _remainingDistance)
	{
		float num = 2f * _hub.Radius;
		if (_hubDistance < num)
		{
			Vector3 position = _hub.transform.position;
			Vector3 vector = new Vector3(_hub.Radius, 0f, _hub.Radius);
			AddPathPoint(position + vector, _currDistance);
			AddPathPoint(position - vector, _currDistance);
			if (_remainingDistance > 0f)
			{
				float num2 = Mathf.Min(_increment, _remainingDistance);
				HandleHub(_hub, _hubDistance + num2, num2, _currDistance + num2, _remainingDistance - num2);
			}
			return;
		}
		float splineDistance = _hubDistance - num;
		for (int i = 0; i < _hub.NumSectors; i++)
		{
			if (_hub.IsSectorOutgoing(i))
			{
				HandleSpline(_hub.SplineAtSector(i), false, splineDistance, _increment, _currDistance, _remainingDistance);
			}
		}
	}

	private void AddPathPoint(Vector3 _point, float _dist)
	{
		if (m_numPathPoints >= m_pathPoints.Length)
		{
			PathPoint[] array = new PathPoint[m_pathPoints.Length * 2];
			m_pathPoints.CopyTo(array, 0);
			m_pathPoints = array;
		}
		m_pathPoints[m_numPathPoints].position = _point - m_position;
		m_pathPoints[m_numPathPoints].inner = _dist < m_innerDist;
		m_numPathPoints++;
	}

	public void TrackObject(Vehicle _vehicle, bool _isPlayable)
	{
		int num = m_numVehicles++;
		m_vehicles[num].m_vehicle = _vehicle;
		m_vehicles[num].m_isPlayable = _isPlayable;
		if (_isPlayable)
		{
			m_numPlayableVehicles++;
		}
		int num2 = ((m_numVehicles != 1) ? 1 : 0);
		m_playableArea = m_playableAreas[num2];
		m_boundingCorners = m_boundingCorners2[num2];
	}

	public void UntrackObject(Vehicle _vehicle)
	{
		VehicleInfoFind vehicleInfoFind = new VehicleInfoFind(_vehicle);
		int num = Array.FindIndex(m_vehicles, vehicleInfoFind.Predicate);
		if (num >= 0)
		{
			if (m_vehicles[num].m_isPlayable)
			{
				m_numPlayableVehicles--;
			}
			int num2 = --m_numVehicles;
			m_vehicles[num] = m_vehicles[num2];
			m_vehicles[num2] = default(VehicleInfo);
		}
	}
}
