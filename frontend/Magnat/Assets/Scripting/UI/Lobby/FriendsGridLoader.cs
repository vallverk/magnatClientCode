using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(UIGrid))]
public class FriendsGridLoader : MonoBehaviour 
{
	public GameObject FriendCardPrefab;

	void Awake()
	{
		if (SocialManager.Instance.IsFriendsLoaded)
			Init(SocialManager.Instance.Friends.ToArray());
		else
		{
			SocialManager.Instance.OnFriendsDataLoaded -= FriendsLoaded;
			SocialManager.Instance.OnFriendsDataLoaded += FriendsLoaded;
		}
	}

	void FriendsLoaded ()
	{
		Init(SocialManager.Instance.Friends.ToArray());
		SocialManager.Instance.OnFriendsDataLoaded -= FriendsLoaded;
	}

	IEnumerator InitUsers(string[] UIDS)
	{
		while (!SocialManager.Instance.IsLoaded)
			yield return new WaitForSeconds(0.1f);

		// запросим пачками по 3 пользователя
		List<string> ids = new List<string>(UIDS);
		while (ids.Count>0)
		{
			if (ids.Count>3)
			{
				ServerInfo.Instance.GetUserInfo(ids.GetRange(0,3).ToArray(),(a)=>{});
				ids.RemoveRange(0,3);
			}
			else
			{
				ServerInfo.Instance.GetUserInfo(ids.ToArray(),(a)=>{});
				ids.Clear();
			}
		}
		// запустим анализатор

		// подождем пока загрузятся все пользователи с сервера
		List<string> waitFor = new List<string>(UIDS);
		while (waitFor.Count!=0)
		{
			if (ServerInfo.Instance.BufferContainsUser(waitFor[0]))
				waitFor.RemoveAt(0);
			else
				yield return new WaitForSeconds(0.1f);
		}
		
		// запишем данные о них
		ServerUserInfo[] users = new ServerUserInfo[UIDS.Length];
		for (int i=0;i<users.Length;i++)
		{
			users[i] = ServerInfo.Instance.GetUserDataFromBuffer(UIDS[i]);
			if (users[i]==null)
			{
				yield return new WaitForSeconds(0.1f);
				i--;
			}
		}

		UIGrid grid = GetComponent<UIGrid>();
		for (int i=0;i<users.Length;i++)
		{
			GameObject tab = NGUITools.AddChild(grid.gameObject,FriendCardPrefab) as GameObject;
			var socUser = SocialManager.Instance.SocialData[users[i].GUID];
			FriendInitiator fi = tab.GetComponent<FriendInitiator>();
			fi.Init(socUser.FirstName,socUser.LastName,socUser.Photo,users[i].Title,users[i].Capital); 
			fi.SetOnClickEvent(users[i].GUID);
		}
	}

	public void Init(string[] UIDS)
	{
		//StartCoroutine(InitUsers(UIDS));
		string f = "";
		foreach (var uid in UIDS)
			f+=uid+"   ";
		Debug.Log("----- START LOADING FRIENDS ------ \r\n"+f);
		ServerInfo.Instance.GetUserInfo(UIDS,(uu)=>{
			Debug.Log("----- Loaded "+uu.Length+" friends ----");
			UIGrid grid = GetComponent<UIGrid>();
			foreach (var u in uu)
			{
				GameObject tab = NGUITools.AddChild(gameObject,FriendCardPrefab);
				grid.AddChild(tab.transform);
				var socUser = SocialManager.GetUserData(u.GUID);
				FriendInitiator fi = tab.GetComponent<FriendInitiator>();
				fi.Init(socUser.FirstName,socUser.LastName,socUser.Photo,u.Title,u.Capital); 
				fi.SetOnClickEvent(u.GUID);
			}
			grid.Reposition();
		},false);
	}
}
