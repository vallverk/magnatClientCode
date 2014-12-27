using UnityEngine;
using System.Collections;

public class ColorTools 
{
	public static string GetHex (int num) 
	{
		string res = num.ToString("X");
		return res.Length == 1 ? "0"+res : res;
	}

	public static int GetDec (string hexValue) 
	{
		return int.Parse(hexValue, System.Globalization.NumberStyles.HexNumber);
	}

	public static string ColorToHex(Color color)
	{
		return GetHex((int)(color.r * 255))+GetHex((int)(color.g * 255))+GetHex((int)(color.b * 255));
	}

	public static string ColorsToHex(Color[] colors)
	{
		string res = "";
		foreach (var col in colors)
			res += (char)(col.r*255)+(char)(col.g*255)+(char)(col.b*255);
		return res;
	}

	public static Color[] HexToColors(string hex)
	{
		Color[] res = new Color[hex.Length/3];
		for (int i=0;i<res.Length;i++)
			res[0] = new Color((int)(hex[i*3])/255.0f,
			                   (int)(hex[i*3+1])/255.0f,
			                   (int)(hex[i*3+2])/255.0f);
		return res;
	}

	public static Color HexToColor(string Hex)
	{
		return new Color(GetDec(Hex.Substring(0,2))/255.0f,
		                 GetDec(Hex.Substring(2,2))/255.0f,
		                 GetDec(Hex.Substring(4,2))/255.0f);
	}
}
