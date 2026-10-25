using System.Collections;
using UnityEngine;

[RequireComponent(typeof(UIBistateInteractivePanel))]
public class PipeGUI_RaceResults_Panel : MonoBehaviour
{
	private RaceManager m_raceManager;

	private UIBistateInteractivePanel m_panel;

	public RaceManager.DisplayResults m_displayOn;

	public RaceManager.DisplayResults m_displayOnAlt = RaceManager.DisplayResults.Null;

	public bool m_hideOnOthers;

	public bool m_waitOnFanfare;

	public bool m_delayedDeactivate;

	private IEnumerator Start()
	{
		m_raceManager = GameObjectUtils.FindComponentWithTag<RaceManager>("RaceControl");
		if (m_raceManager != null)
		{
			m_raceManager.OnDisplayResults += OnDisplayResults;
		}
		m_panel = GetComponent<UIBistateInteractivePanel>();
		if (m_delayedDeactivate)
		{
			yield return null;
			yield return null;
			base.gameObject.SetActiveRecursively(false);
		}
	}

	private void OnDestroy()
	{
		if (m_raceManager != null)
		{
			m_raceManager.OnDisplayResults -= OnDisplayResults;
		}
	}

	private void OnFanfareComplete()
	{
		if (m_waitOnFanfare)
		{
			m_panel.gameObject.active = true;
			m_panel.SendMessage("BringIn");
		}
	}

	private void OnDisplayResults(RaceManager.DisplayResults _display)
	{
		if (_display == m_displayOn || _display == m_displayOnAlt)
		{
			if (!m_waitOnFanfare)
			{
				m_panel.gameObject.active = true;
				m_panel.SendMessage("BringIn");
			}
		}
		else if ((m_hideOnOthers || _display == RaceManager.DisplayResults.Hide) && m_panel.gameObject.active)
		{
			m_panel.SendMessage("Dismiss");
		}
	}
}
