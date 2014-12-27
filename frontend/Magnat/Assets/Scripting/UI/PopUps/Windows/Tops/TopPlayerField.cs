using UnityEngine;
using System.Collections;

public class TopPlayerField : ProfileShower 
{
	public UITexture AvatarTexture;
	public UILabel NumberLabel;
	public UILabel NameLabel;
	public UILabel TitleLabel;
	public UILabel CapitalLabel;

	public void LoadAvatar(string url)
	{
		ImageLoader.Instance.LoadAvatar(url,(tex)=>{
			AvatarTexture.mainTexture = tex;
		});
	}
}
