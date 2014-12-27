public class QueryInviteToClub : Query
{
	public class Request : ClubInfo { }

	public QueryInviteToClub(string ViewerID, string AuthKey, string TargetUserID)
	{
		base.Type = "InviteUserToClub";
		base.UserID = ViewerID;
		base.ViewerID = ViewerID;
		base.AuthKey = AuthKey;
		Args.Add(new {
			UserId = TargetUserID
		});
	}
}
