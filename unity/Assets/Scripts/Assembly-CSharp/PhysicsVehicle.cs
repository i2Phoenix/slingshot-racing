#define DYNAMIC_TRACK_SEGMENTS
using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;
using VehicleTypes;

public class PhysicsVehicle : Vehicle
{
	public enum TryKillResult
	{
		None = 0,
		Shielded = 1,
		Killed = 2
	}

	private const float c_minorCollisionVelocity = 2f;

	private const float c_minorCollisionVelocitySq = 4f;

	private const float c_majorCollisionVelocity = 4f;

	private const float c_majorCollisionVelocitySq = 16f;

	private const float c_handbrakeTorque = 0.05f;

	private const float c_handbrakeFriction = 5f;

	public static bool DebugSurroundColliders;

	private float m_grappleDistOverride;

	private GrapplePoint m_grapplePointOverride;

	private Vector3 m_grapplePointPos;

	private bool m_handbrakeOn;

	private Vector3 m_oldPosition;

	private Quaternion m_oldRotation;

	private Transform m_transform;

	private Rigidbody m_rigidbody;

	private int m_distributor;

	private Vector3 m_surfaceNormal;

	private bool m_isOnSurface;

	private bool m_isFacingForwards;

	private float m_centerOfMassY;

	private float m_prevDeltaT;

	private float m_boostImpulse;

	private float m_allowBoostCountdown;

	private int m_enteredBoost;

	private bool m_isInRumbleArea;

	private float m_rumbleAreaProp;

	private bool m_isOnTrackEdge;

	private bool m_isButtonDown;

	private float m_distToVehicleFront;

	private Rigidbody m_constraint;

	private float m_grappleDist;

	private bool m_backGrappleEnabled = true;

	private bool m_grapplePriming;

	private float m_grappleRampUpScale = 100000000f;

	private float m_buttonDownTime;

	private CapsuleCollider m_trackCollider;

	private CapsuleCollider m_surroundCollider;

	private int m_currentTrackSegment = -1;

	private float m_respawnTimer;

	private float m_fastTurnTimer;

	private int m_numMiniRewards;

	private GrapplePoint m_previousGrapplePoint;

	private float m_grappleStartSpeed;

	private bool m_validGrappleMiniReward;

	private float m_proximityWeight;

	private WheelFrictionCurve m_wheelFrictionCurve;

	private WheelCollider[] m_wheels;

	private Vector3[] m_wheelHitPositions = new Vector3[4];

	private Vector3[] m_groundContactPositions = new Vector3[4];

	private bool[] m_wheelIsGrounded = new bool[4];

	private FinishLine m_finishLine;

	private float m_launchThrustRemaining;

	private bool m_AIControlled;

	private float m_oilSlickTime;

	private float m_obstacleEffectTime;

	private int m_trackSurroundLayer;

	private bool m_dropMiniReward;

	private bool m_triggerPowerBox;

	private bool m_powerBoxDischargeAll;

	private bool m_shieldActive;

	private float m_batteringRamCountdown;

	private Upgrade[] m_upgradeSlots = new Upgrade[3];

	public float m_grappleHalfSpread = 1f;

	public float m_grappleSteering = 1f;

	public float m_grappleForce = 25f;

	public float m_grappleDamping = 10f;

	public float m_backGrappleForceScale = 1f;

	public float m_grappleWindInRumbleZone = 0.005f;

	public float m_grappleRampUpTime;

	public float m_forwardForce = 6f;

	public float m_thrustScale = 1f;

	public float m_downforce = 0.005f;

	public float m_suspensionSpringStrength = 0.3f;

	public float m_wheelFriction = 0.375f;

	public float m_quadraticDrag;

	public float m_launchThrustScale = 3f;

	public float m_launchThrustTime = 0.75f;

	public float m_launchImpulse = 2f;

	public float m_shieldDuration = 0.5f;

	public float m_shieldBoostImpluse = 12f;

	public float m_batteringRamSpeedLateral = 12f;

	public float m_batteringRamSpeedBack = 2f;

	public float m_batteringRamSpin = 90f;

	public float m_batteringRamObstacleScale = 0.5f;

	public override Vector3 GrappleFrontPos
	{
		get
		{
			Vector3 position = m_position;
			position += m_forward * m_grappleHalfSpread;
			return position + m_up * m_centerOfMassY;
		}
	}

	public bool InRumbleArea
	{
		get
		{
			return m_isInRumbleArea;
		}
	}

	public bool IsPressing
	{
		get
		{
			return m_isButtonDown;
		}
	}

	public bool IsGrappling
	{
		get
		{
			return m_grapplePoint != null;
		}
	}

	public override Quaternion Rotation
	{
		get
		{
			return m_rigidbody.rotation;
		}
	}

	public override Vector3 Position
	{
		get
		{
			return m_rigidbody.position;
		}
	}

	public override Vector3 Forward
	{
		get
		{
			return QuaternionUtils.Mul00Z(m_rigidbody.rotation, 1f);
		}
	}

	public override Vector3 Right
	{
		get
		{
			return QuaternionUtils.MulX00(m_rigidbody.rotation, 1f);
		}
	}

	public override Vector3 Up
	{
		get
		{
			return QuaternionUtils.Mul0Y0(m_rigidbody.rotation, 1f);
		}
	}

	public override Vector3 Velocity
	{
		get
		{
			return m_rigidbody.linearVelocity;
		}
	}

	public Rigidbody RigidBody
	{
		get
		{
			return m_rigidbody;
		}
	}

	public Vector3 GrapplePointPosition
	{
		get
		{
			return m_grapplePointPos;
		}
	}

	public override bool IsLocalHuman
	{
		get
		{
			return !m_AIControlled;
		}
	}

	public override bool ScaleAudioVolume
	{
		get
		{
			return !IsLocalHuman;
		}
	}

	public override bool UsesHeadlights
	{
		get
		{
			return IsLocalHuman;
		}
	}

	public GrapplePoint GrapplePointOverride
	{
		set
		{
			m_grapplePointOverride = value;
		}
	}

	public float GrappleDistOverride
	{
		get
		{
			return m_grappleDist;
		}
		set
		{
			m_grappleDist = value;
		}
	}

	public float Speed { get; private set; }

	public bool HandbrakeOn
	{
		get
		{
			return m_handbrakeOn;
		}
		set
		{
			if (m_handbrakeOn != value)
			{
				m_handbrakeOn = value;
				UpdateHandbrake(0f);
			}
		}
	}

	public Vector3 SurfaceNormal
	{
		get
		{
			return m_surfaceNormal;
		}
	}

	public bool AIControlled
	{
		get
		{
			return m_AIControlled;
		}
	}

	public override float OilSlickTime
	{
		get
		{
			return m_oilSlickTime;
		}
	}

	public int LapSequence
	{
		get
		{
			return m_waypoint;
		}
	}

	public event Action OnNewLapEvent;

	public event Action<Vehicle> OnRumbleAreaEvent;

	public event Action<Vehicle> OnTrackEdgeEvent;

	public event Action<Vector3, Vector3, float> OnMovementEvent;

	public event Action<Collision, Vehicle, Vehicle> OnHitByOtherVehicleEvent;

	private void Awake()
	{
		m_transform = base.transform;
		m_rigidbody = base.GetComponent<Rigidbody>();
		m_forwardForce *= 2f;
		m_grappleHalfSpread *= 2f;
		m_trackSurroundLayer = LayerMask.NameToLayer("TrackSurround");
		m_finishLine = FinishLine.Get();
	}

	private CapsuleCollider CreateChildCapsuleCollider(string name)
	{
		GameObject gameObject = new GameObject(name);
		gameObject.transform.parent = m_transform;
		gameObject.transform.localPosition = Vector3.zero;
		gameObject.transform.localRotation = Quaternion.identity;
		CapsuleCollider capsuleCollider = gameObject.AddComponent<CapsuleCollider>();
		capsuleCollider.center = Vector3.zero;
		capsuleCollider.direction = 2;
		return capsuleCollider;
	}

	private void Start()
	{
#if DEVELOPMENT_BUILD || UNITY_EDITOR
        Debug.Log($"QA_SPAWN_START pos={transform.position} rb={m_rigidbody.position} flags={m_rigidbody.detectCollisions}");
#endif
		m_distributor = base.PlayerIndex;
		CapsuleCollider capsuleCollider = CreateChildCapsuleCollider("Trigger");
		capsuleCollider.gameObject.layer = LayerMask.NameToLayer("TriggerActivators");
		capsuleCollider.center = Vector3.zero;
		capsuleCollider.radius = 0.88f;
		capsuleCollider.height = 3.6f;
		capsuleCollider.isTrigger = true;
		PhysicsMaterial sharedMaterial = (PhysicsMaterial)Resources.Load("PhysicsMaterials/Vehicle");
		string[] array = new string[2] { "Left Capsule", "Right Capsule" };
		for (int i = 0; i != array.Length; i++)
		{
			CapsuleCollider capsuleCollider2 = CreateChildCapsuleCollider(array[i]);
			capsuleCollider2.center = new Vector3(((i != 0) ? (-1f) : 1f) * 0.4f, -0.2f, 0f);
			capsuleCollider2.radius = 0.6f;
			capsuleCollider2.height = 3.6f;
			capsuleCollider2.sharedMaterial = sharedMaterial;
		}
		m_trackCollider = CreateChildCapsuleCollider("Track Capsule");
		m_trackCollider.center = new Vector3(0f, -0.2f, 0f);
		m_trackCollider.radius = 0.6f;
		m_trackCollider.height = 3.6f;
		m_trackCollider.sharedMaterial = sharedMaterial;
		m_trackCollider.gameObject.layer = LayerMask.NameToLayer("VehicleInner");
		m_surroundCollider = CreateChildCapsuleCollider("Surround Capsule");
		m_surroundCollider.center = new Vector3(0f, -0.4f, 0f);
		m_surroundCollider.radius = 1f;
		m_surroundCollider.height = 3.8f;
		m_surroundCollider.sharedMaterial = sharedMaterial;
		m_surroundCollider.gameObject.layer = LayerMask.NameToLayer("VehicleOuter");
		float suspensionDistance = 0.25f;
		JointSpring suspensionSpring = default(JointSpring);
		suspensionSpring.spring = m_suspensionSpringStrength * 2f;
		suspensionSpring.damper = 0.2f;
		suspensionSpring.targetPosition = 0f;
		m_wheels = new WheelCollider[4];
		for (int j = 0; j < 4; j++)
		{
			bool flag = (j & 1) == 0;
			bool flag2 = (j & 2) == 0;
			float num = ((!flag) ? (-1f) : 1f);
			float num2 = ((!flag2) ? (-1f) : 1f);
			GameObject gameObject = new GameObject(((!flag2) ? "B" : "F") + ((!flag) ? "L" : "R") + " Wheel");
			gameObject.transform.parent = m_transform;
			gameObject.transform.localPosition = Vector3.zero;
			gameObject.transform.localRotation = Quaternion.identity;
			gameObject.transform.localScale = Vector3.one;
			gameObject.layer = LayerMask.NameToLayer("Wheels");
			WheelCollider wheelCollider = gameObject.AddComponent<WheelCollider>();
			wheelCollider.suspensionDistance = suspensionDistance;
			wheelCollider.center = new Vector3(0.4f * num * 2f, -0.4f, 0.6f * num2 * 2f);
			wheelCollider.radius = 0.5f;
			// PhysX 3+ requires a positive wheel mass. The original body weighs one unit.
			wheelCollider.mass = m_rigidbody.mass * 0.02f;
			wheelCollider.suspensionSpring = suspensionSpring;
			m_wheels[j] = wheelCollider;
			m_wheelFrictionCurve = default(WheelFrictionCurve);
			m_wheelFrictionCurve.extremumSlip = 2f;
			// Unity 3 used force-like values here; modern WheelCollider expects coefficients.
			m_wheelFrictionCurve.extremumValue = 1f;
			m_wheelFrictionCurve.asymptoteSlip = 4f;
			m_wheelFrictionCurve.asymptoteValue = 0.5f;
			m_wheels[j].brakeTorque = ((!m_handbrakeOn) ? 0f : 0.05f);
			m_wheelFrictionCurve.stiffness = ((!m_handbrakeOn) ? 0f : 5f);
			wheelCollider.forwardFriction = m_wheelFrictionCurve;
			m_wheelFrictionCurve.stiffness = ((!m_handbrakeOn) ? m_wheelFriction : 5f);
			wheelCollider.sidewaysFriction = m_wheelFrictionCurve;
		}
		m_rigidbody.inertiaTensor = new Vector3(1.17f, 1.33f, 0.34f);
		GameObject gameObject2 = new GameObject("VehicleConstraint");
		gameObject2.transform.position = Vector3.zero;
		gameObject2.transform.rotation = Quaternion.LookRotation(Vector3.up);
		Rigidbody rigidbody = gameObject2.AddComponent<Rigidbody>();
		rigidbody.isKinematic = true;
		m_constraint = rigidbody;
		ConfigurableJoint configurableJoint = base.gameObject.AddComponent<ConfigurableJoint>();
		configurableJoint.connectedBody = rigidbody;
		configurableJoint.axis = Vector3.up;
		configurableJoint.secondaryAxis = Vector3.right;
		configurableJoint.xMotion = ConfigurableJointMotion.Free;
		configurableJoint.yMotion = ConfigurableJointMotion.Free;
		configurableJoint.zMotion = ConfigurableJointMotion.Free;
		configurableJoint.angularXMotion = ConfigurableJointMotion.Free;
		configurableJoint.angularYMotion = ConfigurableJointMotion.Limited;
		configurableJoint.angularZMotion = ConfigurableJointMotion.Limited;
		SoftJointLimit softJointLimit = new SoftJointLimit
		{
			limit = 45f
		};
		configurableJoint.angularYLimit = softJointLimit;
		softJointLimit.limit = 20f;
		configurableJoint.angularZLimit = softJointLimit;
		m_distToVehicleFront = m_trackCollider.height * 0.5f;
		m_centerOfMassY = m_rigidbody.centerOfMass.y;
		m_currentTrackSpline = TrackSpline.FindClosestToPoint(m_transform.position);
		m_currentTrackHub = null;
		m_currentHubSector = int.MaxValue;
		if (m_currentTrackSpline != null)
		{
			m_currentT = m_currentTrackSpline.ClosestTTo(m_transform.position);
			m_transform.forward = m_currentTrackSpline.TangentAtT(m_currentT).normalized;
			m_currentT = m_currentTrackSpline.ClosestTTo(m_transform.position + m_transform.forward * m_distToVehicleFront);
			GetTrackInfo(out m_trackPoint, out m_trackTangent, out m_trackNormal, out m_trackWidth, out m_trackCentreHeight);
			SplineSegmentInit();
		}
		m_oldPosition = (m_position = m_transform.position);
		m_oldRotation = (m_rotation = m_transform.rotation);
		m_right = m_transform.right;
		m_up = m_transform.up;
		m_forward = m_transform.forward;
		m_AIControlled = GetComponent<VehicleAIController>() != null;
		if (m_visuals == null)
		{
			m_visuals = GetComponentInChildren<VehicleVisuals>();
			m_visualsTransform = m_visuals.transform;
		}
		if (m_visuals != null)
		{
			m_visuals.Init(this);
			m_visualsTransform.rotation = m_rotation;
			m_visualsTransform.position = m_position - m_up * 0.8f;
		}
		m_grappleRampUpScale = ((!(m_grappleRampUpTime > 0f)) ? 100000000f : (1f / m_grappleRampUpTime));
		m_waypoint = 0;
		if (m_currentTrackSpline.HubAtTail == null)
		{
			float trackSplineT = m_finishLine.TrackSplineT;
			float maxT = m_finishLine.TrackSpline.MaxT;
			if (trackSplineT < 0.25f * maxT && m_currentT > 0.75f * maxT)
			{
				m_waypoint = -2;
			}
			else if (trackSplineT > 0.75f * maxT && m_currentT < 0.25f * maxT)
			{
				m_waypoint = 2;
			}
		}
		if (GameObject.FindGameObjectWithTag("PowerBox") != null)
		{
			base.EnableMiniRewards = true;
		}
		m_rigidbody.interpolation = ((!TimeManager.EnableSmoothing) ? RigidbodyInterpolation.Interpolate : RigidbodyInterpolation.None);
	}

	private void OnEnable()
	{
		if (m_constraint != null)
		{
			m_constraint.gameObject.active = true;
		}
	}

	private void OnDisable()
	{
		if (m_constraint != null)
		{
			m_constraint.gameObject.active = false;
		}
	}

	protected override void OnDestroy()
	{
		OnRumbleAreaEvent = null;
		if (m_constraint != null)
		{
			UnityEngine.Object.Destroy(m_constraint.gameObject);
		}
		if (m_grapplePoint != null && !m_AIControlled)
		{
			m_grapplePoint.NumGrapplingObjects--;
		}
	}

	private void Update()
	{
		float num = 1f;
		if (m_rigidbody.interpolation != RigidbodyInterpolation.None)
		{
			Vector3 rhs = m_rigidbody.position - m_position;
			float sqrMagnitude = rhs.sqrMagnitude;
			if (sqrMagnitude > 1E-06f)
			{
				num = Vector3.Dot(m_transform.position - m_position, rhs) / sqrMagnitude;
			}
		}
		else
		{
			num = TimeManager.FrameProp;
			if (m_visualsTransform != null)
			{
				if (num < 0f)
				{
					m_visualsTransform.rotation = Quaternion.Lerp(m_oldRotation, m_rotation, num + 1f);
					m_visualsTransform.position = Vector3Utils.LerpNoClamp(m_oldPosition, m_position, num + 1f) - m_visualsTransform.up * 0.8f;
				}
				else
				{
					m_visualsTransform.rotation = Quaternion.Lerp(m_rotation, m_rigidbody.rotation, num);
					m_visualsTransform.position = Vector3Utils.LerpNoClamp(m_position, m_rigidbody.position, num) - m_visualsTransform.up * 0.8f;
				}
			}
		}
		if (m_visuals != null)
		{
			m_visuals.VehicleUpdate(num);
		}
	}

#if DEVELOPMENT_BUILD || UNITY_EDITOR
    private int qaPhysicsFrames;
#endif
	private void FixedUpdate()
	{
#if DEVELOPMENT_BUILD || UNITY_EDITOR
        if (qaPhysicsFrames++ < 12)
        {
            string grounded = string.Empty;
            foreach (var wheel in m_wheels) grounded += $" {wheel.name}:{wheel.isGrounded}/{wheel.mass}/{wheel.sprungMass}";
            Debug.Log($"QA_PHYSICS_STEP {qaPhysicsFrames} pos={m_rigidbody.position:F4} velocity={m_rigidbody.linearVelocity:F4} rotation={m_rigidbody.rotation} track={m_currentT} kin={m_rigidbody.isKinematic} wheels={grounded}");
        }
#endif
		float fixedDeltaTime = Time.fixedDeltaTime;
		m_distributor++;
		m_buttonDownTime += fixedDeltaTime;
		if (OnMovementEvent != null)
		{
			OnMovementEvent(m_position, m_rigidbody.position, fixedDeltaTime);
		}
		m_oldPosition = m_position;
		m_oldRotation = m_rotation;
		m_position = m_rigidbody.position;
		m_rotation = m_rigidbody.rotation;
		m_right = m_rotation * Vector3.right;
		m_up = m_rotation * Vector3.up;
		m_forward = m_rotation * Vector3.forward;
		UpdateTrackLocation();
		UpdateSuspension();
		if ((m_distributor & 1) == 0)
		{
			UpdateInRumbleArea();
		}
		DispatchRumbleAreaEvent();
		UpdateGrapplePoint();
		ApplyDragAndDownforce();
		if (!base.IsDead && (m_currentTrackSpline != null || m_currentTrackHub != null))
		{
			ApplyStraightening(fixedDeltaTime);
		}
		UpdateTrackEdgeData();
		if (m_visuals != null && base.InnerEdgeType == TrackEdgeType.Air && base.OuterEdgeType == TrackEdgeType.Air)
		{
			Vector3 torque = Vector3.Cross(m_forward, m_trackTangent);
			torque.x = 0f;
			torque.z = 0f;
			Vector3 normalized = torque.normalized;
			torque *= 400f;
			torque -= normalized * Vector3.Dot(m_rigidbody.angularVelocity, normalized) * 50f;
			m_rigidbody.AddTorque(torque);
			Vector3 velocity = m_rigidbody.linearVelocity;
			Vector3 force = m_trackNormal * (Vector3.Dot(velocity, m_trackNormal) * -0.25f);
			m_rigidbody.AddForce(force, ForceMode.VelocityChange);
		}
		ApplyGrappleSteering();
		m_currentGrappleForce = ((!(m_grapplePoint != null)) ? 0f : ApplyGrappling());
		if (m_handbrakeOn)
		{
			UpdateHandbrake(fixedDeltaTime);
		}
		else
		{
			ApplyThrust(fixedDeltaTime);
		}
		ApplyBoost(fixedDeltaTime);
		if (m_visuals != null)
		{
			m_visuals.VehicleFixedUpdate(m_distributor);
		}
		if ((m_distributor & 1) == 0 && !base.IsDead)
		{
			UpdateRespawnTimer(fixedDeltaTime * 2f);
			if (m_respawnTimer > 2.5f)
			{
				Vector3 position = m_rigidbody.position;
				position.y += 2f;
				// Respawn is a teleport of a dynamic body, not a kinematic move.
				m_rigidbody.position = position;
				Quaternion rot = Quaternion.LookRotation(m_trackTangent, m_surfaceNormal);
				m_rigidbody.rotation = rot;
				m_rigidbody.linearVelocity = Vector3.zero;
				m_respawnTimer = 0f;
				m_fastTurnTimer = 0f;
			}
		}
		m_lapTime += fixedDeltaTime;
		if (m_batteringRamCountdown > 0f)
		{
			m_batteringRamCountdown = Mathf.Max(0f, m_batteringRamCountdown - fixedDeltaTime);
		}
	}

	private void UpdateTrackLocation()
	{
		Vector3 point = m_position + m_forward * m_distToVehicleFront;
		for (int i = 0; i < 2; i++)
		{
			if (m_currentTrackSpline != null)
			{
				float tHint = m_currentT + m_prevDeltaT;
				float tHintSpread = Mathf.Clamp(Mathf.Abs(m_prevDeltaT), 0.05f, 0.5f);
				float num = m_currentTrackSpline.ClosestTTo(point, tHint, tHintSpread, m_currentT - 0.5f, m_currentT + 0.5f);
				m_prevDeltaT = num - m_currentT;
				bool flag = m_currentTrackSpline.HubAtTail == null;
				TrackSpline currentTrackSpline = m_currentTrackSpline;
				if (num > m_currentTrackSpline.MaxT)
				{
					if (m_currentTrackSpline.HubAtTail != null)
					{
						if (m_waypoint == m_currentTrackSpline.Waypoint)
						{
							m_waypoint++;
						}
						m_currentTrackHub = m_currentTrackSpline.HubAtTail;
						m_currentHubSector = m_currentTrackSpline.HubSectorAtTail;
						m_currentT = m_currentTrackHub.MinSectorT(base.CurrentHubSector);
						m_currentTrackSpline = null;
						m_prevDeltaT = 0f;
					}
					else
					{
						m_waypoint += 2;
						m_currentT = num - m_currentTrackSpline.MaxT;
					}
				}
				else if (num < 0f)
				{
					if (m_currentTrackSpline.HubAtHead != null)
					{
						int num2 = ((m_waypoint != m_finishLine.NumWaypoints) ? m_waypoint : 0);
						if (num2 == m_currentTrackSpline.Waypoint)
						{
							m_waypoint--;
						}
						m_currentTrackHub = m_currentTrackSpline.HubAtHead;
						m_currentHubSector = m_currentTrackSpline.HubSectorAtHead;
						m_currentT = m_currentTrackHub.MaxSectorT(base.CurrentHubSector);
						m_currentTrackSpline = null;
						m_prevDeltaT = 0f;
					}
					else
					{
						m_waypoint -= 2;
						m_currentT = num + m_currentTrackSpline.MaxT;
					}
				}
				else
				{
					m_currentT = num;
					flag = true;
				}
				if (m_finishLine != null && currentTrackSpline == m_finishLine.TrackSpline && m_waypoint >= m_finishLine.NumWaypoints && m_currentT > m_finishLine.TrackSplineT)
				{
					CompleteLap(m_currentT - m_finishLine.TrackSplineT, m_prevDeltaT);
				}
				if (m_currentTrackHub != null)
				{
					SplineSegmentToHubTransition();
					m_currentTrackSegment = 0;
				}
				else
				{
					int num3 = (int)(m_currentT + 0.667f);
					if (num3 != m_currentTrackSegment)
					{
						SplineSegmentTransition(num3);
						m_currentTrackSegment = num3;
					}
				}
				if (flag)
				{
					break;
				}
			}
			else
			{
				if (!(m_currentTrackHub != null))
				{
					continue;
				}
				float o_t;
				int o_sectorIndex;
				m_currentTrackHub.ClosestInfoTo(point, out o_t, out o_sectorIndex);
				bool flag2 = m_currentTrackHub.IsSectorOutgoing(o_sectorIndex);
				if (flag2 && o_t > m_currentTrackHub.MaxSectorT(o_sectorIndex))
				{
					m_currentTrackSpline = m_currentTrackHub.SplineAtSector(o_sectorIndex);
					HubToSplineSegmentTransition(true);
					m_currentT = 0f;
					m_currentTrackHub = null;
					m_currentHubSector = int.MaxValue;
					int num4 = ((m_waypoint + 1 != m_finishLine.NumWaypoints) ? (m_waypoint + 1) : 0);
					if (num4 == m_currentTrackSpline.Waypoint)
					{
						m_waypoint++;
					}
					continue;
				}
				if (flag2 || !(o_t < m_currentTrackHub.MinSectorT(o_sectorIndex)))
				{
					m_currentT = o_t;
					m_currentHubSector = o_sectorIndex;
					break;
				}
				m_currentTrackSpline = m_currentTrackHub.SplineAtSector(o_sectorIndex);
				HubToSplineSegmentTransition(false);
				m_currentT = m_currentTrackSpline.MaxT;
				m_currentTrackHub = null;
				m_currentHubSector = int.MaxValue;
				if (m_waypoint - 1 == m_currentTrackSpline.Waypoint)
				{
					m_waypoint--;
				}
			}
		}
		GetTrackInfo(out m_trackPoint, out m_trackTangent, out m_trackNormal, out m_trackWidth, out m_trackCentreHeight);
		m_trackForwardSpeed = Vector3.Dot(m_rigidbody.linearVelocity, m_trackTangent);
	}

	private void UpdateSuspension()
	{
		bool flag = false;
		Vector3 vector = Vector3.up;
		Quaternion quaternion = Quaternion.Inverse(m_rotation);
		int num = 0;
		for (int i = 0; i != 2; i++)
		{
			int num2 = i * 2;
			int num3 = i * 2 + 1;
			WheelCollider wheelCollider = m_wheels[num2];
			WheelCollider wheelCollider2 = m_wheels[num3];
			WheelHit hit;
			bool groundHit = wheelCollider.GetGroundHit(out hit);
			WheelHit hit2;
			bool groundHit2 = wheelCollider2.GetGroundHit(out hit2);
			flag |= groundHit | groundHit2;
			m_wheelIsGrounded[num2] = groundHit;
			m_wheelIsGrounded[num3] = groundHit2;
			if (groundHit)
			{
				vector += hit.normal;
				Vector3 vector2 = quaternion * (hit.point - m_position);
				m_wheelHitPositions[num2] = vector2;
				m_groundContactPositions[num++] = hit.point;
			}
			if (groundHit2)
			{
				vector += hit2.normal;
				Vector3 vector3 = quaternion * (hit2.point - m_position);
				m_wheelHitPositions[num3] = vector3;
				m_groundContactPositions[num++] = hit2.point;
			}
		}
		m_isOnSurface = flag;
		if (flag)
		{
			int num4 = num - 2;
			if (num4 > 0)
			{
				vector = Vector3.zero;
				for (int j = 0; j != num4; j++)
				{
					Vector3 lhs = m_groundContactPositions[1] - m_groundContactPositions[j * 3];
					Vector3 rhs = m_groundContactPositions[2] - m_groundContactPositions[j * 3];
					Vector3 normalized = Vector3.Cross(lhs, rhs).normalized;
					if (normalized.y < 0f)
					{
						vector -= normalized;
					}
					else
					{
						vector += normalized;
					}
				}
				if (num4 > 1)
				{
					vector.Normalize();
				}
			}
			else
			{
				vector = (vector - Vector3.up).normalized;
			}
		}
		m_surfaceNormal = vector;
		m_constraint.MoveRotation(Quaternion.LookRotation(m_surfaceNormal));
	}

	private void UpdateInRumbleArea()
	{
		if (m_currentTrackSpline != null)
		{
			float num = Vector3.Dot(m_trackPoint, m_trackNormal);
			float num2 = Vector3.Dot(m_position, m_trackNormal);
			float num3 = num2 - num;
			float num4 = 0.9f;
			float num5 = m_trackWidth * GetTrackRumbleAreaProp();
			float num6 = 1f / (m_trackWidth - num5);
			if (num3 < 0f)
			{
				float num7 = num3 - num4;
				m_rumbleAreaProp = Mathf.Clamp01((0f - num7 - num5) / num6);
				m_isInRumbleArea = m_rumbleAreaProp > 0f;
				m_isOnTrackEdge = m_isInRumbleArea;
				Vector3 lhs = m_currentTrackSpline.TangentDTAtT(m_currentT);
				float num8 = m_currentTrackSpline.DsDtAtT(m_currentT);
				lhs /= num8;
				if (Vector3.Dot(lhs, m_trackNormal) <= 0.5f)
				{
					m_isInRumbleArea = false;
					m_rumbleAreaProp = 0f;
				}
			}
			else
			{
				float num9 = num3 + num4;
				m_rumbleAreaProp = Mathf.Clamp01((num9 - num5) / num6);
				m_isInRumbleArea = m_rumbleAreaProp > 0f;
				m_isOnTrackEdge = m_isInRumbleArea;
				Vector3 lhs2 = m_currentTrackSpline.TangentDTAtT(m_currentT);
				float num10 = m_currentTrackSpline.DsDtAtT(m_currentT);
				lhs2 /= num10;
				if (Vector3.Dot(lhs2, m_trackNormal) >= -0.5f)
				{
					m_isInRumbleArea = false;
					m_rumbleAreaProp = 0f;
				}
			}
		}
		else
		{
			m_isInRumbleArea = false;
			m_isOnTrackEdge = false;
		}
	}

	private void DispatchRumbleAreaEvent()
	{
		if (m_isInRumbleArea && OnRumbleAreaEvent != null)
		{
			OnRumbleAreaEvent(this);
		}
		if (m_isOnTrackEdge && OnTrackEdgeEvent != null)
		{
			OnTrackEdgeEvent(this);
		}
	}

	private void UpdateRespawnTimer(float deltaTime)
	{
		m_isFacingForwards = Vector3.Dot(m_forward, m_trackTangent) > 0f;
		if (!m_isFacingForwards)
		{
			m_respawnTimer += deltaTime;
		}
		else
		{
			m_respawnTimer = 0f;
		}
	}

	private void UpdateGrapplePoint()
	{
		if (m_grapplePriming)
		{
			if (m_grapplePoint != null && !m_AIControlled)
			{
				m_grapplePoint.NumGrapplingObjects--;
			}
			m_grapplePoint = GetTrackGrapplePoint();
			if (m_grapplePoint != null)
			{
				m_grapplePriming = false;
				m_grapplePointPos = m_grapplePoint.Position;
				Vector3 vector = m_forward * m_grappleHalfSpread;
				Vector3 vector2 = m_position + m_up * m_centerOfMassY;
				Vector3 vector3 = vector2 + vector;
				Vector3 lhs = m_grapplePointPos - vector3;
				m_grappleDist = Mathf.Sqrt(lhs.x * lhs.x + lhs.z * lhs.z);
				m_grappleDist = Mathf.Max(m_grappleDist, m_grapplePoint.minGrappleLength);
				m_backGrappleEnabled = false;
				if (m_visuals != null)
				{
					float num = Mathf.Sign(Vector3.Dot(lhs, m_right));
					m_visuals.m_grappleGunOffset.x = Mathf.Abs(m_visuals.m_grappleGunOffset.x) * num;
					m_visuals.StartGrapple(m_grapplePoint, vector3, m_rigidbody.GetPointVelocity(vector3));
				}
				if (base.EnableMiniRewards)
				{
					m_grappleStartSpeed = m_rigidbody.linearVelocity.magnitude;
					m_validGrappleMiniReward = m_grapplePoint != m_previousGrapplePoint;
				}
				if (!m_AIControlled)
				{
					m_grapplePoint.NumGrapplingObjects++;
				}
				SendGrappleEvent(m_grapplePoint);
			}
		}
		else if (m_grapplePoint != null && (!m_isButtonDown || (!IsInGrapplePoint(m_grapplePoint) && !m_grapplePoint.m_noEnd) || m_trackForwardSpeed < 0f))
		{
			SendGrappleReleaseEvent(m_grapplePoint);
			if (!m_AIControlled)
			{
				m_grapplePoint.NumGrapplingObjects--;
			}
			m_previousGrapplePoint = m_grapplePoint;
			if (base.EnableMiniRewards && m_validGrappleMiniReward && m_numMiniRewards < 2 && (base.EnableEasyRewards || m_rigidbody.linearVelocity.magnitude > m_grappleStartSpeed * 0.85f))
			{
				SendCollectMiniRewardEvent();
				m_numMiniRewards++;
			}
			m_grapplePoint = null;
		}
		if (m_numMiniRewards > 0)
		{
			if (m_triggerPowerBox)
			{
				SendDeployMiniRewardEvent(m_powerBoxDischargeAll);
				TriggerBoost((float)m_numMiniRewards * 2f);
				m_numMiniRewards = ((!m_powerBoxDischargeAll) ? (m_numMiniRewards - 1) : 0);
			}
			else if (m_dropMiniReward && !HasUpgrade(Upgrade.MiniRewardShield))
			{
				SendDropMiniRewardEvent();
				m_numMiniRewards = 0;
			}
		}
		m_triggerPowerBox = false;
		m_powerBoxDischargeAll = false;
		m_dropMiniReward = false;
	}

	private void ApplyDragAndDownforce()
	{
		Vector3 velocity = m_rigidbody.linearVelocity;
		float sqrMagnitude = velocity.sqrMagnitude;
		if (sqrMagnitude > 1E-06f)
		{
			Vector3 vector = velocity / Mathf.Sqrt(sqrMagnitude);
			float num = Vector3.Dot(vector, m_forward);
			float num2 = Mathf.Sqrt(Mathf.Max(0f, 1f - num * num));
			float num3 = m_quadraticDrag * (1f + 3f * num2);
			float num4 = sqrMagnitude * num3;
			if (m_isInRumbleArea)
			{
				num4 += num4 * m_rumbleAreaProp * 3f;
			}
			m_rigidbody.AddForce(vector * (0f - num4));
			float num5 = 1f - Mathf.Abs(Vector3.Dot(m_up, vector));
			float num6 = num5 * num5 * sqrMagnitude * m_downforce;
			m_rigidbody.AddForce(m_up * (0f - num6));
		}
		if (m_grapplePoint == null)
		{
			Vector3 angularVelocity = m_rigidbody.angularVelocity;
			float magnitude = angularVelocity.magnitude;
			m_rigidbody.AddTorque(angularVelocity * (-2f - magnitude * 1f));
		}
		float num7 = Vector3.Dot(m_forward, m_surfaceNormal);
		float num8 = 15f * num7;
		m_rigidbody.AddTorque(m_right * num8);
	}

	private void ApplyStraightening(float deltaTime)
	{
		bool flag = false;
		if (m_respawnTimer > 1f)
		{
			m_fastTurnTimer = 0.5f;
		}
		m_fastTurnTimer -= deltaTime;
		if (m_fastTurnTimer > 0f)
		{
			flag = true;
		}
		else
		{
			m_fastTurnTimer = 0f;
		}
		if (m_grapplePoint == null || flag)
		{
			float num = Vector3.Dot(m_position - m_trackPoint, m_trackNormal);
			float num2 = Vector3.Dot(m_forward, m_trackNormal);
			if (flag || num * num2 > 0f)
			{
				float num3 = ((!flag) ? (30f - 1.5f * Mathf.Min(m_trackForwardSpeed, 15f)) : 600f);
				m_rigidbody.AddTorque(m_up * ((0f - num3) * num2));
			}
		}
	}

	private void ApplyGrappleSteering()
	{
		float steerAngle = 0f;
		if (m_grapplePoint != null)
		{
			Vector3 vector = Quaternion.Inverse(m_rotation) * (m_grapplePointPos - m_position);
			if (vector.z < 0f)
			{
				float num = Mathf.Sign(vector.x) * Mathf.Atan2(0f - vector.z, Mathf.Abs(vector.x));
				steerAngle = num * m_grappleSteering * 57.29578f;
			}
		}
		for (int i = 0; i != 2; i++)
		{
			WheelCollider wheelCollider = m_wheels[i];
			wheelCollider.steerAngle = steerAngle;
		}
	}

	private float ApplyGrappling()
	{
		float num = m_grapplePoint.windRate;
		if (m_isInRumbleArea && m_grappleWindInRumbleZone > num)
		{
			num = m_grappleWindInRumbleZone;
		}
		if (num > 0f)
		{
			m_grappleDist -= m_grappleDist * num;
			m_grappleDist = Mathf.Max(m_grapplePoint.minGrappleLength, m_grappleDist);
		}
		Vector3 vector = m_forward * m_grappleHalfSpread;
		Vector3 vector2 = m_position + m_up * m_centerOfMassY;
		Vector3 vector3 = vector2 + vector;
		Vector3 vector4 = vector2 - vector;
		Vector3 vector5 = default(Vector3);
		vector5.x = m_grapplePointPos.x - vector3.x;
		vector5.y = 0f;
		vector5.z = m_grapplePointPos.z - vector3.z;
		Vector3 vector6 = default(Vector3);
		vector6.x = m_grapplePointPos.x - vector4.x;
		vector6.y = 0f;
		vector6.z = m_grapplePointPos.z - vector4.z;
		float num2 = Mathf.Sqrt(vector5.x * vector5.x + vector5.z * vector5.z);
		float num3 = 1f / num2;
		Vector3 vector7 = default(Vector3);
		vector7.x = vector5.x * num3;
		vector7.y = 0f;
		vector7.z = vector5.z * num3;
		float num4 = Mathf.Sqrt(vector6.x * vector6.x + vector6.z * vector6.z);
		float num5 = 1f / num4;
		Vector3 vector8 = default(Vector3);
		vector8.x = vector6.x * num5;
		vector8.y = 0f;
		vector8.z = vector6.z * num5;
		Vector3 pointVelocity = m_rigidbody.GetPointVelocity(vector3);
		Vector3 pointVelocity2 = m_rigidbody.GetPointVelocity(vector4);
		float num6 = 1f;
		if (m_grappleRampUpScale < 9999f)
		{
			num6 = Mathf.Clamp01(m_buttonDownTime * m_grappleRampUpScale);
		}
		float num7 = (num2 - m_grappleDist) * m_grappleForce;
		float num8 = Vector3.Dot(pointVelocity, vector7) * m_grappleDamping;
		if (num8 > 0f)
		{
			num8 = 0f;
		}
		num7 -= num8;
		num7 *= num6;
		num7 *= Mathf.Clamp01((num2 - m_grapplePoint.grappleForceRadius) * m_grapplePoint.grappleForceRamp);
		if (num7 > 0f)
		{
			m_rigidbody.AddForceAtPosition(vector7 * num7, vector3);
		}
		float num9 = (num4 - m_grappleDist) * m_grappleForce * m_backGrappleForceScale;
		float num10 = Vector3.Dot(pointVelocity2, vector8) * m_grappleDamping * m_backGrappleForceScale;
		if (num10 > 0f)
		{
			num10 = 0f;
		}
		else
		{
			m_backGrappleEnabled = true;
		}
		num9 -= num10;
		num9 *= num6;
		num9 *= Mathf.Clamp01((num4 - m_grapplePoint.grappleForceRadius) * m_grapplePoint.grappleForceRamp);
		if (m_backGrappleEnabled && num9 > 0f)
		{
			float num11 = vector7.x * m_forward.x + vector7.z * m_forward.z;
			if (num11 < 0f)
			{
				num11 = 1f + num11 / Mathf.Sqrt(m_forward.x * m_forward.x + m_forward.z * m_forward.z);
				num9 *= num11;
			}
			m_rigidbody.AddForceAtPosition(vector8 * num9, vector4);
		}
		return num7;
	}

	private void UpdateHandbrake(float deltaTime)
	{
		if (m_wheels == null)
		{
			return;
		}
		if (m_handbrakeOn)
		{
			m_wheelFrictionCurve.stiffness = Mathf.Lerp(m_wheelFrictionCurve.stiffness, 5f, deltaTime * 0.125f);
			for (int i = 0; i < 4; i++)
			{
				m_wheels[i].forwardFriction = m_wheelFrictionCurve;
				m_wheels[i].sidewaysFriction = m_wheelFrictionCurve;
				m_wheels[i].brakeTorque = 0.05f;
			}
			return;
		}
		m_wheelFrictionCurve.stiffness = 0f;
		for (int j = 0; j != 4; j++)
		{
			m_wheels[j].forwardFriction = m_wheelFrictionCurve;
			m_wheels[j].brakeTorque = 0f;
		}
		m_wheelFrictionCurve.stiffness = m_wheelFriction;
		for (int k = 0; k != 4; k++)
		{
			m_wheels[k].sidewaysFriction = m_wheelFrictionCurve;
		}
	}

	private void ApplyThrust(float deltaTime)
	{
		if (m_isOnSurface)
		{
			float num = m_forwardForce * m_thrustScale;
			if (m_launchThrustRemaining > 0f)
			{
				m_launchThrustRemaining -= deltaTime;
				num *= m_launchThrustScale;
			}
			if (m_currentGrappleForce > 0f)
			{
				num *= m_grapplePoint.grappleThrustScale;
			}
			m_rigidbody.AddForce(m_forward * num);
		}
	}

	private void ApplyBoost(float deltaTime)
	{
		if (m_allowBoostCountdown > 0f)
		{
			m_allowBoostCountdown -= deltaTime;
			if (m_allowBoostCountdown < 0f)
			{
				m_allowBoostCountdown = 0f;
			}
		}
		if (m_boostImpulse > 0f)
		{
			if (!HandbrakeOn)
			{
				Vector3 force = new Vector3(m_forward.x * m_boostImpulse, -2f, m_forward.z * m_boostImpulse) * 2f;
				m_rigidbody.AddForce(force, ForceMode.Impulse);
			}
			m_boostImpulse = 0f;
		}
	}

	[Conditional("DYNAMIC_TRACK_SEGMENTS")]
	private void ActivateSplineSegment(TrackSpline spline, int segment, bool enable)
	{
		Physics.IgnoreCollision(m_trackCollider, spline.TrackColliders[segment], !enable);
		Physics.IgnoreCollision(m_surroundCollider, spline.SurroundColliders[segment], !enable);
	}

	[Conditional("DYNAMIC_TRACK_SEGMENTS")]
	private void ActivateHub(TrackHub hub, bool enable)
	{
		Physics.IgnoreCollision(m_trackCollider, hub.TrackCollider, !enable);
		Physics.IgnoreCollision(m_surroundCollider, hub.SurroundCollider, !enable);
	}

	[Conditional("DYNAMIC_TRACK_SEGMENTS")]
	private void SplineSegmentInit()
	{
		GameObject[] array = GameObject.FindGameObjectsWithTag("TrackSpline");
		for (int i = 0; i != array.Length; i++)
		{
			TrackSpline component = array[i].GetComponent<TrackSpline>();
			Collider[] surroundColliders = component.SurroundColliders;
			Collider[] trackColliders = component.TrackColliders;
			for (int j = 0; j != surroundColliders.Length; j++)
			{
				Physics.IgnoreCollision(m_trackCollider, trackColliders[j]);
				Physics.IgnoreCollision(m_surroundCollider, surroundColliders[j]);
			}
		}
		GameObject[] array2 = GameObject.FindGameObjectsWithTag("TrackHub");
		for (int k = 0; k != array2.Length; k++)
		{
			TrackHub component2 = array2[k].GetComponent<TrackHub>();
			Physics.IgnoreCollision(m_trackCollider, component2.TrackCollider);
			Physics.IgnoreCollision(m_surroundCollider, component2.SurroundCollider);
		}
		int num = m_currentTrackSpline.SurroundColliders.Length;
		m_currentTrackSegment = (int)(m_currentT + 0.5f);
		if (m_currentTrackSegment <= 0)
		{
			if (m_currentTrackSpline.HubAtHead != null)
			{
				ActivateHub(m_currentTrackSpline.HubAtHead, true);
			}
			else
			{
				ActivateSplineSegment(m_currentTrackSpline, num - 1, true);
			}
			ActivateSplineSegment(m_currentTrackSpline, 0, true);
		}
		else if (m_currentTrackSegment >= num)
		{
			if (m_currentTrackSpline.HubAtTail != null)
			{
				ActivateHub(m_currentTrackSpline.HubAtTail, true);
			}
			else
			{
				ActivateSplineSegment(m_currentTrackSpline, 0, true);
			}
			ActivateSplineSegment(m_currentTrackSpline, num - 1, true);
		}
		else
		{
			ActivateSplineSegment(m_currentTrackSpline, m_currentTrackSegment - 1, true);
			ActivateSplineSegment(m_currentTrackSpline, m_currentTrackSegment, true);
		}
	}

	[Conditional("DYNAMIC_TRACK_SEGMENTS")]
	private void SplineSegmentToHubTransition()
	{
		foreach (TrackSpline incomingSpline in m_currentTrackHub.incomingSplines)
		{
			ActivateSplineSegment(incomingSpline, incomingSpline.SurroundColliders.Length - 1, true);
		}
		foreach (TrackSpline outgoingSpline in m_currentTrackHub.outgoingSplines)
		{
			ActivateSplineSegment(outgoingSpline, 0, true);
		}
	}

	[Conditional("DYNAMIC_TRACK_SEGMENTS")]
	private void HubToSplineSegmentTransition(bool forwards)
	{
		m_currentTrackSegment = ((!forwards) ? (m_currentTrackSpline.SurroundColliders.Length - 1) : 0);
		foreach (TrackSpline incomingSpline in m_currentTrackHub.incomingSplines)
		{
			if (m_currentTrackSpline != incomingSpline || m_currentTrackSegment == 0)
			{
				ActivateSplineSegment(incomingSpline, incomingSpline.SurroundColliders.Length - 1, false);
			}
		}
		foreach (TrackSpline outgoingSpline in m_currentTrackHub.outgoingSplines)
		{
			if (m_currentTrackSpline != outgoingSpline || m_currentTrackSegment != 0)
			{
				ActivateSplineSegment(outgoingSpline, 0, false);
			}
		}
	}

	[Conditional("DYNAMIC_TRACK_SEGMENTS")]
	private void SplineSegmentTransition(int nextSegment)
	{
		int num = m_currentTrackSpline.SurroundColliders.Length;
		if (m_currentTrackSpline.HubAtTail != null)
		{
			if (nextSegment <= 0)
			{
				ActivateHub(m_currentTrackSpline.HubAtHead, true);
				ActivateSplineSegment(m_currentTrackSpline, 0, false);
			}
			else if (nextSegment >= num)
			{
				ActivateHub(m_currentTrackSpline.HubAtTail, true);
				ActivateSplineSegment(m_currentTrackSpline, num - 2, false);
			}
			else if (nextSegment > m_currentTrackSegment)
			{
				ActivateSplineSegment(m_currentTrackSpline, nextSegment, true);
				if (nextSegment > 1)
				{
					ActivateSplineSegment(m_currentTrackSpline, nextSegment - 2, false);
				}
				else
				{
					ActivateHub(m_currentTrackSpline.HubAtHead, false);
				}
			}
			else
			{
				ActivateSplineSegment(m_currentTrackSpline, nextSegment - 1, true);
				if (nextSegment < num - 1)
				{
					ActivateSplineSegment(m_currentTrackSpline, nextSegment + 1, false);
				}
				else
				{
					ActivateHub(m_currentTrackSpline.HubAtTail, false);
				}
			}
			return;
		}
		nextSegment %= num;
		if (nextSegment != m_currentTrackSegment % num)
		{
			if ((m_currentTrackSegment + 1) % num == nextSegment)
			{
				ActivateSplineSegment(m_currentTrackSpline, nextSegment, true);
				ActivateSplineSegment(m_currentTrackSpline, (num + nextSegment - 2) % num, false);
			}
			else
			{
				ActivateSplineSegment(m_currentTrackSpline, (num + nextSegment - 1) % num, true);
				ActivateSplineSegment(m_currentTrackSpline, (nextSegment + 1) % num, false);
			}
		}
	}

	public Vector3 GetTrackPoint()
	{
		if (m_currentTrackSpline != null)
		{
			return m_currentTrackSpline.PointAtT(m_currentT);
		}
		return m_currentTrackHub.PointAt(m_currentT, base.CurrentHubSector);
	}

	public void GetTrackInfo(out Vector3 point, out Vector3 tangent, out Vector3 normal, out float width, out float centreHeight)
	{
		if (m_currentTrackSpline != null)
		{
			m_currentTrackSpline.InfoAtT(m_currentT, out point, out tangent, out normal, out width);
			Vector3Utils.Normalise(ref tangent);
			centreHeight = m_currentTrackSpline.HeightAt(m_currentT, 0f);
		}
		else if (m_currentTrackHub != null)
		{
			m_currentTrackHub.InfoAt(m_currentT, base.CurrentHubSector, out point, out tangent, out normal, out width);
			centreHeight = m_currentTrackHub.HeightAt(0f);
		}
		else
		{
			point = Vector3.zero;
			tangent = Vector3.forward;
			normal = Vector3.right;
			width = 1000f;
			centreHeight = 0f;
		}
	}

	public GrapplePoint GetTrackGrapplePoint()
	{
		if ((bool)m_grapplePointOverride)
		{
			return m_grapplePointOverride;
		}
		if (m_currentTrackSpline != null)
		{
			return m_currentTrackSpline.GrapplePointAt(m_currentT);
		}
		if (m_currentTrackHub.m_allowGrapplePointStart != null)
		{
			return m_currentTrackHub.m_allowGrapplePointStart;
		}
		return (!m_currentTrackHub.m_grappleContinuity) ? null : m_grapplePoint;
	}

	public float GetTrackRumbleAreaProp()
	{
		if (m_currentTrackSpline != null)
		{
			return m_currentTrackSpline.rumbleAreaProp;
		}
		return 1f;
	}

	public float GetTrackRumbleAvoidProp()
	{
		if (m_currentTrackSpline != null)
		{
			return m_currentTrackSpline.rumbleAvoidProp;
		}
		return 1f;
	}

	public bool IsInGrapplePoint(GrapplePoint gp)
	{
		if (gp == m_grapplePointOverride)
		{
			return true;
		}
		if (m_currentTrackSpline != null)
		{
			return m_currentTrackSpline.IsInGrappleArea(gp, m_currentT);
		}
		return m_currentTrackHub.m_grappleContinuity && gp == m_grapplePoint;
	}

	public override void RaceEvent_Launch()
	{
		base.RaceEvent_Launch();
		HandbrakeOn = false;
		m_boostImpulse = m_launchImpulse * 2f;
		m_launchThrustRemaining = m_launchThrustTime;
	}

	private void OnCollisionEnter(Collision _collision)
	{
		Vehicle vehicle = null;
		if (_collision.rigidbody == null)
		{
			if (_collision.gameObject.layer == m_trackSurroundLayer)
			{
				float num = Mathf.Abs(Vector3.Dot(_collision.relativeVelocity, base.TrackNormal));
				if (num > 2f)
				{
					SendSurroundCollisionEvent(num > 4f);
				}
				m_dropMiniReward = true;
				m_validGrappleMiniReward = false;
			}
			else if ((_collision.impulse.y / Time.fixedDeltaTime) > 5f)
			{
				SendTrackCollisionEvent();
			}
		}
		else
		{
			vehicle = _collision.gameObject.GetComponent<Vehicle>();
			float sqrMagnitude = _collision.relativeVelocity.sqrMagnitude;
			if (m_visuals != null && _collision.contacts.Length > 0 && sqrMagnitude > 4f)
			{
				ContactPoint[] contacts = _collision.contacts;
				Vector3 velocity = (_collision.rigidbody.linearVelocity + m_rigidbody.linearVelocity) * 0.5f;
				Vector3 point = contacts[0].point;
				Vector3 normal = contacts[0].normal;
				if (contacts.Length > 1)
				{
					for (int i = 1; i != contacts.Length; i++)
					{
						point += contacts[i].point;
						normal += contacts[i].normal;
					}
					point *= 1f / (float)contacts.Length;
					normal.Normalize();
				}
				bool isMajor = sqrMagnitude > 16f;
				bool playAudio = vehicle == null || vehicle.Visuals == null || vehicle.Visuals.VolumeScale <= m_visuals.VolumeScale || vehicle.PlayerIndex > base.PlayerIndex;
				SendVehicleCollisionEvent(point, normal, velocity, isMajor, playAudio);
				if (HasUpgrade(Upgrade.BatteringRam) && m_batteringRamCountdown <= 0f)
				{
					m_batteringRamCountdown = 0.25f;
					Vector3 lhs = _collision.rigidbody.position - m_rigidbody.position;
					float num2 = Vector3.Dot(lhs, m_trackTangent);
					if (num2 >= 0f)
					{
						float num3 = Vector3.Dot(lhs, m_trackNormal);
						Vector3 vector;
						Vector3 vector2;
						if (num3 >= 0f)
						{
							vector = m_trackNormal;
							vector2 = Vector3.up;
						}
						else
						{
							vector = -m_trackNormal;
							vector2 = Vector3.down;
						}
						_collision.rigidbody.AddForce(vector * m_batteringRamSpeedLateral - m_trackTangent * m_batteringRamSpeedBack, ForceMode.VelocityChange);
						_collision.rigidbody.AddTorque(vector2 * m_batteringRamSpin, ForceMode.VelocityChange);
						SendBatteringRamEvent(point, vector);
					}
				}
			}
		}
		if (vehicle != null && OnHitByOtherVehicleEvent != null)
		{
			OnHitByOtherVehicleEvent(_collision, this, vehicle);
		}
	}

	public void ButtonDown()
	{
		if (!HandbrakeOn)
		{
			m_grapplePriming = true;
		}
		m_isButtonDown = true;
		m_buttonDownTime = 0f;
	}

	public void ButtonUp()
	{
		m_grapplePriming = false;
		m_isButtonDown = false;
		m_buttonDownTime = 0f;
	}

	public bool TriggerBoost(float _impulse)
	{
		if (m_allowBoostCountdown <= 1E-06f)
		{
			m_allowBoostCountdown = 0.25f;
			m_boostImpulse = _impulse * m_thrustScale;
			SendBoostEvent();
			return true;
		}
		return false;
	}

	public void OilSlick(OilSlick _oilSlick)
	{
		if (m_oilSlickTime <= 0f)
		{
			m_oilSlickTime = _oilSlick.m_time;
			StartCoroutine(DoOilSlick());
		}
		else
		{
			m_oilSlickTime += _oilSlick.m_time;
		}
		SendOilSlickEvent(_oilSlick);
	}

	private IEnumerator DoOilSlick()
	{
		float oldThrustScale = m_thrustScale;
		m_thrustScale = oldThrustScale * 0.01f;
		while (m_oilSlickTime > 0f)
		{
			if (m_oilSlickTime > 0.5f)
			{
				yield return YieldCache.WaitForHalfSecond;
				m_oilSlickTime -= 0.5f;
			}
			else
			{
				yield return null;
				m_oilSlickTime -= Time.deltaTime;
			}
		}
		m_thrustScale = oldThrustScale;
		m_oilSlickTime = 0f;
	}

	public void TriggerObstacleHit(TrackObstacle _obstacle, float _dragScalar)
	{
		if (HasUpgrade(Upgrade.BatteringRam))
		{
			_dragScalar *= m_batteringRamObstacleScale;
		}
		m_dropMiniReward = true;
		m_validGrappleMiniReward = false;
		if (m_obstacleEffectTime <= 0f)
		{
			m_obstacleEffectTime = 0.15f;
			StartCoroutine(DoTrackObstacle(_dragScalar));
		}
		else
		{
			m_obstacleEffectTime += 0.15f;
		}
		SendObstacleHitEvent(_obstacle);
	}

	public TryKillResult TryKill()
	{
		if (base.IsDead)
		{
			return TryKillResult.None;
		}
		if (m_shieldActive)
		{
			return TryKillResult.Shielded;
		}
		if (HasUpgrade(Upgrade.ChomperShield))
		{
			UseUpgrade(Upgrade.ChomperShield);
			SendShieldEvent();
			TriggerBoost(m_shieldBoostImpluse);
			m_shieldActive = true;
			Invoke("StopShielding", m_shieldDuration);
			return TryKillResult.Shielded;
		}
		SendDeathEvent();
		ButtonUp();
		HandbrakeOn = true;
		return TryKillResult.Killed;
	}

	private void StopShielding()
	{
		m_shieldActive = false;
	}

	private IEnumerator DoTrackObstacle(float _dragScalar)
	{
		float oldQuadraticDrag = m_quadraticDrag;
		m_quadraticDrag *= _dragScalar;
		while (m_obstacleEffectTime > 0f)
		{
			yield return null;
			m_obstacleEffectTime -= Time.deltaTime;
		}
		m_quadraticDrag = oldQuadraticDrag;
		m_obstacleEffectTime = 0f;
	}

	public void TriggerPowerBox(float _impulse, bool _dischargeAll)
	{
		m_triggerPowerBox = true;
		m_powerBoxDischargeAll |= _dischargeAll;
	}

	private void CompleteLap(float _diffT, float _deltaT)
	{
		base.NumLapsDone++;
		float num = 0f;
		float num2 = m_lapTime;
		if (_deltaT > 1E-06f)
		{
			num = _diffT / _deltaT * Time.deltaTime;
			num2 -= num;
		}
		m_waypoint = 0;
		m_lapTime = num;
		SendLapEvent(this, num2);
		if (OnNewLapEvent != null)
		{
			OnNewLapEvent();
		}
	}

	public override IUpgrade SetUpgrade(int _slot, Upgrade _type)
	{
		if (m_upgradeSlots[_slot] != Upgrade.None)
		{
			Debug.LogError("Upgrade slot already set");
			return null;
		}
		m_upgradeSlots[_slot] = _type;
		switch (_type)
		{
		default:
			return null;
		case Upgrade.Boost:
		{
			GameObject original7 = Resources.Load("Prefabs/Upgrades/Boost") as GameObject;
			GameObject gameObject7 = UnityEngine.Object.Instantiate(original7) as GameObject;
			gameObject7.transform.parent = base.transform;
			gameObject7.transform.localPosition = Vector3.zero;
			gameObject7.transform.localRotation = Quaternion.identity;
			BoostUpgrade component4 = gameObject7.GetComponent<BoostUpgrade>();
			component4.Initialise(this);
			return component4;
		}
		case Upgrade.PassiveMagnet:
		{
			GameObject original6 = Resources.Load("Prefabs/Upgrades/PassiveMagnet") as GameObject;
			GameObject gameObject6 = UnityEngine.Object.Instantiate(original6) as GameObject;
			gameObject6.transform.parent = base.transform;
			gameObject6.transform.localPosition = Vector3.zero;
			gameObject6.transform.localRotation = Quaternion.identity;
			return null;
		}
		case Upgrade.ActivatedMagnet:
		{
			GameObject original5 = Resources.Load("Prefabs/Upgrades/ActivatedMagnet") as GameObject;
			GameObject gameObject5 = UnityEngine.Object.Instantiate(original5) as GameObject;
			gameObject5.transform.parent = base.transform;
			gameObject5.transform.localPosition = Vector3.zero;
			gameObject5.transform.localRotation = Quaternion.identity;
			return gameObject5.GetComponent<MagnetUpgrade>();
		}
		case Upgrade.Harpoon:
		{
			GameObject original4 = Resources.Load("Prefabs/Upgrades/Harpoon") as GameObject;
			GameObject gameObject4 = UnityEngine.Object.Instantiate(original4) as GameObject;
			gameObject4.transform.parent = base.transform;
			gameObject4.transform.localPosition = Vector3.zero;
			gameObject4.transform.localRotation = Quaternion.identity;
			HarpoonUpgrade component3 = gameObject4.GetComponent<HarpoonUpgrade>();
			component3.Initialise(this);
			return component3;
		}
		case Upgrade.MiniRewardShield:
		{
			GameObject original3 = Resources.Load("Prefabs/Upgrades/MiniRewardShield") as GameObject;
			GameObject gameObject3 = UnityEngine.Object.Instantiate(original3) as GameObject;
			gameObject3.transform.parent = base.transform;
			gameObject3.transform.localPosition = Vector3.zero;
			gameObject3.transform.localRotation = Quaternion.identity;
			return null;
		}
		case Upgrade.Heavy:
			m_rigidbody.mass *= 1.5f;
			m_thrustScale *= 1.6f;
			return null;
		case Upgrade.PassiveRepulsor:
		{
			GameObject original2 = Resources.Load("Prefabs/Upgrades/PassiveRepulsor") as GameObject;
			GameObject gameObject2 = UnityEngine.Object.Instantiate(original2) as GameObject;
			gameObject2.transform.parent = base.transform;
			gameObject2.transform.localPosition = Vector3.zero;
			gameObject2.transform.localRotation = Quaternion.identity;
			RepulsorUpgrade component2 = gameObject2.GetComponent<RepulsorUpgrade>();
			component2.Initialise(this);
			return null;
		}
		case Upgrade.ActivatedRepulsor:
		{
			GameObject original = Resources.Load("Prefabs/Upgrades/ActivatedRepulsor") as GameObject;
			GameObject gameObject = UnityEngine.Object.Instantiate(original) as GameObject;
			gameObject.transform.parent = base.transform;
			gameObject.transform.localPosition = Vector3.zero;
			gameObject.transform.localRotation = Quaternion.identity;
			RepulsorUpgrade component = gameObject.GetComponent<RepulsorUpgrade>();
			component.Initialise(this);
			return component;
		}
		}
	}

	public bool HasUpgrade(Upgrade _type)
	{
		for (int i = 0; i < m_upgradeSlots.Length; i++)
		{
			if (_type == m_upgradeSlots[i])
			{
				return true;
			}
		}
		return false;
	}

	public int UseUpgrade(Upgrade _type)
	{
		for (int i = 0; i < m_upgradeSlots.Length; i++)
		{
			if (_type == m_upgradeSlots[i])
			{
				m_upgradeSlots[i] = Upgrade.None;
				return i;
			}
		}
		return -1;
	}

	public bool UseUpgrade(int _slotIndex)
	{
		if (m_upgradeSlots[_slotIndex] != Upgrade.None)
		{
			m_upgradeSlots[_slotIndex] = Upgrade.None;
			return true;
		}
		return false;
	}

	public WheelCollider GetWheel(Wheel _wheel)
	{
		return m_wheels[(int)_wheel];
	}

	public override Vector3 GetWheelHitPosition(Wheel _wheel)
	{
		return m_transform.TransformPoint(m_wheelHitPositions[(int)_wheel]);
	}

	public override bool IsWheelGrounded(Wheel _wheel)
	{
		return m_wheelIsGrounded[(int)_wheel];
	}
}
