using UnityEngine;

public class TrackTraverser
{
	private TrackLocation m_location;

	private float m_prevDeltaT;

	public TrackLocation Location
	{
		get
		{
			return m_location;
		}
	}

	public TrackSpline CurrentTrackSpline
	{
		get
		{
			return m_location.m_spline;
		}
	}

	public TrackHub CurrentTrackHub
	{
		get
		{
			return m_location.m_hub;
		}
	}

	public float CurrentT
	{
		get
		{
			return m_location.m_t;
		}
	}

	public int CurrentHubSector
	{
		get
		{
			return m_location.m_sector;
		}
	}

	public TrackTraverser(Vector3 _point)
	{
		Teleport(_point);
	}

	public void Teleport(Vector3 _point)
	{
		TrackSpline trackSpline = TrackSpline.FindClosestToPoint(_point);
		if (trackSpline == null)
		{
			m_location = default(TrackLocation);
			m_prevDeltaT = 0f;
			return;
		}
		float t = trackSpline.ClosestTTo(_point);
		m_location = new TrackLocation(trackSpline, t);
		m_prevDeltaT = 0f;
		Move(_point);
	}

	public void Move(Vector3 _point)
	{
		for (int i = 0; i < 2; i++)
		{
			if (m_location.m_spline != null)
			{
				TrackSpline spline = m_location.m_spline;
				float t = m_location.m_t;
				float tHint = t + m_prevDeltaT;
				float tHintSpread = Mathf.Clamp(Mathf.Abs(m_prevDeltaT), 0.05f, 0.5f);
				float num = spline.ClosestTTo(_point, tHint, tHintSpread, t - 0.5f, t + 0.5f);
				m_prevDeltaT = num - t;
				if (num > spline.MaxT)
				{
					if (spline.HubAtTail != null)
					{
						TrackHub hubAtTail = spline.HubAtTail;
						int hubSectorAtTail = spline.HubSectorAtTail;
						m_location = new TrackLocation(hubAtTail, hubAtTail.MinSectorT(hubSectorAtTail), hubSectorAtTail);
						m_prevDeltaT = 0f;
					}
					else
					{
						m_location.m_t = num - spline.MaxT;
					}
					continue;
				}
				if (!(num < 0f))
				{
					m_location.m_t = num;
					break;
				}
				if (spline.HubAtHead != null)
				{
					TrackHub hubAtHead = spline.HubAtHead;
					int hubSectorAtHead = spline.HubSectorAtHead;
					m_location = new TrackLocation(hubAtHead, hubAtHead.MaxSectorT(hubSectorAtHead), hubSectorAtHead);
					m_prevDeltaT = 0f;
				}
				else
				{
					m_location.m_t = num + spline.MaxT;
				}
			}
			else
			{
				if (!(m_location.m_hub != null))
				{
					continue;
				}
				TrackHub hub = m_location.m_hub;
				float o_t;
				int o_sectorIndex;
				hub.ClosestInfoTo(_point, out o_t, out o_sectorIndex);
				bool flag = hub.IsSectorOutgoing(o_sectorIndex);
				if (flag && o_t > hub.MaxSectorT(o_sectorIndex))
				{
					TrackSpline spline2 = hub.SplineAtSector(o_sectorIndex);
					m_location = new TrackLocation(spline2, 0f);
					continue;
				}
				if (flag || !(o_t < hub.MinSectorT(o_sectorIndex)))
				{
					m_location.m_t = o_t;
					m_location.m_sector = o_sectorIndex;
					break;
				}
				TrackSpline trackSpline = hub.SplineAtSector(o_sectorIndex);
				m_location = new TrackLocation(trackSpline, trackSpline.MaxT);
			}
		}
	}

	public void GetInfo(out Vector3 o_point, out Vector3 o_tangent, out Vector3 o_normal, out float o_width)
	{
		if (m_location.m_spline != null)
		{
			m_location.m_spline.InfoAtT(m_location.m_t, out o_point, out o_tangent, out o_normal, out o_width);
			Vector3Utils.Normalise(ref o_tangent);
			return;
		}
		if (m_location.m_hub != null)
		{
			m_location.m_hub.InfoAt(m_location.m_t, m_location.m_sector, out o_point, out o_tangent, out o_normal, out o_width);
			return;
		}
		o_point = Vector3.zero;
		o_tangent = Vector3.forward;
		o_normal = Vector3.right;
		o_width = 1000f;
	}

	public Vector3 GetUp()
	{
		if (m_location.m_spline != null)
		{
			return m_location.m_spline.UpAtT(m_location.m_t);
		}
		if (m_location.m_hub != null)
		{
			return m_location.m_hub.UpAt();
		}
		return Vector3.up;
	}

	public float GetHeight(float _widthProp)
	{
		if (m_location.m_spline != null)
		{
			return m_location.m_spline.HeightAt(m_location.m_t, _widthProp);
		}
		if (m_location.m_hub != null)
		{
			return m_location.m_hub.HeightAt(_widthProp);
		}
		return 0f;
	}
}
