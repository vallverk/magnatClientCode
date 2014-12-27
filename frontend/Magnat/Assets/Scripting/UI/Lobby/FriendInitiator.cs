using UnityEngine;
using System.Collections;

public class FriendInitiator : ProfileShower 
{
	public UITexture AvatarUI;
	public UILabel NameLabel;
	public UILabel StatusLabel;
	public UILabel CapitalLabel;

	private string Avatar;

	public void Init(string FirstName,string LastName,string AvatarURL,string Status, long Cash)
	{
		NameLabel.text = FirstName + "\r\n" + LastName;
		StatusLabel.text = Status;
		CapitalLabel.text = Cash.ToString("$###,###,###,###k");
		Avatar = AvatarURL;
		this.AvatarUI.mainTexture = null;

		if (AvatarUI.isVisible)
		{
			ImageLoader.Instance.LoadAvatar(Avatar,(tex) => {this.AvatarUI.mainTexture = tex;});
		}
	}

	private bool loaded = false;
	void Update()
	{
		if (AvatarUI.isVisible && AvatarUI.mainTexture==null && !loaded)
		{
			ImageLoader.Instance.LoadAvatar(Avatar,(tex) => {this.AvatarUI.mainTexture = tex;});
			loaded = true;
		}
	}
}
