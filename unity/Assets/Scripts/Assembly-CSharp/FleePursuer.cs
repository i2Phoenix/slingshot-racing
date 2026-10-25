using System;
using UnityEngine;

[AddComponentMenu("Gameplay/Flee/Pursuer")]
public class FleePursuer : MonoBehaviour, DynamicCamera.Vehicle
{
	private enum State
	{
		CatchUp = 0,
		Survival = 1,
		CoolDown = 2,
		Cruise = 3,
		StoppedPursuit = 4,
		Shielded = 5
	}

	private const float c_trackingThresholdEnter = 30f;

	private const float c_trackingThresholdExit = 40f;

	private const float c_killDistance = 20f;

	private Transform m_transform;

	private Rigidbody m_rigidBody;

	private TrackTraverser m_traverser;

	private FleePursuerVisuals m_visuals;

	private RaceManager m_raceManager;

	private Vector3 m_prevPosition;

	private Quaternion m_prevRotation;

	private float m_interpolation;

	private float m_speed;

	private float m_acceleration;

	private State m_state;

	private float m_timeInState;

	private float m_lastDistance;

	private float m_accumulatedSurvivalTier;

	private int m_respiteLevel;

	private float m_respiteSpeedScale = 1f;

	private bool m_cameraTracking;

	private bool m_started;

	private bool m_stopped;

	private int m_distributer;

	private Vector3 m_trackPoint;

	private Vector3 m_trackTangent;

	private Vector3 m_trackNormal;

	private float m_trackWidth;

	private TrackSpline m_speedSpline;

	private int m_speedSplineIndex = -1;

	public static Vector3[] s_replayPositions;

	public static bool s_replayPlay;

	public static int s_replaySize;

	public static int s_replayIndex;

	public SpeedSchedule m_speedSchedule;

	public float m_speedScheduleScale = 0.5f;

	public float m_speedScheduleOffset = 0.5f;

	public float m_respiteScale = 0.8f;

	public float m_respiteTime = 1f;

	public float m_coolDownDistanceThreshold = 60f;

	public float m_survivalInDistanceThreshold = 30f;

	public float m_survivalOutDistanceThreshold = 40f;

	public float m_chomperPushDistanceThreshold = 20f;

	public float m_boredomDistanceThreshold = 110f;

	public float m_coolDownSpeed = 30f;

	public float m_catchupSpeed = 45f;

	public float m_shieldedSpeed = 10f;

	public float m_survivalCatchupTierSpeed = 1f;

	public float m_survivalCatchupTierTime = 10f;

	public float m_reactionTime = 1f;

	public float m_maxAcceleration = 10f;

	public float m_shutdownTime = 2f;

	public Vector3 Position
	{
		get
		{
			return m_transform.position;
		}
	}

	public float TrackForwardSpeed
	{
		get
		{
			return m_speed;
		}
	}

	public TrackSpline CurrentTrackSpline
	{
		get
		{
			return m_traverser.CurrentTrackSpline;
		}
	}

	public TrackHub CurrentTrackHub
	{
		get
		{
			return m_traverser.CurrentTrackHub;
		}
	}

	public float CurrentT
	{
		get
		{
			return m_traverser.CurrentT;
		}
	}

	public int CurrentHubSector
	{
		get
		{
			return m_traverser.CurrentHubSector;
		}
	}

	public Vector3 TrackPoint
	{
		get
		{
			return m_trackPoint;
		}
	}

	public Vector3 TrackTangent
	{
		get
		{
			return m_trackTangent;
		}
	}

	public Vector3 TrackNormal
	{
		get
		{
			return m_trackNormal;
		}
	}

	public float TrackWidth
	{
		get
		{
			return m_trackWidth;
		}
	}

	public float EnginePitch
	{
		get
		{
			return Mathf.InverseLerp(m_coolDownSpeed, m_catchupSpeed, m_speed);
		}
	}

	private void Awake()
	{
		m_transform = base.transform;
		m_rigidBody = base.GetComponent<Rigidbody>();
		m_visuals = GetComponentInChildren<FleePursuerVisuals>();
		m_traverser = new TrackTraverser(m_transform.position);
		m_raceManager = GameObjectUtils.FindComponentWithTag<RaceManager>("RaceControl");
		m_prevPosition = m_transform.position;
		m_prevRotation = m_transform.rotation;
	}

	private void Start()
	{
		if (m_raceManager != null)
		{
			m_raceManager.OnTrackingCameraReadyEvent += OnTrackingCameraReady;
			m_raceManager.OnRaceLaunchEvent += OnRaceStart;
			m_raceManager.OnRaceReplayEvent += OnRaceStop;
			m_raceManager.OnRaceAbortedEvent += OnRaceStop;
		}
		else
		{
			GotoState(State.CatchUp);
			m_started = true;
			m_distributer = 0;
			EnableCameraTrack();
		}
		m_traverser.GetInfo(out m_trackPoint, out m_trackTangent, out m_trackNormal, out m_trackWidth);
		Vector3 up = m_traverser.GetUp();
		m_rigidBody.MovePosition(m_trackPoint + m_traverser.GetHeight(0f) * up);
		m_rigidBody.MoveRotation(Quaternion.LookRotation(m_trackTangent, up));
		m_interpolation = 1f;
		if (m_visuals != null)
		{
			m_visuals.Init(this);
		}
	}

	private void OnDestroy()
	{
		if (m_raceManager != null)
		{
			m_raceManager.OnTrackingCameraReadyEvent -= OnTrackingCameraReady;
			m_raceManager.OnRaceLaunchEvent -= OnRaceStart;
			m_raceManager.OnRaceReplayEvent -= OnRaceStop;
			m_raceManager.OnRaceAbortedEvent -= OnRaceStop;
		}
	}

	private void Update()
	{
		float deltaTime = Time.deltaTime;
		m_interpolation = Mathf.Clamp01(m_interpolation + deltaTime / Time.fixedDeltaTime);
		if (m_visuals != null)
		{
			m_visuals.Transform.position = Vector3Utils.LerpNoClamp(m_prevPosition, m_transform.position, m_interpolation);
			m_visuals.Transform.rotation = Quaternion.Slerp(m_prevRotation, m_transform.rotation, m_interpolation);
			m_visuals.PursuerUpdate();
		}
	}

	private void FixedUpdate()
	{
		float deltaTime = Time.deltaTime;
		m_prevPosition = m_transform.position;
		m_prevRotation = m_transform.rotation;
		m_interpolation--;
		Vector3 vector = m_prevPosition;
		if (s_replayPlay)
		{
			if (m_started && s_replayIndex < s_replaySize)
			{
				vector = s_replayPositions[s_replayIndex++];
				m_speed = Vector3.Dot(m_trackTangent, vector - m_prevPosition);
			}
			else
			{
				m_speed = 0f;
			}
		}
		else
		{
			if (m_started && !m_stopped && (m_distributer & 1) == 0)
			{
				InfrequentUpdate(deltaTime * 2f);
			}
			if (m_traverser.CurrentTrackSpline != m_speedSpline)
			{
				m_speedSpline = m_traverser.CurrentTrackSpline;
				UpdateSpeedSplineIndex();
			}
			float num = 1f;
			if (m_speedSplineIndex >= 0)
			{
				float num2 = m_speedSpline.ReParamTtoS(m_traverser.CurrentT);
				SpeedSchedule.SplineData splineData = m_speedSchedule.m_splineData[m_speedSplineIndex];
				float num3 = num2 / splineData.m_segmentLength;
				int num4 = Math.Min(Mathf.FloorToInt(num3), splineData.m_normalisedSpeeds.Length - 2);
				float t = num3 - (float)num4;
				float num5 = splineData.m_normalisedSpeeds[num4];
				float to = splineData.m_normalisedSpeeds[num4 + 1];
				num = Mathf.Lerp(num5, to, t) * m_speedScheduleScale + m_speedScheduleOffset;
			}
			m_timeInState += deltaTime;
			m_speed += m_acceleration * deltaTime;
			float num6 = m_speed * num * m_respiteSpeedScale;
			vector += m_transform.forward * deltaTime * num6;
			if (m_started || m_speed != 0f)
			{
				if (s_replayPositions == null)
				{
					s_replayPositions = new Vector3[4096];
				}
				if (s_replaySize == s_replayPositions.Length)
				{
					Vector3[] destinationArray = new Vector3[s_replaySize * 2];
					Array.Copy(s_replayPositions, destinationArray, s_replaySize);
					s_replayPositions = destinationArray;
				}
				s_replayPositions[s_replaySize++] = vector;
			}
		}
		m_traverser.Move(vector);
		m_traverser.GetInfo(out m_trackPoint, out m_trackTangent, out m_trackNormal, out m_trackWidth);
		Vector3 up = m_traverser.GetUp();
		m_rigidBody.MovePosition(m_trackPoint + m_traverser.GetHeight(0f) * up);
		m_rigidBody.MoveRotation(Quaternion.LookRotation(m_trackTangent, up));
		m_distributer++;
	}

	private void OnTrackingCameraReady(object _sender, EventArgs _args)
	{
		EnableCameraTrack();
	}

	private void OnRaceStart(object _sender, EventArgs _args)
	{
		if (!s_replayPlay)
		{
			GotoState(State.CatchUp);
		}
		m_started = true;
		m_distributer = 0;
		if (!s_replayPlay)
		{
			s_replaySize = 0;
		}
		s_replayIndex = 0;
	}

	private void OnRaceStop(object _sender, EventArgs _args)
	{
		m_stopped = true;
	}

	private void OnTriggerEnter(Collider _collider)
	{
		if (m_state == State.StoppedPursuit)
		{
			return;
		}
		PhysicsVehicle component = _collider.transform.root.GetComponent<PhysicsVehicle>();
		if (!(component != null))
		{
			return;
		}
		float num = TrackUtils.ForwardDistance(m_traverser.Location, component.Location);
		if (float.IsNaN(num) || !(num <= 20f))
		{
			return;
		}
		switch (component.TryKill())
		{
		case PhysicsVehicle.TryKillResult.Shielded:
			if (m_state != State.Cruise && m_state != State.StoppedPursuit)
			{
				GotoState(State.Shielded);
			}
			break;
		case PhysicsVehicle.TryKillResult.Killed:
			if (m_state != State.Cruise && m_state != State.StoppedPursuit)
			{
				GotoState(State.CoolDown);
			}
			break;
		}
	}

	private void OnRespite()
	{
		m_respiteLevel++;
		UpdateRespiteSpeedScale();
		Invoke("EndRespite", m_respiteTime);
	}

	private void EndRespite()
	{
		m_respiteLevel--;
		UpdateRespiteSpeedScale();
	}

	private void UpdateRespiteSpeedScale()
	{
		m_respiteSpeedScale = Mathf.Pow(m_respiteScale, m_respiteLevel);
	}

	private void EnableCameraTrack()
	{
		DynamicCamera get = DynamicCamera.Get;
		if (get != null)
		{
			get.TrackObject(this, false);
		}
		m_cameraTracking = true;
	}

	private void DisableCameraTrack()
	{
		DynamicCamera get = DynamicCamera.Get;
		if (get != null)
		{
			get.UntrackObject(this);
		}
		m_cameraTracking = false;
	}

	private void InfrequentUpdate(float deltaTime)
	{
		if (m_state == State.StoppedPursuit)
		{
			return;
		}
		float num = float.MaxValue;
		float num2 = float.MaxValue;
		float num3 = 0f;
		Vehicle vehicle = null;
		bool flag = false;
		for (int i = 0; i < m_raceManager.NumRacers; i++)
		{
			PhysicsVehicle physicsVehicle = m_raceManager.GetVehicle(i) as PhysicsVehicle;
			if (!(physicsVehicle != null) || physicsVehicle.IsDead)
			{
				continue;
			}
			bool flag2 = i < m_raceManager.NumHumans;
			flag |= flag2;
			float num4 = TrackUtils.ForwardDistance(m_traverser.Location, physicsVehicle.Location);
			if (!float.IsNaN(num4))
			{
				if (flag2 && num4 < num)
				{
					num = num4;
				}
				if (num4 < num2)
				{
					num2 = num4;
				}
				if (num4 > num3)
				{
					num3 = num4;
					vehicle = physicsVehicle;
				}
			}
		}
		float num5 = float.MaxValue;
		if (vehicle != null)
		{
			num5 = TrackUtils.ForwardDistance(vehicle.Location, m_traverser.Location);
		}
		float relativeSpeed = 0f;
		if (!float.IsNaN(m_lastDistance) && !float.IsNaN(num2))
		{
			relativeSpeed = (m_lastDistance - num2) / deltaTime;
		}
		m_lastDistance = num2;
		if (num5 < m_chomperPushDistanceThreshold || (flag && num > m_boredomDistanceThreshold))
		{
			if (m_state == State.CoolDown)
			{
				GotoState(State.CatchUp);
			}
			TendToSpeed(m_catchupSpeed);
		}
		else
		{
			switch (m_state)
			{
			case State.Shielded:
				DoShielded();
				break;
			case State.CatchUp:
				DoCatchup(num2);
				break;
			case State.Survival:
				DoSurvival(num2, relativeSpeed);
				break;
			case State.CoolDown:
				DoCoolDown(num2);
				break;
			case State.Cruise:
				TendToSpeed(m_coolDownSpeed);
				break;
			}
		}
		float num6 = ((!m_cameraTracking) ? 30f : 40f);
		bool flag3 = num < num6;
		if (flag3 != m_cameraTracking)
		{
			if (flag3)
			{
				EnableCameraTrack();
			}
			else
			{
				DisableCameraTrack();
			}
		}
	}

	private void DoShielded()
	{
		if (m_speed < m_shieldedSpeed)
		{
			GotoState(State.CatchUp);
		}
		else
		{
			TendToSpeed(0f);
		}
	}

	private void DoCatchup(float _distance)
	{
		if (_distance < m_survivalInDistanceThreshold)
		{
			GotoState(State.Survival);
		}
		else
		{
			TendToSpeed(m_catchupSpeed);
		}
	}

	private void DoSurvival(float _distance, float _relativeSpeed)
	{
		if (_distance > m_survivalOutDistanceThreshold)
		{
			GotoState(State.CatchUp);
			return;
		}
		float num = m_timeInState / m_survivalCatchupTierTime + m_accumulatedSurvivalTier;
		float num2 = num * m_survivalCatchupTierSpeed;
		float num3 = m_coolDownSpeed + num2;
		if (m_speed < num3)
		{
			TendToSpeed(num3);
			return;
		}
		float acceleration = (num2 - _relativeSpeed) / m_reactionTime;
		TendToAcceleration(acceleration);
	}

	private void DoCoolDown(float _distance)
	{
		if (_distance > m_coolDownDistanceThreshold)
		{
			GotoState(State.CatchUp);
		}
		else
		{
			TendToSpeed(m_coolDownSpeed);
		}
	}

	private void TendToSpeed(float _speed)
	{
		float num = _speed - m_speed;
		float acceleration = num / m_reactionTime;
		TendToAcceleration(acceleration);
	}

	private void TendToAcceleration(float _acceleration)
	{
		m_acceleration = Mathf.Clamp(_acceleration, 0f - m_maxAcceleration, m_maxAcceleration);
	}

	private void UpdateSpeedSplineIndex()
	{
		m_speedSplineIndex = -1;
		if (m_speedSchedule == null || m_speedSpline == null)
		{
			return;
		}
		for (int i = 0; i < m_speedSchedule.m_splineData.Length; i++)
		{
			if (m_speedSchedule.m_splineData[i].m_name == m_speedSpline.name)
			{
				m_speedSplineIndex = i;
				break;
			}
		}
	}

	public void FleeManager_StopPursuit()
	{
		if (!s_replayPlay)
		{
			GotoState(State.StoppedPursuit);
		}
	}

	public void FleeManager_Cruise()
	{
		if (!s_replayPlay)
		{
			GotoState(State.Cruise);
		}
	}

	private void GotoState(State _state)
	{
		if (m_state != _state)
		{
			State state = m_state;
			if (state == State.Survival)
			{
				m_accumulatedSurvivalTier = m_timeInState / m_survivalCatchupTierTime;
			}
			switch (_state)
			{
			case State.Shielded:
				m_acceleration = 0f - m_maxAcceleration;
				break;
			case State.CatchUp:
				m_acceleration = m_maxAcceleration;
				break;
			case State.Survival:
				m_acceleration = 0f;
				break;
			case State.CoolDown:
				m_accumulatedSurvivalTier = 0f;
				m_lastDistance = float.NaN;
				m_acceleration = 0f - m_maxAcceleration;
				break;
			case State.Cruise:
				m_accumulatedSurvivalTier = 0f;
				m_lastDistance = float.NaN;
				m_acceleration = 0f;
				break;
			case State.StoppedPursuit:
				m_stopped = true;
				m_acceleration = (0f - m_speed) / m_shutdownTime;
				Invoke("EndShutdown", m_shutdownTime);
				break;
			}
			m_state = _state;
			m_timeInState = 0f;
		}
	}

	private void EndShutdown()
	{
		m_speed = 0f;
		m_acceleration = 0f;
	}
}
