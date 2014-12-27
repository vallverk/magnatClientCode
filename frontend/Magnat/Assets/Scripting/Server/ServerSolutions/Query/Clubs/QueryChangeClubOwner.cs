using UnityEngine;
using System.Collections;
using Newtonsoft.Json;

public class QueryChangeClubOwner : Query
{
	public class Request
	{
		public int Status;
	}

	public QueryChangeClubOwner(string ViewerID, string AuthKey, string NewOwnerID)
	{
		base.Type = "changeClubOwner";
		base.UserID = ViewerID;
		base.ViewerID = ViewerID;
		base.AuthKey = AuthKey;
		Args.Add(new {
			UserID = NewOwnerID
		});
	}
}
