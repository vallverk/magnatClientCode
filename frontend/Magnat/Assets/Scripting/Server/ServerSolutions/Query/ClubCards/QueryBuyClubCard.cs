public class QueryBuyClubCard : Query
{
	public QueryBuyClubCard(string ViewerID, string AuthKey, string clubCardID, string ClubID)
	{
		base.Type = "buyClubCard";
		base.UserID = ViewerID;
		base.ViewerID = ViewerID;
		base.AuthKey = AuthKey;
		Args.Add(new {
			clubCardID = clubCardID,
			ClubID = ClubID
		});
	}
}
