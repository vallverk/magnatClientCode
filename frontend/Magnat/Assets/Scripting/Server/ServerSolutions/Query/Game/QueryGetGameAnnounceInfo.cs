using System;

public class QueryGetGameAnnounceInfo : Query
{
	public class Request : GameInfo { }
	
	public QueryGetGameAnnounceInfo(string UserID, string AuthKey, string ViewerID, long GUID)
	{
		base.Type = "GetGameAnnounceInfo";
		base.UserID = UserID;
		base.ViewerID = ViewerID;
		base.AuthKey = AuthKey;
		Args.Add(new 
		{
			GUID = GUID
		});
	}
}
