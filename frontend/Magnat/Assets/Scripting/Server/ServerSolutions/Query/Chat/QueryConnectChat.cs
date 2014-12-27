using Newtonsoft.Json;

public class QueryConnectChat : Query
{
	private class R
	{
		public string RoomName {get; set;}
	}

	public QueryConnectChat(string UserID, string AuthKey, string ViewerID, string Room)
	{
		base.Type = "connect.chat";
		base.UserID = UserID;
		base.ViewerID = ViewerID;
		base.AuthKey = AuthKey;
		Args = new System.Collections.Generic.List<object>() { Room };
	}
}
