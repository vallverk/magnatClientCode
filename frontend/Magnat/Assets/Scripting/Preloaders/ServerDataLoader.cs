using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class ServerDataLoader : MonoBehaviour {

	void Start () 
	{
#if UNITY_EDITOR
		Init();
#else
		if (SocialManager.Instance.IsLoaded)
			Init();
		else
			SocialManager.Instance.OnBaseDataLoaded += Init;
#endif
	}

	public static bool isLoaded 
	{
		get 
		{
			return actions.Count == 0 && cards.Count == 0;
		}
	}

	private static List<string> actions = new List<string>();
	private static List<string> cards = new List<string>();

	void Init()
	{
		SocialManager.Instance.OnBaseDataLoaded -= Init;

        ServerInfo.Instance.GetTop10Data((ids) =>
        {
            SocialManager.GetUserInfo(ids);
        });

        ServerInfo.Instance.GetWeekTop10Data((ids) =>
        {
            SocialManager.GetUserInfo(ids);
		});

        ServerInfo.Instance.GetTop100Data((res) => { SocialManager.GetUserInfo(res); });
        ServerInfo.Instance.GetWeekTop100Data((res) => { SocialManager.GetUserInfo(res); });
		ServerInfo.Instance.GetTop100Clubs((res)=>{});

		ServerInfo.Instance.GetActionIDList((ids)=>{

			actions.AddRange(ids);
			foreach (var id in ids)
				ServerInfo.Instance.GetActionByID(id,(act)=>{
					actions.Remove(act._id);
				});
		});

		ServerInfo.Instance.GetClubCardsIDList((ids)=>{

			cards.AddRange(ids);
			foreach (var id in ids)
				ServerInfo.Instance.GetClubCard(id,(card)=>{
					cards.Remove(card._id);
				});

		});

	}
}
