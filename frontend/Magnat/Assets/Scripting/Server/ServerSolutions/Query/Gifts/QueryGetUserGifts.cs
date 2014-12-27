public class QueryGetUserGifts : Query
{
	public QueryGetUserGifts(string ViewerID, string AuthKey, string UserID)
	{
		base.Type = "getUserGifts";
		base.UserID = ViewerID;
		base.ViewerID = ViewerID;
		base.AuthKey = AuthKey;
		Args.Add(new {
			UserID = UserID
		});
	}
}
