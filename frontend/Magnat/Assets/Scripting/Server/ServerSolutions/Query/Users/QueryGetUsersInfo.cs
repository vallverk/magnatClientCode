using System;

public class QueryGetUsersInfo : Query
{
	public class Request
	{
		public string GUID;
		public int Capital;
		public int WeekCapital;
		public int CountOfGames;
		public int CountOf2x2Games;
		public int CountOfWins;
		public int CountOf2x2Wins;
		public int MaxWinCash;
		public int MaxWinGold;
		public int Gold;
		public string Registered;
		public string LastActive;
		public int VIP;
	}

	public QueryGetUsersInfo(string UserID, string AuthKey, string ViewerID, params string[] UIDS)
	{
		base.Type = "GetUsersInfo";
		base.UserID = UserID;
		base.ViewerID = ViewerID;
		base.AuthKey = AuthKey;
		foreach (string s in UIDS)
			Args.Add(s);
	}
}
