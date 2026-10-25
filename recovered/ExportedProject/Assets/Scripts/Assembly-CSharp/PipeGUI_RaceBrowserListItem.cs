using UnityEngine;

public class PipeGUI_RaceBrowserListItem : MonoBehaviour
{
	public RaceEvent m_race;

	public void OnRaceSelected()
	{
		UIScrollList scrollList = GetComponent<UIListItemContainer>().GetScrollList();
		scrollList.GetComponent<PipeGUI_RaceBrowser>().OnRaceSelected(m_race);
	}
}
