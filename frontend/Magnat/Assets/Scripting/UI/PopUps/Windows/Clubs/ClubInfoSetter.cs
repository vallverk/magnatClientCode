using UnityEngine;
using System.Collections;

public class ClubInfoSetter : MonoBehaviour 
{
	public UILabel NameLabel;
	public UILabel LevelLabel;
	public UILabel CapitalLabel;
	public UITexture IconTexture;
	public int IconRectSize = 150;
	public UILabel DescriptionLabel;
	public UILabel GoldLabel;
	public UILabel DateOfDeathLabel;

	public void SetInfo(ClubInfo club)
	{
		if (NameLabel) NameLabel.text = club.ClubName;
		if (LevelLabel) LevelLabel.text = club.LevelName;
		if (CapitalLabel) CapitalLabel.text = club.Capital.ToString("$ ### ### ### ##0k");
		if (IconTexture && !string.IsNullOrEmpty(club.Icon)) 
		{
			IconTexture.mainTexture = null;
			ImageLoader.Instance.LoadAvatar(club.Icon,(tex)=>{
				if (tex!=null)
				{
					IconTexture.mainTexture = tex;
					Vector2 size = UITools.ResizeTo(tex,IconRectSize);
					IconTexture.width = (int)size.x;
					IconTexture.height = (int)size.y;
				}
				else
					AlertWindow.Show("ОШИБКА","Ошибка загрузки аватарки клуба",null,null);
			});
		}
		if (DescriptionLabel) 
		{
			DescriptionLabel.text = club.Description;
		}
		if (GoldLabel) GoldLabel.text = club.Gold.ToString();
		if (DateOfDeathLabel) DateOfDeathLabel.text = "до "+ TimeTools.FormatUTSTime(club.DateOfDeath);
	}

	public void SetEmpty()
	{
		if (NameLabel) NameLabel.text = "";
		if (LevelLabel) LevelLabel.text = "";
		if (CapitalLabel) CapitalLabel.text = "$ 0k";
		if (DescriptionLabel) DescriptionLabel.text = "";
		if (GoldLabel) GoldLabel.text = "-";
		if (DateOfDeathLabel) DateOfDeathLabel.text = "-";
	}
}
