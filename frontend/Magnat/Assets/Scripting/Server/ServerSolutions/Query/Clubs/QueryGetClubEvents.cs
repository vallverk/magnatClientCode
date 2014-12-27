public class ClubEvent
{
	public string Description;
	public long Time;
}

public class QueryGetClubEvents : Query
{
	public QueryGetClubEvents(string ViewerID, string AuthKey, string ClubID)
	{
		base.Type = "getClubEvents";
		base.UserID = ViewerID;
		base.ViewerID = ViewerID;
		base.AuthKey = AuthKey;
		Args.Add(new {
			ClubID = ClubID
		});
	}
}
