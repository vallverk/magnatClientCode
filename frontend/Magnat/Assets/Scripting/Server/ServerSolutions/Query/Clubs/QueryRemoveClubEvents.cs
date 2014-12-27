public class QueryRemoveClubEvents : Query
{
	public QueryRemoveClubEvents(string ViewerID, string AuthKey,string ClubID)
	{
		base.Type = "deleteClubEvents";
		base.UserID = ViewerID;
		base.ViewerID = ViewerID;
		base.AuthKey = AuthKey;
		base.Args.Add(new {
			ClubID = ClubID
		});
	}
}
