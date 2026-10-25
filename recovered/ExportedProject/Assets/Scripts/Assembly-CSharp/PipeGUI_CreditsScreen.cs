using UnityEngine;

public class PipeGUI_CreditsScreen : MonoBehaviour
{
	private GameManager m_gameManager;

	public SpriteText m_creditsText;

	private void Start()
	{
		GameObject gameObject = GameObject.FindGameObjectWithTag("RaceControl");
		if (gameObject != null)
		{
			m_gameManager = gameObject.GetComponent<GameManager>();
		}
		string empty = string.Empty;
		string text = empty;
		empty = text + Language.Get("ID_GameTitle") + " " + AppSettings.VersionTrimmed + "\n";
		empty = empty + Language.Get("ID_CreditsCopyright") + "\n\n";
		empty = empty + Language.Get("ID_CreditsDeveloped") + ":\n";
		empty += "Matthew Halpin, Peter Halpin, Tim Hughes,\nDean Searle, Matthew Simper\n\n";
		empty = empty + Language.Get("ID_CreditsSoundDesign") + ": Jerry Leeper\n";
		text = empty;
		empty = text + Language.Get("ID_CreditsGameMusic") + ": " + Language.Get("ID_CreditsCopyrightSymbol") + "2012 Andrew J Goose (www.studio96.co.uk)\n";
		empty = empty + Language.Get("ID_CreditsThemeMusic") + ": Sean Beeson\n\n";
		empty = empty + Language.Get("ID_CreditsPublishedBy") + " Crescent Moon Games\n\n";
		empty = empty + Language.Get("ID_CreditsSpecialThanks") + ": Adhoc Actors, Nikki Braine, Steven Burgess, Andrew Chappell, Oscar Cooper, Matthew Dickinson, Ben Dowie, Phil Duncan, Craig Edwards, Richard Hole, Philip Hughes, Colm Mac Donnchadha, Michael Maidment, Owen McCarthy, Natalie Miller, Matthew Preece, Gary Richards, Ian Shaw, Andrew Scott, Rob Shoebottom";
		m_creditsText.Text = empty;
	}

	private void OnBackSelected()
	{
		if (m_gameManager != null)
		{
			m_gameManager.OnOptionsSelected();
			m_gameManager = null;
		}
	}
}
