public class QueryGetWeekTop10 : Query
{
	public QueryGetWeekTop10(string UserID, string AuthKey, string ViewerID)
	{
		base.Type = "GetWeekTop10";
		base.UserID = UserID;
		base.ViewerID = ViewerID;
		base.AuthKey = AuthKey;
	}
}
