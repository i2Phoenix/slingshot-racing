using UnityEngine;

public class StartFanfare : MonoBehaviour
{
	private struct ParticleSegment
	{
		public Vector3 m_posL;

		public Vector3 m_posR;

		public Vector3 m_dirL;

		public Vector3 m_dirR;
	}

	private struct ParticleType
	{
		public GameObject m_startParticlesGO;

		public ParticleSystem m_startParticleSystem;

		public ParticleEmitter m_startParticleEmitter;
	}

	private enum LaunchState
	{
		PositionLeft = 0,
		EmitLeft = 1,
		PositionRight = 2,
		EmitRight = 3,
		Wait = 4
	}

	private ParticleSegment[] m_particleSegments;

	private ParticleType[] m_particleTypes;

	private bool m_launched;

	private float m_launchParticleTime;

	private LaunchState m_launchState;

	private int m_launchParticleSegment;

	private AudioPlayer m_audioPlayer;

	public GameObject m_startParticles;

	public AudioBank m_startAudioBank;

	private void Awake()
	{
		m_audioPlayer = base.gameObject.AddComponent<AudioPlayer>();
	}

	private void Start()
	{
		Init();
		base.enabled = m_launched;
	}

	private void Init()
	{
		if (m_particleSegments != null)
		{
			return;
		}
		m_particleSegments = new ParticleSegment[16];
		Vector3 position = base.transform.position;
		TrackSpline trackSpline = TrackSpline.FindClosestToPoint(position);
		if (trackSpline != null)
		{
			TrackHub trackHub = null;
			int sectorIndex = 0;
			float num = trackSpline.ClosestTTo(position);
			float num2 = 0f;
			for (int i = 0; i < m_particleSegments.Length; i++)
			{
				Vector3 o_point = Vector3.zero;
				Vector3 o_tangent = Vector3.forward;
				Vector3 o_normal = Vector3.right;
				float o_width = 1f;
				float num3 = 0f;
				float num4 = 0f;
				if (trackSpline != null)
				{
					trackSpline.InfoAtT(num, out o_point, out o_tangent, out o_normal, out o_width);
					o_tangent.Normalize();
					num3 = trackSpline.HeightAt(num, -1f);
					num4 = trackSpline.HeightAt(num, 1f);
				}
				else if (trackHub != null)
				{
					trackHub.InfoAt(num, sectorIndex, out o_point, out o_tangent, out o_normal, out o_width);
					num3 = trackHub.HeightAt(-1f);
					num4 = trackHub.HeightAt(1f);
				}
				Vector3 posL = o_point + o_normal * (0f - o_width);
				posL.y += num3;
				m_particleSegments[i].m_posL = posL;
				Vector3 posR = o_point + o_normal * o_width;
				posR.y += num4;
				m_particleSegments[i].m_posR = posR;
				Vector3 normalized = (Vector3.up + o_normal * 2f).normalized;
				m_particleSegments[i].m_dirL = normalized;
				Vector3 normalized2 = (Vector3.up + o_normal * -2f).normalized;
				m_particleSegments[i].m_dirR = normalized2;
				position = o_point + o_tangent * 1f;
				for (int j = 0; j < 4; j++)
				{
					if (trackSpline != null)
					{
						float tHint = num + num2;
						float tHintSpread = Mathf.Clamp(Mathf.Abs(num2), 0.05f, 0.5f);
						float num5 = trackSpline.ClosestTTo(position, tHint, tHintSpread, num, num + 1f);
						num2 = num5 - num;
						bool flag = false;
						if (num5 > trackSpline.MaxT)
						{
							if (trackSpline.HubAtTail != null)
							{
								trackHub = trackSpline.HubAtTail;
								sectorIndex = trackSpline.HubSectorAtTail;
								num = trackHub.MinSectorT(sectorIndex);
								trackSpline = null;
								num2 = 0f;
							}
							else
							{
								num = num5 - trackSpline.MaxT;
							}
						}
						else if (num5 < 0f)
						{
							if (trackSpline.HubAtHead != null)
							{
								trackHub = trackSpline.HubAtHead;
								sectorIndex = trackSpline.HubSectorAtHead;
								num = trackHub.MaxSectorT(sectorIndex);
								trackSpline = null;
								num2 = 0f;
							}
							else
							{
								num = num5 + trackSpline.MaxT;
							}
						}
						else
						{
							num = num5;
							flag = true;
						}
						if (flag)
						{
							break;
						}
					}
					else
					{
						if (!(trackHub != null))
						{
							continue;
						}
						float o_t;
						int o_sectorIndex;
						trackHub.ClosestInfoTo(position, out o_t, out o_sectorIndex);
						bool flag2 = trackHub.IsSectorOutgoing(o_sectorIndex);
						if (flag2 && o_t > trackHub.MaxSectorT(o_sectorIndex))
						{
							trackSpline = trackHub.SplineAtSector(o_sectorIndex);
							num = 0f;
							trackHub = null;
							sectorIndex = int.MaxValue;
							continue;
						}
						if (flag2 || !(o_t < trackHub.MinSectorT(o_sectorIndex)))
						{
							num = o_t;
							sectorIndex = o_sectorIndex;
							break;
						}
						trackSpline = trackHub.SplineAtSector(o_sectorIndex);
						num = trackSpline.MaxT;
						trackHub = null;
						sectorIndex = int.MaxValue;
					}
				}
			}
		}
		Color[] array = new Color[3];
		array[0].r = 1f;
		array[0].g = 0.73f;
		array[0].b = 0.32f;
		array[1].r = 1f;
		array[1].g = 0.4f;
		array[1].b = 0.4f;
		array[2].r = 0.2f;
		array[2].g = 1f;
		array[2].b = 0.7f;
		m_particleTypes = new ParticleType[array.Length];
		for (int k = 0; k < array.Length; k++)
		{
			m_particleTypes[k].m_startParticlesGO = (GameObject)Object.Instantiate(m_startParticles);
			m_particleTypes[k].m_startParticlesGO.transform.parent = base.transform;
			m_particleTypes[k].m_startParticleSystem = m_particleTypes[k].m_startParticlesGO.GetComponent<ParticleSystem>();
			if (!(m_particleTypes[k].m_startParticleSystem == null))
			{
				continue;
			}
			m_particleTypes[k].m_startParticleEmitter = m_particleTypes[k].m_startParticlesGO.GetComponent<ParticleEmitter>();
			ParticleAnimator component = m_particleTypes[k].m_startParticlesGO.GetComponent<ParticleAnimator>();
			if (component != null)
			{
				Color[] colorAnimation = component.colorAnimation;
				for (int l = 0; l < colorAnimation.Length; l++)
				{
					colorAnimation[l].r = array[k].r;
					colorAnimation[l].g = array[k].g;
					colorAnimation[l].b = array[k].b;
				}
				component.colorAnimation = colorAnimation;
			}
			for (int m = 0; m != 256; m++)
			{
				m_particleTypes[k].m_startParticleEmitter.Emit(Vector3.zero, Vector3.zero, 0f, 0.001f, Color.white, 0f, 0f);
			}
		}
	}

	public void LaunchFanfare()
	{
		Init();
		if (m_startAudioBank != null)
		{
			m_audioPlayer.PlayRandomClip(m_startAudioBank);
		}
		base.enabled = true;
		m_launched = true;
		m_launchParticleTime = 0f;
		m_launchState = LaunchState.PositionLeft;
		m_launchParticleSegment = 0;
	}

	private void Update()
	{
		m_launchParticleTime += Time.deltaTime;
		if (m_particleTypes[0].m_startParticleSystem != null)
		{
			switch (m_launchState)
			{
			case LaunchState.PositionLeft:
				m_particleTypes[0].m_startParticlesGO.transform.position = m_particleSegments[m_launchParticleSegment].m_posL;
				m_launchState = LaunchState.EmitLeft;
				break;
			case LaunchState.EmitLeft:
				m_particleTypes[0].m_startParticleSystem.Emit(20);
				m_launchState = LaunchState.PositionRight;
				break;
			case LaunchState.PositionRight:
				m_particleTypes[0].m_startParticlesGO.transform.position = m_particleSegments[m_launchParticleSegment].m_posR;
				m_launchState = LaunchState.EmitRight;
				break;
			case LaunchState.EmitRight:
				m_particleTypes[0].m_startParticleSystem.Emit(20);
				m_launchState = LaunchState.Wait;
				break;
			case LaunchState.Wait:
				if (m_launchParticleTime >= 0f)
				{
					m_launchParticleTime = m_launchParticleTime;
					m_launchState = LaunchState.PositionLeft;
					m_launchParticleSegment++;
					if (m_launchParticleSegment >= m_particleSegments.Length)
					{
						m_launched = false;
						base.enabled = false;
					}
				}
				break;
			}
		}
		else
		{
			if (!m_particleTypes[0].m_startParticleEmitter)
			{
				return;
			}
			while (m_launchParticleTime >= 0.04f)
			{
				m_launchParticleTime -= 0.04f;
				EmitLegacyParticles(m_particleSegments[m_launchParticleSegment].m_posL, m_particleSegments[m_launchParticleSegment].m_dirL, 20);
				EmitLegacyParticles(m_particleSegments[m_launchParticleSegment].m_posR, m_particleSegments[m_launchParticleSegment].m_dirR, 20);
				m_launchParticleSegment++;
				if (m_launchParticleSegment >= m_particleSegments.Length)
				{
					m_launched = false;
					base.enabled = false;
					break;
				}
			}
		}
	}

	private void EmitLegacyParticles(Vector3 _pos, Vector3 _dir, int _numParticles)
	{
		for (int i = 0; i < _numParticles; i++)
		{
			Vector3 velocity = _dir;
			velocity += RandomTables.InsideUnitSphere * 0.3f;
			velocity *= RandomTables.Value * 15f;
			float size = RandomTables.Value * 0.6f + 0.6f;
			float energy = 0.7f;
			int num = Random.Range(0, m_particleTypes.Length);
			m_particleTypes[num].m_startParticleEmitter.Emit(_pos, velocity, size, energy, Color.white, RandomTables.Value * 360f, RandomTables.Value * 540f - 270f);
		}
	}
}
