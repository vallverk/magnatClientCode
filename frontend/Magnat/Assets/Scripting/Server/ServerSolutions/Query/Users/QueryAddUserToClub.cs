public class QueryAddUserToClub : Query
{
	public class Request
	{
		public int Status;
	}

	public QueryAddUserToClub(string ViewerID, string AuthKey, string ClubID)
	{
		base.Type = "addUserToClub";
		base.UserID = ViewerID;
		base.ViewerID = ViewerID;
		base.AuthKey = AuthKey;
		Args.Add(new 
		         { 
			ClubId = ClubID
		});
	}
}
