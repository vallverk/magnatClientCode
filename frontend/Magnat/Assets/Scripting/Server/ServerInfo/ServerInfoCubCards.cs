using System;
using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public partial class ServerInfo : Singleton<ServerInfo> 
{
	public void GetClubCardList(string ClubID, Action<ClubCard[]> Callback)
	{
		Query q = new QueryGetClubCardList(viewerID,auth,ClubID);
		Pool.SendPostRequestAsync(q,(res)=>{
            ClubCard[] cards = JSONSerializer.Deserialize<ClubCard[]>(res.Args[0].ToString());
            for (int i = 0; i < cards.Length; i++)
                cards[i].image = "http://magnatgame.com" + cards[i].image;
			Callback(cards);
		});
	}

	public void BuyClubCard(string ClubID, string CardID, Action<int> Callback)
	{
		Query q = new QueryBuyClubCard(viewerID,auth,CardID,ClubID);
		Pool.SendPostRequestAsync(q,(res)=>{
			Callback(JSONSerializer.Deserialize<QueryBuyAction.Request>(res.Args[0].ToString()).Status );
		});
	}

	private string[] clubCardsIDsBuffer = null;
	public void GetClubCardsIDList(Action<string[]> Callback)
	{
		if (clubCardsIDsBuffer != null)
		{
			Callback(clubCardsIDsBuffer);
			return;
		}

		Query q = new QueryGetClubCardsIDList(viewerID,auth);
		Pool.SendPostRequestAsync(q,(res)=>{
			clubCardsIDsBuffer = JSONSerializer.Deserialize<string[]>(res.Args[0].ToString());
			Callback(clubCardsIDsBuffer);
		});
	}


	private Dictionary<string, ClubCard> clubCardsBuffer = new Dictionary<string, ClubCard>();
	public void GetClubCard(string CardID, Action<ClubCard> Callback)
	{
		if (clubCardsBuffer.ContainsKey(CardID))
		{
			Callback(clubCardsBuffer[CardID]);
			return;
		}

		Query q = new QueryGetClubCard(viewerID,auth,CardID);
		Pool.SendPostRequestAsync(q,(res)=>{
			if (!clubCardsBuffer.ContainsKey(CardID))
				clubCardsBuffer.Add(CardID,JSONSerializer.Deserialize<ClubCard>(res.Args[0].ToString()));
			Callback(clubCardsBuffer[CardID]);
		});
	}
}
