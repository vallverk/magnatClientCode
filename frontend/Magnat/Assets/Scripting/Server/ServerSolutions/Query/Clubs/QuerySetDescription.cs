using UnityEngine;
using System.Collections;
using Newtonsoft.Json;

public class QuerySetDescription : Query
{
	public class Request
	{
		public int Status;
	}

	public QuerySetDescription(string ViewerID, string AuthKey, string ClubID, string Description)
	{
		base.Type = "setDescription";
		base.UserID = ViewerID;
		base.ViewerID = ViewerID;
		base.AuthKey = AuthKey;
		Args.Add(new {
			ClubID = ClubID,
			Description = Description
		});
	}
}
