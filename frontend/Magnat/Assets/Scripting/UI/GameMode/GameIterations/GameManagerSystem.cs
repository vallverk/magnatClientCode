using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public partial class GameManager : MonoBehaviour 
{
	public void LogToMainChat(string Message)
	{
		ChatManager.AddMessage(0,Message);
	}
	
	public void Write(string Message)
	{
		//ChatManager.AddMessage(0,Message);
	}

	// сатистика по полю
	private StartedGameData GetCurrentGameData()
	{
		StartedGameData data = new StartedGameData();
		data.GI = ServerDataManager.GetGameInfo();
		data.T = TimeTools.GetUTCTimeStamp();
		data.STT = startTime;
		for (int i=0;i<players.Length;i++)
			players[i].TablePosition = GameFieldsManager.GetPosition(i);
		data.Ps = players;
		data.CID = currentPlayerID;
		data.TST = timerTime;
		data.F = new StartedGameFieldData[GameFieldsManager.Manager.Fields.Length];
		for (int i=0;i<GameFieldsManager.Manager.Fields.Length;i++)
		{
			GameField gf = GameFieldsManager.Manager.Fields[i].FieldGO.GetComponent<GameField>();
			data.F[i].O = (int)gf.Owner;
			data.F[i].R = (int)gf.CurrentMonopolyRank;
			data.F[i].L = gf.Locked?1:0;
		}
		return data;
	}

	// обновить инфу по полю...
	private bool SetCurrentGameData(StartedGameData data)
	{
		// если ID не совпадают, то кто-то пытается наиметь судьбу, отсеем
		if (data.GI.GUID != ServerDataManager.GetGameInfo().GUID)
			return false;
		// проверим целостность юзеров
		if (data.Ps.Length != players.Length)
			return false;
		for (int i=0;i<data.Ps.Length;i++)
			if (data.Ps[i].SocialID != players[i].SocialID)
				return false;
		
		double ctime = TimeTools.GetUTCTimeStamp();
		timerTime = data.TST;
		currentPlayerID = data.CID;
		// сложим словарь цветов - по нему будем понимать какой локальный цвет у удаленного юзера
		Dictionary<GameField.Owners,GameField.Owners> ownersTranslater = new Dictionary<GameField.Owners, GameField.Owners>();
		for (int i=0;i<data.Ps.Length;i++)
			ownersTranslater.Add((GameField.Owners)data.Ps[i].OwnerID,players[i].OwnerID);
		ownersTranslater.Add(GameField.Owners.None, GameField.Owners.None);
		// прокачаем юзеров
		for (int i=0;i<data.Ps.Length;i++)
		{
			if (!players[i].Bankrout && data.Ps[i].Bankrout)
				SetBankrot(data.Ps[i].SocialID);
			players[i].Bankrout = data.Ps[i].Bankrout;
			players[i].Capital = data.Ps[i].Capital;
			players[i].Cash = data.Ps[i].Cash;
			UpdateUserData(players[i],false);
			players[i].TablePosition = data.Ps[i].TablePosition;
			GameFieldsManager.SetPosition(i,players[i].TablePosition);
		}
		// обновим игровые фишки
		for (int i=0;i<data.F.Length;i++)
		{
			GameField gf = GameFieldsManager.Manager.Fields[i].FieldGO.GetComponent<GameField>();
			gf.Owner =  ownersTranslater[(GameField.Owners)data.F[i].O];
			gf.CurrentMonopolyRank = (MonopolyRank)data.F[i].R;
			FieldData fdata =  GameFieldsManager.Manager.GetFieldData(gf);
			if (gf.CurrentMonopolyRank == MonopolyRank.Base && gf.Owner == GameField.Owners.None)
				gf.Price = fdata.BuyPrice;
			else
				gf.Price = fdata.GetCostByRank(gf.CurrentMonopolyRank);
			gf.Locked = data.F[i].L == 1;
		}
		MakeStepIteration(-1);
		return true;
	}

	public void UpdateUserData(Player P, bool SendSystem)
	{
		if (SendSystem)
		{
			P.TablePosition = GameFieldsManager.GetPosition(GetPlayerIDBySocialID(P.SocialID));
			LogToSystemChat("UpdateUser_"+WWW.EscapeURL(JSONSerializer.Serialize(P)));
		}
		PlayersGrid.UpdateUserData(P.OwnerID,P.Cash,P.Capital);
	}

	public Player GetPlayerByOwnerID(GameField.Owners Owner)
	{
		foreach (var p in players)
			if (p.OwnerID == Owner) return p;
		return null;
	}
	
	void SetActivePlayer(string SocID)
	{
		int p = GetPlayerIDBySocialID(SocID);
		currentPlayerID = p;
		PlayersGrid.SetActive(currentPlayer.OwnerID);
	}
	
	public Player GetPlayerBySocialID(string SocID)
	{
		foreach (var p in players)
			if (p.SocialID == SocID)
				return p;
		return null;
	}
	
	public int GetPlayerIDBySocialID(string SocID)
	{
		for (int i=0;i<players.Length;i++)
			if (players[i].SocialID == SocID)
				return i;
		return -1;
	}
	
	private Player[] GetPlayerList()
	{
		string[] uids = ServerDataManager.GetUserGameUsers();
		Player[] res = new Player[uids.Length];
		for (int i =0;i<res.Length;i++)
		{
			res[i] = GetDefaultPlayer();
			res[i].SocialID = uids[i];
			SocialManager.GetUserInfo(uids[i]);
		}
		return res;
	}
}
