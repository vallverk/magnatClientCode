using UnityEngine;
using System.Collections;

public class UserProfileWindow : WindowBehavoiur 
{
	public static void Show(string UID)
	{
		if (UID == SocialManager.User.ViewerId)
			GameObject.FindObjectOfType<MyAccountWindow>().Show();
		else
			GameObject.FindObjectOfType<UserProfileWindow>().Init(UID);
	}

	private string currentUID;

	private void Init(string UID)
	{
		currentUID = UID;

		clubInited = false;
		avatarLoaded = false;
		mainInfoLoaded = false;
		userID = UID;

		if (WindowBehavoiur.current!=null)
		{
			WindowBehavoiur.current.Hide();
			WindowBehavoiur.current = null;
		}
		Show();
	}

	public UITexture AvatarTexture;
	public UIWidget CrownWiget;
	public UILabel NameLabel;
	public UILabel TitleLibel;
	
	public UILabel PlayerInfoBlock1;
	public UILabel PlayerInfoBlock2;
	public UILabel PlayerInfoBlock3;
	
	public GameObject WithClubGO;
	public GameObject WithoutClubGO;
	public GameObject ClubInviteButton;
	
	public ClubInfoSetter ClubInfoTab;

	public UserActionsInitiator ActionsManager;
	public UserGiftsInitiator GiftsManager;
	
	private bool clubInited = false;
	private bool avatarLoaded = false;
	private bool mainInfoLoaded = false;
	
	private string clubID = "";
	private string userID = "";

	public void SendGift()
	{
		GameObject.FindObjectOfType<SelectGiftWindow>().Show(userID);
	}
	
	public void GoToClub()
	{
		Hide();
		ClubWindow.Show(clubID);
	}

	public void OpenPage()
	{
		if (SocialManager.Platform == "VK")
			Application.ExternalEval("window.open('http://vk.com/id"+this.userID+"','User Profile')");
		else
			Application.ExternalEval("window.open('http://www.odnoklassniki.ru/profile/"+this.userID+"','User Profile')");
	}

	public void SendUserMessage()
	{
		SendMessageWindow.Show(userID);
	}

	public void InviteToClub()
	{
        ServerInfo.Instance.GetUserInfo(SocialManager.User,(u)=>{
            ServerInfo.Instance.GetClub(u.ClubId, (club) => {
                if (club.UsersLimit <= club.UserList.Count)
                {
                    AlertWindow.Show("ОШИБКА", "Для текущего клуба уже достигнуто максимальное количество пользователей, нельзя пригласить новых.");
                }
                else
                {
                    ServerInfo.Instance.InviteToClub(userID, () =>
                    {
                        AlertWindow.Show("Уведомление", "Пользователь получил ваше приглашение");
                    });
                }
            });
        },false);
	}
	
	public override void Show ()
	{
		base.Show();

		SocialData user;
#if UNITY_EDITOR
		user = new SocialData()
		{
			ViewerId = userID,
			FirstName = userID,
			LastName = userID,
			Photo = userID
		};
#else
		user = SocialManager.GetUserData(userID);
#endif
		GetComponent<TabController>().SetActiveTab(0);
		GiftsManager.Init(user.ViewerId);

		if (!mainInfoLoaded)
		{
			ActionsManager.Init(user.ViewerId);

			NameLabel.text = user.FormatName;
			ServerInfo.Instance.GetUserInfo(new string[1] {this.userID},(info)=>{
				var uinfo = info[0];

				// показывать ли кнопочку "пригласить"?
				ClubInviteButton.SetActive(ClubContainer.OwnerOf!="" && userID!=SocialManager.Instance.ViewerID && string.IsNullOrEmpty(uinfo.ClubId));

				TitleLibel.text = uinfo.Title;
				PlayerInfoBlock1.text = string.Format("{0}\r\n{1}\r\n{2}\r\n{3}\r\n{4}\r\n{5}\r\n{6}\r\n{7}",
				                                      uinfo.CountOfGames.ToString(),
				                                      uinfo.CountOfWins.ToString(),
				                                      uinfo.CountOf2x2Games.ToString(),
				                                      uinfo.CountOf2x2Wins.ToString(),
				                                      uinfo.MaxWinGold.ToString("0 кг"),
				                                      uinfo.MaxWinCash.ToString("$###,###,###,###,##0"),
				                                      uinfo.Capital.ToString("$###,###,###,###,##0"),
				                                      uinfo.WeekCapital.ToString("$###,###,###,###,##0"));
				string vip = "Стандарт";
				if (uinfo.VIP!=0)
				{
					UITools.FadeIn(CrownWiget.gameObject,0.4f);
					vip = "VIP до ";
					vip += TimeTools.UnixTimeStampToDateTime(uinfo.VIP/1000.0).ToString("dd.MM.yyyy hh:mm:ss");
				}
				CrownWiget.gameObject.SetActive(uinfo.VIP!=0);
				PlayerInfoBlock2.text = string.Format("{0}\r\n{1}\r\n{2}",
				                                      vip,
				                                      uinfo.Gold.ToString("0 кг"),
				                                      uinfo.GameCards.ToString());
				PlayerInfoBlock3.text = string.Format("{0}\r\n{1}",
				                                      TimeTools.FormatUTSTime(uinfo.LastActive),
				                                      TimeTools.FormatUTSTime(uinfo.Registered));
				mainInfoLoaded = true;
			});
		}
		
		if (!clubInited)
		{
			// инициализируем вкладку с клубом
			WithClubGO.SetActive(false);
			WithoutClubGO.SetActive(false);
			ClubInfoTab.SetEmpty();
			ServerInfo.Instance.GetUserInfo(new string[1]{userID},(info)=>{
				var uinfo = info[0];
				WithClubGO.SetActive(!string.IsNullOrEmpty(uinfo.ClubId));
				WithoutClubGO.SetActive(string.IsNullOrEmpty(uinfo.ClubId));
				
				if (!string.IsNullOrEmpty(uinfo.ClubId))
				{
					clubID = uinfo.ClubId;
					ServerInfo.Instance.GetClub(uinfo.ClubId,(club)=>{
						ClubInfoTab.SetInfo(club);
					});
				}
				
				clubInited = true;
			});
		}
		
		#if !UNITY_EDITOR
		if (!avatarLoaded)
		{
			if (user != null)
				ImageLoader.Instance.LoadAvatar(user.Photo,(tex)=>{
					AvatarTexture.mainTexture = tex;
					avatarLoaded = true;
				});
		}
		#endif
	}
}
