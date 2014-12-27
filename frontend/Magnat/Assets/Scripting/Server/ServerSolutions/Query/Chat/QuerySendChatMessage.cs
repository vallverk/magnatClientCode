public class QuerySendChatMessage : Query
{
	public QuerySendChatMessage(string UserID, string AuthKey, string ViewerID, string RoomName, string Message)
	{
		base.Type = "send.chat.message";
		base.UserID = UserID;
		base.ViewerID = ViewerID;
		base.AuthKey = AuthKey;
		Args = new System.Collections.Generic.List<object>() 
		{ new 
			{
				RoomName = RoomName, Message = Message
			} 
		};
	}
}
