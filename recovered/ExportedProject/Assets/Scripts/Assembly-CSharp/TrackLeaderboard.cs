using System.Collections;
using UnityEngine;

public class TrackLeaderboard : MonoBehaviour
{
	private struct VehicleData
	{
		public float m_start;

		public float m_end;

		public ITBPlayer m_player;

		public int m_points;
	}

	private TurnBasedMatchManager m_turnBasedMatchManager;

	private ITBTournament m_tournament;

	private VehicleData[] m_vehicleData;

	public RaceCampaign m_campaign;

	public UISlider[] m_sliders;

	public SpriteText[] m_playerNames;

	private void Start()
	{
		ITBTournament tour = null;
		GameObject gameObject = GameObject.FindGameObjectWithTag("RaceControl");
		if (gameObject != null)
		{
			m_turnBasedMatchManager = gameObject.GetComponent<TurnBasedMatchManager>();
			if (m_turnBasedMatchManager != null)
			{
				tour = m_turnBasedMatchManager.CurrentMatchData;
				m_turnBasedMatchManager.OnMatchDataUpdatedEvent += OnMatchDataUpdated;
			}
		}
		SetTour(tour);
	}

	private void OnDestroy()
	{
		if (m_turnBasedMatchManager != null)
		{
			m_turnBasedMatchManager.OnMatchDataUpdatedEvent -= OnMatchDataUpdated;
		}
	}

	public void SetTour(ITBTournament _tour)
	{
		m_tournament = _tour;
		Refresh();
	}

	private void Refresh()
	{
		StopAllCoroutines();
		DestroyVehicles();
		int numRaces = m_tournament.NumRaces;
		m_vehicleData = new VehicleData[4];
		int num = 0;
		int num2 = 0;
		for (int i = 0; i < numRaces; i++)
		{
			ITBRace race = m_tournament.GetRace(i);
			if (race.RaceState != TurnBasedRaceState.Completed && race.RaceState != TurnBasedRaceState.CompleteWithGhost)
			{
				break;
			}
			num++;
			for (int j = 0; j < race.NumRaceResults; j++)
			{
				ITBPlayerResults raceResults = race.GetRaceResults(j);
				ITBPlayer player = raceResults.Player;
				int colourIndex = player.ColourIndex;
				if (colourIndex >= 0)
				{
					m_vehicleData[colourIndex].m_player = player;
					m_vehicleData[colourIndex].m_points += RaceCampaign.GetPointsForPosition(raceResults.Position);
					num2 = Mathf.Max(num2, m_vehicleData[colourIndex].m_points);
				}
			}
		}
		int num3 = 0;
		VehicleData[] vehicleData = m_vehicleData;
		for (int k = 0; k < vehicleData.Length; k++)
		{
			VehicleData vehicleData2 = vehicleData[k];
			num3 += ((vehicleData2.m_player != null) ? 1 : 0);
		}
		float num4 = 1f;
		float num5 = num4 / (float)numRaces;
		float num6 = num5 * (float)num;
		for (int l = 0; l < 4; l++)
		{
			string empty = string.Empty;
			if (m_vehicleData[l].m_player != null)
			{
				float num7 = 0f;
				if (num2 > 0)
				{
					num7 = num6 * ((float)m_vehicleData[l].m_points / (float)num2);
				}
				float num8 = 0f;
				float end = num8 + num7;
				m_vehicleData[l].m_start = num8;
				m_vehicleData[l].m_end = end;
				empty = m_vehicleData[l].m_player.Name;
			}
			m_playerNames[l].Text = empty;
			m_sliders[l].Value = 0f;
		}
	}

	public void Animate()
	{
		if (m_vehicleData == null)
		{
			return;
		}
		StopAllCoroutines();
		for (int i = 0; i < 4; i++)
		{
			if (m_vehicleData[i].m_player != null)
			{
				StartCoroutine(PlayAnimation(m_vehicleData[i], m_sliders[i], 0.5f));
			}
		}
	}

	private IEnumerator PlayAnimation(VehicleData _data, UISlider _slider, float _speed)
	{
		yield return YieldCache.WaitForOneSecond;
		float totalDistance = _data.m_end - _data.m_start;
		if (totalDistance > 0.001f)
		{
			for (float distance = 0f; distance < totalDistance; distance = (_slider.Value = distance + _speed * Time.deltaTime))
			{
				yield return null;
			}
			_slider.Value = _data.m_end;
		}
	}

	private void OnMatchDataUpdated()
	{
		if (m_turnBasedMatchManager != null)
		{
			ITBTournament currentMatchData = m_turnBasedMatchManager.CurrentMatchData;
			SetTour(currentMatchData);
		}
		else
		{
			Refresh();
		}
	}

	private void DestroyVehicles()
	{
		m_vehicleData = null;
	}
}
