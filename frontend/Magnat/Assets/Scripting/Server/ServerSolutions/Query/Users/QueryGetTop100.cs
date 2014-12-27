public class QueryGetTop100 : Query
{
	public QueryGetTop100(string UserID, string AuthKey, string ViewerID)
	{
		base.Type = "GetTop100";
		base.UserID = UserID;
		base.ViewerID = ViewerID;
		base.AuthKey = AuthKey;
	}
}
