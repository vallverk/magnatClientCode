using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LobbyTopsManager : MonoBehaviour 
{
	public LobbyTopField[] AllTop;
	public LobbyTopField[] WeekTop;

	void Start()
	{
		for (int i=0;i<AllTop.Length;i++)
		{
			AllTop[i].GetComponent<UIWidget>().alpha = 0;
		}

		for (int i=0;i<WeekTop.Length;i++)
		{
			WeekTop[i].GetComponent<UIWidget>().alpha = 0;
		}

		StartCoroutine(StartLoadData());
		StartCoroutine(StartLoadWeekData());
	}

	IEnumerator StartLoadData()
	{
		while (!SocialManager.Instance.IsLoaded)
			yield return new WaitForSeconds(0.1f);

		ServerInfo.Instance.GetTop10Data((uids)=>{
#if !UNITY_EDITOR
			foreach (string uid in uids)
				SocialManager.GetUserInfo(uid);
#endif
			// запросим пачками по 3 пользователя
			List<string> ids = new List<string>(uids);
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
			SetTop10Data(uids);
		});
	}

	IEnumerator StartLoadWeekData()
	{
		while (!SocialManager.Instance.IsLoaded)
			yield return new WaitForSeconds(0.1f);
		
		ServerInfo.Instance.GetWeekTop10Data((uids)=>{
			#if !UNITY_EDITOR
			foreach (string uid in uids)
				SocialManager.GetUserInfo(uid);
			#endif
			// запросим пачками по 3 пользователя
			List<string> ids = new List<string>(uids);
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
			SetWeekTop10Data(uids);
		});
	}

	void SetTop10Data(string[] Data)
	{
		StartCoroutine(WaitForUsersAndSet(Data,AllTop));
	}

	void SetWeekTop10Data(string[] Data)
	{
		StartCoroutine(WaitForUsersAndSet(Data,WeekTop));
	}

	IEnumerator WaitForUsersAndSet(string[] Users, LobbyTopField[] Fields)
	{
		// дадим время чтоб подумать
		yield return new WaitForSeconds(0.1f);

		// подождем пока загрузятся все пользователи с сервера
		List<string> waitFor = new List<string>(Users);
		while (waitFor.Count!=0)
		{
			if (ServerInfo.Instance.BufferContainsUser(waitFor[0]))
				waitFor.RemoveAt(0);
			else
				yield return new WaitForSeconds(0.1f);
		}

		// запишем данные о них
		ServerUserInfo[] users = new ServerUserInfo[Users.Length];
		for (int i=0;i<users.Length;i++)
			users[i] = ServerInfo.Instance.GetUserDataFromBuffer(Users[i]);

		for (int i=0;i<Fields.Length;i++)
		{
			if (i<users.Length)
			{
				Fields[i].SetOnClickEvent(users[i].GUID);
				Fields[i].Number.text = (i+1).ToString();
				//Fields[i].Name.text = users[i].GUID;

#if UNITY_EDITOR
				Fields[i].Name.text = users[i].GUID;
#else
				var udata = SocialManager.GetUserData(users[i].GUID);
				while (udata == null)
				{
					yield return new WaitForSeconds(0.1f);
					udata = SocialManager.GetUserData(users[i].GUID);
				}
				Fields[i].Name.text = udata.FirstName;
#endif
				Fields[i].Capital.text = users[i].Capital.ToString("$###,###,##0k");

				TweenAlpha tween = NGUITools.AddMissingComponent<TweenAlpha>(Fields[i].gameObject);
				tween.duration = 0.4f;
				tween.from = 0;
				tween.to = 1;
				tween.style = UITweener.Style.Once;
			}
		}
	}
}