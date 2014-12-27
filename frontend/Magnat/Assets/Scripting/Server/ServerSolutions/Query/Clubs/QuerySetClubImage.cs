using UnityEngine;
using System.Collections;
using Newtonsoft.Json;

public class QuerySetClubImage:Query
{
	public class Request
	{
		public int Status;
	}

	public QuerySetClubImage(string ViewerID, string AuthKey, string ClubID, string ImageName)
	{
		base.Type = "setClubIcon";
		base.UserID = ViewerID;
		base.ViewerID = ViewerID;
		base.AuthKey = AuthKey;
		Args.Add(new {
			ClubID = ClubID,
			ImageName = ImageName
		});
	}
}
