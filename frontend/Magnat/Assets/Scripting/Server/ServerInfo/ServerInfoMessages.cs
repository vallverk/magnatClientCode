using System;
using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public partial class ServerInfo : Singleton<ServerInfo> 
{
	public void SendUserMessage(string ReceiverID, string Text, Action Callback)
	{
		Query q = new QuerySendMessage(viewerID,auth,ReceiverID,Text,(int)TimeTools.GetLinuxTimeStamp());
		Pool.SendPostRequestAsync(q,(res)=>{Callback();});
	}

	public void GetUserMessagesCount(Action<int> Callback)
	{
		Query q = new QueryGetMessagesCount(viewerID,auth);
		Pool.SendPostRequestAsync(q,(res)=>{
			Callback(JSONSerializer.Deserialize<QueryGetMessagesCount.Request>(res.Args[0].ToString()).count);
		});
	}

	public void GetUserMessagesIDs(Action<long[]> Callback)
	{
		Query q = new QueryGetMessagesIDList(viewerID,auth);
		Pool.SendPostRequestAsync(q,(res)=>{
			List<long> ids = new List<long>();
			foreach (var id in res.Args)
				ids.Add(long.Parse(id.ToString()));
			Callback(ids.ToArray());
		});
	}

	public void GetUserMessageByID(long ID, Action<UserMessage> Callback)
	{
		Query q = new QueryGetMessageByID(viewerID,auth,ID);
		Pool.SendPostRequestAsync(q,(res)=>{
			Callback(JSONSerializer.Deserialize<UserMessage>(res.Args[0].ToString()));
		});
	}
}
