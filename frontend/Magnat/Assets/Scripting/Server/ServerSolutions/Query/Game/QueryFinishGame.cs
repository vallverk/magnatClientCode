using UnityEngine;
using System.Collections;

public class QueryFinishGame : Query
{
	public class Request
	{
		public int Status;
	}
	
	public QueryFinishGame(string UserID, string AuthKey, long GameID, string WinnerID, int WinCapital, string[] ActivePlayers)
	{
		base.Type = "gameOver";
		base.UserID = UserID;
		base.ViewerID = UserID;
		base.AuthKey = AuthKey;
		Args.Add(new 
		         {
			gameId = GameID,
			winerId = WinnerID,
			winCapital = WinCapital,
			vipList = ActivePlayers
		});
	}
}
