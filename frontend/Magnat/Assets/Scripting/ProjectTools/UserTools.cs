using UnityEngine;
using System.Collections;

public class UserTools
{
	public static string GetTitleByCapital(long Cash, PlayerStatusPair[] Ranks)
	{
		int rankID=-1;
		for (int i=Ranks.Length-1;i>=0;i--)
		{
			if (Cash>Ranks[i].MinCash)
			{
				rankID = i;
				break;
			}
		}
		if (rankID==-1) rankID = 0;
		return Ranks[rankID].StatusName;
	}

	public static float GetLevelProgressByCapital(long Cash, PlayerStatusPair[] Ranks)
	{
		int rankID=-1;
		for (int i=Ranks.Length-1;i>=0;i--)
		{
			if (Cash>Ranks[i].MinCash)
			{
				rankID = i;
				break;
			}
		}
		if (rankID==-1) rankID = 0;

		if (rankID < Ranks.Length-1)
			return (((Cash-Ranks[rankID].MinCash)*1.0f)/(Ranks[rankID+1].MinCash-Ranks[rankID].MinCash)*1.0f);
		else
			return 1;
	}

	public static void UpdateStatuses(ServerUserInfo[] Users, System.Action<ServerUserInfo[]> Callback)
	{
		ServerInfo.Instance.GetStatuses((stats)=>{
			for (int i=0;i<Users.Length;i++)
				if (string.IsNullOrEmpty(Users[i].Title))
					Users[i].Title = GetTitleByCapital(Users[i].Capital,stats);
			Callback(Users);
		});
	}
}
