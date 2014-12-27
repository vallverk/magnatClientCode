public class QueryDeleteUserFromClub : Query
{
	public class Request
	{
		public int Status;
	}

	public QueryDeleteUserFromClub(string ViewerID, string AuthKey, string UserToDelete)
	{
		base.Type = "deleteUserFromClub";
		base.UserID = ViewerID;
		base.ViewerID = ViewerID;
		base.AuthKey = AuthKey;
		Args.Add(new {
			UserID = UserToDelete
		});
	}
}
