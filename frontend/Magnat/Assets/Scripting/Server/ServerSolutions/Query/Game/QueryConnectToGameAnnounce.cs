using System;

public class QueryConnectToGameAnnounce : Query
{
	public class Request
	{
		public int Status;
	}

	public QueryConnectToGameAnnounce(string UserID, string AuthKey, string ViewerID, long GameID, string Password)
	{
		base.Type = "ConnectToGameAnnounce";
		base.UserID = UserID;
		base.ViewerID = ViewerID;
		base.AuthKey = AuthKey;
		Args.Add(new 
		{
			GUID = GameID,
			Password = string.IsNullOrEmpty(Password)?"-":MD5Convertor.getMd5Hash(Password)
		});
	}
}
