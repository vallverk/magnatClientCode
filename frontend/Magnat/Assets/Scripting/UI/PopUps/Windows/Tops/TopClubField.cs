using UnityEngine;
using System.Collections;

public class TopClubField : MonoBehaviour 
{
	public UITexture AvatarTexture;
	public UILabel NumberLabel;
	public UILabel ClubNameLabel;
	public UILabel ClubLevelLabel;
	public UILabel ClubCapitalLabel;

	private string currentClubID;

	public void Init(string ClubID)
	{
		this.currentClubID = ClubID;

		ServerInfo.Instance.GetClub(ClubID,(club)=>{
			if (!string.IsNullOrEmpty(club.Icon))
				ImageLoader.Instance.LoadAvatar(club.Icon,(tex)=>{
					Vector2 size = UITools.ResizeTo(tex,65);
					AvatarTexture.width = (int)size.x;
					AvatarTexture.height = (int)size.y;
					AvatarTexture.mainTexture = tex;
				});
			ClubNameLabel.text = club.ClubName;
			ClubLevelLabel.text = club.LevelName;
			ClubCapitalLabel.text = club.Capital.ToString("$###,###,##0k");
		});

		UIButton button = GetComponent<UIButton>();
		button.onClick.Clear();
		button.onClick.Add(new EventDelegate(()=>{this.OpenClub();}));
	}

	public void OpenClub()
	{
		ClubWindow.Show(currentClubID);
	}
}
