using UnityEngine;

namespace StreamerStructs
{
	public struct UpdateParticlesParams
	{
		public uint m_numStreamers;

		public uint m_numParticlesPerStreamer;

		public Vector3 m_accel;

		public Vector3 m_tailAccel;

		public Vector3 m_wind;

		public float m_timeStep;

		public float m_normalLerp;

		public float m_segmentDistance;

		public float m_segmentDistanceTest;

		public float m_constraintScale;
	}
}
