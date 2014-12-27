public class QueryBuyAction : Query
{
	public class Request
	{
		public int Status;
	}

	public QueryBuyAction(string ViewerID, string AuthKey, string ActionID)
	{
		base.Type = "buyShare";
		base.UserID = ViewerID;
		base.ViewerID = ViewerID;
		base.AuthKey = AuthKey;
		Args.Add(new {
			ActionID = ActionID
		});
	}
}
