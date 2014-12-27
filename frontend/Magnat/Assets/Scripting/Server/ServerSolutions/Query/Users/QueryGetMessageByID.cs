public class QueryGetMessageByID : Query
{
	public class Request : UserMessage { } 

	public QueryGetMessageByID(string ViewerID, string AuthKey, long MessageID)
	{
		base.Type = "getMessage";
		base.UserID = ViewerID;
		base.ViewerID = ViewerID;
		base.AuthKey = AuthKey;
		Args.Add(new {
			Receiver = ViewerID,
			Time = MessageID
		});
	}
}
