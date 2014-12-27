public class QueryGetActionByID : Query
{
	public QueryGetActionByID(string ViewerID, string AuthKey, string ActionID)
	{
		base.Type = "getAction";
		base.UserID = ViewerID;
		base.ViewerID = ViewerID;
		base.AuthKey = AuthKey;
		Args.Add(new {
			ActionID = ActionID
		});
	}
}
