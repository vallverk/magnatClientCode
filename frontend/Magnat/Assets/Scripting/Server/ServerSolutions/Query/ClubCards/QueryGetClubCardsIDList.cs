public class QueryGetClubCardsIDList : Query
{
	public QueryGetClubCardsIDList(string ViewerID, string AuthKey)
	{
		base.Type = "getClubCardIdList";
		base.UserID = ViewerID;
		base.ViewerID = ViewerID;
		base.AuthKey = AuthKey;
	}
}
