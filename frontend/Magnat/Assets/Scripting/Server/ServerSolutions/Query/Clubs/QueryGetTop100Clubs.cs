public class QueryGetTop100Clubs : Query
{
	public QueryGetTop100Clubs(string ViewerID, string AuthKey)
	{
		base.Type = "getTop100Club";
		base.UserID = ViewerID;
		base.ViewerID = ViewerID;
		base.AuthKey = AuthKey;
	}
}
