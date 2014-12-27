using UnityEngine;
using System.Collections;

public class TextureTools
{
	public static byte[] GetTexturePNGData(Texture2D Tex)
	{
		return Tex.EncodeToPNG();
	}

	public static Texture2D GetTextureFromPNG(byte[] PNG)
	{
		Texture2D tex = new Texture2D(4,4);
		tex.LoadImage(PNG);
		return tex;
	}
}
