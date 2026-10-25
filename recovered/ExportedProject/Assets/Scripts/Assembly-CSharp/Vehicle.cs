using System;
using UnityEngine;
using VehicleTypes;

public abstract class Vehicle : MonoBehaviour, DynamicCamera.Vehicle
{
	public enum Upgrade
	{
		None = 0,
		ChomperShield = 1,
		Boost = 2,
		BatteringRam = 3,
		PassiveMagnet = 4,
		ActivatedMagnet = 5,
		Harpoon = 6,
		MiniRewardShield = 7,
		Heavy = 8,
		PassiveRepulsor = 9,
		ActivatedRepulsor = 10
	}

	public interface IUpgrade
	{
		bool Expended { get; }

		bool CanBeActivated { get; }

		float Charge { get; }

		void Activate();
	}

	public delegate void OnVehicleCollisionDelegate(Vector3 point, Vector3 normal, Vector3 velocity, bool isMajor, bool playAudio);

	public delegate void OnBatteringRamDelegate(Vector3 _point, Vector3 _direction);

	public const float c_vehicleLength = 3.6f;

	public static Color[] s_playerColours = new Color[8]
	{
		new Color(0.5f, 0.1f, 0.1f),
		new Color(0.1f, 0.5f, 0.1f),
		new Color(0.1f, 0.1f, 0.5f),
		new Color(0.5f, 0.5f, 0.1f),
		new Color(0.5f, 0.1f, 0.5f),
		new Color(0.1f, 0.5f, 0.5f),
		new Color(0.5f, 0.5f, 0.5f),
		new Color(0.1f, 0.1f, 0.1f)
	};

	public static Color[] s_brightPlayerColours = new Color[8]
	{
		new Color(0.78f, 0f, 0f),
		new Color(0f, 0.78f, 0f),
		new Color(0f, 0f, 0.78f),
		new Color(0.78f, 0.78f, 0f),
		new Color(0.78f, 0f, 0.78f),
		new Color(0f, 0.78f, 0.78f),
		new Color(0.78f, 0.78f, 0.78f),
		new Color(0f, 0f, 0f)
	};

	protected VehicleVisuals m_visuals;

	protected Transform m_visualsTransform;

	protected TrackSpline m_currentTrackSpline;

	protected TrackHub m_currentTrackHub;

	protected float m_currentT;

	protected int m_currentHubSector;

	protected Vector3 m_position;

	protected Quaternion m_rotation;

	protected Vector3 m_forward;

	protected Vector3 m_up;

	protected Vector3 m_right;

	protected Vector3 m_trackPoint;

	protected Vector3 m_trackTangent;

	protected Vector3 m_trackNormal;

	protected float m_trackWidth;

	protected float m_trackCentreHeight;

	protected float m_trackForwardSpeed;

	protected GrapplePoint m_grapplePoint;

	protected float m_currentGrappleForce;

	protected float m_lapTime;

	protected int m_waypoint;

	protected TrackEdgeType m_innerEdgeType = TrackEdgeType.Snow;

	protected TrackEdgeType m_outerEdgeType = TrackEdgeType.Snow;

	public Color Colour
	{
		get
		{
			if (NonCompetitive)
			{
				return Color.grey;
			}
			return s_playerColours[ColourIndex];
		}
	}

	public Color BrightColour
	{
		get
		{
			if (NonCompetitive)
			{
				return Color.grey;
			}
			return s_brightPlayerColours[ColourIndex];
		}
	}

	public int ColourIndex { get; set; }

	public int PlayerIndex { get; set; }

	public bool NonCompetitive { get; set; }

	public bool IsGhost { get; set; }

	public bool IsDead { get; protected set; }

	public int NumLapsDone { get; protected set; }

	public bool EnableMiniRewards { get; protected set; }

	public bool EnableEasyRewards { get; set; }

	public VehicleVisuals Visuals
	{
		get
		{
			return m_visuals;
		}
	}

	public bool EnableFrustumCulling
	{
		set
		{
			m_visuals.EnableFrustumCulling = value;
		}
	}

	public TrackSpline CurrentTrackSpline
	{
		get
		{
			return m_currentTrackSpline;
		}
	}

	public TrackHub CurrentTrackHub
	{
		get
		{
			return m_currentTrackHub;
		}
	}

	public float CurrentT
	{
		get
		{
			return m_currentT;
		}
	}

	public int CurrentHubSector
	{
		get
		{
			return m_currentHubSector;
		}
	}

	public int LastWaypoint
	{
		get
		{
			return m_waypoint;
		}
	}

	public virtual Vector3 Position
	{
		get
		{
			return m_position;
		}
	}

	public virtual Quaternion Rotation
	{
		get
		{
			return m_rotation;
		}
	}

	public virtual Vector3 Forward
	{
		get
		{
			return m_forward;
		}
	}

	public virtual Vector3 Up
	{
		get
		{
			return m_up;
		}
	}

	public virtual Vector3 Right
	{
		get
		{
			return m_right;
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

	public float TrackCentreHeight
	{
		get
		{
			return m_trackCentreHeight;
		}
	}

	public float TrackForwardSpeed
	{
		get
		{
			return m_trackForwardSpeed;
		}
	}

	public GrapplePoint GrapplePoint
	{
		get
		{
			return m_grapplePoint;
		}
	}

	public float GrappleForce
	{
		get
		{
			return m_currentGrappleForce;
		}
	}

	public TrackEdgeType InnerEdgeType
	{
		get
		{
			return m_innerEdgeType;
		}
	}

	public TrackEdgeType OuterEdgeType
	{
		get
		{
			return m_outerEdgeType;
		}
	}

	public TrackLocation Location
	{
		get
		{
			TrackLocation result = default(TrackLocation);
			result.m_spline = m_currentTrackSpline;
			result.m_hub = m_currentTrackHub;
			result.m_t = m_currentT;
			result.m_sector = m_currentHubSector;
			return result;
		}
	}

	public float CurrentLapTime
	{
		get
		{
			return Mathf.Max(0f, m_lapTime - UnityEngine.Random.Range(0f, Time.fixedDeltaTime));
		}
	}

	public abstract bool IsLocalHuman { get; }

	public abstract bool ScaleAudioVolume { get; }

	public abstract bool UsesHeadlights { get; }

	public abstract Vector3 Velocity { get; }

	public abstract Vector3 GrappleFrontPos { get; }

	public abstract float OilSlickTime { get; }

	public event Action OnBoostEvent;

	public event Action OnCollectMiniReward;

	public event Action OnDropMiniReward;

	public event Action<bool> OnDeployMiniReward;

	public event Action<Vehicle, float> OnLapEvent;

	public event Action<GrapplePoint> OnGrappleEvent;

	public event Action<GrapplePoint> OnGrappleReleaseEvent;

	public event Action<GameObject> OnDeathEvent;

	public event Action<OilSlick> OnOilSlickEvent;

	public event Action<int> OnPickupEvent;

	public event Action<TrackObstacle> OnObstacleHitEvent;

	public event Action<bool> OnSurroundCollisionEvent;

	public event Action OnTrackCollisionEvent;

	public event Action OnShieldEvent;

	public event OnVehicleCollisionDelegate OnVehicleCollisionEvent;

	public event OnBatteringRamDelegate OnBatteringRamEvent;

	public static Color GetColourForPlayer(int _colourIndex)
	{
		return s_playerColours[_colourIndex];
	}

	public static Color GetBrightColourForPlayer(int _colourIndex)
	{
		return s_brightPlayerColours[_colourIndex];
	}

	public void SetVisualsGO(GameObject _visualsGO)
	{
		m_visuals = _visualsGO.GetComponent<VehicleVisuals>();
		m_visualsTransform = m_visuals.transform;
	}

	public virtual void RaceEvent_Launch()
	{
		m_lapTime = 0f;
	}

	public virtual IUpgrade SetUpgrade(int _slot, Upgrade _type)
	{
		return null;
	}

	public abstract Vector3 GetWheelHitPosition(Wheel _wheel);

	public abstract bool IsWheelGrounded(Wheel _wheel);

	protected virtual void OnDestroy()
	{
		OnLapEvent = null;
	}

	public void SendPickupEvent(int _index)
	{
		if (OnPickupEvent != null)
		{
			OnPickupEvent(_index);
		}
	}

	public void SendBoostEvent()
	{
		if (OnBoostEvent != null)
		{
			OnBoostEvent();
		}
	}

	protected void SendLapEvent(Vehicle _v, float _t)
	{
		if (OnLapEvent != null)
		{
			OnLapEvent(_v, _t);
		}
	}

	protected void SendGrappleEvent(GrapplePoint _p)
	{
		if (OnGrappleEvent != null)
		{
			OnGrappleEvent(_p);
		}
	}

	protected void SendGrappleReleaseEvent(GrapplePoint _p)
	{
		if (OnGrappleReleaseEvent != null)
		{
			OnGrappleReleaseEvent(_p);
		}
	}

	public void SendDeathEvent()
	{
		if (OnDeathEvent != null)
		{
			OnDeathEvent(base.gameObject);
		}
		IsDead = true;
		base.gameObject.SetActiveRecursively(false);
	}

	public void SendShieldEvent()
	{
		if (OnShieldEvent != null)
		{
			OnShieldEvent();
		}
	}

	protected void SendOilSlickEvent(OilSlick _os)
	{
		if (OnOilSlickEvent != null)
		{
			OnOilSlickEvent(_os);
		}
	}

	public void SendCollectMiniRewardEvent()
	{
		if (OnCollectMiniReward != null)
		{
			OnCollectMiniReward();
		}
	}

	public void SendDropMiniRewardEvent()
	{
		if (OnDropMiniReward != null)
		{
			OnDropMiniReward();
		}
	}

	public void SendDeployMiniRewardEvent(bool _dischargeAll)
	{
		if (OnDeployMiniReward != null)
		{
			OnDeployMiniReward(_dischargeAll);
		}
	}

	protected void SendObstacleHitEvent(TrackObstacle _obstacle)
	{
		if (OnObstacleHitEvent != null)
		{
			OnObstacleHitEvent(_obstacle);
		}
	}

	public void SendSurroundCollisionEvent(bool isMajor)
	{
		if (OnSurroundCollisionEvent != null)
		{
			OnSurroundCollisionEvent(isMajor);
		}
	}

	public void SendTrackCollisionEvent()
	{
		if (OnTrackCollisionEvent != null)
		{
			OnTrackCollisionEvent();
		}
	}

	public void SendVehicleCollisionEvent(Vector3 point, Vector3 normal, Vector3 velocity, bool isMajor, bool playAudio)
	{
		if (OnVehicleCollisionEvent != null)
		{
			OnVehicleCollisionEvent(point, normal, velocity, isMajor, playAudio);
		}
	}

	public void SendBatteringRamEvent(Vector3 _point, Vector3 _direction)
	{
		if (OnBatteringRamEvent != null)
		{
			OnBatteringRamEvent(_point, _direction);
		}
	}

	protected void UpdateTrackEdgeData()
	{
		if (CurrentTrackSpline != null)
		{
			TrackEdgeData edgeData = CurrentTrackSpline.EdgeData;
			if (edgeData != null)
			{
				float currentT = CurrentT;
				m_innerEdgeType = edgeData.GetInnerWallType(currentT);
				m_outerEdgeType = edgeData.GetOuterWallType(currentT);
			}
		}
	}
}
