public class QueryGetWeekTop100 : Query
{
	public QueryGetWeekTop100(string UserID, string AuthKey, string ViewerID)
	{
		base.Type = "GetWeekTop100";
		base.UserID = UserID;
		base.ViewerID = ViewerID;
		base.AuthKey = AuthKey;
	}
}
