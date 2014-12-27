using UnityEngine;
using System.Collections;

public class LobbyHeaderLoader : MonoBehaviour 
{
	// лейба статуса
	public PlayerStatusController StatusController; 

	public UILabel PlayerCapitalLabel;
	public UILabel PlayerWeekCapitalLabel;

	public UILabel PlayerGoldLabel;
	public UILabel PlayerCardsLabel;

	public ClubContainer Club;

	public UISprite UserAvatarCrownSprite;

	private static LobbyHeaderLoader current;

	private void Init(ServerUserInfo UInfo)
	{
		StatusController.Init(UInfo);
		
		if (PlayerCapitalLabel != null)
			PlayerCapitalLabel.text = ((int)(UInfo.Capital/1000)).ToString("###,###,###,##0k");
		
		if (PlayerWeekCapitalLabel!=null)
			PlayerWeekCapitalLabel.text = ((int)(UInfo.WeekCapital/1000)).ToString("###,###,###,##0k");
		
		if (PlayerGoldLabel!=null)
			PlayerGoldLabel.text = UInfo.Gold.ToString();
		
		if (PlayerCardsLabel!=null)
			PlayerCardsLabel.text = UInfo.GameCards.ToString();

		Club.Init(UInfo.ClubId);
	}

	private void Load()
	{
		try
		{
			ServerInfo.Instance.GetUserInfo(SocialManager.GetUserData(SocialManager.Instance.ViewerID),(res)=>{
				if (res.VIP==0)
					UserAvatarCrownSprite.spriteName = "avatar_no_shadow";
				else
					UserAvatarCrownSprite.spriteName = "avatar";
				Init(res);
			},true);
			enabled = false;
		}
		catch
		{}
	}

	public static void ReInit()
	{
		current.Load();
	}

#if UNITY_EDITOR
	void Start()
	{
		current = this;
		Load();
	}
#else
	void Start()
	{
		current = this;
		enabled = true;
	}

	void Update()
	{
		if (SocialManager.Instance.IsLoaded)
		{
			Load();
		}
	}
#endif
}






































