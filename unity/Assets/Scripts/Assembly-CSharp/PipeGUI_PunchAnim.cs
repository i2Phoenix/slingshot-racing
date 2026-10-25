using System.Collections;
using UnityEngine;

public class PipeGUI_PunchAnim : MonoBehaviour
{
	private Vector3 m_defaultScale;

	private float m_timeElapsed;

	private bool m_active;

	public float m_duration = 0.8f;

	public float m_magnitude = 0.1f;

	public Transform m_target;

	public bool m_includeChildren = true;

	public static void EnableInChildren(GameObject gameObject)
	{
		PipeGUI_PunchAnim[] componentsInChildren = GameObjectUtils.GetComponentsInChildren<PipeGUI_PunchAnim>(gameObject);
		PipeGUI_PunchAnim[] array = componentsInChildren;
		foreach (PipeGUI_PunchAnim pipeGUI_PunchAnim in array)
		{
			pipeGUI_PunchAnim.enabled = true;
		}
	}

	private void Start()
	{
		AutoSpriteControlBase[] array = ((!m_includeChildren) ? base.gameObject.GetComponents<AutoSpriteControlBase>() : GameObjectUtils.GetComponentsInChildren<AutoSpriteControlBase>(base.gameObject));
		AutoSpriteControlBase[] array2 = array;
		foreach (AutoSpriteControlBase autoSpriteControlBase in array2)
		{
			autoSpriteControlBase.AddValueChangedDelegate(OnValueChanged);
		}
		ControlBase[] array3 = ((!m_includeChildren) ? base.gameObject.GetComponents<ControlBase>() : GameObjectUtils.GetComponentsInChildren<ControlBase>(base.gameObject));
		ControlBase[] array4 = array3;
		foreach (ControlBase controlBase in array4)
		{
			controlBase.AddValueChangedDelegate(OnValueChanged);
		}
		if (m_target == null)
		{
			m_target = base.transform;
		}
		m_defaultScale = m_target.localScale;
	}

	private void OnValueChanged(IUIObject obj)
	{
		GameObject gameObject = obj.gameObject;
		UIRadioBtn component = gameObject.GetComponent<UIRadioBtn>();
		if (!(component != null) || component.Value)
		{
			StartPunchScale();
		}
	}

	private void StartPunchScale()
	{
		if (m_active)
		{
			StopCoroutine("PunchScale");
			m_target.localScale = m_defaultScale;
		}
		StartCoroutine("PunchScale");
	}

	private void OnScaleChange()
	{
		if (m_active)
		{
			StopCoroutine("PunchScale");
		}
	}

	private IEnumerator PunchScale()
	{
		m_active = true;
		m_timeElapsed = 0f;
		float startTime = Time.realtimeSinceStartup;
		m_defaultScale = m_target.localScale;
		do
		{
			float factor = m_timeElapsed / m_duration;
			Vector3 scale = m_defaultScale;
			scale.x -= EZAnimation.punch(m_magnitude, factor);
			scale.y -= EZAnimation.punch(m_magnitude, factor);
			m_target.localScale = scale;
			yield return null;
		}
		while ((m_timeElapsed = Time.realtimeSinceStartup - startTime) < m_duration);
		m_target.localScale = m_defaultScale;
		m_active = false;
	}
}
