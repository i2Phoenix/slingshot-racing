using UnityEngine;

[AddComponentMenu("GUI/Player Select Manager")]
public class PlayerSelectManager : MonoBehaviour
{
	public enum Point
	{
		BottomLeft = 0,
		BottomRight = 1,
		TopLeft = 2,
		TopRight = 3,
		Centre = 4
	}

	private class VehicleInfo
	{
		public PhysicsVehicle m_component;

		public Point m_currentPoint;

		public Point m_targetPoint;

		public float m_circleRadius;

		public float m_angle;

		public bool[] m_validTargets = new bool[5];
	}

	private class PlayerInfo
	{
		public int m_vehicleIndex = -1;

		public bool m_enabled;

		public TouchRegion m_touchRegion;
	}

	private static string[] c_grapplePointNames = new string[5] { "GrapplePoint_BL", "GrapplePoint_BR", "GrapplePoint_TL", "GrapplePoint_TR", "GrapplePoint_Centre" };

	private static float[] c_baseAngles = new float[4] { 45f, 135f, 315f, 225f };

	private float m_posAngle0;

	private float m_posAngle1;

	private float m_posAngle2;

	private float m_posAngle3;

	private GrapplePoint[] m_grapplePoints = new GrapplePoint[5];

	private PlayerInfo[] m_players = new PlayerInfo[4];

	private VehicleInfo[] m_vehicles;

	public int m_maxPlayers = 4;

	public float m_circleRadius = 9f;

	public float m_extraRadius = 3f;

	public float m_grappleWindRate = 1f;

	public float m_targetInnerFactor = 0.5f;

	public float m_targetOuterFactor = 2f;

	private void Start()
	{
		for (int i = 0; i != 5; i++)
		{
			m_grapplePoints[i] = GameObject.Find(c_grapplePointNames[i]).GetComponent<GrapplePoint>();
		}
		GrapplePoint grapplePoint = m_grapplePoints[4];
		m_vehicles = new VehicleInfo[m_maxPlayers];
		float num = 360f / (float)m_maxPlayers;
		for (int j = 0; j != m_maxPlayers; j++)
		{
			Quaternion quaternion = Quaternion.Euler(0f, (float)j * num, 0f);
			Vector3 position = quaternion * Vector3.right * m_circleRadius + grapplePoint.Position;
			PhysicsVehicle physicsVehicle = VehicleFactory.Create("PlayerSelect", j, j, position, quaternion) as PhysicsVehicle;
			m_vehicles[j] = new VehicleInfo();
			m_vehicles[j].m_component = physicsVehicle;
			m_vehicles[j].m_currentPoint = Point.Centre;
			m_vehicles[j].m_targetPoint = Point.Centre;
			physicsVehicle.GrapplePointOverride = grapplePoint;
			physicsVehicle.GrappleDistOverride = m_circleRadius;
			physicsVehicle.ButtonDown();
		}
		for (int k = 0; k != 4; k++)
		{
			float num2 = 0.4f;
			float left = 0f;
			float top = 0f;
			switch ((Point)k)
			{
			case Point.BottomLeft:
				left = 0f;
				top = 0f;
				break;
			case Point.BottomRight:
				left = 1f - num2;
				top = 0f;
				break;
			case Point.TopLeft:
				left = 0f;
				top = 1f - num2;
				break;
			case Point.TopRight:
				left = 1f - num2;
				top = 1f - num2;
				break;
			}
			m_players[k] = new PlayerInfo();
			m_players[k].m_touchRegion = new TouchRegion(new Rect(left, top, num2, num2));
		}
	}

	private void Update()
	{
		for (int i = 0; i != m_players.Length; i++)
		{
			PlayerInfo playerInfo = m_players[i];
			if (playerInfo.m_touchRegion.GetTouchDown())
			{
				playerInfo.m_enabled = !playerInfo.m_enabled;
				if (playerInfo.m_enabled)
				{
					m_grapplePoints[i].transform.localScale = Vector3.one * 4f;
				}
				else
				{
					m_grapplePoints[i].transform.localScale = Vector3.one * 2f;
				}
			}
		}
	}

	private void FixedUpdate()
	{
		for (int i = 0; i != m_vehicles.Length; i++)
		{
			FixedUpdate(m_vehicles[i]);
		}
		for (int j = 0; j != m_players.Length; j++)
		{
			FixedUpdate(j, m_players[j]);
		}
	}

	private void FixedUpdate(int p, PlayerInfo player)
	{
		if (player.m_vehicleIndex == -1 && player.m_enabled)
		{
			int num = -1;
			float num2 = -1f;
			for (int i = 0; i != m_vehicles.Length; i++)
			{
				if (m_vehicles[i].m_targetPoint == Point.Centre && m_vehicles[i].m_currentPoint == Point.Centre && m_vehicles[i].m_validTargets[p])
				{
					float num3 = Mod360(m_vehicles[i].m_angle - c_baseAngles[p]);
					if (num3 > num2)
					{
						num2 = num3;
						num = i;
					}
				}
			}
			if (num != -1)
			{
				player.m_vehicleIndex = num;
				m_vehicles[num].m_targetPoint = (Point)p;
				m_grapplePoints[p].transform.localScale = Vector3.one * 3f;
			}
		}
		else if (player.m_vehicleIndex != -1 && !player.m_enabled)
		{
			VehicleInfo vehicleInfo = m_vehicles[player.m_vehicleIndex];
			if (vehicleInfo.m_currentPoint == vehicleInfo.m_targetPoint && vehicleInfo.m_targetPoint != Point.Centre)
			{
				vehicleInfo.m_targetPoint = Point.Centre;
				player.m_vehicleIndex = -1;
			}
		}
	}

	private void FixedUpdate(VehicleInfo vehicle)
	{
		PhysicsVehicle component = vehicle.m_component;
		Vector2 vector = XZPos(component.Position);
		Vector3 vector2 = XZDir(component.Right);
		Vector3 vector3 = XZDir(component.Forward);
		Point currentPoint = vehicle.m_currentPoint;
		Point targetPoint = vehicle.m_targetPoint;
		if (currentPoint != targetPoint)
		{
			if (component.IsPressing)
			{
				if (vehicle.m_validTargets[(int)targetPoint])
				{
					component.GrapplePointOverride = null;
					component.ButtonUp();
				}
			}
			else
			{
				GrapplePoint grapplePoint = m_grapplePoints[(int)targetPoint];
				Vector2 vector4 = XZPos(grapplePoint.Position) - vector;
				float num = Vector3.Dot(vector3, vector4);
				if (num < 0f)
				{
					component.GrapplePointOverride = grapplePoint;
					component.ButtonDown();
					vehicle.m_currentPoint = targetPoint;
				}
			}
		}
		vehicle.m_circleRadius = m_circleRadius;
		if (currentPoint == Point.Centre && targetPoint == Point.Centre)
		{
			float num2 = 360f;
			float num3 = 360f;
			for (int i = 0; i != m_vehicles.Length; i++)
			{
				VehicleInfo vehicleInfo = m_vehicles[i];
				if (vehicleInfo != vehicle && vehicleInfo.m_currentPoint == Point.Centre && vehicleInfo.m_targetPoint == Point.Centre)
				{
					num2 = Mathf.Min(num2, Mod360(vehicle.m_angle - vehicleInfo.m_angle));
					num3 = Mathf.Min(num3, Mod360(vehicleInfo.m_angle - vehicle.m_angle));
				}
			}
			float num4 = Mathf.Clamp((num2 - num3) / 10f, -1f, 1f);
			vehicle.m_circleRadius += m_extraRadius * num4;
		}
		GrapplePoint grapplePoint2 = m_grapplePoints[(int)currentPoint];
		Vector2 vector5 = XZPos(grapplePoint2.Position) - vector;
		float num5 = Mathf.Atan2(vector5.x, 0f - vector5.y) * 57.29578f;
		if (currentPoint != Point.Centre)
		{
			num5 = 0f - num5;
		}
		vehicle.m_angle = Mod360(num5);
		for (Point point = Point.BottomLeft; point <= Point.Centre; point++)
		{
			vehicle.m_validTargets[(int)point] = false;
			if ((currentPoint != Point.Centre || point == Point.Centre) && (currentPoint == Point.Centre || point != Point.Centre))
			{
				continue;
			}
			GrapplePoint grapplePoint3 = m_grapplePoints[(int)point];
			Vector2 vector6 = XZPos(grapplePoint3.Position) - vector;
			Vector2 vector7 = ((currentPoint != Point.Centre) ? (-vector2) : vector2);
			if (Vector2.Dot(vector6, vector3) > 0f)
			{
				float num6 = Vector3.Dot(vector6, vector7);
				if (num6 >= m_circleRadius * m_targetInnerFactor && num6 <= m_circleRadius * m_targetOuterFactor)
				{
					vehicle.m_validTargets[(int)point] = true;
				}
			}
		}
		float grappleDistOverride = component.GrappleDistOverride;
		float f = vehicle.m_circleRadius - grappleDistOverride;
		float num7 = Mathf.Min(Mathf.Abs(f), m_grappleWindRate * Time.deltaTime);
		grappleDistOverride += Mathf.Sign(f) * num7;
		component.GrappleDistOverride = grappleDistOverride;
	}

	private static Vector2 XZDir(Vector3 vec)
	{
		return new Vector2(vec.x, vec.z).normalized;
	}

	private static Vector2 XZPos(Vector3 vec)
	{
		return new Vector2(vec.x, vec.z);
	}

	private static float Mod360(float angle)
	{
		while (angle > 360f)
		{
			angle -= 360f;
		}
		while (angle < 0f)
		{
			angle += 360f;
		}
		return angle;
	}
}
