public class QueryGetActionIdList : Query
{
	public QueryGetActionIdList(string ViewerID, string AuthKey)
	{
		base.Type = "getActionIdList";
		base.UserID = ViewerID;
		base.ViewerID = ViewerID;
		base.AuthKey = AuthKey;
	}
}
