using System;
using System.Collections;
using UnityEngine;

[AddComponentMenu("Gameplay/Race Tutorial 3")]
public class RaceTutorial : MonoBehaviour
{
	private class GrappleWatcher
	{
		private TutorialTrigger.GrappleZone m_currentZone;

		private bool m_buttonDown;

		public void OnButtonDown()
		{
		}

		public void OnButtonUp()
		{
		}

		public bool OnTrigger(TutorialTrigger _trigger, bool _buttonDown)
		{
			m_currentZone = _trigger.m_zone;
			switch (m_currentZone)
			{
			case TutorialTrigger.GrappleZone.StartBegin:
			case TutorialTrigger.GrappleZone.StartEnd:
				return true;
			default:
				return false;
			}
		}

		public bool IsButtonDownOK()
		{
			switch (m_currentZone)
			{
			case TutorialTrigger.GrappleZone.StartBegin:
			case TutorialTrigger.GrappleZone.StartEnd:
			case TutorialTrigger.GrappleZone.FinishBegin:
				return true;
			default:
				return false;
			}
		}

		public bool IsButtonUpOK()
		{
			switch (m_currentZone)
			{
			case TutorialTrigger.GrappleZone.StartBegin:
			case TutorialTrigger.GrappleZone.FinishBegin:
			case TutorialTrigger.GrappleZone.FinishEnd:
				return true;
			default:
				return false;
			}
		}
	}

	private RaceManager m_raceManager;

	private PhysicsVehicle m_vehicle;

	private VehicleController m_controller;

	private GrappleWatcher[] m_watchers = new GrappleWatcher[2];

	private GrappleWatcher m_activeWatcher;

	private bool m_buttonDown;

	private bool m_raceCompleted;

	private PositionPopup m_hintPopup;

	private string m_popupText;

	public PackedSprite[] m_grappleIndicators;

	public Transform[] m_targets;

	private void Awake()
	{
		for (int i = 0; i < m_watchers.Length; i++)
		{
			m_watchers[i] = new GrappleWatcher();
		}
	}

	private void Start()
	{
		m_raceManager = GameObjectUtils.FindComponentWithTag<RaceManager>("RaceControl");
		if (m_raceManager != null)
		{
			m_raceManager.OnRaceStartCountdownEvent += OnCountdownStarted;
			m_raceManager.OnRaceLaunchEvent += OnLaunch;
			m_raceManager.OnRaceAbortedEvent += OnRaceAborted;
			m_raceManager.PostponeLaunch += PostponeLaunch;
			m_raceManager.OnRaceCompleteEvent += OnRaceComplete;
		}
		UnityEngine.Object original = Resources.Load("Prefabs/Popups/TutorialHintPopup");
		GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(original);
		m_hintPopup = gameObject.GetComponent<PositionPopup>();
		string key = "ID_TutorialTouchToGrapple" + ((!UIGamepadManager.IsGamepadActive) ? string.Empty : AppSettings.LocSuffixGamepad);
		m_popupText = Language.Get(key);
	}

	private void OnDestroy()
	{
		if (m_raceManager != null)
		{
			m_raceManager.OnRaceStartCountdownEvent -= OnCountdownStarted;
			m_raceManager.OnRaceLaunchEvent -= OnLaunch;
			m_raceManager.OnRaceAbortedEvent -= OnRaceAborted;
			m_raceManager.PostponeLaunch -= PostponeLaunch;
			m_raceManager.OnRaceCompleteEvent -= OnRaceComplete;
		}
		if (m_hintPopup != null)
		{
			m_hintPopup.Target = null;
			UnityEngine.Object.Destroy(m_hintPopup.gameObject);
			m_hintPopup = null;
		}
	}

	private void OnLaunch(object _sender, EventArgs _args)
	{
	}

	private void OnRaceComplete(object _sender, EventArgs _args)
	{
		m_raceCompleted = true;
		HighlightGrappleNeeded(false);
		m_hintPopup.Target = null;
	}

	private void OnCountdownStarted(object _sender, EventArgs _args)
	{
		m_vehicle = m_raceManager.GetVehicle(0) as PhysicsVehicle;
		if (m_vehicle != null)
		{
			m_vehicle.OnLapEvent += OnLapComplete;
			m_controller = m_vehicle.GetComponent<VehicleController>();
			if (m_controller != null)
			{
				m_controller.OnButtonDownEvent += OnButtonDown;
				m_controller.OnButtonUpEvent += OnButtonUp;
			}
		}
		m_raceCompleted = false;
	}

	private void OnRaceAborted(object _sender, EventArgs _args)
	{
		Debug.Log("Race Aborted");
		StopAllCoroutines();
		if (m_controller != null)
		{
			m_controller.OnButtonDownEvent -= OnButtonDown;
			m_controller.OnButtonUpEvent -= OnButtonUp;
		}
		m_vehicle = null;
		m_controller = null;
		m_hintPopup.Target = null;
	}

	private void OnLapComplete(Vehicle vehicle, float lapTime)
	{
	}

	private bool PostponeLaunch()
	{
		return false;
	}

	private void OnButtonDown(object _sender, EventArgs _args)
	{
		m_buttonDown = true;
		if (m_activeWatcher != null)
		{
			m_activeWatcher.OnButtonDown();
		}
	}

	private void OnButtonUp(object _sender, EventArgs _args)
	{
		m_buttonDown = false;
		if (m_activeWatcher != null)
		{
			m_activeWatcher.OnButtonUp();
		}
	}

	private void TutorialEvent_EnterTrigger(TutorialTrigger _trigger)
	{
		if (m_vehicle == null)
		{
			return;
		}
		int triggerID = _trigger.m_triggerID;
		GrappleWatcher grappleWatcher = m_watchers[triggerID];
		if (grappleWatcher != m_activeWatcher)
		{
			m_activeWatcher = grappleWatcher;
		}
		bool flag = m_activeWatcher.OnTrigger(_trigger, m_buttonDown);
		if (!m_raceCompleted)
		{
			if (_trigger.m_zone == TutorialTrigger.GrappleZone.StartBegin && m_hintPopup != null && m_vehicle != null)
			{
				m_hintPopup.Target = m_vehicle;
				m_hintPopup.Colour = m_vehicle.Colour;
				m_hintPopup.Popup(m_popupText);
			}
			if (flag)
			{
				HighlightGrappleNeeded(flag);
			}
			else if (_trigger.m_zone == TutorialTrigger.GrappleZone.FinishBegin && m_vehicle != null)
			{
				StartCoroutine(WatchForGrappleRelease(m_targets[triggerID]));
			}
		}
		if (_trigger.m_zone == TutorialTrigger.GrappleZone.FinishEnd)
		{
			m_activeWatcher = null;
			StopAllCoroutines();
			HighlightGrappleNeeded(false);
		}
	}

	private static float AngleDir(Vector3 fwd, Vector3 targetDir, Vector3 up)
	{
		Vector3 lhs = Vector3.Cross(fwd, targetDir);
		float num = Vector3.Dot(lhs, up);
		if (num > 0f)
		{
			return 1f;
		}
		if (num < 0f)
		{
			return -1f;
		}
		return 0f;
	}

	private IEnumerator WatchForGrappleRelease(Transform _target)
	{
		Vector3 targetPos = _target.position;
		while (true && m_vehicle != null)
		{
			Vector3 pos = m_vehicle.Position;
			Vector3 idealDir = targetPos - pos;
			idealDir.Normalize();
			Vector3 curDir = m_vehicle.Forward;
			curDir.Normalize();
			float angle = Vector3.Angle(curDir, idealDir);
			float right = AngleDir(curDir, idealDir, m_vehicle.Up);
			float closeEnough = 35f;
			Vector3 trackPos = m_vehicle.TrackPoint;
			Vector3 trackToVehicleDir = pos - trackPos;
			float trackPosToVehiclePosDistSq = trackToVehicleDir.sqrMagnitude;
			if (trackPosToVehiclePosDistSq > 0.5f)
			{
				trackToVehicleDir.Normalize();
				float trackWidthDiv2 = m_vehicle.TrackWidth * 0.5f;
				Vector3 trackNormal = m_vehicle.TrackNormal;
				float dist = Mathf.Sqrt(trackPosToVehiclePosDistSq);
				float outside = Vector3.Dot(trackToVehicleDir, trackNormal);
				float compensation = Mathf.Lerp(0f, 10f, dist / trackWidthDiv2);
				compensation = Mathf.Clamp(compensation, 0f, 10f);
				closeEnough += compensation * (0f - outside);
			}
			if (angle < closeEnough || right > 0f)
			{
				break;
			}
			yield return YieldCache.WaitForFixedUpdate;
		}
		HighlightGrappleNeeded(false);
	}

	private void HighlightGrappleNeeded(bool _highlight)
	{
#if DEVELOPMENT_BUILD || UNITY_EDITOR
		Debug.Log("QA_INPUT_HINT " + _highlight);
#endif
		if (!UIGamepadManager.IsGamepadActive)
		{
			PackedSprite[] grappleIndicators = m_grappleIndicators;
			foreach (PackedSprite packedSprite in grappleIndicators)
			{
				packedSprite.Hide(!_highlight);
			}
		}
	}

	private void ButtonDown()
	{
		if (m_vehicle != null)
		{
			m_vehicle.ButtonDown();
		}
	}

	private void ButtonUp()
	{
		if (m_vehicle != null)
		{
			m_vehicle.ButtonUp();
		}
	}
}
