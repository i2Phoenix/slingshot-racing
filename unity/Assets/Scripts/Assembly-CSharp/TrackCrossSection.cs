using UnityEngine;

public class TrackCrossSection : ScriptableObject
{
	private class Sample
	{
		public Sample prev;

		public Sample next;

		public float x;

		public float y;

		public float error;

		public void UpdateError()
		{
			error = Mathf.Abs((prev.x - x) * (next.y - y) - (prev.y - y) * (next.x - x));
		}
	}

	private float[] m_distribution;

	public AnimationCurve curve;

	public bool symmetric;

	public bool mirror;

	private void OnEnable()
	{
		m_distribution = null;
	}

	public float Evaluate(float widthProp)
	{
		if (mirror)
		{
			widthProp = 0f - widthProp;
		}
		if (symmetric)
		{
			widthProp = Mathf.Abs(widthProp);
		}
		return curve.Evaluate(widthProp);
	}

	public float[] GetDistribution(int _numPoints)
	{
		if (m_distribution == null || m_distribution.Length != _numPoints)
		{
			m_distribution = ComputeApproximation(_numPoints);
		}
		return m_distribution;
	}

	private float[] ComputeApproximation(int _numPoints)
	{
		int numSamples = 512;
		int num = ((!symmetric) ? _numPoints : ((_numPoints + 1) / 2));
		float num2 = ((!symmetric) ? curve.keys[0].time : Mathf.Max(curve.keys[0].time, 0f));
		float time = curve.keys[curve.length - 1].time;
		Sample samples = MakeSamples(numSamples, num2, time);
		if (num2 > ((!symmetric) ? (-1f) : 0f))
		{
			num--;
		}
		if (time < 1f)
		{
			num--;
		}
		samples = OptimiseSamples(samples, numSamples, num);
		int num3 = 0;
		float[] array = new float[_numPoints];
		if (symmetric)
		{
			if (num2 > 0f)
			{
				array[_numPoints / 2 + num3] = 0f;
				array[(_numPoints + 1) / 2 - num3 - 1] = 0f;
				num++;
				num3++;
			}
			Sample sample = samples;
			while (sample != null)
			{
				array[_numPoints / 2 + num3] = sample.x;
				array[(_numPoints + 1) / 2 - num3 - 1] = 0f - sample.x;
				sample = sample.next;
				num3++;
			}
			if (time < 1f)
			{
				array[_numPoints / 2 + num3] = 1f;
				array[(_numPoints + 1) / 2 - num3 - 1] = -1f;
				num++;
				num3++;
			}
		}
		else
		{
			if (num2 > -1f)
			{
				array[num3] = -1f;
				num++;
				num3++;
			}
			Sample sample2 = samples;
			while (sample2 != null)
			{
				array[num3] = sample2.x;
				sample2 = sample2.next;
				num3++;
			}
			if (time < 1f)
			{
				array[num3] = 1f;
				num++;
				num3++;
			}
		}
		return array;
	}

	private Sample MakeSamples(int numSamples, float startx, float endx)
	{
		Sample sample = new Sample();
		Sample sample2 = sample;
		float num = (endx - startx) / (float)(numSamples - 1);
		float num2 = 0f;
		int num3 = 0;
		while (num3 != numSamples)
		{
			Sample sample3 = new Sample();
			sample3.y = Evaluate(sample3.x = startx + num2 * num);
			sample3.prev = sample2;
			sample2.next = sample3;
			sample2 = sample3;
			num3++;
			num2++;
		}
		sample.next.prev = null;
		return sample.next;
	}

	private Sample OptimiseSamples(Sample samples, int numSamples, int targetSamples)
	{
		Sample next = samples.next;
		while (next.next != null)
		{
			next.UpdateError();
			next = next.next;
		}
		while (numSamples > targetSamples)
		{
			float num = float.MaxValue;
			Sample sample = null;
			Sample next2 = samples.next;
			while (next2.next != null)
			{
				if (next2.error < num)
				{
					sample = next2;
					num = next2.error;
				}
				next2 = next2.next;
			}
			Sample prev = sample.prev;
			Sample sample2 = (prev.next = sample.next);
			sample2.prev = prev;
			if (prev.prev != null)
			{
				prev.UpdateError();
			}
			if (sample2.next != null)
			{
				sample2.UpdateError();
			}
			numSamples--;
		}
		return samples;
	}
}
