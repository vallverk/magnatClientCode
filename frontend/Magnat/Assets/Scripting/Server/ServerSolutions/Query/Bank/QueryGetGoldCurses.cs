public class QueryGetGoldCurses : Query
{
	public QueryGetGoldCurses(string ViewerID, string AuthKey)
	{
		base.Type = "getGoldCurses";
		base.UserID = ViewerID;
		base.ViewerID = ViewerID;
		base.AuthKey = AuthKey;
	}
}
