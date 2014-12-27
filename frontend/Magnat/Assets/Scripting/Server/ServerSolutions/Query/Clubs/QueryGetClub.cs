public class QueryGetClub : Query
{
	public class Request : ClubInfo { }

	public QueryGetClub(string ViewerID, string AuthKey, string ClubID)
	{
		base.Type = "getClub";
		base.UserID = ViewerID;
		base.ViewerID = ViewerID;
		base.AuthKey = AuthKey;
		Args.Add(new {
			ClubID = ClubID
		});
	}
}
