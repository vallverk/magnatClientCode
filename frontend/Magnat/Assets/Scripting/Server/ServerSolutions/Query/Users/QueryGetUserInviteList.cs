public class QueryGetUserInviteList : Query
{
	public QueryGetUserInviteList(string ViewerID, string AuthKey)
	{
		base.Type = "getUserInviteList";
		base.UserID = ViewerID;
		base.ViewerID = ViewerID;
		base.AuthKey = AuthKey;
	}
}
