using System.Collections;
using UnityEngine;

[AddComponentMenu("AI/Vehicle AI Controller")]
[RequireComponent(typeof(Vehicle))]
public class VehicleAIController : MonoBehaviour
{
	public delegate void OnBadGrapple();

	private const float c_minGrappleSpeed = 2f;

	private const float c_minGrappleSpeedSq = 4f;

	private static readonly string[] c_grudgeShouts = new string[3] { "ID_GrudgeStart0", "ID_GrudgeStart1", "ID_GrudgeStart2" };

	private AISettings m_aiSettings;

	private GlobalAI m_globalAI;

	private RubberBanding m_rubberBanding = RubberBanding.None;

	private GrapplePoint m_lastGrapplePoint;

	private float m_lastGrappleTimer;

	private PhysicsVehicle m_vehicle;

	private float m_actionTimer;

	private float m_defaultForwardForce;

	private AIDifficulty m_dynamicDifficultyLevel;

	private bool m_dynamicDifficulty;

	private bool m_hasGrappled;

	private bool m_noMistakes;

	private bool m_skipMistake;

	private PositionPopup m_grudgePopup;

	public AIDifficulty m_difficulty;

	public bool DynamicDifficulty
	{
		get
		{
			return m_dynamicDifficulty;
		}
		set
		{
			if (m_dynamicDifficulty == value)
			{
				return;
			}
			m_dynamicDifficulty = value;
			if (m_dynamicDifficulty)
			{
				m_globalAI = GlobalAI.Get();
				if (m_globalAI != null)
				{
					m_globalAI.RegisterAI(m_vehicle);
				}
				m_vehicle.OnLapEvent += OnLap;
			}
			else
			{
				if (m_globalAI != null)
				{
					m_globalAI.UnregisterAI(m_vehicle);
					m_globalAI = null;
				}
				m_vehicle.OnLapEvent -= OnLap;
			}
			UpdateDynamicDifficultyLevel();
		}
	}

	public bool NoMistakes
	{
		get
		{
			return m_noMistakes;
		}
		set
		{
			m_noMistakes = value;
		}
	}

	public event OnBadGrapple OnBadGrappleEvent;

	private void Awake()
	{
		m_vehicle = GetComponent<PhysicsVehicle>();
		m_aiSettings = AISettings.Load();
		m_actionTimer = m_aiSettings.m_reactionTime;
	}

	private IEnumerator Start()
	{
		m_defaultForwardForce = m_vehicle.m_forwardForce;
		UpdateDynamicDifficultyLevel();
		if (m_vehicle != null)
		{
			m_vehicle.OnHitByOtherVehicleEvent += OnHitByOtherVehicle;
		}
		Object popupPrefab = Resources.Load("Prefabs/Popups/GrudgePopup");
		GameObject popupGO = (GameObject)Object.Instantiate(popupPrefab);
		m_grudgePopup = popupGO.GetComponent<PositionPopup>();
		yield return null;
		SuppressCurrentGrapple();
	}

	private void OnDestroy()
	{
		if (m_vehicle != null)
		{
			m_vehicle.OnHitByOtherVehicleEvent -= OnHitByOtherVehicle;
		}
		if (m_dynamicDifficulty && m_vehicle != null)
		{
			if (m_globalAI != null)
			{
				m_globalAI.UnregisterAI(m_vehicle);
			}
			m_vehicle.OnLapEvent -= OnLap;
		}
		m_globalAI = null;
		m_vehicle = null;
		m_lastGrapplePoint = null;
		if (m_grudgePopup != null)
		{
			m_grudgePopup.Target = null;
			Object.Destroy(m_grudgePopup.gameObject);
			m_grudgePopup = null;
		}
	}

	private void FixedUpdate()
	{
		m_actionTimer -= Time.deltaTime;
		if (m_actionTimer <= 0f)
		{
			DoSimpleAI();
		}
	}

	private bool IsInGrappleRegion(GrapplePoint _grapplePoint)
	{
		AIGrapplePoint aIGrapplePoint = _grapplePoint.AIGrapplePoint;
		if (aIGrapplePoint != null)
		{
			float trackWidth = m_vehicle.TrackWidth;
			float value = Vector3.Dot(base.transform.position - m_vehicle.TrackPoint, m_vehicle.TrackNormal);
			AIGrapplePoint.Data data = aIGrapplePoint.GetData(m_dynamicDifficultyLevel);
			float t = Mathf.InverseLerp(0f - trackWidth, trackWidth, value);
			float num = Mathf.Lerp(data.m_innerStartT, data.m_outerStartT, t);
			float num2 = Mathf.Lerp(data.m_innerEndT, data.m_outerEndT, t);
			float currentT = m_vehicle.CurrentT;
			return currentT >= num && currentT <= num2;
		}
		return false;
	}

	private void DoSimpleAI()
	{
		GrapplePoint grapplePoint = m_vehicle.GrapplePoint;
		if (grapplePoint == null)
		{
			grapplePoint = m_vehicle.GetTrackGrapplePoint();
		}
		if (m_vehicle.IsPressing && !m_vehicle.IsGrappling)
		{
			ButtonUpAction();
			return;
		}
		bool flag = base.rigidbody.velocity.sqrMagnitude >= 4f;
		if (!flag && m_vehicle.IsGrappling)
		{
			ButtonUpAction();
			if (OnBadGrappleEvent != null)
			{
				OnBadGrappleEvent();
			}
			return;
		}
		if (m_vehicle.IsGrappling)
		{
			m_lastGrapplePoint = grapplePoint;
			m_lastGrappleTimer = 0f;
			if (!IsInGrappleRegion(grapplePoint))
			{
				ButtonUpAction();
			}
			return;
		}
		if (m_lastGrapplePoint != null)
		{
			if (!m_hasGrappled)
			{
				m_hasGrappled = true;
				UpdateDynamicDifficultyLevel();
			}
			m_lastGrappleTimer -= Time.deltaTime;
			if (m_lastGrappleTimer <= 0f)
			{
				m_lastGrapplePoint = null;
				m_lastGrappleTimer = 0f;
			}
		}
		if (flag && grapplePoint != null && grapplePoint != m_lastGrapplePoint && IsInGrappleRegion(grapplePoint))
		{
			ButtonDownAction();
		}
	}

	private void ButtonDownAction()
	{
		if (m_dynamicDifficulty && !m_noMistakes && !m_skipMistake)
		{
			float num = m_aiSettings.m_difficultyLevelMistakeChance[(int)m_dynamicDifficultyLevel];
			float value = Random.value;
			if (value <= num)
			{
				float num2 = m_aiSettings.m_difficultyLevelMistakeTime[(int)m_dynamicDifficultyLevel];
				float t = Mathf.InverseLerp(0f, num, value);
				m_actionTimer = Mathf.Lerp(num2, m_aiSettings.m_reactionTime, t);
				m_skipMistake = true;
				return;
			}
		}
		m_vehicle.ButtonDown();
		m_actionTimer = m_aiSettings.m_reactionTime;
		m_skipMistake = false;
	}

	private void ButtonUpAction()
	{
		if (m_dynamicDifficulty && !m_noMistakes && !m_skipMistake)
		{
			float num = m_aiSettings.m_difficultyLevelMistakeChance[(int)m_dynamicDifficultyLevel];
			float value = Random.value;
			if (value <= num)
			{
				float num2 = m_aiSettings.m_difficultyLevelMistakeTime[(int)m_dynamicDifficultyLevel];
				float t = Mathf.InverseLerp(0f, num, value);
				m_actionTimer = Mathf.Lerp(num2, m_aiSettings.m_reactionTime, t);
				m_skipMistake = true;
				return;
			}
		}
		m_vehicle.ButtonUp();
		m_actionTimer = m_aiSettings.m_reactionTime;
		m_skipMistake = false;
	}

	private void UpdateDynamicDifficultyLevel()
	{
		if (m_dynamicDifficulty)
		{
			if (m_hasGrappled)
			{
				int i = (int)m_difficulty + (int)(m_rubberBanding - 2);
				i = MathUtils.Clamp(i, 0, 3);
				m_dynamicDifficultyLevel = (AIDifficulty)i;
				if (m_vehicle.NumLapsDone > 0)
				{
					UpdateForwardForce();
				}
			}
			else
			{
				m_dynamicDifficultyLevel = AIDifficulty.Optimal;
			}
		}
		else
		{
			m_dynamicDifficultyLevel = m_difficulty;
			UpdateForwardForce();
		}
	}

	private void UpdateForwardForce()
	{
		m_vehicle.m_forwardForce = m_defaultForwardForce * m_aiSettings.m_difficultyForwardForceScale[(int)m_dynamicDifficultyLevel];
	}

	private void OnLap(Vehicle _vehicle, float _lapTime)
	{
		UpdateDynamicDifficultyLevel();
	}

	private void SuppressCurrentGrapple()
	{
		m_lastGrapplePoint = m_vehicle.GetTrackGrapplePoint();
		m_lastGrappleTimer = m_aiSettings.m_suppressGrappleTime;
	}

	public void RaceEvent_Launch()
	{
		m_actionTimer = m_aiSettings.m_reactionTime;
		SuppressCurrentGrapple();
	}

	public void UpdateRubberBanding(RubberBanding _rubberBanding)
	{
		m_rubberBanding = _rubberBanding;
		UpdateDynamicDifficultyLevel();
	}

	public void OnHitByOtherVehicle(Collision _collision, Vehicle _vehicle, Vehicle _otherVehicle)
	{
		if (m_dynamicDifficulty && !m_noMistakes && !m_skipMistake)
		{
			float num = m_aiSettings.m_difficultyLevelMistakeTime[(int)m_dynamicDifficultyLevel];
			float value = Random.value;
			m_actionTimer = Mathf.Max(m_actionTimer, num * value);
			if (m_vehicle.IsGrappling)
			{
				float num2 = m_aiSettings.m_difficultyLevelMistakeChance[(int)m_dynamicDifficultyLevel];
				if (1f - value >= num2)
				{
					m_vehicle.ButtonUp();
					m_skipMistake = true;
				}
			}
		}
		else
		{
			m_skipMistake = false;
		}
		if (!_vehicle.NonCompetitive && !_otherVehicle.NonCompetitive && _vehicle.NumLapsDone > 0)
		{
			float magnitude = _collision.relativeVelocity.magnitude;
			if (magnitude > m_aiSettings.m_grudgeThresholds[(int)m_dynamicDifficultyLevel] && _otherVehicle.GetComponent<VehicleController>() != null && m_grudgePopup != null && !m_grudgePopup.gameObject.active)
			{
				int num3 = Random.Range(0, c_grudgeShouts.Length);
				string text = Language.Get(c_grudgeShouts[num3]);
				m_grudgePopup.Target = m_vehicle;
				m_grudgePopup.Colour = _vehicle.Colour;
				m_grudgePopup.Popup(text);
			}
		}
	}
}
