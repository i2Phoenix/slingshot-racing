using System.Collections;
using UnityEngine;
using VehicleTypes;

public class GhostVehicle : Vehicle
{
	private Transform m_transform;

	private Vector3 m_velocity;

	private Vector3 m_prevPosition1;

	private Vector3 m_prevPosition2;

	private Quaternion m_prevRotation1 = Quaternion.identity;

	private Quaternion m_prevRotation2 = Quaternion.identity;

	private float m_prevDeltaT;

	private int m_distributor;

	private Vector3[] m_wheelPositions = new Vector3[2];

	public override bool IsLocalHuman
	{
		get
		{
			return false;
		}
	}

	public override bool ScaleAudioVolume
	{
		get
		{
			return base.IsGhost;
		}
	}

	public override bool UsesHeadlights
	{
		get
		{
			return !base.IsGhost;
		}
	}

	public override float OilSlickTime
	{
		get
		{
			return 0f;
		}
	}

	public override Vector3 GrappleFrontPos
	{
		get
		{
			Vector3 position = m_position;
			position += m_forward * 1f;
			return position + m_up * -0.2f;
		}
	}

	public override Vector3 Velocity
	{
		get
		{
			return m_velocity;
		}
	}

	private void Awake()
	{
		m_transform = base.transform;
	}

	private void Start()
	{
		m_visuals = GetComponentInChildren<VehicleVisuals>();
		if (m_visuals != null)
		{
			m_visuals.Init(this);
			m_visuals.transform.localPosition = new Vector3(0f, -0.8f, 0f);
		}
		m_wheelPositions[0] = new Vector3(0.8f, -0.9f, 1.2f);
		m_wheelPositions[1] = new Vector3(-0.8f, -0.9f, 1.2f);
		m_position = m_transform.position;
		m_rotation = m_transform.rotation;
		m_forward = QuaternionUtils.Mul00Z(m_rotation, 1f);
		m_up = QuaternionUtils.Mul0Y0(m_rotation, 1f);
		m_right = QuaternionUtils.MulX00(m_rotation, 1f);
		m_currentTrackSpline = TrackSpline.FindClosestToPoint(m_transform.position);
		m_currentTrackHub = null;
		m_currentHubSector = int.MaxValue;
		if (m_currentTrackSpline != null)
		{
			Vector3 point = m_position + m_forward * 1.8f;
			m_currentT = m_currentTrackSpline.ClosestTTo(point);
		}
		RepeatFrame();
		RepeatFrame();
	}

	public void RepeatFrame()
	{
		m_prevPosition2 = m_prevPosition1;
		m_prevRotation2 = m_prevRotation1;
		m_prevPosition1 = m_position;
		m_prevRotation1 = m_rotation;
	}

	public void NewFrame(Vector3 _position, Quaternion _rotation, Vector3 _velocity, GrapplePoint _grapplePoint, float _grappleForce, TrackSpline _currentTrackSpline, TrackHub _currentTrackHub, int _waypoint, bool _reset)
	{
		if (_reset)
		{
			m_prevPosition2 = (m_prevPosition1 = _position);
			m_prevRotation2 = (m_prevRotation1 = _rotation);
		}
		else
		{
			m_prevPosition2 = m_prevPosition1;
			m_prevRotation2 = m_prevRotation1;
			m_prevPosition1 = m_position;
			m_prevRotation1 = m_rotation;
		}
		if (m_currentTrackSpline != _currentTrackSpline || m_currentTrackHub != _currentTrackHub)
		{
			m_prevDeltaT = 0f;
			if (_currentTrackSpline != null)
			{
				if (m_currentTrackHub != null)
				{
					if (m_currentTrackHub.IsSectorOutgoing(m_currentHubSector))
					{
						m_currentT = 0f;
					}
					else
					{
						m_currentT = _currentTrackSpline.MaxT;
					}
				}
				else
				{
					m_currentT = 0f;
				}
				m_currentHubSector = int.MaxValue;
			}
			else if (_currentTrackHub != null)
			{
				Vector3 point = m_position + m_forward * 1.8f;
				_currentTrackHub.ClosestInfoTo(point, out m_currentT, out m_currentHubSector);
			}
			else
			{
				m_currentT = 0f;
				m_currentHubSector = int.MaxValue;
			}
		}
		GrapplePoint grapplePoint = m_grapplePoint;
		m_position = _position;
		m_rotation = _rotation;
		m_velocity = _velocity;
		m_grapplePoint = _grapplePoint;
		m_currentGrappleForce = _grappleForce;
		m_currentTrackSpline = _currentTrackSpline;
		m_currentTrackHub = _currentTrackHub;
		m_waypoint = _waypoint;
		m_forward = QuaternionUtils.Mul00Z(_rotation, 1f);
		m_up = QuaternionUtils.Mul0Y0(_rotation, 1f);
		m_right = QuaternionUtils.MulX00(_rotation, 1f);
		if (_grapplePoint != null)
		{
			if (grapplePoint == null)
			{
				m_visuals.StartGrapple(m_grapplePoint, GrappleFrontPos, _velocity);
				SendGrappleEvent(_grapplePoint);
			}
		}
		else if (grapplePoint != null)
		{
			SendGrappleReleaseEvent(grapplePoint);
		}
	}

	public void SendLapEvent(float _lapTime)
	{
		m_lapTime -= _lapTime;
		SendLapEvent(this, _lapTime);
	}

	public void StopPlayback(float _time)
	{
		StartCoroutine(FadeAndHide(_time));
	}

	private void Update()
	{
		float frameProp = TimeManager.FrameProp;
		if (frameProp < 0f)
		{
			m_transform.rotation = Quaternion.Lerp(m_prevRotation2, m_prevRotation1, frameProp + 1f);
			m_transform.position = Vector3Utils.LerpNoClamp(m_prevPosition2, m_prevPosition1, frameProp + 1f);
		}
		else
		{
			m_transform.rotation = Quaternion.Lerp(m_prevRotation1, m_rotation, frameProp);
			m_transform.position = Vector3Utils.LerpNoClamp(m_prevPosition1, m_position, frameProp);
		}
		if (m_visuals != null)
		{
			m_visuals.VehicleUpdate(frameProp);
		}
	}

	private void FixedUpdate()
	{
		m_distributor++;
		UpdateTrackLocation();
		UpdateTrackEdgeData();
		if (m_visuals != null)
		{
			m_visuals.VehicleFixedUpdate(m_distributor);
		}
		m_lapTime += Time.fixedDeltaTime;
	}

	private void UpdateTrackLocation()
	{
		Vector3 point = m_position + m_forward * 1.8f;
		if (m_currentTrackSpline != null)
		{
			if (float.IsNaN(m_prevDeltaT))
			{
				m_currentT = m_currentTrackSpline.ClosestTTo(point);
				m_prevDeltaT = 0f;
			}
			else
			{
				float tHint = m_currentT + m_prevDeltaT;
				float tHintSpread = Mathf.Clamp(Mathf.Abs(m_prevDeltaT), 0.05f, 0.5f);
				float num = m_currentTrackSpline.ClosestTTo(point, tHint, tHintSpread, m_currentT - 0.5f, m_currentT + 0.5f);
				m_prevDeltaT = num - m_currentT;
				float maxT = m_currentTrackSpline.MaxT;
				if (num > maxT)
				{
					if (m_currentTrackSpline.HubAtTail == null)
					{
						m_currentT = num - maxT;
					}
					else
					{
						m_currentT = maxT;
					}
				}
				else if (num < 0f)
				{
					if (m_currentTrackSpline.HubAtHead == null)
					{
						m_currentT = num + maxT;
					}
					else
					{
						m_currentT = 0f;
					}
				}
				else
				{
					m_currentT = num;
				}
			}
			m_currentTrackSpline.InfoAtT(m_currentT, out m_trackPoint, out m_trackTangent, out m_trackNormal, out m_trackWidth);
			Vector3Utils.Normalise(ref m_trackTangent);
			m_trackCentreHeight = m_currentTrackSpline.HeightAt(m_currentT, 0f);
		}
		else if (m_currentTrackHub != null)
		{
			m_currentTrackHub.ClosestInfoTo(point, out m_currentT, out m_currentHubSector);
			m_currentTrackHub.InfoAt(m_currentT, m_currentHubSector, out m_trackPoint, out m_trackTangent, out m_trackNormal, out m_trackWidth);
			Vector3Utils.Normalise(ref m_trackTangent);
			m_trackCentreHeight = m_currentTrackHub.HeightAt(0f);
		}
		else
		{
			m_trackPoint = Vector3.zero;
			m_trackTangent = Vector3.forward;
			m_trackNormal = Vector3.right;
			m_trackWidth = 1000f;
		}
		m_trackForwardSpeed = Vector3.Dot(m_trackTangent, m_velocity);
	}

	private IEnumerator FadeAndHide(float _time)
	{
		float alpha = 1f;
		float rate = 1f / _time;
		do
		{
			alpha = Mathf.Max(alpha - Time.deltaTime * rate, 0f);
			m_visuals.Alpha = alpha;
			yield return null;
		}
		while (alpha > 0f);
		base.gameObject.SetActiveRecursively(false);
	}

	public override Vector3 GetWheelHitPosition(Wheel _wheel)
	{
		return m_transform.TransformPoint(m_wheelPositions[(int)_wheel]);
	}

	public override bool IsWheelGrounded(Wheel _wheel)
	{
		return true;
	}
}
