using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public partial class GameManager : MonoBehaviour 
{
	public void MakeStep(int Len)
	{
		GameFieldsManager.MakeStep(currentPlayerID,Len);
		buttonsManager.HideButtons();
		SetState(IterationStep.Step);
	}
	
	public void EndStep(bool SendSystem)
	{
		buttonsManager.HideButtons();

        if (currentPostEffect == PostEffect.StepPlusBack)
        {
            currentPostEffect = PostEffect.StepPlus;
            currentState = IterationStep.Start;
        } else
		if (currentPostEffect == PostEffect.StepPlus)
		{
			currentPostEffect = PostEffect.None;
			currentState = IterationStep.Start;
		}
		else
		{
			IncCurrentPlayer(Player:null,SendSystem:SendSystem);
			currentState = IterationStep.Start;
		}
		NextTurn(currentPlayer);
        if (CurrentPostEffect != PostEffect.StepNone)
		    MakeStepIteration(-1);
	}

	private void MakeEffect(Player P, GameField.FieldEffects Effect)
	{
		switch (Effect)
		{
		case GameField.FieldEffects.Customs:
			GameFieldsManager.GoToVacation(currentPlayerID);
			currentPostEffect = PostEffect.Vacation;
			EndStep(true);
			LogToSystemChat("MakeEffect_Customs");
			break;
			
		case GameField.FieldEffects.Jackpot:
			P.Cash += JackpotWinCash;
			LogToSystemChat("MakeEffect_Jackpot");
			UpdateUserData(P,true);
			EndStep(true);
			break;
			
		case GameField.FieldEffects.Lottery:
			int win = (new int[3]{100000,200000,300000})[Random.Range(0,3)];
			P.Cash+=win;
			if (currentPostEffect!=PostEffect.StepPlus)
				SetPostEffect(PostEffect.None);
			LogToSystemChat("MakeEffect_Lottery|"+win);
			UpdateUserData(P,true);
			break;
			
		case GameField.FieldEffects.SkipStep:
			if (!GetClubCardActivePlayer())
				currentPostEffect = PostEffect.StepNone;
			LogToSystemChat("MakeEffect_SkipStep");
			EndStep(true);
			break;
			
		case GameField.FieldEffects.StepBack:
            if (currentPostEffect == PostEffect.StepPlus)
                currentPostEffect = PostEffect.StepPlusBack;
            else
                currentPostEffect = PostEffect.StepBack;
			LogToSystemChat("MakeEffect_StepBack");
			break;
			
		case GameField.FieldEffects.Vacation:
			GameFieldsManager.GoToCustoms(currentPlayerID);
			LogToSystemChat("MakeEffect_Vacation");
            if (GetClubCardCustomsFriend())
            {
                currentPostEffect = PostEffect.StepPlus;
                LogToSystemChat("CustomsFriend");
            }
            else
            {
                currentPostEffect = PostEffect.Customs;
                LogToMainChat("Вы попали на поле таможни. Для выхода необходимо выбросить дубль или выкупиться");
                EndStep(true);
            }
			break;

		case GameField.FieldEffects.Tax:
			
			LogToMainChat("Вы попали на налог, оплатите "+((int)(currentPlayer.Cash*0.2f)).ToString("### ### ##0 $"));
			break;
		}
	}

    internal void MakeStepIteration()
    {
        MakeStepIteration(-1);
    }

	public int StepIteration { get; private set; }
	
	public void MakeStepIteration(int PlayerID)
	{
        if (PlayerID != -1)
            currentPlayerID = PlayerID;

		StepIteration++;
		PlayersGrid.SetActive(currentPlayer.OwnerID);

        if (CurrentState == IterationStep.Start && currentPostEffect == PostEffect.StepNone && 
            currentPlayer.SocialID == SocialManager.Instance.ViewerID)
        {
            LogToMainChat("Вы пропускаете ход");
            LogToSystemChat("NoneStep_" + SocialManager.User.ViewerId);
            EndStep(true);
            SetPostEffect(PostEffect.None);
            return;
        }

		if (currentPlayer.SocialID == SocialManager.Instance.ViewerID)
		{
			// разрешим сделки с игроками
			List<GameField.Owners> cols = new List<GameField.Owners>();
			foreach (var p in Players)
				if (p.Bankrout==false)
					cols.Add(p.OwnerID);
			PlayersGrid.SetTransactionsEnable(cols.ToArray());

            if (currentState == IterationStep.Start)
                CreditStep();
		}
		else
		{
			// запретим сделки
            PlayersGrid.SetTransactionsEnable(false);
            buttonsManager.HideButtons();
		}
		
		if (currentState == IterationStep.Step)
		{
			SetState(IterationStep.FinishStep);
			var p = currentPlayer;
			var field = GameFieldsManager.GetFieldAtPlayer(p.OwnerID);
			if (field.Effect != GameField.FieldEffects.GameEffect)
				MakeEffect(p,field.Effect);
		}
		
		buttonsManager.ShowButtons();
		
		if (!buttonsManager.CanChoise() && currentPlayer.SocialID == SocialManager.Instance.ViewerID)
			EndStep(true);
	}

	public void SetBankrot (string ID)
	{
		SoundManager.StopStepOverSound();
		Player target = GetPlayerBySocialID(ID);
		if ((target.Bankrout && target.Cash<0) || (!target.Bankrout))
		{
			// освободим поля проигравшего
			foreach (var f in GameFieldsManager.Manager.GetPlayerFieldsData(target.OwnerID))
			{
				GameField gf = GameFieldsManager.Manager.GetGameField(f);
				gf.Owner = GameField.Owners.None;
				gf.Price = f.BuyPrice;
                gf.CurrentMonopolyRank = MonopolyRank.Base;
                gf.Locked = false;
			}
			// скроем его фишку
			GameFieldsManager.SetInactive(target.OwnerID);
			target.Bankrout = true;
			PlayersGrid.SetBankrout(target.OwnerID,true);
			LogToSystemChat("SetBankrout_"+ID);
			LogToMainChat("Игрок {0} обанкротился",target);
			target.Cash = 0;
			UpdateUserData(target,false);
			int live = 0;
			int liveID = 0;
			// если победа в режиме "1 за всех"
			for (int i=0;i<players.Length;i++)
				if (!players[i].Bankrout) 
			{
				live++;
				liveID = i;
			}
			if (live <=1 && ServerDataManager.GetGameInfo().GameType == 0)
			{
				Player winner = players[liveID];
				FindObjectOfType<WinnerWindow>().Init(winner.SocialID,winner.Cash+winner.Capital,GameInfoManager.Bank);

				ServerInfo.Instance.GetUserInfo(ServerDataManager.GetGameInfo().UserList.ToArray(),(users)=>{
					List<string> ids = new List<string>();
                    bool vip = false;
                    foreach (var u in users)
                    {
                        if (u.VIP != 0 || GetClubCardLeader(u.GUID))
                            ids.Add(u.GUID);
                        if (u.VIP != 0 && u.GUID == players[liveID].SocialID)
                            vip = true;
                    }
                    int winCap = winner.Cash+winner.Capital;
                    if (vip) winCap = (int)(winCap * 1.2f);
					ServerDataManager.FinishGame(players[liveID].SocialID,winCap,ids.ToArray());
				});
			} else
			{
				// если победа в режиме "2 на 2"
				if (ServerDataManager.GetGameInfo().GameType == 1)
				{
					if (players[0].Bankrout && players[1].Bankrout && (!players[2].Bankrout || !players[3].Bankrout))
					{
						// выиграла команда 2
						Player u1 = players[2];
						Player u2 = players[3];
						int win = u1.Cash+u1.Capital+u2.Cash+u2.Capital;
						FindObjectOfType<Winner2x2Window>().Init(u1.SocialID,u2.SocialID,
						                                         win,(int)(GameInfoManager.Bank/2f));

						ServerInfo.Instance.GetUserInfo(ServerDataManager.GetGameInfo().UserList.ToArray(),(users)=>{
							List<string> ids = new List<string>();
							foreach (var u in users)
								if (u.VIP!=0 || GetClubCardLeader(u.GUID))
									ids.Add(u.GUID);
							ServerDataManager.FinishGame2x2(new string[2]{u1.SocialID,u2.SocialID},
									(int)((u1.Cash+u1.Capital+u2.Cash+u2.Capital)/2.0f),
									ids.ToArray());
						});
					} 
					else
					if (players[2].Bankrout && players[3].Bankrout && (!players[0].Bankrout || !players[1].Bankrout))
					{
						// выиграла команда 1
						Player u1 = players[0];
						Player u2 = players[1];
						int win = u1.Cash+u1.Capital+u2.Cash+u2.Capital;
						FindObjectOfType<Winner2x2Window>().Init(u1.SocialID,u2.SocialID,
						                                         win,(int)(GameInfoManager.Bank/2f));
						ServerInfo.Instance.GetUserInfo(ServerDataManager.GetGameInfo().UserList.ToArray(),(users)=>{
							List<string> ids = new List<string>();
							foreach (var u in users)
								if (u.VIP!=0 || GetClubCardLeader(u.GUID))
									ids.Add(u.GUID);
							ServerDataManager.FinishGame2x2(new string[2]{u1.SocialID,u2.SocialID},
								(int)((u1.Cash+u1.Capital+u2.Cash+u2.Capital)/2.0f),
								ids.ToArray());
						});
					}
					else
					if (ID == currentPlayer.SocialID) 
					{
						IncCurrentPlayer("",false);
						MakeStepIteration(-1);
					}
				} 
				else
				if (ID == currentPlayer.SocialID) 
				{
					IncCurrentPlayer("",false);
					MakeStepIteration(-1);
				}
			}
		}
	}
}
