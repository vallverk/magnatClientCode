public class QueryConnectChatPrivat : Query
{
	public QueryConnectChatPrivat(string UserID, string AuthKey, string ViewerID, string UserTo)
	{
		base.Type = "connect.chat.privat";
		base.UserID = UserID;
		base.ViewerID = ViewerID;
		base.AuthKey = AuthKey;
		Args = new System.Collections.Generic.List<object>() { UserTo };
	}
}
