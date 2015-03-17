using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ServerData : MonoBehaviour 
{
	public void FinishGame(string WinnerSocialID, int WinCapital, string[] ActivePlayers)
	{
		ServerInfo.Instance.FinishGame(GetUserGameID(),WinnerSocialID,WinCapital,ActivePlayers,(a)=>{});
		ServerInfo.Instance.DisconnectImmediatly();
	}

	public void FinishGame2x2(string[] WinnerSocialID, int WinCapital, string[] ActivePlayers)
	{
		ServerInfo.Instance.FinishGame2x2(GetUserGameID(),WinnerSocialID,WinCapital,ActivePlayers,(a)=>{});
		ServerInfo.Instance.DisconnectImmediatly();
	}

	/*
	 * array (
  '_id' => 
  MongoId::__set_state(array(
     '$id' => '5462644145540b1a4ebc168e',
  )),
  'GUID' => 1415734337820,
  'GameName' => 'Александра Дяглева',
  'GameType' => 0,
  'PlayersCount' => 2,
  'Bet' => 0,
  'Password' => '-',
  'UserList' => 
  array (
    0 => '42048261',
    1 => '15361226',
  ),
  'Status' => 1,
)*/

#if UNITY_EDITOR
	private GameInfo GetDebugGame()
	{
		return new GameInfo()
		{ 
			GUID = 1416689059012,
			GameName = "Радомир Слабошпицкий",
			GameType = 0,
			PlayersCount = 3,
			Bet = 0,
			Password = "-",
			Status = 1,
			UserList = new System.Collections.Generic.List<string>()
			{
				"15361226",
				"42048261"
			}
		};
	}
#endif

	public long GetUserGameID()
	{
		return GetGameInfo().GUID;
	}

	public static void AddGameToBlackList(string GameID)
	{
		string bl = PlayerPrefs.GetString("GamesBlackList");
		if (string.IsNullOrEmpty(bl))
			bl = GameID;
		else
			bl += "|"+GameID;
		PlayerPrefs.SetString("GamesBlackList",bl);
	}

	public static bool IsGameAtBlackList(string GameID)
	{
		string bl = PlayerPrefs.GetString("GamesBlackList");
		if (string.IsNullOrEmpty(bl)) return false;
		List<string> list = new List<string>(bl.Split('|'));
		return list.Contains(GameID);
	}

	public GameInfo GetGameInfo()
	{
	    return JSONSerializer.Deserialize<GameInfo>(ServerGameList.GameInfo);

#if UNITY_EDITOR
		return GetDebugGame();
#else
		return JSONSerializer.Deserialize<GameInfo>(PlayerPrefs.GetString("LoadGame"));
#endif
	}

	public string[] GetUserGameUsers()
	{
		return GetGameInfo().UserList.ToArray();
	}
}
