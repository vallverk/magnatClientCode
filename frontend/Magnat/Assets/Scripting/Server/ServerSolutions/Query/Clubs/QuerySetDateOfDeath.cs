using UnityEngine;
using System.Collections;
using Newtonsoft.Json;

public class QuerySetDateOfDeath : Query
{
	public class Request
	{
		public int Status;
	}

	public QuerySetDateOfDeath(string ViewerID, string AuthKey, string ClubID, long UTSTime)
	{
		base.Type = "setDateOfDeath";
		base.UserID = ViewerID;
		base.ViewerID = ViewerID;
		base.AuthKey = AuthKey;
		Args.Add(new {
			ClubID = ClubID,
			DateOfDeath = UTSTime
		});
	}
}
