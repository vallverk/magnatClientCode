using System;

public class QueryConnectTo2x2GameAnnounce : Query
{
	public class Request
	{
		public int Status;
	}

	public QueryConnectTo2x2GameAnnounce(string UserID, string AuthKey, string ViewerID, long GameID, int TeamNum)
	{
		base.Type = "connectTo2x2Game";
		base.UserID = UserID;
		base.ViewerID = ViewerID;
		base.AuthKey = AuthKey;
		Args.Add(new 
		{
			GUID = GameID,
			Team = TeamNum
		});
	}
}
