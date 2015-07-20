using UnityEngine;
using System.Collections;

public class UserGiftField : MonoBehaviour 
{
	public UITexture IconTexture;
	public UILabel NameLabel;
	public UILabel DescriptionLabel;
	public UILabel TimeLabel;
	
	private Gift gift;
	
	public void Init(Gift data)
	{
		gift = data;
		if (!string.IsNullOrEmpty(data.image))
			data.image = "https://moderator.magnatgame.com"+data.image;
			ImageLoader.Instance.LoadAvatar(data.image,(tex)=>{
				Vector2 size = UITools.ResizeTo(tex,120);
				IconTexture.width = (int)size.x;
				IconTexture.height = (int)size.y;
				IconTexture.mainTexture = tex;
			});
		
		NameLabel.text = data.title;
		DescriptionLabel.text = gift.description;
		TimeLabel.text = TimeTools.FormatUTSTime(long.Parse(data.term));
	}
}
