using System;
using System.Collections.Generic;

public partial class ServerInfo : Singleton<ServerInfo> 
{
	private Dictionary<string,ServerUserInfo> usersCach = new Dictionary<string, ServerUserInfo>();

	public void UpdateBonusActive()
	{
		Query q = new QueryUpdateBonusActive(viewerID,auth);
		Pool.SendPostRequestAsync(q,(res)=>{});
	}

	public void UpdateUserCapital(long CapitalAdditive, Action Callback)
	{
		Query q = new QueryUpdateUserCapital(viewerID,auth,CapitalAdditive);
		Pool.SendPostRequestAsync(q,(res)=>{
			Callback();
		});
	}

	public void UpdateUserGold(int GoldAdditive, Action Callback)
	{
		Query q = new QueryUpdateUserGold(viewerID,auth,GoldAdditive);
		Pool.SendPostRequestAsync(q,(res)=>{
			Callback();
		});
	}

	public void SetUserVIP(int Term,Action Callback)
	{
		Query q = new QuerySetUserVipStatus(viewerID,auth,Term);
		Pool.SendPostRequestAsync(q,(res)=>{
			Callback();
		});
	}

	public void RefuseClubInvation(string ClubID, Action Callback)
	{
		Query q = new QueryRefuseClubInvation(viewerID,auth,ClubID);
		Pool.SendPostRequestAsync(q,(res)=>{ Callback(); });
	}

	public void AddUserToClub(string ClubID, Action<bool> Callback)
	{
		Query q = new QueryAddUserToClub(viewerID,auth,ClubID);
		Pool.SendPostRequestAsync(q,(res)=>{
			Callback( JSONSerializer.Deserialize<QueryAddUserToClub.Request>(res.Args[0].ToString()).Status == 200 );
		});
	}

	public void GetUserInviteList(Action<string[]> Callback)
	{
		Query q = new QueryGetUserInviteList(viewerID,auth);
		Pool.SendPostRequestAsync(q,(res)=>{
			List<string> ids = new List<string>();
			foreach (var data in res.Args)
				ids.Add(data.ToString());
			Callback(ids.ToArray());
		});
	}

	public void GetUserInfo(string[] UIDS, Action<ServerUserInfo[]> Callback)
	{
		List<ServerUserInfo> res = new List<ServerUserInfo>();
		List<string> update = new List<string>();
		foreach (string UID in UIDS)
			if (usersCach.ContainsKey(UID))
				res.Add(usersCach[UID]);
		else
			update.Add(UID);
		// если все пользователи есть в кеше - отправим их по вызову, 
		// в противном случае - добавим в кеш и перезапустим ф-ию
		if (update.Count>0)
		{
			Query q = new QueryGetUsersInfo(viewerID,auth,viewerID,update.ToArray());
			Pool.SendPostRequestAsync(q, (a) => { 
				// обновим титулы
				List<ServerUserInfo> usersToUpdate = new List<ServerUserInfo>();
				for (int i=0;i<a.Args.Count;i++)
					usersToUpdate.Add(JSONSerializer.Deserialize<ServerUserInfo>(a.Args[i].ToString()));
				UserTools.UpdateStatuses(usersToUpdate.ToArray(),(updated)=>{
					foreach (var user in updated)
						if (!usersCach.ContainsKey(user.GUID))
							usersCach.Add(user.GUID,user);
					GetUserInfo(UIDS,Callback); 
				});
			});
		} else
			Callback(res.ToArray());
	}

	public void GetUserInfo(string[] UIDS, Action<ServerUserInfo[]> Callback, bool useCache)
	{
		if (useCache)
			GetUserInfo(UIDS,Callback);
		else
		{
			Query q = new QueryGetUsersInfo(viewerID,auth,viewerID,UIDS);
			Pool.SendPostRequestAsync(q, (a) => { 
				// обновим титулы
				List<ServerUserInfo> usersToUpdate = new List<ServerUserInfo>();
				for (int i=0;i<a.Args.Count;i++)
					usersToUpdate.Add(JSONSerializer.Deserialize<ServerUserInfo>(a.Args[i].ToString()));
				UserTools.UpdateStatuses(usersToUpdate.ToArray(),(updated)=>{
					Callback(updated);
				});
			});
		}
	}

	private Action<ServerUserInfo> getUCallbacks = null;
	public void GetUserInfo(SocialData udata, Action<ServerUserInfo> Callback, bool RewriteCash)
	{
		if (RewriteCash && usersCach.ContainsKey(udata.ViewerId))
			usersCach.Remove(udata.ViewerId);
		if (usersCach.ContainsKey(udata.ViewerId))
			Callback(usersCach[udata.ViewerId]);
		else
		{
			if (getUCallbacks != null)
			{
				getUCallbacks+=Callback;
				return;
			} else
				getUCallbacks = Callback;
			Query q = new QueryGetUserInfo(viewerID,auth,viewerID,udata.ViewerId,udata.FormatName);
			Pool.SendPostRequestAsync(q,(a) =>
			                          {
				try
				{
					ServerUserInfo res = JSONSerializer.Deserialize<ServerUserInfo>(a.Args[0].ToString());
					// если титул пустой - поставим стандартный
					if (string.IsNullOrEmpty(res.Title))
					{
						GetStatuses((stats)=>{
							res.Title = UserTools.GetTitleByCapital(res.Capital,stats);
							if (!usersCach.ContainsKey(udata.ViewerId))
								usersCach.Add(udata.ViewerId,res);
							getUCallbacks(res);
							getUCallbacks = null;
						});
					}
					else
					{
						if (!usersCach.ContainsKey(udata.ViewerId))
							usersCach.Add(udata.ViewerId,res);
						getUCallbacks(res);
						getUCallbacks = null;
					}
				} catch
				{
					if (getUCallbacks!=null) 
						getUCallbacks-=Callback;
					Callback(null);
				}
			});
		}
	}

	public bool BufferContainsUser(string GUID) 
	{
		return usersCach.ContainsKey(GUID);
	}
	
	public ServerUserInfo GetUserDataFromBuffer(string GUID)
	{
		if (!BufferContainsUser(GUID)) return null;
		return usersCach[GUID];
	}
}
