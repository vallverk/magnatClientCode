using System;

public class QueryGetGameAnnounceList : Query
{
	public class Request:GameInfo {}
	
	public QueryGetGameAnnounceList(string UserID, string AuthKey, string ViewerID)
	{
		base.Type = "GetGameAnnounceList";
		base.UserID = UserID;
		base.ViewerID = ViewerID;
		base.AuthKey = AuthKey;
	}
}

