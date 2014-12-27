public class QueryGetGiftList : Query
{
	public QueryGetGiftList(string ViewerID, string AuthKey)
	{
		base.Type = "getGiftList";
		base.UserID = ViewerID;
		base.ViewerID = ViewerID;
		base.AuthKey = AuthKey;
	}
}
