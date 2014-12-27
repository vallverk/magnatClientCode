public class QueryExitClub : Query
{
	public class Request
	{
		public int Status;
	}

	public QueryExitClub(string ViewerID, string AuthKey)
	{
		base.Type = "leaveClub";
		base.UserID = ViewerID;
		base.ViewerID = ViewerID;
		base.AuthKey = AuthKey;
	}
}
