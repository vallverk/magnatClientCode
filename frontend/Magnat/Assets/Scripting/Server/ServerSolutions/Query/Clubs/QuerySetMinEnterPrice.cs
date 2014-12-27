using UnityEngine;
using System.Collections;
using Newtonsoft.Json;

public class QuerySetMinEnterPrice:Query
{
	public class Request
	{
		public int Status;
	}

	public QuerySetMinEnterPrice(string ViewerID, string AuthKey, string ClubID, int MinEnterPrice)
	{
		base.Type = "setMinEnterPrice";
		base.UserID = ViewerID;
		base.ViewerID = ViewerID;
		base.AuthKey = AuthKey;
		Args.Add(new {
			ClubID = ClubID,
			MinEnterPrice = MinEnterPrice
		});
	}
}
