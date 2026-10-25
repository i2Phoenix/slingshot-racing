using UnityEngine;

[AddComponentMenu("GUI/In Race Grapple Buttons")]
public class InRaceGrappleButtons : MonoBehaviour
{
	public GameObject m_grappleButton;

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
					float num = (float)Screen.width / (float)Screen.height;
					float num2 = component.width + 5f;
					float num3 = component.height + 5f;
					float num4 = 100f * num - num2 * 0.5f;
					float num5 = 100f - num3 * 0.5f;
					float z = 0f;
					switch (j)
					{
					case 0:
						num4 = 0f - num4;
						num5 = 0f - num5;
						z = 270f;
						break;
					case 1:
						num5 = 0f - num5;
						break;
					case 2:
						num4 = 0f - num4;
						z = 180f;
						break;
					case 3:
						z = 90f;
						break;
					}
					Vector3 eulerAngles = component.transform.eulerAngles;
					eulerAngles.z = z;
					component.transform.eulerAngles = eulerAngles;
					gameObject.transform.parent = base.transform;
					gameObject.transform.position = new Vector3(num4, num5, 0f);
				}
			}
		}
	}
}
