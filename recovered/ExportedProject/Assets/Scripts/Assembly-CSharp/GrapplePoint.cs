using UnityEngine;

public class GrapplePoint : IndexedMonoBehaviour<GrapplePoint>
{
	public static bool c_drawGizmosReversed;

	private Transform m_parent;

	private Transform m_transform;

	private TrackSpline m_cachedTrackSpline;

	private AIGrapplePoint m_aiGrapplePoint;

	private int m_numGrapplingObjects;

	public float radius = 0.9f;

	public float minGrappleLength;

	public float grappleForceRadius;

	public float grappleForceRamp = 1f;

	public float grappleThrustScale = 1f;

	public float windRate;

	public float startProp;

	public float endProp;

	public float startPropReversed;

	public float endPropReversed;

	public bool m_noEnd;

	public bool m_noEndReversed;

	public TrackSpline TargetTrackSpline
	{
		get
		{
			if (m_transform == null)
			{
				m_transform = base.transform;
			}
			Transform parent = m_transform.parent;
			if (parent != m_parent)
			{
				m_parent = parent;
				m_cachedTrackSpline = ((!parent) ? null : parent.GetComponent<TrackSpline>());
			}
			return m_cachedTrackSpline;
		}
	}

	public AIGrapplePoint AIGrapplePoint
	{
		get
		{
			return m_aiGrapplePoint;
		}
	}

	public int NumGrapplingObjects
	{
		get
		{
			return m_numGrapplingObjects;
		}
		set
		{
			m_numGrapplingObjects = value;
		}
	}

	public Transform Transform
	{
		get
		{
			return m_transform;
		}
	}

	public Vector3 Position
	{
		get
		{
			return m_transform.position;
		}
	}

	public int GlobalIndex
	{
		get
		{
			return base.Index;
		}
	}

	private new void Awake()
	{
		base.Awake();
		m_transform = base.transform;
		m_aiGrapplePoint = GetComponent<AIGrapplePoint>();
	}

	public void TrackNodeAdded(int nodeIndex)
	{
		float num = (float)nodeIndex - 0.5f;
		if (startProp > num)
		{
			startProp++;
		}
		if (endProp > num)
		{
			endProp++;
		}
	}

	public void TrackNodeDeleted(int nodeIndex)
	{
		float num = (float)nodeIndex - 0.5f;
		if (startProp > num)
		{
			startProp--;
		}
		if (endProp > num)
		{
			endProp--;
		}
	}

	public void OnReverseTrack()
	{
		float num = startProp;
		startProp = startPropReversed;
		startPropReversed = num;
		num = endProp;
		endProp = endPropReversed;
		endPropReversed = num;
		bool noEnd = m_noEnd;
		m_noEnd = m_noEndReversed;
		m_noEndReversed = noEnd;
	}
}
