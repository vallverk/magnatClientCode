using System;

public class QueryDisconnectFromGameAnnounce2x2 : Query
{
	public class Request
	{
		public int Status;
	}

	public QueryDisconnectFromGameAnnounce2x2(string UserID, string AuthKey, string ViewerID, long GameID)
	{
		base.Type = "DisconnectFromG2x2ameAnnounce";
		base.UserID = UserID;
		base.ViewerID = ViewerID;
		base.AuthKey = AuthKey;
		Args.Add(new 
		{
			GUID = GameID
		});
	}
}
