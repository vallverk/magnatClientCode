public class QueryGetClubCard : Query
{
	public QueryGetClubCard(string ViewerID, string AuthKey, string clubCardID)
	{
		base.Type = "getClubCard";
		base.UserID = ViewerID;
		base.ViewerID = ViewerID;
		base.AuthKey = AuthKey;
		Args.Add(new {
			clubCardID = clubCardID
		});
	}
}
