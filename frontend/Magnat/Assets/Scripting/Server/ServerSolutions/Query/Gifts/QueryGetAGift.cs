public class QueryGetAGift : Query
{
	public QueryGetAGift(string ViewerID, string AuthKey, string GiftID, string UserID, string Description)
	{
		base.Type = "giveAgift";
		base.UserID = ViewerID;
		base.ViewerID = ViewerID;
		base.AuthKey = AuthKey;
		Args.Add(new {
			GiftID = GiftID,
			UserID = UserID,
			Description	= Description
		});
	}
}
