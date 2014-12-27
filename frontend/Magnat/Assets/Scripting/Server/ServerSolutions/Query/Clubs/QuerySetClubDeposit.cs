public class QuerySetClubDeposit : Query
{
	public class Request : ClubInfo { }

	public QuerySetClubDeposit(string ViewerID, string AuthKey, ClubInfo Club, ServerUserInfo User, int Deposit)
	{
		base.Type = "setClubDeposit";
		base.UserID = ViewerID;
		base.ViewerID = ViewerID;
		base.AuthKey = AuthKey;
		Args.Add(new {
			UserDeposit = Deposit,
			ClubGold = Club.Gold,
			ClubID = Club.ID,
			UserGold = User.Gold,
			Deposit = User.Deposit
		});
	}
}
