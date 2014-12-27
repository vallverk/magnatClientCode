public class QueryGetMessagesIDList : Query
{
	public QueryGetMessagesIDList(string ViewerID, string AuthKey)
	{
		base.Type = "getMessagesIDList";
		base.UserID = ViewerID;
		base.ViewerID = ViewerID;
		base.AuthKey = AuthKey;
		Args.Add(new {
			Receiver = ViewerID
		});
	}
}
