using System;
using UnityEngine;

[ExecuteInEditMode]
public class MenuScript : MonoBehaviour
{
	private const int c_maxTotalPlayers = 4;

	public GUISkin guiSkin;

	private GUIStyle titleStyle;

	private GUIStyle menuLabelStyle;

	private GUIStyle buttonStyle;

	private GUIContent grappleRacerText;

	private GUIContent loadingText;

	private GUIContent playersText;

	private GUIContent aiPlayersText;

	private GUIContent plusText;

	private GUIContent minusText;

	private GUIContent[] aiOptionTexts;

	private GUIContent trackText;

	private GUIContent quitText;

	private PersistentState persistentState;

	private float alphaFade;

	private bool isLoading;

	private void Awake()
	{
		base.useGUILayout = false;
		titleStyle = guiSkin.FindStyle("Title");
		menuLabelStyle = guiSkin.FindStyle("MenuLabel");
		buttonStyle = guiSkin.FindStyle("button");
		grappleRacerText = new GUIContent("Grapple Racer");
		loadingText = new GUIContent("Loading...");
		playersText = new GUIContent("Players");
		aiPlayersText = new GUIContent("AI Players");
		plusText = new GUIContent("+");
		minusText = new GUIContent("-");
		aiOptionTexts = new GUIContent[3]
		{
			new GUIContent("Easy"),
			new GUIContent("Medium"),
			new GUIContent("Hard")
		};
		trackText = new GUIContent("Track");
		quitText = new GUIContent("Quit");
	}

	private void Start()
	{
		Time.timeScale = 1f;
		Time.maximumDeltaTime = 0.1f;
		persistentState = PersistentState.Get();
		alphaFade = ((!Application.isPlaying) ? 1f : 0f);
		isLoading = false;
	}

	private void OnDestroy()
	{
		guiSkin = null;
		persistentState = null;
		GC.Collect();
	}

	private void Update()
	{
		float num = Time.deltaTime * 2f;
		alphaFade = Mathf.Min(1f, alphaFade + num);
	}

	private void OnGUI()
	{
		GUI.skin = guiSkin;
		GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3((float)Screen.width / 1920f, (float)Screen.height / 1200f, 1f));
		Color color = GUI.color;
		color.a = alphaFade;
		GUI.color = color;
		GUI.Label(new Rect(800f, 50f, 300f, 100f), grappleRacerText, titleStyle);
		if (isLoading)
		{
			GUI.Label(new Rect(800f, 500f, 300f, 100f), loadingText, titleStyle);
			return;
		}
		if (persistentState != null)
		{
			GUI.Label(new Rect(400f, 200f, 300f, 100f), playersText, menuLabelStyle);
			if (GUI.Button(new Rect(400f, 280f, 60f, 60f), minusText, buttonStyle))
			{
				int a = ((persistentState.numAIPlayersToSpawn <= 0) ? 1 : 0);
				persistentState.numPlayersToSpawn = Mathf.Max(a, persistentState.numPlayersToSpawn - 1);
			}
			GUI.Label(new Rect(400f, 250f, 300f, 100f), persistentState.numPlayersToSpawn.ToString(), menuLabelStyle);
			if (GUI.Button(new Rect(660f, 280f, 60f, 60f), plusText, buttonStyle))
			{
				persistentState.numPlayersToSpawn = Mathf.Min(4, persistentState.numPlayersToSpawn + 1);
				int val = 4 - persistentState.numPlayersToSpawn;
				persistentState.numAIPlayersToSpawn = Math.Min(val, persistentState.numAIPlayersToSpawn);
			}
			GUI.Label(new Rect(400f, 350f, 300f, 100f), aiPlayersText, menuLabelStyle);
			if (GUI.Button(new Rect(400f, 430f, 60f, 60f), minusText, buttonStyle))
			{
				int a2 = ((persistentState.numPlayersToSpawn <= 0) ? 1 : 0);
				persistentState.numAIPlayersToSpawn = Mathf.Max(a2, persistentState.numAIPlayersToSpawn - 1);
			}
			GUI.Label(new Rect(400f, 400f, 300f, 100f), persistentState.numAIPlayersToSpawn.ToString(), menuLabelStyle);
			if (GUI.Button(new Rect(660f, 430f, 60f, 60f), plusText, buttonStyle))
			{
				persistentState.numAIPlayersToSpawn = Mathf.Min(4, persistentState.numAIPlayersToSpawn + 1);
				int val2 = 4 - persistentState.numAIPlayersToSpawn;
				persistentState.numPlayersToSpawn = Math.Min(val2, persistentState.numPlayersToSpawn);
			}
			persistentState.aiDifficulty = (AIDifficulty)GUI.SelectionGrid(new Rect(400f, 540f, 300f, 200f), (int)persistentState.aiDifficulty, aiOptionTexts, 1);
			int num = 820;
			Settings.DynamicCamera = GUI.Toggle(new Rect(400f, num, 300f, 100f), Settings.DynamicCamera, "Dynamic Camera");
			num += 80;
			Settings.DynamicCameraTrackAI = GUI.Toggle(new Rect(400f, num, 300f, 100f), Settings.DynamicCameraTrackAI, "Track AI Players");
			num += 80;
			persistentState.m_aiControlHumans = GUI.Toggle(new Rect(400f, num, 300f, 100f), persistentState.m_aiControlHumans, "AI Control Humans");
			num += 80;
		}
		string[] array = new string[14]
		{
			"TitleScreen", "Track_Tutorial_1.3_Blockout", "Track_1", "Track_DJS_1", "Track_MAH_1", "Track_MAH_2", "Track_MAH_3", "Track_MAH_4", "Track_MJS_1", "Track_MJS_2",
			"Track_TJH_2_Blockout", "Track_TJH_3", "Track_1_Blockout", "SoakTest"
		};
		float num2 = 800f;
		float num3 = 250f;
		float num4 = 400f;
		float num5 = 60f;
		int num6 = 5;
		float num7 = num2;
		float num8 = num3;
		int num9 = 0;
		for (int i = 0; i < array.Length; i++)
		{
			string text = array[i];
			text = text.Replace('_', ' ');
			Vector2 vector = buttonStyle.CalcSize(trackText);
			if (GUI.Button(new Rect(num7, num8, vector.x + 200f, vector.y + 30f), text, buttonStyle))
			{
				isLoading = true;
				Application.LoadLevel(array[i]);
			}
			num8 += vector.y + num5;
			num9++;
			if (num9 > num6)
			{
				num7 += num4;
				num8 = num3;
				num9 = 0;
			}
		}
		if (GUI.Button(new Rect(800f, 1000f, 300f, 100f), quitText, buttonStyle))
		{
			Application.Quit();
		}
	}
}
