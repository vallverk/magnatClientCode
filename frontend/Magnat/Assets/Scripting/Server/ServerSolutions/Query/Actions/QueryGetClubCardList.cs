public class QueryGetClubCardList : Query
{
	public QueryGetClubCardList(string ViewerID, string AuthKey, string ClubID)
	{
		base.Type = "getClubCardList";
		base.UserID = ViewerID;
		base.ViewerID = ViewerID;
		base.AuthKey = AuthKey;
		Args.Add(new {
			ClubID = ClubID
		});
	}
}
