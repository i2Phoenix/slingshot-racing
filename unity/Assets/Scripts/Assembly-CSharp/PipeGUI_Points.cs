using System.Collections;
using UnityEngine;

public class PipeGUI_Points : MonoBehaviour
{
	private IEnumerator Start()
	{
		RaceCampaignSave saveData = null;
		GameManager gameManager = GameObjectUtils.FindComponentWithTag<GameManager>("RaceControl");
		if (gameManager != null)
		{
			saveData = gameManager.SaveData;
		}
		else
		{
			yield return null;
			GameObject uiController = GameObject.FindGameObjectWithTag("UIController");
			saveData = uiController.GetComponent<RaceCampaignSave>();
		}
		SpriteText text = GetComponent<SpriteText>();
		if (text != null && saveData != null)
		{
			text.Text = string.Format("{0}/{1}", saveData.TotalPointsScored, saveData.TotalPointsAvailable);
		}
	}
}
