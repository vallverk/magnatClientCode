using System;

public class QueryDisconnectFromGameAnnounce : Query
{
	public class Request
	{
		public int Status;
	}

	public QueryDisconnectFromGameAnnounce(string UserID, string AuthKey, string ViewerID, long GameID)
	{
		base.Type = "DisconnectFromGameAnnounce";
		base.UserID = UserID;
		base.ViewerID = ViewerID;
		base.AuthKey = AuthKey;
		Args.Add(new 
		{
			GUID = GameID
		});
	}
}
