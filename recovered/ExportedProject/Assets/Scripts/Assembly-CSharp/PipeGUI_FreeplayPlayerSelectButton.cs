using UnityEngine;

[AddComponentMenu("GUI/PipeGUI/FreeplayPlayerSelectionPlate")]
public class PipeGUI_FreeplayPlayerSelectButton : MonoBehaviour
{
	public enum State
	{
		Inactive = 0,
		AI = 1,
		Human = 2
	}

	private PipeGUI_FreeplayPlayerSelection m_controller;

	public UIStateToggleBtn m_overlay;

	public State StateNum
	{
		get
		{
			return (State)m_overlay.StateNum;
		}
		set
		{
			m_overlay.SetToggleState((int)value);
		}
	}

	public PipeGUI_FreeplayPlayerSelection Controller
	{
		set
		{
			m_controller = value;
		}
	}

	private void Start()
	{
	}

	private void Update()
	{
		BoxCollider component = m_overlay.GetComponent<BoxCollider>();
		PackedSprite component2 = GetComponent<PackedSprite>();
		component.center = -m_overlay.transform.localPosition;
		component.size = new Vector3(component2.width, component2.height, 0f);
	}

	private void OnToggleActive()
	{
		m_controller.Refresh();
	}
}
