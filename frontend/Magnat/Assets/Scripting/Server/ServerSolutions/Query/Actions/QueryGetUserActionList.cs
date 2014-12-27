public class QueryGetUserActionList : Query
{
	public class Request
	{
		public int Status;
	}

	public QueryGetUserActionList(string ViewerID, string AuthKey, string UserID)
	{
		base.Type = "getUserActionList";
		base.UserID = ViewerID;
		base.ViewerID = ViewerID;
		base.AuthKey = AuthKey;
		Args.Add(new {
			UserID = UserID
		});
	}
}
