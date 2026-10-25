using System;
using System.Collections;
using UnityEngine;

public class FrontendVehicleList : MonoBehaviour
{
	[Serializable]
	public class ButtonHandler : IUIGamepadHotkey
	{
		[NonSerialized]
		public FrontendVehicleList m_owner;

		public string m_keyName;

		public bool IsHotkeyActive
		{
			get
			{
				return true;
			}
		}

		public int HotkeyLayer
		{
			get
			{
				return 0;
			}
		}

		public string HotkeyName
		{
			get
			{
				return m_keyName;
			}
		}

		public void OnEvent(POINTER_INFO.INPUT_EVENT _event)
		{
			if (_event == POINTER_INFO.INPUT_EVENT.TAP)
			{
				m_owner.NextVehicleColour();
			}
		}
	}

	private Collider[] m_colliders;

	private UIScrollList m_list;

	private AudioPlayer m_audioPlayer;

	private int m_lastClickScrollPosition;

	private float m_flickerTime;

	private Vector3 m_lastMoverPosition;

	private bool m_registered;

	public GameObject m_listItemPrefab;

	public int[] m_vehicleOrder;

	public AudioBank m_railClick;

	public AudioBank m_railSelect;

	public float m_railClickFrequency = 0.015f;

	public Animation m_railAnimation;

	public float m_railAnimationSpeedScale = 10f;

	public Renderer m_signRenderer;

	public PipeGUI_InstructorWelcome m_instructor;

	public AudioBank m_signFlicker1;

	public AudioBank m_signFlicker2;

	public ParticleSystem m_signSparks;

	public ButtonHandler m_changeColourHotkey;

	private void Awake()
	{
		m_audioPlayer = GetComponent<AudioPlayer>();
		if (m_audioPlayer == null)
		{
			m_audioPlayer = base.gameObject.AddComponent<AudioPlayer>();
		}
		m_changeColourHotkey.m_owner = this;
	}

	protected virtual void OnDestroy()
	{
		if (m_registered)
		{
			UIGamepadManager.Unregister(m_changeColourHotkey);
			m_registered = false;
		}
	}

	protected virtual void OnEnable()
	{
		UIGamepadManager.Register(m_changeColourHotkey);
		m_registered = true;
	}

	protected virtual void OnDisable()
	{
		UIGamepadManager.Unregister(m_changeColourHotkey);
		m_registered = false;
	}

	private IEnumerator Start()
	{
		int scrollToIndex = 0;
		UIScrollList list = GetComponent<UIScrollList>();
		for (int index = 0; index < 9; index++)
		{
			IUIListObject listObj = list.CreateItem(m_listItemPrefab);
			FrontendListItem listItem = listObj.gameObject.GetComponent<FrontendListItem>();
			listItem.VehicleIndex = m_vehicleOrder[index];
			if (Settings.VehicleStyleIndex == m_vehicleOrder[index])
			{
				scrollToIndex = index;
			}
		}
		list.ScrollToItem(scrollToIndex, 0f);
		yield return null;
		list.AddItemSnappedDelegate(OnItemSnapped);
		m_list = list;
		m_lastClickScrollPosition = Mathf.RoundToInt(list.ScrollPosition / m_railClickFrequency);
		m_flickerTime = 4f;
	}

	private void Update()
	{
		if (!(m_list != null))
		{
			return;
		}
		float scrollPosition = m_list.ScrollPosition;
		int num = Mathf.RoundToInt(scrollPosition / m_railClickFrequency);
		if (m_railAnimation != null && base.transform.GetChildCount() > 0)
		{
			Vector3 position = base.transform.GetChild(0).position;
			Vector3 lhs = position - m_lastMoverPosition;
			float speed = Vector3.Dot(lhs, Vector3.right) * m_railAnimationSpeedScale;
			m_lastMoverPosition = position;
			foreach (AnimationState item in m_railAnimation)
			{
				item.speed = speed;
			}
		}
		if (num != m_lastClickScrollPosition)
		{
			if (m_railClick != null)
			{
				m_audioPlayer.PlayRandomGUIClip(m_railClick);
			}
			m_lastClickScrollPosition = num;
		}
		m_signRenderer.enabled = scrollPosition < 0.95f;
		m_flickerTime -= Time.deltaTime;
		if (m_flickerTime <= 0f)
		{
			StartCoroutine(DoSignFlicker());
			m_flickerTime = UnityEngine.Random.Range(4f, 6f);
		}
	}

	private IEnumerator DoSignFlicker()
	{
		bool alternate = UnityEngine.Random.value < 0.3f;
		if (alternate && m_signFlicker2 != null)
		{
			m_audioPlayer.PlayRandomClip(m_signFlicker2);
		}
		else if (m_signFlicker1 != null)
		{
			m_audioPlayer.PlayRandomClip(m_signFlicker1);
		}
		int count = UnityEngine.Random.Range((!alternate) ? 6 : 8, 12);
		count &= -2;
		for (int index = 0; index < count; index++)
		{
			float upper = ((index + 1 == count) ? 1f : UnityEngine.Random.Range(0.9f, 1f));
			float lower = UnityEngine.Random.Range(0f, 0.1f);
			float flickerTime = UnityEngine.Random.Range(0.025f, 0.05f);
			float ft = flickerTime;
			if ((index & 1) == 1)
			{
				float temp = upper;
				upper = lower;
				lower = temp;
				if (m_signSparks != null)
				{
					m_signSparks.Emit(4);
				}
			}
			while (flickerTime > 0f)
			{
				m_signRenderer.material.color = Color.white * Mathf.SmoothStep(lower, upper, flickerTime / ft);
				flickerTime -= Time.deltaTime;
				yield return null;
			}
			if (alternate && index == 4)
			{
				yield return YieldCache.WaitForOneSecond;
				if (m_signFlicker1 != null)
				{
					m_audioPlayer.PlayRandomClip(m_signFlicker1);
				}
			}
		}
		m_signRenderer.material.color = Color.white;
	}

	public void EnableInput(bool _enable)
	{
		if (_enable)
		{
			if (m_colliders != null)
			{
				Collider[] colliders = m_colliders;
				foreach (Collider collider in colliders)
				{
					collider.enabled = true;
				}
				m_colliders = null;
			}
		}
		else
		{
			if (m_colliders != null)
			{
				return;
			}
			m_colliders = base.gameObject.GetComponentsInChildren<Collider>();
			if (m_colliders != null)
			{
				Collider[] colliders2 = m_colliders;
				foreach (Collider collider2 in colliders2)
				{
					collider2.enabled = false;
				}
			}
		}
	}

	private void OnItemSnapped(IUIListObject _item)
	{
		Settings.VehicleStyleIndex = _item.gameObject.GetComponent<FrontendListItem>().VehicleIndex;
		if (m_railSelect != null)
		{
			m_audioPlayer.PlayRandomGUIClip(m_railSelect);
		}
	}

	private void OnSignClicked()
	{
		Debug.Log("Sign Clicked");
		m_instructor.OpenDialog(Language.Get("ID_SelectVehicleHint"));
	}

	public void NextVehicleColour()
	{
		IUIListObject snappedItem = m_list.SnappedItem;
		if (snappedItem != null)
		{
			FrontendListItem component = snappedItem.gameObject.GetComponent<FrontendListItem>();
			component.Brace.Hoist.PaintNextColour();
		}
	}
}
