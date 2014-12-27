using System;

public enum GameType
{
	Standart = 0,
	TwoVSTwo = 1
}

public class QueryCreateGameAnnounce : Query
{
	public class Request
	{
		public int Status;
	}
	
	public QueryCreateGameAnnounce(string UserID, string AuthKey, string ViewerID, string GameName, GameType GameType, int PlayersCount, int Bet, string Password)
	{
		base.Type = "CreateGameAnnounce";
		base.UserID = UserID;
		base.ViewerID = ViewerID;
		base.AuthKey = AuthKey;
		string pass = string.IsNullOrEmpty(Password)?"-":MD5Convertor.getMd5Hash(Password);
		int gt = (int)GameType;
		Args.Add(new 
		{
			GameName = GameName,
			GameType = gt,
			PlayersCount = PlayersCount,
			Bet = Bet,
			Password = pass
		});
	}
}
