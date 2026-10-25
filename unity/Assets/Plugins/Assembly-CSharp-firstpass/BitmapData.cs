using UnityEngine;

public class BitmapData
{
	public int height;

	public int width;

	private Color32[] pixels;

	public BitmapData(Texture2D texture)
	{
		height = texture.height;
		width = texture.width;
		pixels = texture.GetPixels32();
	}

	public BitmapData(Color32[] _pixels, int _width, int _height)
	{
		height = _height;
		width = _width;
		pixels = _pixels;
	}

	public Color32 getPixelColor(int x, int y)
	{
		if (x >= width)
		{
			x = width - 1;
		}
		if (y >= height)
		{
			y = height - 1;
		}
		if (x < 0)
		{
			x = 0;
		}
		if (y < 0)
		{
			y = 0;
		}
		return pixels[y * width + x];
	}
}
