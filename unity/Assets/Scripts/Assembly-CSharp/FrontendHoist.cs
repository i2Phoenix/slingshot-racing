using System.Collections;
using UnityEngine;

[RequireComponent(typeof(RateAppPrompt))]
public class FrontendHoist : MonoBehaviour
{
	private static string[] c_colourNames = new string[4] { "red", "green", "blue", "yellow" };

	private InAppPurchaseManager m_iapManager;

	private RateAppPrompt m_rateAppPrompt;

	private Material m_vehicleMaterial;

	private int m_vehicleIndex;

	private LoadscreenManager m_loadscreen;

	private bool m_insideIAPBlock;

	private GameObject m_lockedIndicator;

	private float m_prevUOffset;

	private float m_nextUOffset;

	private float m_uOffsetLerp;

	private AudioPlayer m_audioPlayer;

	public Renderer[] m_colourIndexedRenderers;

	public GameObject[] m_vehiclePrefabs;

	public ProductInfo[] m_unlockProducts;

	public GameObject[] m_tins;

	public AudioBank m_paintCanClicked;

	public Transform m_iconAttach;

	public GameObject m_purchasePrefab;

	public GameObject m_mysteryPrefab;

	public GameObject m_reviewPrefab;

	public PipeGUI_InstructorWelcome m_instructor;

	public bool m_debugReviewOffered;

	public int VehicleIndex
	{
		get
		{
			return m_vehicleIndex;
		}
		set
		{
			m_vehicleIndex = value;
		}
	}

	public UIListItemContainer ListItem { get; set; }

	private bool IsReviewOffered
	{
		get
		{
			return Settings.ReviewOffered;
		}
	}

	private void Awake()
	{
		m_audioPlayer = GetComponent<AudioPlayer>();
		if (m_audioPlayer == null)
		{
			m_audioPlayer = base.gameObject.AddComponent<AudioPlayer>();
		}
	}

	private void Start()
	{
		m_iapManager = InAppPurchaseManager.Get;
		m_rateAppPrompt = GetComponent<RateAppPrompt>();
		m_rateAppPrompt.OnRatingSelected += OnRatingSelected;
		GameObject gameObject = Object.Instantiate(m_vehiclePrefabs[m_vehicleIndex]) as GameObject;
		if (gameObject != null)
		{
			Quaternion localRotation = gameObject.transform.localRotation;
			Vector3 localScale = gameObject.transform.localScale;
			gameObject.transform.parent = base.transform;
			gameObject.transform.localPosition = Vector3.zero;
			gameObject.transform.localRotation = localRotation;
			gameObject.transform.localScale = localScale;
			gameObject.layer = base.gameObject.layer;
			m_vehicleMaterial = gameObject.GetComponent<Renderer>().material;
			ApplyVehicleColour();
			m_prevUOffset = m_nextUOffset;
			m_uOffsetLerp = 1f;
		}
		if (!AppSettings.SupportsIAP && !GlobalSave.IsVehicleStyleUnlocked(m_vehicleIndex) && m_unlockProducts[m_vehicleIndex] != null)
		{
			GlobalSave.UnlockVehicleStyle(m_vehicleIndex);
		}
		UpdateLockedIndicator();
		ControlBase[] componentsInChildren = base.transform.GetComponentsInChildren<ControlBase>(true);
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].Container = ListItem;
		}
	}

	private void OnDestroy()
	{
		m_rateAppPrompt.OnRatingSelected -= OnRatingSelected;
	}

	private void OnRatingSelected(Settings.RatedType _type)
	{
		UpdateLockedIndicator();
		Settings.VehicleStyleIndex = m_vehicleIndex;
	}

	private void UpdateLockedIndicator()
	{
		if (!GlobalSave.IsVehicleStyleUnlocked(m_vehicleIndex))
		{
			if (m_lockedIndicator == null)
			{
				GameObject gameObject = null;
				gameObject = ((!(m_unlockProducts[m_vehicleIndex] != null)) ? ((!IsReviewOffered) ? m_mysteryPrefab : m_reviewPrefab) : m_purchasePrefab);
				m_lockedIndicator = Object.Instantiate(gameObject) as GameObject;
				m_lockedIndicator.transform.parent = m_iconAttach;
				m_lockedIndicator.transform.localPosition = Vector3.zero;
				m_lockedIndicator.transform.localRotation = Quaternion.identity;
			}
		}
		else if (m_lockedIndicator != null)
		{
			Object.Destroy(m_lockedIndicator);
			m_lockedIndicator = null;
		}
	}

	private void Update()
	{
		if (m_uOffsetLerp < 1f)
		{
			m_uOffsetLerp += Time.deltaTime * 0.85f;
			m_uOffsetLerp = Mathf.Clamp01(m_uOffsetLerp);
		}
		m_vehicleMaterial.SetFloat("_UOffset", m_prevUOffset);
		m_vehicleMaterial.SetFloat("_UOffsetNext", m_nextUOffset);
		Vector3 position = base.transform.position;
		Vector3 vector = base.transform.rotation * Vector3.forward;
		vector = -vector;
		position += vector * -1.5f;
		position += vector * (m_uOffsetLerp * 4f);
		m_vehicleMaterial.SetVector("_TransitionPos", new Vector4(position.x, position.y, position.z, 1f));
		m_vehicleMaterial.SetVector("_TransitionDir", new Vector4(vector.x, vector.y, vector.z, 0f));
	}

	public void PaintNextColour()
	{
		int num = Settings.GetVehicleColourIndex(m_vehicleIndex) + 1;
		if (num >= 4)
		{
			num = 0;
		}
		OnColourSelected(num);
	}

	private void OnColourSelected(int _index)
	{
		Analytics.LogEvent("SetColour", "colour", c_colourNames[_index]);
		StartCoroutine(SetVehicleColourIndexDelayed(_index));
		TinPressed(_index);
	}

	private void OnRedSelected()
	{
		OnColourSelected(0);
	}

	private void OnGreenSelected()
	{
		OnColourSelected(1);
	}

	private void OnBlueSelected()
	{
		OnColourSelected(2);
	}

	private void OnYellowSelected()
	{
		OnColourSelected(3);
	}

	private IEnumerator SetVehicleColourIndexDelayed(int _index)
	{
		yield return YieldCache.Wait10Hz;
		SetVehicleColourIndex(_index);
	}

	private void ShowIAPPurchase()
	{
		if (GlobalSave.IsVehicleStyleUnlocked(m_vehicleIndex) || m_unlockProducts == null || !(m_unlockProducts[m_vehicleIndex] != null) || !(m_iapManager != null))
		{
			return;
		}
		FindLoadscreen();
		m_insideIAPBlock = true;
		if (m_iapManager.Purchase(m_unlockProducts[m_vehicleIndex], OnPurchase))
		{
			if (m_insideIAPBlock && m_loadscreen != null)
			{
				StartCoroutine(m_loadscreen.ShowSystemBusy(true, 0.2f));
				UIManager.instance.LockInput();
			}
		}
		else
		{
			m_insideIAPBlock = false;
		}
	}

	private void OnUnlockSelected()
	{
		if (!GlobalSave.IsVehicleStyleUnlocked(m_vehicleIndex))
		{
			if (m_unlockProducts != null && m_unlockProducts[m_vehicleIndex] != null)
			{
				ShowIAPPurchase();
			}
			else if (IsReviewOffered)
			{
				m_rateAppPrompt.PresentDialog(0.5f);
			}
			else if (m_instructor != null)
			{
				m_instructor.OpenDialog(Language.Get("ID_ReviewVehicleLocked"));
			}
		}
	}

	private void FindLoadscreen()
	{
		if (!(m_loadscreen == null))
		{
			return;
		}
		GameObject gameObject = GameObject.Find("PersistentGUI");
		if (gameObject != null)
		{
			PersistentGUI component = gameObject.GetComponent<PersistentGUI>();
			if (component != null)
			{
				m_loadscreen = component.m_loadscreen;
			}
		}
	}

	private void OnPurchase(InAppPurchaseManager.PurchaseStatus _status)
	{
		if (m_loadscreen != null)
		{
			StartCoroutine(m_loadscreen.ShowSystemBusy(false, 0.2f));
			UIManager.instance.UnlockInput();
		}
		m_insideIAPBlock = false;
		Settings.VehicleStyleIndex = m_vehicleIndex;
		Invoke("UpdateLockedIndicator", 0.1f);
	}

	private void SetVehicleColourIndex(int _index)
	{
		Settings.SetVehicleColourIndex(m_vehicleIndex, _index);
		ApplyVehicleColour();
	}

	private void ApplyVehicleColour()
	{
		float num = (float)Settings.GetVehicleColourIndex(m_vehicleIndex) * 0.25f;
		m_prevUOffset = m_nextUOffset;
		m_nextUOffset = num;
		m_uOffsetLerp = 0f;
		Renderer[] colourIndexedRenderers = m_colourIndexedRenderers;
		foreach (Renderer renderer in colourIndexedRenderers)
		{
			renderer.material.SetFloat("_UOffset", num);
			renderer.material.SetFloat("_UOffsetNext", num);
		}
	}

	private void TinPressed(int _index)
	{
		if (m_tins != null && _index < m_tins.Length)
		{
			Animation component = m_tins[_index].GetComponent<Animation>();
			if (component != null)
			{
				component.Stop();
				component.Play();
			}
		}
		if (m_paintCanClicked != null)
		{
			m_audioPlayer.PlayRandomGUIClip(m_paintCanClicked);
		}
	}
}
