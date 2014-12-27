using UnityEngine;
using System.Collections;
using Newtonsoft.Json;

public class QuerySetGold : Query
{
	public class Request
	{
		public int Status;
	}

	public QuerySetGold(string ViewerID, string AuthKey, string ClubID, int Gold)
	{
		base.Type = "setGold";
		base.UserID = ViewerID;
		base.ViewerID = ViewerID;
		base.AuthKey = AuthKey;
		Args.Add(new {
			ClubID = ClubID,
			Gold = Gold
		});
	}
}
