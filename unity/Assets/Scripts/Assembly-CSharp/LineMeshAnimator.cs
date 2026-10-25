using UnityEngine;

[AddComponentMenu("Graphics/Line Mesh Animator")]
[RequireComponent(typeof(LineMeshBase))]
public class LineMeshAnimator : MonoBehaviour
{
	private const string c_nextFrame = "NextFrame";

	private LineMeshBase m_lineMeshBase;

	private SimpleAtlas m_atlas;

	private int m_animationIndex;

	private int m_frameIndex;

	private bool m_isPlaying;

	private void Awake()
	{
		m_lineMeshBase = GetComponent<LineMeshBase>();
	}

	private void NextFrame()
	{
		SimpleAtlas.Animation animation = m_atlas.m_animations[m_animationIndex];
		int num = animation.m_endFrame - animation.m_beginFrame;
		m_frameIndex = (m_frameIndex + 1) % num;
		m_lineMeshBase.SetUVRectHorizontal(m_atlas.GetFrameUVRect(m_frameIndex + animation.m_beginFrame));
	}

	public void SetAtlas(SimpleAtlas _atlas)
	{
		StopAnimation();
		m_atlas = _atlas;
	}

	public void PlayAnimation(int _animationIndex)
	{
		m_animationIndex = _animationIndex;
		m_frameIndex = 0;
		m_isPlaying = true;
		SimpleAtlas.Animation animation = m_atlas.m_animations[m_animationIndex];
		m_lineMeshBase.SetUVRectHorizontal(m_atlas.GetFrameUVRect(animation.m_beginFrame));
		if (base.GetComponent<Renderer>().isVisible)
		{
			float num = 1f / (float)m_atlas.m_animations[m_animationIndex].m_frameRate;
			InvokeRepeating("NextFrame", num, num);
		}
	}

	public void StopAnimation()
	{
		CancelInvoke();
		m_animationIndex = -1;
		m_frameIndex = -1;
		m_isPlaying = false;
	}

	private void OnBecameVisible()
	{
		if (m_isPlaying)
		{
			float num = 1f / (float)m_atlas.m_animations[m_animationIndex].m_frameRate;
			InvokeRepeating("NextFrame", num, num);
		}
	}

	private void OnBecameInvisible()
	{
		CancelInvoke();
	}
}
