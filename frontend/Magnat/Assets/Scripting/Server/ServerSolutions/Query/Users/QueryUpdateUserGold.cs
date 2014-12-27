public class QueryUpdateUserGold : Query
{
	public QueryUpdateUserGold(string ViewerID, string AuthKey, int Gold)
	{
		base.Type = "updateUserGold";
		base.UserID = ViewerID;
		base.ViewerID = ViewerID;
		base.AuthKey = AuthKey;
		Args.Add(new {
			Gold = Gold
		});
	}
}
