using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

[AddComponentMenu("GUI/PipeGUI/TournamentTrackSelection")]
public class PipeGUI_TournamentTrackSelection : MonoBehaviour
{
	private enum RaceLength
	{
		Short = 0,
		Medium = 1,
		Long = 2
	}

	private enum TimeOfDay
	{
		Random = 0,
		Day = 1,
		Night = 2
	}

	private enum TrackDirection
	{
		Random = 0,
		Forward = 1,
		Reverse = 2
	}

	private class RaceSequence : GameManager.IFreeplayRaceSequence, ITBRaceSequence
	{
		public struct Track
		{
			public RaceEvent raceEvent;

			public int playCount;
		}

		private class TBRaceEvent : ITBRaceEvent
		{
			public RaceEvent m_raceEvent;

			public string RaceEventName
			{
				get
				{
					return m_raceEvent.name;
				}
			}

			public int NumLaps { get; set; }

			public bool Reversed { get; set; }

			public RaceLayout.Variant Variant { get; set; }
		}

		private int m_sequence;

		public Track[] tracks;

		public bool repeat;

		public int numLaps;

		public RaceLayout.Variant m_variant;

		public bool m_randomiseVariant;

		public bool m_reverseTrack;

		public bool m_randomiseDirection;

		public bool IsEmpty
		{
			get
			{
				return !repeat && m_sequence == tracks.Length;
			}
		}

		public RaceEvent NextEvent
		{
			get
			{
				int num = m_sequence / tracks.Length;
				for (int i = 0; i != tracks.Length; i++)
				{
					int num2 = i % tracks.Length;
					if (tracks[num2].playCount == num)
					{
						tracks[num2].playCount++;
						RaceEvent raceEvent = tracks[num2].raceEvent;
						raceEvent.m_numLaps = numLaps;
						raceEvent.m_variant = ((!m_randomiseVariant) ? m_variant : ((RaceLayout.Variant)UnityEngine.Random.Range(0, Enum.GetNames(typeof(RaceLayout.Variant)).Length)));
						raceEvent.m_forceReverseTrack = raceEvent.m_layout.CanReverse && ((!m_randomiseDirection) ? m_reverseTrack : (UnityEngine.Random.Range(0, 2) == 1));
						m_sequence++;
						return raceEvent;
					}
				}
				return null;
			}
		}

		public int NumEvents
		{
			get
			{
				return tracks.Length;
			}
		}

		public bool IsFinite
		{
			get
			{
				return !repeat;
			}
		}

		public int NumTurns
		{
			get
			{
				return 1;
			}
		}

		public int MaxRetries
		{
			get
			{
				return 3;
			}
		}

		public ITBRaceEvent GetEvent(int _eventIndex)
		{
			TBRaceEvent tBRaceEvent = new TBRaceEvent();
			tBRaceEvent.m_raceEvent = tracks[_eventIndex].raceEvent;
			tBRaceEvent.NumLaps = numLaps;
			tBRaceEvent.Reversed = tBRaceEvent.m_raceEvent.m_layout.CanReverse && ((!m_randomiseDirection) ? m_reverseTrack : (UnityEngine.Random.Range(0, 2) == 1));
			tBRaceEvent.Variant = ((!m_randomiseVariant) ? m_variant : ((RaceLayout.Variant)UnityEngine.Random.Range(0, Enum.GetNames(typeof(RaceLayout.Variant)).Length)));
			return tBRaceEvent;
		}
	}

	public enum Mode
	{
		Freeplay = 0,
		TurnBased = 1
	}

	private GameManager m_gameManager;

	private AudioSource m_audioSource;

	private int m_numSelected;

	private bool m_playAudio;

	private int m_trackListHash;

	public Mode m_mode;

	public UIButton m_selectAllButton;

	public UIButton m_selectNoneButton;

	public UIButton m_randomiseButton;

	public UIStateToggleBtn m_raceLengthButton;

	public UIStateToggleBtn m_timeOfDayButton;

	public UIStateToggleBtn m_trackDirectionButton;

	public UIButton m_nextButton;

	public UIScrollList m_trackList;

	public GameObject m_listItemPrefab;

	public int[] m_lapCounts;

	public RaceTour m_testTour;

	public RaceCampaign m_testCampaign;

	private string RaceLengthOption
	{
		get
		{
			return SettingsPrefix + "_RaceLength";
		}
	}

	private string TrackSelection
	{
		get
		{
			return SettingsPrefix + "_TrackSelection";
		}
	}

	private string TimeOfDayOption
	{
		get
		{
			return SettingsPrefix + "_TimeOfDay";
		}
	}

	private string TrackDirectionOption
	{
		get
		{
			return SettingsPrefix + "_TrackDirection";
		}
	}

	public AudioSource AudioSource
	{
		get
		{
			return m_audioSource;
		}
	}

	private string SettingsPrefix
	{
		get
		{
			return Enum.GetNames(typeof(RaceLayout.Variant))[(int)m_mode];
		}
	}

	private void Awake()
	{
		m_audioSource = base.gameObject.AddComponent<AudioSource>();
		m_audioSource.rolloffMode = AudioRolloffMode.Linear;
		m_audioSource.minDistance = 0f;
		m_audioSource.maxDistance = 200f;
	}

	private void Start()
	{
		m_gameManager = GameObjectUtils.FindComponentWithTag<GameManager>("RaceControl");
		if (m_gameManager == null && m_testTour != null)
		{
			SetTour(m_testTour);
		}
		m_nextButton.controlIsEnabled = false;
		m_selectNoneButton.controlIsEnabled = false;
		m_randomiseButton.controlIsEnabled = false;
		StartCoroutine(LateStart());
	}

	private IEnumerator LateStart()
	{
		yield return null;
		LoadSettings();
		m_nextButton.controlIsEnabled = m_numSelected != 0;
		m_playAudio = true;
		PipeGUI_Audio.EnableAudioInChildren(base.gameObject);
		PipeGUI_PunchAnim.EnableInChildren(base.gameObject);
	}

	private void LoadSettings()
	{
		SetRaceLength((RaceLength)Settings.GetInt(RaceLengthOption, 1));
		SetTimeOfDay((TimeOfDay)Settings.GetInt(TimeOfDayOption, 0));
		SetTrackDirection((TrackDirection)Settings.GetInt(TrackDirectionOption, 0));
		string text = Settings.GetString(TrackSelection, string.Empty);
		string[] array = text.Split(',');
		int result = 0;
		if (array.Length <= 0 || !int.TryParse(array[0], out result) || result != m_trackListHash)
		{
			return;
		}
		for (int i = 1; i < array.Length; i++)
		{
			string s = array[i];
			int result2 = 0;
			if (int.TryParse(s, out result2) && result2 < m_trackList.Count)
			{
				OnWidgetSelected(m_trackList.GetItem(result2).gameObject.GetComponent<PipeGUI_FreeplayTrackWidget>());
			}
		}
	}

	private void SaveSettings()
	{
		Settings.SetInt(RaceLengthOption, (int)GetRaceLength());
		Settings.SetInt(TimeOfDayOption, (int)GetTimeOfDay());
		Settings.SetInt(TrackDirectionOption, (int)GetTrackDirection());
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(m_trackListHash);
		for (int i = 1; i <= m_numSelected; i++)
		{
			for (int j = 0; j != m_trackList.Count; j++)
			{
				if (i == m_trackList.GetItem(j).gameObject.GetComponent<PipeGUI_FreeplayTrackWidget>().Index)
				{
					stringBuilder.Append(",");
					stringBuilder.Append(j);
				}
			}
		}
		Settings.SetString(TrackSelection, stringBuilder.ToString());
	}

	public void SetTour(RaceTour _tour)
	{
		m_trackList.ClearList(true);
		int num = _tour.m_races.Length;
		for (int i = 0; i < num; i++)
		{
			PipeGUI_FreeplayTrackWidget component = m_trackList.CreateItem(m_listItemPrefab).gameObject.GetComponent<PipeGUI_FreeplayTrackWidget>();
			component.Manager = this;
		}
		int num2 = 0;
		int num3 = 0;
		for (int j = 0; j < m_trackList.Count; j++)
		{
			IUIListObject item = m_trackList.GetItem(j);
			PipeGUI_FreeplayTrackWidget component2 = item.gameObject.GetComponent<PipeGUI_FreeplayTrackWidget>();
			if (component2 != null)
			{
				RaceEvent raceEvent = _tour.m_races[num2];
				component2.SetRaceData(raceEvent);
				num2++;
				num3 = num3 * 31 + raceEvent.DisplayName.GetHashCode();
			}
		}
		m_trackListHash = num3;
	}

	public void OnWidgetSelected(PipeGUI_FreeplayTrackWidget widget)
	{
		OnWidgetSelected(widget, true);
	}

	public void OnWidgetSelected(PipeGUI_FreeplayTrackWidget widget, bool _playAudio)
	{
		int index = widget.Index;
		if (index == 0)
		{
			widget.Index = ++m_numSelected;
		}
		else
		{
			widget.Index = 0;
			for (int i = 0; i != m_trackList.Count; i++)
			{
				PipeGUI_FreeplayTrackWidget component = m_trackList.GetItem(i).gameObject.GetComponent<PipeGUI_FreeplayTrackWidget>();
				if (component.Index > index)
				{
					component.Index--;
				}
			}
			m_numSelected--;
		}
		if (widget.m_selectedAudioBank != null && m_playAudio && _playAudio)
		{
			widget.m_selectedAudioBank.PlayRandomClip(m_audioSource);
		}
		RefreshButtonStates();
	}

	private void RefreshButtonStates()
	{
		m_nextButton.controlIsEnabled = m_numSelected != 0;
		m_selectAllButton.controlIsEnabled = m_numSelected != m_trackList.Count;
		m_selectNoneButton.controlIsEnabled = m_numSelected != 0;
		m_randomiseButton.controlIsEnabled = m_numSelected > 1;
	}

	private void OnBackSelected()
	{
		SaveSettings();
		if (m_gameManager != null)
		{
			m_gameManager.FreeplayRaceSequence = null;
			m_gameManager.GoBack();
			m_gameManager = null;
		}
	}

	private void OnNextSelected()
	{
		SaveSettings();
		if (!(m_gameManager != null))
		{
			return;
		}
		RaceSequence raceSequence = new RaceSequence();
		raceSequence.repeat = false;
		raceSequence.numLaps = GetNumLaps();
		raceSequence.m_randomiseVariant = GetTimeOfDay() == TimeOfDay.Random;
		raceSequence.m_variant = ((GetTimeOfDay() == TimeOfDay.Night) ? RaceLayout.Variant.Night : RaceLayout.Variant.Standard);
		raceSequence.m_reverseTrack = GetTrackDirection() == TrackDirection.Reverse;
		raceSequence.m_randomiseDirection = GetTrackDirection() == TrackDirection.Random;
		raceSequence.tracks = new RaceSequence.Track[m_numSelected];
		for (int i = 0; i != m_numSelected; i++)
		{
			for (int j = 0; j < m_trackList.Count; j++)
			{
				IUIListObject item = m_trackList.GetItem(j);
				PipeGUI_FreeplayTrackWidget component = item.gameObject.GetComponent<PipeGUI_FreeplayTrackWidget>();
				if (component.Index == i + 1)
				{
					raceSequence.tracks[i].raceEvent = component.RaceEvent;
				}
			}
		}
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary["numRaces"] = m_numSelected.ToString();
		if (m_mode == Mode.Freeplay)
		{
			dictionary["aiDifficulty"] = m_gameManager.FreeplayAiDifficulty.ToString();
			dictionary["numHumans"] = BitUtils.BitCount(m_gameManager.FreeplayHumanPlayerMask).ToString();
			dictionary["numAIs"] = BitUtils.BitCount(m_gameManager.FreeplayAIPlayerMask).ToString();
		}
		Analytics.LogEvent((m_mode != Mode.Freeplay) ? "TurnBased" : "Freeplay", dictionary);
		if (m_mode == Mode.Freeplay)
		{
			m_gameManager.FreeplayRaceSequence = raceSequence;
		}
		else if (m_mode == Mode.TurnBased)
		{
			TurnBasedMatchManager component2 = m_gameManager.GetComponent<TurnBasedMatchManager>();
			if (component2 != null)
			{
				if (!component2.SetRaceSequence(raceSequence))
				{
					Debug.LogError("Unable to set race sequence");
				}
			}
			else
			{
				Debug.LogError("Unable to find turn based match manager");
			}
		}
		m_gameManager.GoForward();
		m_gameManager = null;
	}

	private void OnSelectAll()
	{
		for (int i = 0; i != m_trackList.Count; i++)
		{
			PipeGUI_FreeplayTrackWidget component = m_trackList.GetItem(i).gameObject.GetComponent<PipeGUI_FreeplayTrackWidget>();
			if (component.Index == 0)
			{
				component.Index = ++m_numSelected;
			}
		}
		RefreshButtonStates();
	}

	private void OnSelectNone()
	{
		for (int i = 0; i != m_trackList.Count; i++)
		{
			PipeGUI_FreeplayTrackWidget component = m_trackList.GetItem(i).gameObject.GetComponent<PipeGUI_FreeplayTrackWidget>();
			component.Index = 0;
		}
		m_numSelected = 0;
		RefreshButtonStates();
	}

	private void OnRandomise()
	{
		List<int> list = new List<int>();
		for (int i = 1; i <= m_numSelected; i++)
		{
			for (int j = 0; j != m_trackList.Count; j++)
			{
				if (i == m_trackList.GetItem(j).gameObject.GetComponent<PipeGUI_FreeplayTrackWidget>().Index)
				{
					list.Add(j);
				}
			}
		}
		List<int> list3;
		bool flag;
		do
		{
			List<int> list2 = new List<int>(list);
			list3 = new List<int>();
			flag = false;
			while (list2.Count != 0)
			{
				int index = UnityEngine.Random.Range(0, list2.Count);
				int num = list2[index];
				flag |= num != list[list3.Count];
				list3.Add(num);
				list2.RemoveAt(index);
			}
		}
		while (!flag);
		OnSelectNone();
		foreach (int item in list3)
		{
			OnWidgetSelected(m_trackList.GetItem(item).gameObject.GetComponent<PipeGUI_FreeplayTrackWidget>(), false);
		}
	}

	private int GetNumLaps()
	{
		return m_lapCounts[(int)GetRaceLength()];
	}

	private RaceLength GetRaceLength()
	{
		return (RaceLength)m_raceLengthButton.StateNum;
	}

	private void SetRaceLength(RaceLength _length)
	{
		m_raceLengthButton.SetToggleState((int)_length);
		OnRaceLengthChanged();
	}

	private void OnRaceLengthChanged()
	{
		SpriteText componentInChildren = m_raceLengthButton.GetComponentInChildren<SpriteText>();
		componentInChildren.Text = GetNumLaps() + Language.Get("ID_LapsButton");
	}

	private TimeOfDay GetTimeOfDay()
	{
		return (TimeOfDay)m_timeOfDayButton.StateNum;
	}

	private void SetTimeOfDay(TimeOfDay _timeOfDay)
	{
		m_timeOfDayButton.SetToggleState((int)_timeOfDay);
	}

	private TrackDirection GetTrackDirection()
	{
		return (TrackDirection)m_trackDirectionButton.StateNum;
	}

	private void SetTrackDirection(TrackDirection _direction)
	{
		m_trackDirectionButton.SetToggleState((int)_direction);
	}
}
