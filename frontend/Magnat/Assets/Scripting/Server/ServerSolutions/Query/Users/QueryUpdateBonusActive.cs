public class QueryUpdateBonusActive : Query
{
	public QueryUpdateBonusActive(string ViewerID, string AuthKey)
	{
		base.Type = "updateUserActive";
		base.UserID = ViewerID;
		base.ViewerID = ViewerID;
		base.AuthKey = AuthKey;
	}
}
