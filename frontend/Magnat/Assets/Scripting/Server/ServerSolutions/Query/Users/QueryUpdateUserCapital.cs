public class QueryUpdateUserCapital : Query
{
	public QueryUpdateUserCapital(string ViewerID, string AuthKey, long Capital)
	{
		base.Type = "addUserCapitals";
		base.UserID = ViewerID;
		base.ViewerID = ViewerID;
		base.AuthKey = AuthKey;
		Args.Add(new {
			Capital = Capital
		});
	}
}
