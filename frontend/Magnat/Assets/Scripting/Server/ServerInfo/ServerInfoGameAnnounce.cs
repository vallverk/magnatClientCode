using System;
using System.Collections.Generic;

public partial class ServerInfo : Singleton<ServerInfo> 
{
	public void CreateNewGameAnnounce(string GameName, GameType GameType, int PlayersCount, int Bet, string Password, Action<bool> Callback)
	{
		if (GameInfoController.ConnectedGameID!=-1)
		{
			Callback(false);
			return;
		}
		QueryCreateGameAnnounce q = new QueryCreateGameAnnounce(viewerID,auth,viewerID,
		                                                        GameName,GameType,PlayersCount,Bet,Password);
		Pool.SendPostRequestAsync(q,(a) => { 
			Callback(JSONSerializer.Deserialize<QueryCreateGameAnnounce.Request>(a.Args[0].ToString()).Status == 200); 
		});
	}
	
	public void GetGameAnnounceList(Action<GameInfo[]> Callback)
	{
		Query q = new QueryGetGameAnnounceList(viewerID,auth,viewerID);
		Pool.SendPostRequestAsync(q, (a) =>
		                          {
			List<GameInfo> res = new List<GameInfo>();
			for (int i=0;i<a.Args.Count;i++)
				res.Add(JSONSerializer.Deserialize<GameInfo>(a.Args[i].ToString()));
			Callback(res.ToArray());
		});
	}

	public void ConnectToGameAnnounce(long GameID, string Password, Action<bool> Callback)
	{
		Query q = new QueryConnectToGameAnnounce(viewerID,auth,viewerID,GameID,Password);
		Pool.SendPostRequestAsync(q,(a) => {
			Callback(JSONSerializer.Deserialize<QueryConnectToGameAnnounce.Request>(a.Args[0].ToString()).Status == 200);
		});
	}

	public void ConnectTo2x2GameAnnounce(long GameID, string Password, int Team, Action<int> Callback)
	{
		Query q = new QueryConnectTo2x2GameAnnounce(viewerID,auth,viewerID,GameID,Team);
		Pool.SendPostRequestAsync(q,(a) => {
			Callback(JSONSerializer.Deserialize<QueryConnectTo2x2GameAnnounce.Request>(a.Args[0].ToString()).Status);
		});
	}
	
	public void DisconnectFromGameAnnounce(long GameID, Action<bool> Callback)
	{
		Query q = new QueryDisconnectFromGameAnnounce(viewerID,auth,viewerID,GameID);
		Pool.SendPostRequestAsync(q, (a)=>{
			Callback(JSONSerializer.Deserialize<QueryDisconnectFromGameAnnounce.Request>(a.Args[0].ToString()).Status == 200);
		});
	}

	public void DisconnectFrom2x2GameAnnounce(long GameID, Action<bool> Callback)
	{
		Query q = new QueryDisconnectFromGameAnnounce2x2(viewerID,auth,viewerID,GameID);
		Pool.SendPostRequestAsync(q, (a)=>{
			Callback(JSONSerializer.Deserialize<QueryDisconnectFromGameAnnounce.Request>(a.Args[0].ToString()).Status == 200);
		});
	}
}
