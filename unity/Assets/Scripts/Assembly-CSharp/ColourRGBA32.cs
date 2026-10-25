using UnityEngine;

public struct ColourRGBA32
{
	public byte r;

	public byte g;

	public byte b;

	public byte a;

	public ColourRGBA32(byte r, byte g, byte b, byte a)
	{
		this.r = r;
		this.g = g;
		this.b = b;
		this.a = a;
	}

	public static implicit operator ColourRGBA32(Color c)
	{
		ColourRGBA32 result = default(ColourRGBA32);
		result.r = (byte)(Mathf.Clamp01(c.r) * 255f + 0.5f);
		result.g = (byte)(Mathf.Clamp01(c.g) * 255f + 0.5f);
		result.b = (byte)(Mathf.Clamp01(c.b) * 255f + 0.5f);
		result.a = (byte)(Mathf.Clamp01(c.a) * 255f + 0.5f);
		return result;
	}

	public static implicit operator ColourRGBA32(Color32 c)
	{
		ColourRGBA32 result = default(ColourRGBA32);
		result.r = c.r;
		result.g = c.g;
		result.b = c.b;
		result.a = c.a;
		return result;
	}

	public static implicit operator Color(ColourRGBA32 c)
	{
		Color result = default(Color);
		result.r = (float)(int)c.r * 1f / 255f;
		result.g = (float)(int)c.g * 1f / 255f;
		result.b = (float)(int)c.b * 1f / 255f;
		result.a = (float)(int)c.a * 1f / 255f;
		return result;
	}

	public static implicit operator Color32(ColourRGBA32 c)
	{
		Color32 result = default(Color32);
		result.r = c.r;
		result.g = c.g;
		result.b = c.b;
		result.a = c.a;
		return result;
	}
}
