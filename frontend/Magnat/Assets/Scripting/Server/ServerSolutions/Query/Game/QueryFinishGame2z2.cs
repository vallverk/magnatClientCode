using UnityEngine;
using System.Collections;

public class QueryFinishGame2z2 : Query
{
	public class Request
	{
		public int Status;
	}
	
	public QueryFinishGame2z2(string UserID, string AuthKey, long GameID, string[] Winners, int WinCapital, string[] ActivePlayers)
	{
		base.Type = "game2x2Over";
		base.UserID = UserID;
		base.ViewerID = UserID;
		base.AuthKey = AuthKey;
		Args.Add(new 
		         {
			gameId = GameID,
			WinnerArray = Winners,
			Capital = WinCapital,
			vipList = ActivePlayers
		});
	}
}
