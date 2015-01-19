using UnityEngine;
using System.Collections;

public class EveryDayBonusWindow : WindowBehavoiur 
{
	public UISprite[] Chests;
	public UILabel InvitedCashLabel;

	private void InitChests(long count)
	{
		for (int i=0;i<Chests.Length;i++)
		{
			Chests[i].spriteName = (i<count ? "CHEST_TODAY" : "CHEST_TOMOROW");
		}
	}

	protected override void Start()
	{
		base.Start();
		#if !UNITY_EDITOR
        if (SocialManager.Instance.IsFriendsLoaded)
            Init();
        else
		    SocialManager.Instance.OnFriendsDataLoaded+=Init;
		#else
		Init();
		#endif
	}

	private void Init()
	{
		SocialManager.Instance.OnFriendsDataLoaded -= Init;
		ServerInfo.Instance.GetUserInfo(SocialManager.User,(u)=>{
			if (u.Active == 1)
			{
				u.Nunber++;
				InitChests(u.Nunber);

				long cash = u.Nunber * 1000000;
				long frCash = 0;
#if !UNITY_EDITOR
				frCash = SocialManager.Instance.Friends.Count*50000;
#endif
				frCash += cash;
				if (u.Nunber == 6)
					ServerInfo.Instance.UpdateUserGold(1,()=>{});
				if (u.Nunber == 7)
					ServerInfo.Instance.UpdateUserGold(2,()=>{});

				InvitedCashLabel.text = "[b]У вас "+frCash.ToString("### ### ##0")+" $ за сегодня![/b]";
				Show();
				
				ServerInfo.Instance.UpdateUserCapital(cash + frCash,()=>{});

				ServerInfo.Instance.UpdateBonusActive();
			}
		},true);
	}
}
