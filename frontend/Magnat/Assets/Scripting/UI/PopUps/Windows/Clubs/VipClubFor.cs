using UnityEngine;
using System.Collections;

public class VipClubFor : WindowBehavoiur 
{
	public ClubInfoSetter InfoSetter;
	public UIGrid HistoryGrid;
	public GameObject HistoryFieldPrefab;

	public ClubCardsInitiator CardsManager;

	public ClubMembersInitiator MembersList;

	protected ClubInfo currentinfo;

	public void OnDeposit()
	{
		GameObject.FindObjectOfType<DepositClubWindow>().Show((res)=>{
			
			string udata = SocialManager.User.FormatName;
			string msg = "[9b9b9b][ffffff]"+udata+"[-] пополнил бюджет клуба на "+res+" кг.[-]";
			ServerInfo.Instance.AddClubEvent(currentinfo.ID,msg,()=>{});

			ServerInfo.Instance.GetUserInfo(SocialManager.User,(user)=>{
				ServerInfo.Instance.DepositToClub(user,currentinfo,res,()=>{
					AlertWindow.Show("УВЕДОМЛЕНИЕ","Операция совершена успешно.");
					LobbyHeaderLoader.ReInit();
					UpdateWindow();
				});
			},false);
		});
	}

	public virtual void Show(ClubInfo club)
	{
		currentinfo = club;
		InfoSetter.SetInfo(club);
		Show();

		MembersList.Init(club);

		ServerInfo.Instance.GetClubEvents(club.ID,(events)=>{
			if (HistoryGrid.GetChildList().Count!=0)
			{
				foreach (Transform t in HistoryGrid.GetChildList())
					NGUITools.Destroy(t.gameObject);
				HistoryGrid.GetChildList().Clear();
			}
			foreach (var e in events)
			{
				GameObject field = NGUITools.AddChild(HistoryGrid.gameObject,HistoryFieldPrefab);
				HistoryGrid.AddChild(field.transform);
				ClubEventLabel cel = field.GetComponent<ClubEventLabel>();
				cel.Init(e.Description,TimeTools.FormatUTSTime(e.Time));
			}
		});

		if (CardsManager!=null)
			CardsManager.Init(currentinfo);

		GetComponent<TabController>().SetActiveTab(0);
	}

	public void UpdateWindow()
	{
		Hide();
		ServerInfo.Instance.GetClub(currentinfo.ID,(club)=>{
			Show(club);
		});
	}
}
