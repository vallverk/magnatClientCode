public class QueryAddClubEvent : Query
{
	public class Request 
	{
		public int Status;
	}

	public QueryAddClubEvent(string ViewerID, string AuthKey, string ClubID, string Description, long DescTime)
	{
		base.Type = "setClubEvent";
		base.UserID = ViewerID;
		base.ViewerID = ViewerID;
		base.AuthKey = AuthKey;
		Args.Add(new {
			Description = Description,
			Time = DescTime,
			ClubID = ClubID
		});
	}
}
