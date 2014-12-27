public class QueryRefuseClubInvation : Query
{
	public class Request
	{
		public int Status;
	}

	public QueryRefuseClubInvation(string ViewerID, string AuthKey, string ClubID)
	{
		base.Type = "rejectInvitation";
		base.UserID = ViewerID;
		base.ViewerID = ViewerID;
		base.AuthKey = AuthKey;
		Args.Add(new 
		         { 
			ClubId = ClubID
		});
	}
}
