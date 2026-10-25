using UnityEngine;

[RequireComponent(typeof(EZScreenPlacement))]
public class PipeGUI_InRaceButtonPlacement : MonoBehaviour
{
	public Transform m_child;

	private void Start()
	{
		EZScreenPlacement component = GetComponent<EZScreenPlacement>();
		if (Settings.RaceButtonOnRight)
		{
			component.relativeTo.horizontal = EZScreenPlacement.HORIZONTAL_ALIGN.SCREEN_RIGHT;
			component.PositionOnScreen();
			Vector3 localPosition = m_child.localPosition;
			localPosition.x = 0f - localPosition.x;
			m_child.localPosition = localPosition;
		}
	}
}
