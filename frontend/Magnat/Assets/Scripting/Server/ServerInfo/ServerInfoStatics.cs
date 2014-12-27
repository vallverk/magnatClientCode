using System;

public partial class ServerInfo : Singleton<ServerInfo> 
{
	public class GoldStatus
	{
		public string kg1;
		public string kg5;
		public string kg10;
		public string kg20;
		public string kg50;
        public string term;
        public string avatar;
        public string name;
	}

    private GoldStatus goldBuffer = null;
	public void GetGoldCurses(Action<GoldStatus> Callback)
	{
        if (goldBuffer != null)
        {
            Callback(goldBuffer);
            return;
        }

		Query q = new QueryGetGoldCurses(viewerID,auth);
		Pool.SendPostRequestAsync(q,(r)=>{
            goldBuffer = JSONSerializer.Deserialize<GoldStatus>(r.Args[0].ToString());
			Callback(goldBuffer);
		});
	}

	public void GetStatuses(Action<PlayerStatusPair[]> Callback)
	{
		Callback(new PlayerStatusPair[10]{ 
			new PlayerStatusPair() { StatusName="менеджер", MinCash=0 },
			new PlayerStatusPair() { StatusName="бизнесмен", MinCash=200000000 },
			new PlayerStatusPair() { StatusName="коммерсант", MinCash=500000000 },
			new PlayerStatusPair() { StatusName="банкир", MinCash=1000000000 },
			new PlayerStatusPair() { StatusName="гранд", MinCash=2000000000 },
			new PlayerStatusPair() { StatusName="богач", MinCash=5000000000 },
			new PlayerStatusPair() { StatusName="капиталист", MinCash=10000000000 },
			new PlayerStatusPair() { StatusName="олигарх", MinCash=15000000000 },
			new PlayerStatusPair() { StatusName="монополист", MinCash=30000000000 },
			new PlayerStatusPair() { StatusName="магнат", MinCash=80000000000 }
		});
	}

	public int GetLevelByStatus(string status)
	{
		switch (status)
		{
			case "менеджер": return 0;
			case "бизнесмен": return 1;
			case "коммерсант": return 2;
			case "банкир": return 3;
			case "гранд": return 4;
			case "богач": return 5;
			case "капиталист": return 6;
			case "олигарх": return 7;
			case "монополист": return 8;
			case "магнат": return 9;
		}
		return 0;
	}

	public void GetClubStatuses(Action<ClubStatusPair[]> Callback)
	{
		Callback(new ClubStatusPair[5]{
			new ClubStatusPair() { Level = 1, Title = "Бронзовый", MaxPlayers = 10, MinCapital = 0 },
			new ClubStatusPair() { Level = 2, Title = "Серебряный", MaxPlayers = 10, MinCapital = 1000000000 },
			new ClubStatusPair() { Level = 3, Title = "Золотой", MaxPlayers = 30, MinCapital = 5000000000 },
			new ClubStatusPair() { Level = 4, Title = "Платиновый", MaxPlayers = 50, MinCapital = 10000000000 },
			new ClubStatusPair() { Level = 5, Title = "Бриллиантовый", MaxPlayers = 100, MinCapital = 30000000000 }
		});
	}
}
