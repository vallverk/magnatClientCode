public class QuerySendMessage : Query
{
	public QuerySendMessage(string ViewerID, string AuthKey, string ReceiverID, string Text, long TimeStamp)
	{
		base.Type = "sendMessage";
		base.UserID = ViewerID;
		base.ViewerID = ViewerID;
		base.AuthKey = AuthKey;
		Args.Add(new {
			Receiver = ReceiverID,
			Status = 0,
			Text = Text,
			Time = TimeStamp.ToString()
		});
	}
}
