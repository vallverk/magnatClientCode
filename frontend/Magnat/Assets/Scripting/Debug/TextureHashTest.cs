using UnityEngine;
using System.Collections;

public class TextureHashTest : MonoBehaviour 
{
	public UITexture Target;

	public Texture2D Tex;

	void Start()
	{
		Target.mainTexture = null;
		//ServerInfo.Instance.SetClubImage("54468caddb4502c61df6f969",Tex,(a)=>{});
		ServerInfo.Instance.GetClub("5448b50dec2ac3ac69d27b80",(res)=>{
			NGUIDebugConsole.Log(JSONSerializer.Serialize(res));
		});

		//byte[] hash = TextureTools.GetTexturePNGData(Tex);
		//Target.mainTexture = TextureTools.GetTextureFromPNG(hash);
	}

	[ContextMenu("Color transfer test")]
	void test()
	{
		string hex = "fafaFA"; 
		print(string.Format("Before {0} after {1}",hex,ColorTools.ColorToHex(ColorTools.HexToColor(hex))));
	}

	[ContextMenu("Color transfer test2")]
	void test2()
	{
		Color col = Color.magenta;
		print(string.Format("Before {0} hex {2} after {1}",col,ColorTools.HexToColor(ColorTools.ColorToHex(col)),ColorTools.ColorToHex(col)));
	}

	[ContextMenu("Get tex hash")]
	void gethash()
	{
		//print (TextureTools.GetTextureHash(Tex));
	}
}
