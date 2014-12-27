using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class AuctionInfo
{
	[SerializeField] public string IniterID;
	[SerializeField] public int FieldID;
	[SerializeField] public int CurrentPrice;
	[SerializeField] public int StepPrice;
	[SerializeField] public List<string> Bidders;
	[SerializeField] public string CurrentPlayer;
	[SerializeField] public bool FirstStep;
}

public class AuctionController : MonoBehaviour 
{
	public GameManager GManager;
	private PlayersManager GameFieldsManager { get { return GManager.GameFieldsManager; } }
	GameModeManager GMode;

	private AuctionInfo info;

	void Start()
	{
		GMode = GManager.GetComponent<GameModeManager>();
		GMode.OnAuctionAccepted += OnAcceptAuction;
		GMode.OnAuctionRefused += OnRefuseAuction;
	}

	// инициализируем диалог
	public void InitAuction()
	{
		Player cp = GManager.currentPlayer;
		FieldData data = GameFieldsManager.GetFieldDataAtPlayer(cp.OwnerID);
		info = new AuctionInfo();
		info.StepPrice = 50000;
		info.CurrentPrice = data.BuyPrice + info.StepPrice;
		// соберем список людей, которые могут участвовать в аукционе
		info.Bidders = new List<string>();
		string players = "";
		foreach (var p in GManager.Players)
			if (p.Cash>=info.CurrentPrice && p != cp)
				info.Bidders.Add(p.SocialID);
		if (info.Bidders.Count == 0)
		{
			// в аукционе никто не может принять участие
			GManager.LogToSystemChat("AuctionFinish_NoPlayers");
			GManager.EndStep(false);
			GManager.LogToMainChat("Аукцион не состоялся.");
			return;
		}
		info.CurrentPlayer = info.Bidders[0];
		info.IniterID = cp.SocialID;
		info.FieldID = GameFieldsManager.GetPlayerCurrentCardID(cp.OwnerID);
		info.FirstStep = true;
		GManager.LogToSystemChat(string.Format("Auction_{0}",JSONSerializer.Serialize(info)));
		GManager.TimerPause = true;
	}

	public void MakeAuctionIteration(AuctionInfo Info)
	{
		info = Info;
		if (info.Bidders.Count == 1 && !info.FirstStep)
		{
			GManager.EndStep(false);
			// есть победитель
			GManager.TimerPause = false;
			Player targetP = GManager.GetPlayerBySocialID(info.IniterID);
			Player newOwner = GManager.GetPlayerBySocialID(info.Bidders[0]);
			GameField field = GameFieldsManager.Manager.GamePoolSteps[info.FieldID];
			FieldData data = GameFieldsManager.Manager.GetFieldData(field);
			field.Owner = newOwner.OwnerID;
			newOwner.Cash -= info.CurrentPrice;
			newOwner.Capital += data.BuyPrice;
			GManager.UpdateUserData(newOwner,false);
			GManager.LogToMainChat("{0} приобрел на аукционе фирму [FFFFFF]"+data.FieldName+"[-]" +
			                       "по цене [ffffff]"+info.CurrentPrice.ToString("### ### ##0$")+"[-]",newOwner);
			// обновим данные поля
			AliasData alias = GameFieldsManager.Manager.GetAliasFromField(field);
			GameField[] af = GameFieldsManager.Manager.GetFieldInAlias(alias);
			// но, для начала, обработаем случай, если пользователь приобрел город
			if (GameFieldsManager.Manager.IsCity(alias))
			{
				int ownerOf = 0;
				foreach (var f in af)
					if (f.Owner == newOwner.OwnerID)
						ownerOf++;
				// проставим им правильно ранг и цены
				foreach (var f in af)
					if (f.Owner == newOwner.OwnerID)
				{
					f.CurrentMonopolyRank = MonopolyRank.Monopoly+ownerOf;
					f.Price = GameFieldsManager.Manager.GetFieldData(f).GetCostByRank(f.CurrentMonopolyRank);
				}
			}
			else
			{
				bool oneOwner = true;
				for (int i=1;i<af.Length;i++)
					if (af[0].Owner != af[i].Owner)
				{
					oneOwner = false;
					break;
				}
				if (oneOwner)
				{
					foreach (var f in af)
					{
						f.CurrentMonopolyRank = MonopolyRank.Monopoly;
						f.Price = GameFieldsManager.Manager.GetFieldData(f).GetCostByRank(f.CurrentMonopolyRank);
					}
				}
			}
		} else if (info.Bidders.Count == 0) // знач аукцион отменяется...
		{
			GManager.LogToMainChat("Аукцион не состоялся.");
			GManager.EndStep(false);
			GManager.TimerPause = false;
		} else
		// а если нет победителя - продолжим аукцион...
		{
			GManager.TimerPause = true;
			// только в том случае, если наш ход
			if (info.CurrentPlayer == SocialManager.Instance.ViewerID)
			{
				// если денег хватает - участвуем, если нет - отказываемся
				if (GManager.GetPlayerBySocialID(SocialManager.Instance.ViewerID).Cash >= info.CurrentPrice)
				{
					Player targetP = GManager.GetPlayerBySocialID(info.IniterID);
					GameField field = GameFieldsManager.Manager.GamePoolSteps[info.FieldID];
					GMode.ShowAuctionDialog(targetP,GameFieldsManager.Manager.GetFieldData(field),Info.CurrentPrice,Info.StepPrice);
				}
				else OnRefuseAuction();
			}
		}
	}

	private void OnAcceptAuction()
	{
		if (info.Bidders.Count != 0 && info.Bidders[info.Bidders.Count-1] == info.CurrentPlayer)
		{
			if (info.Bidders.Count>1)
				info.CurrentPrice += info.StepPrice;
			info.FirstStep = false;
			info.CurrentPlayer = info.Bidders[0];
		} 
		else
			if (info.Bidders.Count>1)
				info.CurrentPlayer = info.Bidders[info.Bidders.IndexOf(info.CurrentPlayer)+1];
		GManager.LogToSystemChat(string.Format("Auction_{0}",JSONSerializer.Serialize(info)));
		GMode.HideAuctionDialog();
	}

	private void OnRefuseAuction()
	{
		if (info.Bidders.Count == 1)
		{
			GManager.LogToSystemChat("AuctionFinish_NoPlayers");
			GManager.EndStep(false);
			GMode.HideAuctionDialog();
			return;
		}

		int idPos = info.Bidders.IndexOf(SocialManager.Instance.ViewerID);
		idPos++;
		if (idPos>=info.Bidders.Count) 
		{
			info.CurrentPrice += info.StepPrice;
			info.FirstStep = false;
			idPos = 0;
		}
		info.CurrentPlayer = info.Bidders[idPos];
		info.Bidders.Remove(SocialManager.Instance.ViewerID);
		GManager.LogToSystemChat(string.Format("Auction_{0}",JSONSerializer.Serialize(info)));

		GMode.HideAuctionDialog();
	}
}

















































