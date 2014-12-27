using System;

public class QueryGetUserInfo : Query
{
	public class Request: ServerUserInfo {}

	public QueryGetUserInfo(string UserID, string AuthKey, string ViewerID, string UID, string UserName)
	{
		base.Type = "GetUserInfo";
		//base.Type = "";
		base.UserID = UserID;
		base.ViewerID = ViewerID;
		base.AuthKey = AuthKey;
		Args.Add(new 
		         { 
			UID = UID,
			UserName = UserName
		});
	}
}
