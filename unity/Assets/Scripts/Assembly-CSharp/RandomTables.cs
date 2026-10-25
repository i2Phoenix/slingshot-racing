using System;
using UnityEngine;

public static class RandomTables
{
	private const int c_tableSize = 256;

	private static Vector3[] s_insideUnitSphereTable;

	private static Vector2[] s_insideUnitCircleTable;

	private static Vector2[] s_onUnitCircleTable;

	private static float[] s_valueTable;

	private static int s_nextInsideUnitSphere;

	private static int s_nextInsideUnitCircle;

	private static int s_nextOnUnitCircle;

	private static int s_nextValue;

	public static Vector3 InsideUnitSphere
	{
		get
		{
			int num = s_nextInsideUnitSphere++;
			if (s_nextInsideUnitSphere >= 256)
			{
				s_nextInsideUnitSphere = 0;
			}
			return s_insideUnitSphereTable[num];
		}
	}

	public static Vector2 InsideUnitCircle
	{
		get
		{
			int num = s_nextInsideUnitCircle++;
			if (s_nextInsideUnitCircle >= 256)
			{
				s_nextInsideUnitCircle = 0;
			}
			return s_insideUnitCircleTable[num];
		}
	}

	public static Vector2 OnUnitCircle
	{
		get
		{
			int num = s_nextOnUnitCircle++;
			if (s_nextOnUnitCircle >= 256)
			{
				s_nextOnUnitCircle = 0;
			}
			return s_onUnitCircleTable[num];
		}
	}

	public static float Value
	{
		get
		{
			int num = s_nextValue++;
			if (s_nextValue >= 256)
			{
				s_nextValue = 0;
			}
			return s_valueTable[num];
		}
	}

	static RandomTables()
	{
		s_insideUnitSphereTable = new Vector3[256];
		s_insideUnitCircleTable = new Vector2[256];
		s_onUnitCircleTable = new Vector2[256];
		s_valueTable = new float[256];
		for (int i = 0; i < 256; i++)
		{
			s_insideUnitSphereTable[i] = UnityEngine.Random.insideUnitSphere;
			s_insideUnitCircleTable[i] = UnityEngine.Random.insideUnitCircle;
			float f = UnityEngine.Random.value * (float)Math.PI * 2f;
			s_onUnitCircleTable[i].x = Mathf.Cos(f);
			s_onUnitCircleTable[i].y = Mathf.Sin(f);
			s_valueTable[i] = UnityEngine.Random.value;
		}
	}
}
