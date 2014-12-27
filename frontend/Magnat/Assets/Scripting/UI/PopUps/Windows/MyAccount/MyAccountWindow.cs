using UnityEngine;
using System.Collections;

public class MyAccountWindow : WindowBehavoiur 
{
	public UITexture AvatarTexture;
	public UIWidget CrownWiget;
	public UILabel NameLabel;
	public UILabel TitleLibel;

	public UILabel PlayerInfoBlock1;
	public UILabel PlayerInfoBlock2;
	public UILabel PlayerInfoBlock3;

	public GameObject WithClubGO;
	public GameObject WithoutClubGO;
	public UIInput CreateClubName;

	public ClubInfoSetter ClubInfoTab;

	public UIGrid InviteGrid;
	public GameObject InvitePrefab;

	public UILabel NewMessagesLabel;
	public UILabel InvationsLabel;

	public UserActionsInitiator ActionsManager;
	public UserGiftsInitiator GiftsManager;

	public GameObject ExitButtonClub;

	private bool clubInited = false;
	private bool avatarLoaded = false;
	private bool mainInfoLoaded = false;

	private string clubID = "";

	public void UpdateWindow()
	{
		clubInited = false;
		mainInfoLoaded = false;
        InvationsLabel.text = "";
        UITools.RemoveChildrens(InviteGrid);
		Hide();
		Show();
	}

	public static ServerUserInfo MyProfile = null;

	public void CreateClub()
	{
		ServerInfo.Instance.GetUserInfo(SocialManager.User,(user)=>{
			if (user.VIP==0)
				AlertWindow.Show("ОШИБКА","Для создания клуба необходим VIP-статус.");
			else
			{
				if (user.Gold>=200)
				{
					string cname = CreateClubName.value;
					if (string.IsNullOrEmpty(cname)) cname = "Новый клуб";
					ServerInfo.Instance.CreateClub(cname,(a)=>{
						ServerInfo.Instance.GetUserInfo(SocialManager.User,(u)=>{
							ServerInfo.Instance.SetClubFileIcon(u.ClubId,"vip-club_icon.png",(z)=>{});
						},false);
						LobbyHeaderLoader.ReInit();
					});
					clubInited = false;
					mainInfoLoaded = false;
					Hide();
					Show();
				} 
				else
					AlertWindow.Show("ОШИБКА","Недостаточно золота для создания клуба. На вашем счету должно быть, как минимум, 200 кг. золота.");
			}

		},false);
	}

	public void ExitFromClub()
	{
		AlertWindow.Show("ПОДТВЕРЖДЕНИЕ","Вы уверены, что хотите выйти из клуба?",()=>{
			ServerInfo.Instance.ExitFromClub((res)=>{
				LobbyHeaderLoader.ReInit();	
				clubInited = false;
				mainInfoLoaded = false;
				Hide();
				Show();
			});
		},()=>{});
	}

	public void GoToClub()
	{
		Hide();
		ClubWindow.Show(clubID);
	}

	public override void Show ()
	{
		base.Show();

		GetComponent<TabController>().SetActiveTab(0);

		if (!mainInfoLoaded)
		{
			NameLabel.text = SocialManager.User.FormatName;
			ServerInfo.Instance.GetUserInfo(SocialManager.User,(uinfo)=>{

				if (uinfo == null) return;

				MyProfile = uinfo;

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
				PlayerInfoBlock2.text = string.Format("{0}\r\n{1}\r\n{2}",
				                                      vip,
				                                      uinfo.Gold.ToString("0 кг"),
				                                      uinfo.GameCards.ToString());
				PlayerInfoBlock3.text = string.Format("{0}\r\n{1}",
				                                      TimeTools.FormatUTSTime(uinfo.LastActive),
				                                      TimeTools.FormatUTSTime(uinfo.Registered));
				mainInfoLoaded = true;
			},true);
		}

		if (!clubInited)
		{
			// инициализируем вкладку с клубом
			WithClubGO.SetActive(false);
			WithoutClubGO.SetActive(false);
			ClubInfoTab.SetEmpty();

            UITools.RemoveChildrens(InviteGrid);

			ServerInfo.Instance.GetUserInfo(SocialManager.User,(uinfo)=>{
				WithClubGO.SetActive(!string.IsNullOrEmpty(uinfo.ClubId));
				WithoutClubGO.SetActive(string.IsNullOrEmpty(uinfo.ClubId));

				if (string.IsNullOrEmpty(uinfo.ClubId))
				{
					// если без клуба
					ServerInfo.Instance.GetUserInviteList((clubs)=>{
						if (clubs.Length!=0)
							InvationsLabel.text = clubs.Length+" приглашений";
						else
							InvationsLabel.text = "";

                        UITools.RemoveChildrens(InviteGrid);

						foreach (var id in clubs)
						{
							GameObject field = NGUITools.AddChild(InviteGrid.gameObject,InvitePrefab);
							InviteGrid.AddChild(field.transform);
							InviteToClubField itf = field.GetComponent<InviteToClubField>();
							itf.Init(id);
						}
					});
				}
				else
				{
					// если с клубом
                    clubID = uinfo.ClubId;
					ExitButtonClub.SetActive(false);
					ServerInfo.Instance.GetClub(uinfo.ClubId,(club)=>{
					    ExitButtonClub.SetActive(club.CreatorID != SocialManager.User.ViewerId);
						ClubInfoTab.SetInfo(club);
					});
				}

				clubInited = true;
			},false);
		}

		ServerInfo.Instance.GetUserMessagesCount((count)=>{
			if (count!=0)
				NewMessagesLabel.text = count.ToString("##0 новых");
			else
				NewMessagesLabel.text = "";
		});

		ActionsManager.Init(SocialManager.User.ViewerId);
		GiftsManager.Init(SocialManager.User.ViewerId);

#if !UNITY_EDITOR
		if (!avatarLoaded)
		{
			var user = SocialManager.User;
			if (user != null)
				ImageLoader.Instance.LoadAvatar(user.Photo,(tex)=>{
					AvatarTexture.mainTexture = tex;
					avatarLoaded = true;
				});
		}
#endif
	}
}















































