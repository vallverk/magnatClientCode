using System;

public partial class ServerInfo : Singleton<ServerInfo> 
{
	public void GetGameInfo(long GUID, Action<GameInfo> Callback)
	{
		Query q = new QueryGetGameAnnounceInfo(viewerID,auth,viewerID,GUID);
		Pool.SendPostRequestAsync(q,(a)=>{
			Callback(JSONSerializer.Deserialize<GameInfo>(a.Args[0].ToString()));
		});
	}
	
	public void FinishGame(long GameID, string WinnerSocialID, int WinCapital, string[] ActivePlayers, Action<bool> Callback)
	{
		Query q = new QueryFinishGame(viewerID,auth,GameID,WinnerSocialID,WinCapital, ActivePlayers);
		Pool.SendPostRequestAsync(q, (a) =>{Callback(true);});
	}

	public void FinishGame2x2(long GameID, string[] WinnerSocialID, int WinCapital, string[] ActivePlayers, Action<bool> Callback)
	{
		Query q = new QueryFinishGame2z2(viewerID,auth,GameID,WinnerSocialID,WinCapital,ActivePlayers);
		Pool.SendPostRequestAsync(q, (a) =>{Callback(true);});
	}
}
