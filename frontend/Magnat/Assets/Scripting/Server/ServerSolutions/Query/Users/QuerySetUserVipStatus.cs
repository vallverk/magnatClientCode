public class QuerySetUserVipStatus : Query
{
	public class Request
	{
		public int Status;
	}

	public QuerySetUserVipStatus(string ViewerID, string AuthKey, int Term)
	{
		base.Type = "seUserVipStatus";
		base.UserID = ViewerID;
		base.ViewerID = ViewerID;
		base.AuthKey = AuthKey;
		Args.Add(new 
		         { 
			Term = Term
		});
	}
}
