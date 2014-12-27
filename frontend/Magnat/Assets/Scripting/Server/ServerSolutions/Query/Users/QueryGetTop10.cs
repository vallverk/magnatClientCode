public class QueryGetTop10 : Query
{
	public QueryGetTop10(string UserID, string AuthKey, string ViewerID)
	{
		base.Type = "GetTop10";
		base.UserID = UserID;
		base.ViewerID = ViewerID;
		base.AuthKey = AuthKey;
	}
}
