using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("GUI/In Race Grapple Buttons")]
public class InRaceGrappleButtons : MonoBehaviour
{
	public GameObject m_grappleButton;

	private readonly Dictionary<PackedSprite, int> m_buttons = new Dictionary<PackedSprite, int>();

	public void Configure(RaceManager raceManager, TouchInputManager inputManager)
	{
		int numHumans = inputManager.NumHumans;
		if (numHumans <= 1 || UIGamepadManager.IsGamepadActive)
		{
			return;
		}
		for (int i = 0; i != numHumans; i++)
		{
			TouchInputManager.CornerMask cornerMask = inputManager.GetCornerMask(i);
			int colourIndex = raceManager.GetVehicle(i).ColourIndex;
			for (int j = 0; j != 4; j++)
			{
				if (((uint)(1 << j) & (uint)cornerMask) != 0)
				{
					GameObject gameObject = (GameObject)Object.Instantiate(m_grappleButton);
					PackedSprite component = gameObject.GetComponent<PackedSprite>();
					component.defaultAnim = colourIndex;
					component.PlayAnim(colourIndex);
					float[] rotations = { 270f, 0f, 180f, 90f };
					component.transform.rotation = Quaternion.Euler(0f, 0f, rotations[j]);
					gameObject.transform.parent = transform;
					m_buttons.Add(component, j);
					PositionButton(component, j);
				}
			}
		}
	}

	private void LateUpdate()
	{
		foreach (var button in m_buttons) PositionButton(button.Key, button.Value);
	}

	private static void PositionButton(PackedSprite button, int corner)
	{
		if (button == null || !UIManager.Exists()) return;
		Camera camera = UIManager.instance.uiCameras[0].camera;
		Rect safe = Screen.safeArea;
		bool left = (corner & 1) == 0;
		bool bottom = corner < 2;
		Vector3 position = camera.ScreenToWorldPoint(new Vector3(
			left ? safe.xMin : safe.xMax, bottom ? safe.yMin : safe.yMax, 10f));
		position.x += (left ? 1f : -1f) * (button.width + 5f) * 0.5f;
		position.y += (bottom ? 1f : -1f) * (button.height + 5f) * 0.5f;
		position.z = 0f;
		button.transform.position = position;
	}
}
