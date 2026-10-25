using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine;

public abstract class PipeGUI_PlateWidget : MonoBehaviour
{
	[StructLayout((LayoutKind)0, Size = 1)]
	protected struct FanfareData
	{
		public RaceCampaignSave.AwardType Award { get; set; }

		public bool Locked { get; set; }
	}

	private UIListItemContainer m_listObject;

	private bool m_enableInput = true;

	private bool m_isSelected;

	private EZTransition m_clicked;

	protected EZTransition m_awardFanfare;

	private EZTransition m_unlockedFanfare;

	private EZTransition m_selected;

	private EZTransition m_unselected;

	private EZTransition m_infoClicked;

	private bool m_autoScroll = true;

	public PackedSprite m_plate;

	public PackedSprite m_locked;

	public GameObject m_information;

	public PackedSprite[] m_awards;

	public UIButton3D m_button;

	public AudioBank m_selectedAudioBank;

	public AudioBank m_lockedAudioBank;

	public AudioBank m_awardFanfareAudioBank;

	public AudioBank m_unlockFanfareAudioBank;

	public AudioBank m_infoClickedAudioBank;

	public GameObject m_stampEffectPrefab1;

	public GameObject m_stampEffectPrefab2;

	public float m_stampAudioTime;

	public float m_stampParticleTime1;

	public float m_stampParticleTime2;

	public GameObject m_transitionsPrefab;

	public TransitionStore m_transitions;

	protected abstract RaceCampaignSave.AwardType Award { get; }

	protected abstract bool Locked { get; }

	protected abstract string PlateID { get; }

	public bool EnableInput
	{
		get
		{
			return m_enableInput;
		}
		set
		{
			m_enableInput = value;
			m_button.controlIsEnabled = value;
		}
	}

	public bool ScrollToSelect { get; set; }

	public bool AutoScroll
	{
		get
		{
			return m_autoScroll;
		}
		set
		{
			m_autoScroll = value;
		}
	}

	public bool IsSelected
	{
		get
		{
			return m_isSelected;
		}
		set
		{
			if (value != m_isSelected)
			{
				if (value)
				{
					m_unselected.StopSafe();
					m_selected.Start();
				}
				else
				{
					m_selected.StopSafe();
					m_unselected.Start();
				}
				m_isSelected = value;
			}
		}
	}

	protected AudioSource AudioSourceToUse
	{
		get
		{
			if (base.GetComponent<AudioSource>() != null)
			{
				return base.GetComponent<AudioSource>();
			}
			UIScrollList uIScrollList = ((!(m_listObject != null)) ? null : m_listObject.GetScrollList());
			return (!(uIScrollList != null)) ? null : uIScrollList.GetComponent<AudioSource>();
		}
	}

	protected virtual void Start()
	{
		if (m_transitions == null && m_transitionsPrefab != null)
		{
			GameObject gameObject = Object.Instantiate(m_transitionsPrefab) as GameObject;
			gameObject.name = "Transitions";
			gameObject.transform.parent = base.transform;
			gameObject.transform.localPosition = Vector3.zero;
			m_transitions = gameObject.GetComponent<TransitionStore>();
		}
		if (m_transitions != null)
		{
			m_awardFanfare = m_transitions.FindTransition("AwardFanfare");
			m_clicked = m_transitions.FindTransition("OnClicked");
			if (m_clicked != null)
			{
				m_clicked.MainSubject = m_plate.gameObject;
			}
			m_unlockedFanfare = m_transitions.FindTransition("Unlocked");
			if (m_unlockedFanfare != null && m_locked != null)
			{
				m_unlockedFanfare.MainSubject = m_locked.gameObject;
			}
			m_selected = m_transitions.FindTransition("OnSelected");
			if (m_selected != null)
			{
				m_selected.MainSubject = m_plate.gameObject;
			}
			m_unselected = m_transitions.FindTransition("OnUnselected");
			if (m_unselected != null)
			{
				m_unselected.MainSubject = m_plate.gameObject;
			}
			if (m_information != null)
			{
				m_infoClicked = m_transitions.FindTransition("OnInfoClicked");
				if (m_infoClicked != null)
				{
					m_infoClicked.MainSubject = m_plate.gameObject;
				}
			}
		}
		m_listObject = base.gameObject.GetComponent<UIListItemContainer>();
	}

	protected virtual void Refresh()
	{
		UVAnimation anim = m_plate.GetAnim(PlateID);
		if (anim != null)
		{
			m_plate.PlayAnim(anim);
			m_plate.defaultAnim = anim.index;
		}
		if (m_locked != null)
		{
			m_locked.gameObject.SetActiveRecursively(Locked);
		}
		if (m_information != null)
		{
		}
		if (m_awards == null)
		{
			return;
		}
		int award = (int)Award;
		for (int i = 0; i < m_awards.Length; i++)
		{
			if (m_awards[i] != null)
			{
				m_awards[i].gameObject.active = award > i;
			}
		}
	}

	public void OnSelected()
	{
		if (m_enableInput)
		{
			OnSelectedImpl();
		}
	}

	public void OnSnapped()
	{
		if (m_enableInput)
		{
			OnSnappedImpl();
		}
	}

	public void OnInformationSelected()
	{
		if (m_enableInput)
		{
			OnInformationSelectedImpl();
		}
	}

	protected virtual void OnSelectedImpl()
	{
		if (!ScrollToSelect || IsSelected)
		{
			if (Locked)
			{
				if (m_lockedAudioBank != null)
				{
					m_lockedAudioBank.PlayRandomClip(AudioSourceToUse);
				}
			}
			else if (m_selectedAudioBank != null)
			{
				m_selectedAudioBank.PlayRandomClip(AudioSourceToUse);
			}
		}
		if ((!ScrollToSelect || !IsSelected) && m_autoScroll && m_listObject != null)
		{
			UIScrollList scrollList = m_listObject.GetScrollList();
			if (scrollList != null)
			{
				m_selectedAudioBank.PlayRandomClip(AudioSourceToUse);
				scrollList.ScrollToItem(m_listObject, 1f);
			}
		}
	}

	protected virtual void OnSnappedImpl()
	{
	}

	protected virtual void OnInformationSelectedImpl()
	{
		if (m_infoClickedAudioBank != null)
		{
			m_infoClickedAudioBank.PlayRandomClip(AudioSourceToUse);
		}
	}

	protected FanfareData PrepareFanfare()
	{
		return new FanfareData
		{
			Award = Award,
			Locked = Locked
		};
	}

	protected IEnumerator PlayFanfare(FanfareData _data)
	{
		if (!Locked && _data.Locked)
		{
			m_unlockedFanfare.Start();
			m_unlockFanfareAudioBank.PlayRandomClip(AudioSourceToUse);
			while (m_unlockedFanfare.IsRunning())
			{
				yield return null;
			}
			yield return YieldCache.WaitForSeconds(1.5f);
			m_locked.gameObject.active = false;
			yield return StartCoroutine(PlayUnlockFanfare());
		}
		if (Award > _data.Award)
		{
			int awardType = (int)Award;
			for (int index = (int)_data.Award; index < m_awards.Length; index++)
			{
				bool activate = awardType > index;
				if (activate)
				{
					m_awards[index].gameObject.active = activate;
					Vector3 originalPosition = m_awards[index].transform.localPosition;
					Vector3 fanfarePosition = originalPosition;
					fanfarePosition.z -= 3f;
					m_awards[index].transform.localPosition = fanfarePosition;
					m_awardFanfare.MainSubject = m_awards[index].gameObject;
					m_awardFanfare.Start();
					StartCoroutine(StampEffect(m_awards[index].transform));
					while (m_awardFanfare.IsRunning())
					{
						yield return null;
					}
					yield return YieldCache.WaitForSeconds(0.05f);
					m_awards[index].transform.localPosition = originalPosition;
					m_awardFanfare.MainSubject = null;
				}
			}
		}
		yield return null;
	}

	protected IEnumerator StampEffect(Transform _transform)
	{
		GameObject stampEffectGO = null;
		GameObject stampEffect2GO = null;
		bool firstFrame = true;
		bool firstFrame2 = true;
		bool finished = false;
		bool finished2 = false;
		bool playedAudio = false;
		AudioPlayer ap = GetComponent<AudioPlayer>();
		float timeSpent = 0f;
		while (!finished || !finished2)
		{
			timeSpent += Time.deltaTime;
			if (timeSpent >= m_stampAudioTime && !playedAudio)
			{
				ap.PlayRandomGUIClip(m_awardFanfareAudioBank);
				playedAudio = true;
			}
			if (timeSpent >= m_stampParticleTime1 && stampEffectGO == null && !finished)
			{
				Vector3 stampEffectPos = _transform.position;
				stampEffectPos.z = 0f;
				stampEffectGO = Object.Instantiate(m_stampEffectPrefab1, stampEffectPos, Quaternion.identity) as GameObject;
			}
			if (timeSpent >= m_stampParticleTime2 && stampEffect2GO == null && !finished2)
			{
				Vector3 stampEffectPos2 = _transform.position;
				stampEffectPos2.z = 0f;
				stampEffect2GO = Object.Instantiate(m_stampEffectPrefab2, stampEffectPos2, Quaternion.identity) as GameObject;
			}
			if (stampEffectGO != null)
			{
				if (!firstFrame)
				{
					ParticleEmitter emitter = stampEffectGO.GetComponent<ParticleEmitter>();
					emitter.emit = false;
					Vector3 stampEffectPos3 = _transform.position;
					stampEffectPos3.z += 0.15f;
					stampEffectGO.transform.position = stampEffectPos3;
					if (emitter.particleCount <= 0)
					{
						Object.DestroyImmediate(stampEffectGO);
						finished = true;
					}
				}
				if (firstFrame)
				{
					firstFrame = false;
				}
			}
			if (stampEffect2GO != null)
			{
				if (!firstFrame2)
				{
					ParticleEmitter emitter2 = stampEffect2GO.GetComponent<ParticleEmitter>();
					emitter2.emit = false;
					Vector3 stampEffectPos4 = _transform.position;
					stampEffectPos4.z += 0.1f;
					stampEffect2GO.transform.position = stampEffectPos4;
					if (emitter2.particleCount <= 0)
					{
						Object.DestroyImmediate(stampEffect2GO);
						finished2 = true;
					}
				}
				if (firstFrame2)
				{
					firstFrame2 = false;
				}
			}
			yield return null;
		}
		yield return null;
	}

	protected virtual IEnumerator PlayUnlockFanfare()
	{
		yield return null;
	}
}
