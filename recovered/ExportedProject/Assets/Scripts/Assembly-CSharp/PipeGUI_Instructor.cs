using System.Collections;
using UnityEngine;

public class PipeGUI_Instructor : MonoBehaviour
{
	private PackedSprite m_base;

	private GameObject m_activeChild;

	private PackedSprite[] m_sprites;

	public bool m_hidden;

	private void Start()
	{
		if (m_hidden)
		{
			base.gameObject.SetActiveRecursively(false);
		}
		m_base = base.gameObject.GetComponent<PackedSprite>();
	}

	public void Show()
	{
		SetDefaultStance();
		ShowSprites();
	}

	public void ShowTalking()
	{
		SetTalkingStance();
		ShowSprites();
	}

	private void ShowSprites()
	{
		if (m_sprites != null)
		{
			PackedSprite[] sprites = m_sprites;
			foreach (PackedSprite sprt in sprites)
			{
				FadeSpriteAlpha.Do(sprt, EZAnimation.ANIM_MODE.To, Color.white, EZAnimation.cubicInOut, 0.2f, 0f, null, null);
			}
		}
	}

	public void Hide()
	{
		Color dest = new Color(0f, 0f, 0f, 0f);
		EZAnimation.CompletionDelegate del = OnHideComplete;
		if (m_sprites != null)
		{
			PackedSprite[] sprites = m_sprites;
			foreach (PackedSprite sprt in sprites)
			{
				FadeSpriteAlpha.Do(sprt, EZAnimation.ANIM_MODE.To, dest, EZAnimation.cubicInOut, 0.2f, 0f, null, del);
				del = null;
			}
		}
	}

	public void HideImmediate()
	{
		StopAllCoroutines();
		if (m_sprites != null)
		{
			PackedSprite[] sprites = m_sprites;
			foreach (PackedSprite packedSprite in sprites)
			{
				packedSprite.SetColor(new Color(1f, 1f, 1f, 0f));
			}
		}
		OnHideComplete(null);
	}

	public void SetTalkingStance()
	{
		if (m_activeChild != null)
		{
			m_activeChild.SetActiveRecursively(false);
			m_activeChild = null;
		}
		m_activeChild = base.transform.FindChild("Talking").gameObject;
		m_activeChild.SetActiveRecursively(true);
		base.gameObject.active = true;
		m_sprites = base.gameObject.GetComponentsInChildren<PackedSprite>();
		m_base.PlayAnim("Talking");
	}

	public void PlayTalkAnim()
	{
		StartCoroutine(PlayTalkAnimImpl());
	}

	private IEnumerator PlayTalkAnimImpl()
	{
		yield return null;
		GameObject moustache = m_activeChild.transform.FindChild("Moustache").gameObject;
		TransitionPlayer player = moustache.GetComponent<TransitionPlayer>();
		player.Play(player.FindIndex("Talking"), false);
	}

	public void SetDefaultStance()
	{
		if (m_activeChild != null)
		{
			m_activeChild.SetActiveRecursively(false);
			m_activeChild = null;
		}
		base.gameObject.active = true;
		m_base.StopAnim();
	}

	private void OnHideComplete(EZAnimation _anim)
	{
		base.gameObject.SetActiveRecursively(false);
	}
}
