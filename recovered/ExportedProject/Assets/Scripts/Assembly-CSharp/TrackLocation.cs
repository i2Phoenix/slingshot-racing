public struct TrackLocation
{
	public TrackSpline m_spline;

	public TrackHub m_hub;

	public float m_t;

	public int m_sector;

	public TrackLocation(TrackSpline _spline, float _t)
	{
		m_spline = _spline;
		m_hub = null;
		m_t = _t;
		m_sector = 0;
	}

	public TrackLocation(TrackHub _hub, float _t, int _sector)
	{
		m_spline = null;
		m_hub = _hub;
		m_t = _t;
		m_sector = _sector;
	}

	public override string ToString()
	{
		if (m_spline != null)
		{
			return string.Format("{0} t={1}", m_spline.name, m_t);
		}
		if (m_hub != null)
		{
			return string.Format("{0} t={1} sector={2}", m_hub.name, m_t, m_sector);
		}
		return "Nowhere";
	}
}
