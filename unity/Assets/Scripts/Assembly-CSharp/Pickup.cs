using System;
using System.Collections;
using UnityEngine;

[AddComponentMenu("Gameplay/Pickups/Pickup")]
public class Pickup : MonoBehaviour
{
	private bool m_triggered;

	public GameObject m_visualsPrefab;

	public GameObject m_dropShadowPrefab;

	public float m_visualsScale = 1f;

	public AudioBank m_audioBank;

	public bool m_suppressHeightSample;

	public Quaternion CollectedRotation { get; private set; }

	public int Index { get; set; }

	public event Action<Pickup> OnCollected;

	private IEnumerator Start()
	{
		yield return null;
		Vector3 rootPos = base.transform.position;
		Vector3 floorPos = rootPos;
		RaycastHit hitInfo;
		if (Physics.Raycast(layerMask: 1 << LayerMask.NameToLayer("Track"), origin: base.transform.position, direction: -Vector3.up, hitInfo: out hitInfo, maxDistance: 10f))
		{
			floorPos = hitInfo.point;
			floorPos.y += 0.05f;
		}
		if (!m_suppressHeightSample)
		{
			rootPos = floorPos;
			base.transform.position = rootPos;
		}
		AddChild(_go: UnityEngine.Object.Instantiate(m_visualsPrefab) as GameObject, _parent: base.transform.Find("Visuals"));
		if (m_dropShadowPrefab != null)
		{
			GameObject shadow = UnityEngine.Object.Instantiate(m_dropShadowPrefab) as GameObject;
			AddChild(base.transform, shadow);
			shadow.transform.position = floorPos;
		}
	}

	private void SetScale(Vector3 _scale)
	{
		base.transform.localScale = _scale;
	}

	private void OnTriggerEnter(Collider other)
	{
		if (!m_triggered)
		{
			MagnetUpgrade component = other.GetComponent<MagnetUpgrade>();
			if (component != null)
			{
				component.OnEnter(this);
				return;
			}
			Transform collector = ((!other.attachedRigidbody) ? base.transform : other.attachedRigidbody.transform);
			TriggerCollection(collector);
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (!m_triggered)
		{
			MagnetUpgrade component = other.GetComponent<MagnetUpgrade>();
			if (component != null)
			{
				component.OnExit(this);
			}
		}
	}

	public void TriggerCollection(Transform collector)
	{
		if (!m_triggered)
		{
			CollectedRotation = collector.rotation;
			if (OnCollected != null)
			{
				OnCollected(this);
			}
			SendMessageUpwards("OnCollected", this, SendMessageOptions.RequireReceiver);
			UnityEngine.Object.Destroy(base.gameObject);
			m_triggered = true;
		}
	}

	private void PlayCollected()
	{
		AudioPlayer component = GetComponent<AudioPlayer>();
		if (component != null)
		{
			component.PlayRandomClip(m_audioBank);
		}
	}

	private void PlayCollectedFinal()
	{
		AudioPlayer component = GetComponent<AudioPlayer>();
		if (component != null)
		{
			component.PlayRandomClip(m_audioBank, 1f, 0.25f);
		}
	}

	private void AddChild(Transform _parent, GameObject _go)
	{
		Vector3 localScale = _go.transform.localScale;
		Quaternion localRotation = _go.transform.localRotation;
		_go.transform.parent = _parent;
		_go.transform.localPosition = Vector3.zero;
		_go.transform.localRotation = localRotation;
		_go.transform.localScale = localScale * m_visualsScale;
	}
}
