using UnityEngine;
using System.Collections;

public class WinnerWindow : WindowBehavoiur 
{
	public TweenAlpha Tween;
	public UILabel NameLabel;
	public UILabel CashLabel;
	public UILabel GoldLabel;
    public UILabel AdditionalCash;

	public UITexture Avatar;
	
	public void Init(string UserID, int Cash, int Gold)
	{
        AdditionalCash.text = "";

        ServerInfo.Instance.GetUserInfo(new string[1] { UserID }, (u) => {
            if (u[0].VIP!=0)
            {
                AdditionalCash.text = "+ $ " + ((int)(Cash * 1.2f)).ToString("###,###,###,###") + " (20% VIP)";
            }
        });

		var u1 = SocialManager.GetUserData(UserID);
		if (u1==null)
		{
			u1 = new SocialData(){
				FirstName = "temp",
				LastName = "sec"
			};
		}
		NameLabel.text = u1.FirstName+"\r\n"+u1.LastName;
		CashLabel.text = Cash.ToString("$###,###,###,##0");
		GoldLabel.text = Gold.ToString("0 кг Золота");

		if (!string.IsNullOrEmpty(u1.Photo))
		{
			ImageLoader.Instance.LoadAvatar(u1.Photo,(tex)=>{
				Avatar.mainTexture = tex;
			});
		}

		Tween.enabled = true;
        Tween.PlayForward();
		GetComponent<UIPanel>().depth = 3;
	}
}
