using System;
using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public partial class ServerInfo : Singleton<ServerInfo> 
{
	public void GetUserActions(string UserID, Action<UserAction[]> Callback)
	{
		Query q = new QueryGetUserActionList(viewerID,auth,UserID);
		Pool.SendPostRequestAsync(q,(res)=>{
			Callback(JSONSerializer.Deserialize<UserAction[]>(res.Args[0].ToString()));
		});
	}

	public void BuyAction(string ActionID, Action<int> Callback)
	{
		Query q = new QueryBuyAction(viewerID,auth,ActionID);
		Pool.SendPostRequestAsync(q,(res)=>{
			Callback( JSONSerializer.Deserialize<QueryBuyAction.Request>(res.Args[0].ToString()).Status );
		});
	}

	private Dictionary<string,UserAction> actionsByID = new Dictionary<string, UserAction>();
	public void GetActionByID(string ActionID, Action<UserAction> Callback)
	{
		if (actionsByID.ContainsKey(ActionID))
		{
			Callback(actionsByID[ActionID]);
			return;
		}

		Query q = new QueryGetActionByID(viewerID,auth,ActionID);
		Pool.SendPostRequestAsync(q,(res)=>{
			if (!actionsByID.ContainsKey(ActionID))
				actionsByID.Add(ActionID,JSONSerializer.Deserialize<UserAction>(res.Args[0].ToString()));
			Callback(actionsByID[ActionID]);
		},true);
	}

	private string[] allActionsIDs = null;
	public void GetActionIDList(Action<string[]> Callback)
	{
		if (allActionsIDs != null)
		{
			Callback(allActionsIDs);
			return;
		}

		Query q = new QueryGetActionIdList(viewerID,auth);
		Pool.SendPostRequestAsync(q,(res)=>{
			allActionsIDs = JSONSerializer.Deserialize<string[]>(res.Args[0].ToString());
			Callback(allActionsIDs);
		},true);
	}
}
