using System;
using System.Collections;
using UnityEngine;

public class FinishLine : MonoBehaviour
{
	private class SplineOrdering
	{
		private BitArray m_bitArray;

		private int m_numSplines;

		public SplineOrdering(int numSplines)
		{
			m_bitArray = new BitArray(numSplines * numSplines);
			m_numSplines = numSplines;
		}

		public bool Less(int left, int right)
		{
			return m_bitArray[left * m_numSplines + right];
		}

		public void SetLess(int left, int right)
		{
			m_bitArray[left * m_numSplines + right] = true;
		}

		public int Compare(TrackSpline left, TrackSpline right)
		{
			if (Less(left.Index, right.Index))
			{
				return -1;
			}
			if (Less(right.Index, left.Index))
			{
				return 1;
			}
			return 0;
		}
	}

	private TrackSpline m_trackSpline;

	private GameObject m_grid;

	private float m_splineT;

	private int m_numWaypoints;

	private int m_numSplines;

	private bool m_reversed;

	public int m_priority;

	public GameObject DynamicCamera;

	public GameObject m_gridPrefab;

	public float m_gridScaleTweak = 1f;

	public float m_gridYOffset;

	public bool m_disableRendering;

	public bool m_spawnVehicles = true;

	public float m_bestLapTime;

	public float m_bestLapTimeRev;

	public float m_aiAdjust;

	public float m_aiAdjustRev;

	public TrackSpline TrackSpline
	{
		get
		{
			return m_trackSpline;
		}
	}

	public float TrackSplineT
	{
		get
		{
			return m_splineT;
		}
	}

	public int NumWaypoints
	{
		get
		{
			return m_numWaypoints;
		}
	}

	public int RequiredLapSequence
	{
		get
		{
			return m_numWaypoints;
		}
	}

	public float BestLapTime
	{
		get
		{
			return (!m_reversed) ? m_bestLapTime : m_bestLapTimeRev;
		}
	}

	public float AIAdjust
	{
		get
		{
			return (!m_reversed) ? m_aiAdjust : m_aiAdjustRev;
		}
	}

	public TrackLocation Location
	{
		get
		{
			return new TrackLocation(m_trackSpline, m_splineT);
		}
	}

	public void OnReverseTrack()
	{
		m_reversed = !m_reversed;
	}

	private void Awake()
	{
		GameObject[] array = GameObject.FindGameObjectsWithTag("Finish");
		GameObject[] array2 = array;
		foreach (GameObject gameObject in array2)
		{
			if (!(gameObject != base.gameObject))
			{
				continue;
			}
			FinishLine component = gameObject.GetComponent<FinishLine>();
			if (component.m_priority <= m_priority)
			{
				gameObject.active = false;
				if (component.m_grid != null)
				{
					component.m_grid.SetActiveRecursively(false);
				}
			}
			else if (m_priority < component.m_priority)
			{
				base.gameObject.active = false;
				if (m_grid != null)
				{
					m_grid.SetActiveRecursively(false);
				}
			}
		}
	}

	private void Start()
	{
		Transform transform = base.transform;
		m_trackSpline = TrackSpline.FindClosestToPoint(transform.position);
		PersistentState persistentState = PersistentState.Get();
		m_splineT = m_trackSpline.ClosestTTo(transform.position);
		float length = m_trackSpline.Length;
		float num = m_trackSpline.ReParamTtoS(m_splineT);
		if (m_trackSpline.HubAtTail == null)
		{
			if (num > length - 1.25f)
			{
				Debug.LogWarning("FinishLine is too close to spline end, snapping to start");
				m_splineT = 0f;
			}
			else if (num > length - 2.5f)
			{
				Debug.LogWarning("FinishLine is too close to spline end, snapping to end limit");
				m_splineT = m_trackSpline.ReParamStoT(length - 2.5f);
			}
		}
		else if (num < 2.5f)
		{
			Debug.LogWarning("FinishLine is too close to spline start, snapping to start limit");
			m_splineT = m_trackSpline.ReParamStoT(2.5f);
		}
		else if (num > length - 2.5f)
		{
			Debug.LogWarning("FinishLine is too close to spline end, snapping to end limit");
			m_splineT = m_trackSpline.ReParamStoT(length - 2.5f);
		}
		Vector3 o_point;
		Vector3 o_tangent;
		Vector3 o_normal;
		float o_width;
		m_trackSpline.InfoAtT(m_splineT, out o_point, out o_tangent, out o_normal, out o_width);
		Vector3 vector = m_trackSpline.UpAtT(m_splineT);
		float num2 = m_trackSpline.HeightAt(m_splineT, 0f);
		Vector3 vector2 = o_point;
		vector2.y += num2;
		Vector3 vector3 = vector2;
		vector3.y++;
		transform.position = vector3;
		transform.rotation = Quaternion.LookRotation(o_tangent, vector);
		if (!m_disableRendering)
		{
			LineRenderer lineRenderer = base.gameObject.AddComponent<LineRenderer>();
			lineRenderer.SetPosition(0, vector3 + vector * 0.01f - o_width * o_normal);
			lineRenderer.SetPosition(1, vector3 + vector * 0.01f + o_width * o_normal);
			lineRenderer.SetColors(Color.green, Color.green);
			lineRenderer.sharedMaterial = (Material)Resources.Load("Materials/VertexColour");
		}
		if (m_gridPrefab != null)
		{
			m_grid = UnityEngine.Object.Instantiate(m_gridPrefab) as GameObject;
			float num3 = o_width / 6f;
			num3 *= m_gridScaleTweak;
			float y = m_grid.GetComponentInChildren<MeshFilter>().mesh.bounds.size.y;
			y *= num3;
			Vector3 position = vector2;
			position += transform.forward * (y * 0.5f);
			position.y += m_gridYOffset;
			m_grid.transform.position = position;
			m_grid.transform.rotation = transform.rotation;
			m_grid.transform.localScale = new Vector3(num3, num3, num3);
			m_grid.transform.parent = base.transform;
		}
		Vehicle[] array = null;
		if (m_spawnVehicles && GameObject.FindWithTag("RaceControl") == null)
		{
			GameObject gameObject = GameObject.FindWithTag("MainCamera");
			DynamicCamera dynamicCamera = null;
			if (Settings.DynamicCamera)
			{
				GameObject gameObject2 = UnityEngine.Object.Instantiate(DynamicCamera) as GameObject;
				dynamicCamera = gameObject2.GetComponent<DynamicCamera>();
				Color backgroundColor = gameObject.GetComponent<Camera>().backgroundColor;
				backgroundColor.a = 1f;
				dynamicCamera.GetComponent<Camera>().backgroundColor = backgroundColor;
				UnityEngine.Object.Destroy(gameObject);
				gameObject = gameObject2;
			}
			StartGrid component2 = GetComponent<StartGrid>();
			float num4 = m_trackSpline.rumbleAvoidProp * 2f;
			float num5 = 0f - m_trackSpline.rumbleAvoidProp;
			array = new Vehicle[persistentState.totalToSpawn];
			for (int i = 0; i < persistentState.totalToSpawn; i++)
			{
				float num6 = (float)(i + 1) / (float)(persistentState.totalToSpawn + 1);
				num6 = num6 * num4 + num5;
				Vector3 o_launchPosition = o_point + num6 * o_width * o_normal;
				o_launchPosition.y += num2;
				Quaternion o_launchRotation = Quaternion.LookRotation(o_tangent);
				float o_spawnHeightOffset = 1f;
				if (component2 != null)
				{
					component2.GetLaunchTransform(i, persistentState.totalToSpawn, out o_launchPosition, out o_launchRotation, out o_spawnHeightOffset);
				}
				bool flag = i < persistentState.numPlayersToSpawn;
				DynamicCamera dynamicCamera2 = ((!flag && !Settings.DynamicCameraTrackAI) ? null : dynamicCamera);
				Vehicle vehicle = (array[i] = VehicleFactory.Create(Settings.VehicleType, i, i, o_launchPosition, o_launchRotation));
				if (flag)
				{
					Rect region = new Rect(0f, 0f, 1f, 1f);
					if (persistentState.numPlayersToSpawn > 1)
					{
						if (persistentState.numPlayersToSpawn == 2)
						{
							region = new Rect((float)i * 0.5f, 0f, 0.5f, 1f);
						}
						else
						{
							int num7 = ((i >= 2) ? 1 : 0);
							int num8 = i % 2;
							float left = (float)num8 * 0.5f;
							float top = (float)num7 * 0.5f;
							float width = 0.5f;
							float height = 0.5f;
							region = new Rect(left, top, width, height);
						}
					}
					VehicleFactory.AddHumanController(vehicle, new TouchRegion(region), null);
				}
				else
				{
					VehicleFactory.AddAIController(vehicle, persistentState.aiDifficulty, true);
				}
				if (dynamicCamera2 != null)
				{
					dynamicCamera2.TrackObject(vehicle, flag);
				}
			}
			StartFanfare component3 = GetComponent<StartFanfare>();
			if (component3 != null)
			{
				component3.LaunchFanfare();
			}
			SceneSettings.RenderSettings.MeshBatcher.Batch();
		}
		TrackSpline[] all = IndexedMonoBehaviour<TrackSpline>.All;
		m_numSplines = all.Length;
		SplineOrdering ordering = new SplineOrdering(m_numSplines);
		GenerateSplineOrdering(ordering, new SetList<int>(), m_trackSpline);
		AssignSplineWaypoints(ordering, all);
		if (array != null)
		{
			StartCoroutine(Launch(array));
		}
	}

	private IEnumerator Launch(Vehicle[] _vehicles)
	{
		yield return null;
		foreach (Vehicle vehicle in _vehicles)
		{
			vehicle.gameObject.BroadcastMessage("RaceEvent_Launch", SendMessageOptions.DontRequireReceiver);
			if (vehicle.Visuals != null)
			{
				vehicle.Visuals.EmitLaunchParticles(0.4f);
			}
		}
	}

	private void GenerateSplineOrdering(SplineOrdering ordering, SetList<int> prevSplines, TrackSpline spline)
	{
		foreach (int prevSpline in prevSplines)
		{
			ordering.SetLess(prevSpline, spline.Index);
		}
		prevSplines.Add(spline.Index);
		if (spline.HubAtTail != null)
		{
			GenerateSplineOrdering(ordering, prevSplines, spline.HubAtTail);
		}
	}

	private void GenerateSplineOrdering(SplineOrdering ordering, SetList<int> prevSplines, TrackHub hub)
	{
		foreach (TrackSpline outgoingSpline in hub.outgoingSplines)
		{
			if (!prevSplines.Contains(outgoingSpline.Index))
			{
				SetList<int> prevSplines2 = new SetList<int>(prevSplines);
				GenerateSplineOrdering(ordering, prevSplines2, outgoingSpline);
			}
		}
	}

	private void AssignSplineWaypoints(SplineOrdering ordering, TrackSpline[] splines)
	{
		Array.Sort(splines, ordering.Compare);
		m_numWaypoints = 0;
		for (int i = 0; i != m_numSplines; i++)
		{
			TrackSpline trackSpline = splines[i];
			bool flag = true;
			for (int j = 0; j != m_numSplines; j++)
			{
				if (trackSpline.Index != j && !ordering.Less(trackSpline.Index, j) && !ordering.Less(j, trackSpline.Index))
				{
					flag = false;
					break;
				}
			}
			if (flag)
			{
				trackSpline.Waypoint = m_numWaypoints;
				m_numWaypoints += 2;
			}
			else
			{
				trackSpline.Waypoint = -1;
			}
		}
	}

	public GameObject SpawnAIVehicle(int _playerIndex, float _widthProp, AIDifficulty _difficulty, bool _withVisuals)
	{
		Vector3 o_point;
		Vector3 o_tangent;
		Vector3 o_normal;
		float o_width;
		m_trackSpline.InfoAtT(m_splineT, out o_point, out o_tangent, out o_normal, out o_width);
		float num = m_trackSpline.HeightAt(m_splineT, _widthProp) + 1f;
		Vector3 vector = m_trackSpline.UpAtT(m_splineT);
		Vector3 position = o_point + _widthProp * o_width * o_normal + num * vector;
		Quaternion rotation = Quaternion.LookRotation(o_tangent);
		GameObject gameObject = (GameObject)Resources.Load("Prefabs/Vehicles/" + Settings.VehicleType);
		if (gameObject == null)
		{
			gameObject = Resources.Load("Prefabs/Vehicles/Default") as GameObject;
		}
		GameObject gameObject2 = (GameObject)UnityEngine.Object.Instantiate(gameObject, position, rotation);
		VehicleAIController vehicleAIController = gameObject2.AddComponent<VehicleAIController>();
		vehicleAIController.m_difficulty = _difficulty;
		vehicleAIController.DynamicDifficulty = false;
		PhysicsVehicle component = gameObject2.GetComponent<PhysicsVehicle>();
		component.PlayerIndex = _playerIndex;
		if (_withVisuals)
		{
			GameObject original = (GameObject)Resources.Load(VehicleFactory.GetVisualsPrefabPath(component.PlayerIndex));
			GameObject gameObject3 = (GameObject)UnityEngine.Object.Instantiate(original);
			gameObject3.transform.parent = component.transform;
			gameObject3.transform.localPosition = Vector3.zero;
			gameObject3.transform.localRotation = Quaternion.identity;
			component.SetVisualsGO(gameObject3);
		}
		return gameObject2;
	}

	public static FinishLine Get()
	{
		GameObject[] array = GameObject.FindGameObjectsWithTag("Finish");
		GameObject[] array2 = array;
		foreach (GameObject gameObject in array2)
		{
			if (gameObject.active)
			{
				return gameObject.GetComponent<FinishLine>();
			}
		}
		return null;
	}
}
