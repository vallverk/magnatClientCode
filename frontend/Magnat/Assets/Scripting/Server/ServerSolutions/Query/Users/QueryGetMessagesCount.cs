public class QueryGetMessagesCount : Query
{
	public class Request
	{
		public int count;
	}

	public QueryGetMessagesCount(string ViewerID, string AuthKey)
	{
		base.Type = "getMessageCount";
		base.UserID = ViewerID;
		base.ViewerID = ViewerID;
		base.AuthKey = AuthKey;
		Args.Add(new {
			UserID = ViewerID
		});
	}
}
