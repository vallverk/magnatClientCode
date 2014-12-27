using UnityEngine;
using System.Collections;
using Newtonsoft.Json;

public class QuerySetClubName : Query
{
	public class Request
	{
		public int Status;
	}

	public QuerySetClubName(string ViewerID, string AuthKey, string ClubID, string ClubName)
	{
		base.Type = "setClubName";
		base.UserID = ViewerID;
		base.ViewerID = ViewerID;
		base.AuthKey = AuthKey;
		Args.Add(new {
			ClubID = ClubID,
			ClubName = ClubName
		});
	}
}
