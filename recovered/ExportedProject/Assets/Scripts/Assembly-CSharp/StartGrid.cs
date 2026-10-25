using UnityEngine;

public class StartGrid : MonoBehaviour
{
	private TrackSpline m_trackSpline;

	private float m_splineT;

	private Vector3 m_trackPoint;

	private Vector3 m_trackTangent;

	private Vector3 m_trackNormal;

	private float m_trackWidth;

	private float m_trackHeight;

	private bool m_reverseTrack;

	public bool m_leftAdvantageous = true;

	public bool m_reverseLeftAdvantageous = true;

	public bool m_columnFlip;

	public float m_staggerAmount;

	public float m_bias;

	public float m_offset;

	public float m_rowSeparation = 4f;

	public float m_winchDistance = 2f;

	public float m_widthScale = 1f;

	public float m_randomAngle;

	private void Start()
	{
		m_trackSpline = TrackSpline.FindClosestToPoint(base.transform.position);
		m_splineT = m_trackSpline.ClosestTTo(base.transform.position);
		Vector3 o_point;
		Vector3 o_tangent;
		Vector3 o_normal;
		float o_width;
		m_trackSpline.InfoAtT(m_splineT, out o_point, out o_tangent, out o_normal, out o_width);
		m_trackHeight = m_trackSpline.HeightAt(m_splineT, 0f);
		m_trackPoint = o_point;
		m_trackTangent = o_tangent.normalized;
		m_trackNormal = o_normal;
		m_trackWidth = o_width;
	}

	public void OnReverseTrack()
	{
		m_reverseTrack = !m_reverseTrack;
	}

	public void GetLaunchTransform(int _playerIndex, int _numPlayers, out Vector3 o_launchPosition, out Quaternion o_launchRotation, out float o_spawnHeightOffset)
	{
		Vector3 o_startPosition;
		Quaternion o_startRotation;
		GetLaunchTransform(_playerIndex, _numPlayers, out o_launchPosition, out o_launchRotation, out o_startPosition, out o_startRotation, out o_spawnHeightOffset);
	}

	public void GetLaunchTransform(int _playerIndex, int _numPlayers, out Vector3 o_launchPosition, out Quaternion o_launchRotation, out Vector3 o_startPosition, out Quaternion o_startRotation, out float o_spawnHeightOffset)
	{
		bool flag = ((!m_reverseTrack) ? m_leftAdvantageous : m_reverseLeftAdvantageous);
		int num = 2;
		if (_numPlayers <= 1)
		{
			num = 1;
		}
		else if (_numPlayers > 4)
		{
			num = 4;
		}
		int num2 = (_numPlayers - 1 - (_numPlayers - 1) % num) / num + 1;
		int num3 = _playerIndex % num;
		int num4 = (_playerIndex - num3) / num;
		bool flag2 = !flag;
		if (m_columnFlip)
		{
			flag2 = !flag2;
		}
		if (flag2)
		{
			num3 = num - 1 - num3;
		}
		float num5 = m_trackSpline.rumbleAvoidProp * m_widthScale;
		float num6 = num5 * 2f;
		float num7 = 0f - num5;
		float num8 = 1f / (float)(num + 1);
		float num9 = (float)(num3 + 1) * num8;
		float num10 = num8;
		float num11 = (float)num * num8;
		float t = 0.5f;
		if (num2 > 1)
		{
			bool flag3 = (num4 & 1) == 0;
			if (!flag)
			{
				flag3 = !flag3;
			}
			float num12 = ((!flag3) ? (0f - num8) : num8) * 0.25f;
			num12 *= m_staggerAmount;
			num9 += num12;
			num10 += num12;
			num11 += num12;
		}
		if (num > 1)
		{
			t = (num9 - num10) / (num11 - num10);
		}
		float num13 = m_bias;
		if (num <= 1)
		{
			num13 = 0f;
		}
		if (!flag)
		{
			num13 = 0f - num13;
		}
		num13 = ((!(num13 < 0f)) ? Mathf.Lerp(0f, num13, t) : Mathf.Lerp(0f - num13, 0f, t));
		num13 += m_offset;
		num9 = num9 * num6 + num7;
		Vector3 vector = m_trackPoint + num9 * m_trackWidth * m_trackNormal;
		vector -= m_trackTangent * ((float)num4 * m_rowSeparation + num13);
		Quaternion quaternion = Quaternion.LookRotation(m_trackTangent);
		o_spawnHeightOffset = 1f;
		o_startPosition = vector + m_trackTangent * m_winchDistance;
		o_startPosition.y += o_spawnHeightOffset;
		Quaternion quaternion2 = quaternion;
		Vector3 eulerAngles = quaternion2.eulerAngles;
		eulerAngles.y += Random.Range(-0.5f, 0.5f) * m_randomAngle;
		quaternion2.eulerAngles = eulerAngles;
		o_startRotation = quaternion2;
		o_launchPosition = vector;
		o_launchPosition.y += o_spawnHeightOffset;
		o_launchRotation = quaternion;
		o_startPosition.y += m_trackHeight;
		o_launchPosition.y += m_trackHeight;
	}
}
