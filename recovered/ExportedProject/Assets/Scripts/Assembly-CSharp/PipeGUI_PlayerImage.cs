using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PipeGUI_PlayerImage : MonoBehaviour
{
	private RaceManager m_raceManager;

	private string m_playerID;

	public bool m_useSignedInPlayer;

	public SpriteText m_text;

	public Renderer m_renderer;

	private void Start()
	{
		GameCenterManager.playerAuthenticated += OnPlayerAuthenticated;
		GameCenterManager.profilePhotoLoaded += OnProfilePhotoLoaded;
		GameCenterManager.playerDataLoaded += OnPlayerDataLoaded;
		GameObject gameObject = GameObject.FindGameObjectWithTag("RaceControl");
		if (gameObject != null)
		{
			m_raceManager = gameObject.GetComponent<RaceManager>();
			if (m_raceManager != null)
			{
				m_raceManager.OnRacePrepared += OnRacePrepared;
			}
		}
		m_renderer.material = new Material(m_renderer.material);
		if (m_useSignedInPlayer && GameCenterBinding.isPlayerAuthenticated())
		{
			OnPlayerAuthenticated();
		}
	}

	private void OnDestroy()
	{
		GameCenterManager.playerAuthenticated -= OnPlayerAuthenticated;
		GameCenterManager.profilePhotoLoaded -= OnProfilePhotoLoaded;
		GameCenterManager.playerDataLoaded -= OnPlayerDataLoaded;
		if (m_raceManager != null)
		{
			m_raceManager.OnRacePrepared -= OnRacePrepared;
		}
	}

	private void OnRacePrepared()
	{
		GameCenterChallenge currentChallenge = m_raceManager.CurrentChallenge;
		if (currentChallenge != null && m_playerID != currentChallenge.issuingPlayerID)
		{
			m_playerID = currentChallenge.issuingPlayerID;
			string[] playerIdArray = new string[1] { m_playerID };
			GameCenterBinding.loadPlayerData(playerIdArray, true);
		}
	}

	private void OnPlayerAuthenticated()
	{
		if (m_useSignedInPlayer)
		{
			GameCenterBinding.loadProfilePhotoForLocalPlayer();
		}
	}

	private void OnProfilePhotoLoaded(string _name)
	{
		Debug.Log(_name);
		string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(_name);
		Debug.Log(fileNameWithoutExtension);
		if (!(m_playerID == fileNameWithoutExtension))
		{
			return;
		}
		try
		{
			using (FileStream fileStream = File.Open(_name, FileMode.Open, FileAccess.Read, FileShare.Read))
			{
				if (fileStream != null)
				{
					long length = fileStream.Length;
					int offset = 0;
					int num = (int)length;
					byte[] array = new byte[num];
					int num2 = fileStream.Read(array, offset, num);
					if (num2 == length)
					{
						Texture2D texture2D = new Texture2D(128, 128, TextureFormat.ARGB32, true);
						texture2D.LoadImage(array);
						texture2D.Apply();
						m_renderer.material.mainTexture = texture2D;
					}
				}
			}
		}
		catch (Exception message)
		{
			Debug.Log(message);
		}
	}

	private void OnPlayerDataLoaded(List<GameCenterPlayer> _players)
	{
		if (!(m_text != null))
		{
			return;
		}
		foreach (GameCenterPlayer _player in _players)
		{
			if (_player.playerId == m_playerID)
			{
				m_text.Text = _player.alias;
				break;
			}
		}
	}
}
