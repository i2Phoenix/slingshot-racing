using System.Collections;
using UnityEngine;

public class SlowMotion : MonoBehaviour
{
	private static SlowMotion s_slowMotion;

	private float m_oldTimeScale;

	private bool m_active;

	public static bool DoSlowMotion(GameObject _gameObject, float _scale, float _fadeIn, float _fadeOut)
	{
		if (s_slowMotion != null)
		{
			return false;
		}
		s_slowMotion = _gameObject.AddComponent<SlowMotion>();
		s_slowMotion.StartCoroutine(s_slowMotion.SlowMotionCoroutine(_scale, _fadeIn, _fadeOut));
		return true;
	}

	private void OnDestroy()
	{
		s_slowMotion = null;
		if (m_active)
		{
			TimeEx.TimeScale = m_oldTimeScale;
		}
	}

	private IEnumerator SlowMotionCoroutine(float _scale, float _fadeIn, float _fadeOut)
	{
		yield return YieldCache.WaitForFixedUpdate;
		m_oldTimeScale = TimeEx.TimeScale;
		m_active = true;
		float time = 0f;
		float step = (_scale - m_oldTimeScale) / _fadeIn;
		while (time < _fadeIn)
		{
			yield return YieldCache.WaitForFixedUpdate;
			float deltaTime = Time.deltaTime;
			time += deltaTime;
			TimeEx.TimeScale += step * deltaTime;
		}
		float time2 = 0f;
		float step2 = (m_oldTimeScale - Time.timeScale) / _fadeOut;
		while (time2 < _fadeOut)
		{
			yield return YieldCache.WaitForFixedUpdate;
			float deltaTime2 = Time.deltaTime;
			time2 += deltaTime2;
			TimeEx.TimeScale += step2 * deltaTime2;
		}
		Object.Destroy(this);
	}
}
