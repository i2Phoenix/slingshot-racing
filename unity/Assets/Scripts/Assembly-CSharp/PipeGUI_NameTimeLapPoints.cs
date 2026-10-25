using System;
using UnityEngine;

public class PipeGUI_NameTimeLapPoints : MonoBehaviour
{
	[Serializable]
	public class TextPlate
	{
		public GameObject m_object;

		private SpriteText m_text;

		public float Time
		{
			set
			{
				if (m_text != null)
				{
					if (value > 0f)
					{
						TimeSpan timeSpan = TimeSpan.FromSeconds(value);
						m_text.Text = string.Format("{0:D1}:{1:D2}.{2:D3}", timeSpan.Minutes, timeSpan.Seconds, timeSpan.Milliseconds);
					}
					else
					{
						m_text.Text = "--:--.---";
					}
				}
			}
		}

		public int Points
		{
			set
			{
				if (m_text != null)
				{
					m_text.Text = value.ToString();
				}
			}
		}

		public bool Active
		{
			set
			{
				if (m_object != null)
				{
					m_object.GetComponent<Renderer>().enabled = value;
				}
			}
		}

		public void FindText()
		{
			m_text = ((!(m_object != null)) ? null : m_object.GetComponentInChildren<SpriteText>());
		}

		public void SetTime(int _lapsBehind, float _time)
		{
			if (m_text != null)
			{
				if (_lapsBehind < 0)
				{
					TimeSpan timeSpan = TimeSpan.FromSeconds(_time);
					m_text.Text = string.Format("{0:D1}:{1:D2}.{2:D3}", timeSpan.Minutes, timeSpan.Seconds, timeSpan.Milliseconds);
				}
				else if (_lapsBehind == 0)
				{
					m_text.Text = "+" + UIUtils.FormatBestLapTime(_time);
				}
				else
				{
					m_text.Text = "+" + _lapsBehind + " " + Language.Get("ID_laps");
				}
			}
		}
	}

	private bool m_dataValid;

	public PackedSprite m_playerPlate;

	public SpriteText m_playerName;

	public TextPlate m_raceTime;

	public TextPlate m_bestLapTime;

	public TextPlate m_points;

	public UIBistateInteractivePanel m_displayName;

	public UIBistateInteractivePanel m_displayTimes;

	public UIBistateInteractivePanel m_displayPoints;

	public bool IsValid
	{
		get
		{
			return m_dataValid;
		}
	}

	private void Start()
	{
		if (m_raceTime != null)
		{
			m_raceTime.FindText();
		}
		if (m_bestLapTime != null)
		{
			m_bestLapTime.FindText();
		}
		if (m_points != null)
		{
			m_points.FindText();
		}
	}

	public void SetData(string _name, int _colourIndex, int _points)
	{
		if (m_playerPlate != null)
		{
			m_playerPlate.gameObject.active = true;
			m_playerPlate.PlayAnim(_colourIndex);
			m_playerPlate.defaultAnim = _colourIndex;
		}
		if (m_playerName != null)
		{
			m_playerName.Text = _name;
		}
		if (m_points != null)
		{
			m_points.Points = _points;
		}
		m_dataValid = true;
	}

	public void SetData(string _name, int _colourIndex, int _lapsBehind, float _raceTime, float _bestLap, int _points)
	{
		SetData(_name, _colourIndex, _points);
		if (m_raceTime != null)
		{
			m_raceTime.SetTime(_lapsBehind, _raceTime);
		}
		if (m_bestLapTime != null)
		{
			m_bestLapTime.Time = _bestLap;
		}
		m_dataValid = true;
	}

	public void Invalidate()
	{
		m_dataValid = false;
	}

	public void ShowTimes()
	{
		BringInHelper(m_displayName);
		BringInHelper(m_displayTimes);
		DismissHelper(m_displayPoints);
		RefreshPlayerPlate();
	}

	public void ShowPoints()
	{
		BringInHelper(m_displayName);
		BringInHelper(m_displayPoints);
		DismissHelper(m_displayTimes);
		RefreshPlayerPlate();
	}

	private void RefreshPlayerPlate()
	{
		// SetData can arrive while this row's ancestors are inactive. EZ GUI
		// ignores PlayAnim in that state, so apply its saved colour on reveal.
		if (m_playerPlate != null)
			m_playerPlate.PlayAnim(m_playerPlate.defaultAnim);
	}

	public void Hide()
	{
		DismissHelper(m_displayName);
		DismissHelper(m_displayTimes);
		DismissHelper(m_displayPoints);
	}

	private static void BringInHelper(UIBistateInteractivePanel _panel)
	{
		// Restore the row and Results container without revealing sibling panels.
		for (Transform parent = _panel.transform; parent != null; parent = parent.parent)
			parent.gameObject.SetActive(true);
		_panel.SendMessage("BringIn");
	}

	private static void DismissHelper(UIBistateInteractivePanel _panel)
	{
		if (_panel.gameObject.active)
		{
			_panel.SendMessage("Dismiss");
		}
	}
}
