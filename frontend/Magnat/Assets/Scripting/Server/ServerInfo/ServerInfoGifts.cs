using System;
using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public partial class ServerInfo : Singleton<ServerInfo> 
{
	public void GetUserGifts(string UserID, Action<Gift[]> Callback)
	{
		Query q = new QueryGetUserGifts(viewerID,auth,UserID);
		Pool.SendPostRequestAsync(q,(res)=>{
			Callback( JSONSerializer.Deserialize<Gift[]>(res.Args[0].ToString()));
		});
	}

	public void TakeGift(string GiftID, string UserID, string Description, Action Callback)
	{
		Query q = new QueryGetAGift(viewerID,auth,GiftID,UserID,Description);
		Pool.SendPostRequestAsync(q,(res)=>{
			Callback();
		});
	}

	public void GetGiftsList(Action<Gift[]> Callback)
	{
		Query q = new QueryGetGiftList(viewerID,auth);
		Pool.SendPostRequestAsync(q,(res)=>{
			List<Gift> gifts = new List<Gift>();
			foreach (var gift in res.Args)
				gifts.Add(JSONSerializer.Deserialize<Gift>(gift.ToString()));
			Callback( gifts.ToArray());
		});
	}
}
