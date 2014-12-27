using UnityEngine;
using System.Collections;

public class QueryCreateClub : Query
{
	public class Request
	{
		public int Status;
	}
	
	public QueryCreateClub(string UserID, string AuthKey, string ClubName)
	{
		base.Type = "createClub";
		base.UserID = UserID;
		base.ViewerID = UserID;
		base.AuthKey = AuthKey;
		Args.Add(new {
			ClubName = ClubName,
			CreatorID = UserID
		});
	}
}
