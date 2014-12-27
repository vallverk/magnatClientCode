using UnityEngine;
using System.Collections;

public class Winner2x2Window : MonoBehaviour 
{
	public TweenAlpha Tween;
	
	public UILabel NameLabel1;
	public UILabel CashLabel1;
	public UILabel GoldLabel1;
    public UITexture Avatar1;
    public UILabel AdditionalCash1;
	
	public UILabel NameLabel2;
	public UILabel CashLabel2;
	public UILabel GoldLabel2;
	public UITexture Avatar2;
    public UILabel AdditionalCash2;

	
	public void Init(string UserID1, string UserID2, int Cash, int Gold)
	{
		var u1 = SocialManager.GetUserData(UserID1);
		var u2 = SocialManager.GetUserData(UserID2);

        AdditionalCash1.text = "";
        AdditionalCash2.text = "";
        ServerInfo.Instance.GetUserInfo(new string[2] { UserID1, UserID2 }, (uu) =>
        {
            if (uu[0].VIP != 0)
                AdditionalCash1.text = "+ $ " + ((int)(Cash * 1.2f)).ToString("###,###,###,###") + " (20% VIP)";
            if (uu[1].VIP != 0)
                AdditionalCash1.text = "+ $ " + ((int)(Cash * 1.2f)).ToString("###,###,###,###") + " (20% VIP)";
        });

#if UNITY_EDITOR
		u2 = new SocialData(){
			FirstName = "FName",
			LastName = "LName",
			ViewerId = UserID2,
			Photo = ""
		};
#endif
		
		NameLabel1.text = u1.FirstName+"\r\n"+u1.LastName;
		CashLabel1.text = Cash.ToString("$###,###,###,##0");
		GoldLabel1.text = Gold.ToString("0 кг Золота");

		if (!string.IsNullOrEmpty(u1.Photo))
		{
			ImageLoader.Instance.LoadAvatar(u1.Photo,(tex)=>{
				Avatar1.mainTexture = tex;
			});
		}
		
		NameLabel2.text = u2.FirstName+"\r\n"+u2.LastName;
		CashLabel2.text = Cash.ToString("$###,###,###,##0");
		GoldLabel2.text = Gold.ToString("0 кг Золота");

		if (!string.IsNullOrEmpty(u2.Photo))
		{
			ImageLoader.Instance.LoadAvatar(u2.Photo,(tex)=>{
				Avatar2.mainTexture = tex;
			});
		}
		
		Tween.enabled = true;
		Tween.Play();
		GetComponent<UIPanel>().depth = 3;
	}
}
