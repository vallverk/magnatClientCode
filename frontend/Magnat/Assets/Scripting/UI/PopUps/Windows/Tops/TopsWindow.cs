using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TopsWindow : WindowBehavoiur 
{
	// общий топ
	public GameObject[] TopPlayersPrefabs;
	public UIGrid TopPlayersGrid;
	private bool topInited = false;

	// топ недели
	public GameObject[] TopWeekPlayersPrefabs;
	public UIGrid TopWeekPlayersGrid;
	private bool weekTopInited = false;

	// топ клубов
	public GameObject TopClubPrefab;
	public UIGrid TopClubsGrid;
	private bool topClubsInited = false;

	IEnumerator InitWeekTop()
	{
		while (!SocialManager.Instance.IsLoaded)
			yield return new WaitForSeconds(0.1f);
		
		ServerInfo.Instance.GetWeekTop100Data((uids)=>{
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
			StartCoroutine(SetWeekTopData(uids));
		});
	}

	IEnumerator SetWeekTopData(string[] Users)
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
		
		for (int i=0;i<users.Length;i++)
		{
			GameObject prefab = TopWeekPlayersPrefabs[users[i].VIP!=0?0:1];
			GameObject field = NGUITools.AddChild(TopWeekPlayersGrid.gameObject,prefab);
			TopWeekPlayersGrid.AddChild(field.transform);
			TopPlayerField ufield = field.GetComponent<TopPlayerField>();
			ufield.SetOnClickEvent(users[i].GUID);
			ufield.NumberLabel.text = (i+1).ToString();
			ufield.CapitalLabel.text = users[i].Capital.ToString("$###,###,##0k");
            ufield.TitleLabel.text = users[i].Title;
			
			#if UNITY_EDITOR
			ufield.NameLabel.text = users[i].GUID;
			#else
			var udata = SocialManager.GetUserData(users[i].GUID);
			while (udata == null)
			{
				yield return new WaitForSeconds(0.1f);
				udata = SocialManager.GetUserData(users[i].GUID);
			}
			ufield.NameLabel.text = udata.FormatName;
			ufield.LoadAvatar(udata.Photo);
			#endif
			
			field.GetComponent<UIWidget>().alpha = 0;
			TweenAlpha tween = NGUITools.AddMissingComponent<TweenAlpha>(field);
			tween.duration = 0.4f;
			tween.from = 0;
			tween.to = 1;
			tween.style = UITweener.Style.Once;
		}
		weekTopInited = true;
	}

	IEnumerator InitTop()
	{
		while (!SocialManager.Instance.IsLoaded)
			yield return new WaitForSeconds(0.1f);
		
		ServerInfo.Instance.GetTop100Data((uids)=>{
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
			StartCoroutine(SetTopData(uids));
		});
	}

	IEnumerator SetTopData(string[] Users)
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
		
		for (int i=0;i<users.Length;i++)
		{
			GameObject prefab = TopPlayersPrefabs[users[i].VIP!=0?0:1];
			GameObject field = NGUITools.AddChild(TopPlayersGrid.gameObject,prefab);
			TopPlayersGrid.AddChild(field.transform);
			TopPlayerField ufield = field.GetComponent<TopPlayerField>();
			ufield.SetOnClickEvent(users[i].GUID);
			ufield.NumberLabel.text = (i+1).ToString();
			ufield.CapitalLabel.text = users[i].Capital.ToString("$###,###,##0k");
            ufield.TitleLabel.text = users[i].Title;

			#if UNITY_EDITOR
			ufield.NameLabel.text = users[i].GUID;
			#else
			var udata = SocialManager.GetUserData(users[i].GUID);
			while (udata == null)
			{
				yield return new WaitForSeconds(0.1f);
				udata = SocialManager.GetUserData(users[i].GUID);
			}
			ufield.NameLabel.text = udata.FormatName;
			ufield.LoadAvatar(udata.Photo);
			#endif

			field.GetComponent<UIWidget>().alpha = 0;
			TweenAlpha tween = NGUITools.AddMissingComponent<TweenAlpha>(field);
			tween.duration = 0.4f;
			tween.from = 0;
			tween.to = 1;
			tween.style = UITweener.Style.Once;
		}
		topInited = true;
	}

	void InitTopClubs()
	{
		ServerInfo.Instance.GetTop100Clubs((ids)=>{
			for (int i=0;i<ids.Length;i++)
			{
				GameObject field = NGUITools.AddChild(TopClubsGrid.gameObject,this.TopClubPrefab);
				TopClubsGrid.AddChild(field.transform);
				TopClubField tcf = field.GetComponent<TopClubField>();
				tcf.NumberLabel.text = (i+1).ToString();
				tcf.Init(ids[i]);
			}
			topClubsInited = true;
		});
	}

	public override void Show()
	{
		base.Show();

		if (!topInited)
			StartCoroutine("InitTop");

		if (!weekTopInited)
			StartCoroutine("InitWeekTop");

		if (!topClubsInited)
			InitTopClubs();
	}
}
