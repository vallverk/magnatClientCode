using UnityEngine;
using System.Collections;

public class ButtonsRuller
{
	public static int CustomsExitPrice = 200000;

	// чуть-чуть прелюдий для комфортной работы
	private GameManager manager;
	private int currentPlayerID { get { return manager.CurrentPlayerID; } }
	private Player currentPlayer { get { return manager.currentPlayer; } }
	private IterationStep currentState { get { return manager.CurrentState; } }
	private PlayersManager GameFieldsManager { get { return manager.GameFieldsManager; } }
	private ControlPanelButtonManager buttons { get { return manager.ButtonsManager; } }

	public ButtonsRuller(GameManager Manager)
	{
		manager = Manager;
	}

	public void OnPayButton ()
	{
		Player p = currentPlayer;
		GameField f = GameFieldsManager.GetFieldAtPlayer(p.OwnerID);

		// чужая клетка
		if (f.Effect == GameField.FieldEffects.GameEffect && f.Owner != GameField.Owners.None && f.Owner!=p.OwnerID)
		{
			FieldData fdata = GameFieldsManager.GetFieldDataAtPlayer(p.OwnerID);
            float disc = manager.GetDiscountOfMonopoly(GameFieldsManager.Manager.GetAliasFromField(f).ID);

			int price = (int)( fdata.GetCostByRank(f.CurrentMonopolyRank) * disc *
			                  manager.GetClubCardTrueBussinesCoef(p.SocialID));
			if (p.Cash < price)
			{
				manager.LogToMainChat("У вас недостаточно денег для оплаты. Вы можете заложить фирму, продать филиал, обменяться фирмами или взять кредит (только для VIP)");
			} 
			else
			{
				SoundManager.PlayPayOutSound();
				// содрем бабки с не везучего
				p.Cash -= price;
				manager.UpdateUserData(p,true);
				// и подарим им новому счтастливому обладателю
				Player happyUser = manager.GetPlayerByOwnerID(f.Owner);
				happyUser.Cash += price;

                string taxText = "";
                if (disc == 1)
                {
                    // и сообщим остальным пользователям чтоб поржали
                    manager.LogToSystemChat(string.Format("PayForField_{0}|{1}|{2}|{3}",
                                                          p.SocialID, happyUser.SocialID, price, fdata.FieldName));
                    // и, естессно, скажем нашему юзеру - пускай поплачет...
                    taxText = string.Format("[FFFFFF]{0} $[-] игроку {1}.", price.ToString("### ### ##0"),
                                                   manager.ChatManager.MakeColoredBB(happyUser.OwnerID, happyUser.LongName));
                    manager.LogToMainChat("{0} оплатил аренду фирмы [FFFFFF]" + fdata.FieldName + "[-] - " + taxText, p);
                }
                else
                {
                    GameDataManager gdata = GameObject.FindObjectOfType<GameDataManager>();
                    string discText = gdata.GetAliasFromField(f).AliasName + " " + (int)(disc * 100) + "%";
                    //  Игорь, имея пакет акций «Общепит 1», оплачивает аренду игроку Марина – $4,500
                    manager.LogToSystemChat(string.Format("PayForFieldWithAction_{0}|{1}|{2}|{3}|{4}",
                                                          p.SocialID, happyUser.SocialID, price, fdata.FieldName,discText));
                    taxText = string.Format("[FFFFFF]{0} $[-] игроку {1}.", price.ToString("### ### ##0"),
                                                   manager.ChatManager.MakeColoredBB(happyUser.OwnerID, happyUser.LongName));
                    manager.LogToMainChat("{0}, имея пакет акций «"+discText+"», оплатил аренду фирмы [FFFFFF]" + fdata.FieldName + "[-] - " + taxText, p);
                }
                manager.UpdateUserData(happyUser,true);

				if (manager.CurrentPostEffect != PostEffect.StepPlus)
					manager.EndStep(true);
				else
				{
					manager.SetState(IterationStep.Start);
					manager.MakeStepIteration(-1);
				}
			}
		}

		// налог
		if (f.Effect == GameField.FieldEffects.Tax)
		{
			int tax = (int)(p.Cash * 0.2f * manager.GetClubCardTaxCoef());
			if (p.Cash < tax)
				// чувак на мели...
				manager.LogToMainChat("У вас недостаточно денег для оплаты. Вы можете заложить фирму, продать филиал, обменяться фирмами или взять кредит (только для VIP)");
			else
			{
				SoundManager.PlayPayOutSound();
				// крикнем в игровую комнату
				manager.LogToSystemChat(string.Format("PayForTax_{0}|{1}",p.SocialID,tax));
				// и в общий чат
				manager.LogToMainChat("{0} оплатил налог [FFFFFF]"+tax.ToString("### ### ##0 $")+"[-].",p);
                p.Cash -= tax;
                manager.UpdateUserData(p, true);
				if (manager.CurrentPostEffect != PostEffect.StepPlus)
					manager.EndStep(true);
				else
				{
					manager.SetState(IterationStep.Start);
					manager.MakeStepIteration(-1);
				}
			}
		}

		// таможня
		if (f.Effect == GameField.FieldEffects.Customs && manager.CurrentPostEffect ==  PostEffect.Customs)
		{
			if (p.Cash < CustomsExitPrice)
				// таможенники требуют денег за проход. no money - no honey!
				manager.LogToMainChat("У вас недостаточно денег для оплаты. Вы можете заложить фирму, продать филиал, обменяться фирмами или взять кредит (только для VIP)");
			else
			{
				// он смог!
				CustomsFails = 0;
				manager.LogToSystemChat(string.Format("PayForCustoms_{0}|{1}",p.SocialID,CustomsExitPrice));
				manager.LogToMainChat("{0} оплатил [FFFFFF]"+CustomsExitPrice+"[-] за выход из таможни.",p);
				p.Cash-=CustomsExitPrice;
				manager.UpdateUserData(p,true);

				// походим...
				manager.SetPostEffect(PostEffect.None);
                manager.SetState(IterationStep.Start);
                manager.MakeStepIteration();
			    //OnThrowDiceClick();
			}
		}
	}

	public void OnBuyButton ()
	{
		try
		{
			var p = currentPlayer;
			GameField f = GameFieldsManager.GetFieldAtPlayer(p.OwnerID);
			if (f.Effect == GameField.FieldEffects.GameEffect && f.Owner == GameField.Owners.None)
			{
				// поработаем над полем
				FieldData data = GameFieldsManager.GetFieldDataAtPlayer(p.OwnerID);
				f.Price = data.BasePrice;
				p.Capital+=data.BuyPrice;
				p.Cash-=data.BuyPrice;
				f.Owner = p.OwnerID;
				// сообщим другим
                manager.LogToSystemChat(string.Format("BuyField_{0}|{1}", p.SocialID, GameFieldsManager.GetPlayerCurrentCardID(p.OwnerID)));
                manager.UpdateUserData(p, true);
				// сообщим юзеру
				manager.LogToMainChat("{0} приобрел фирму [FFFFFF]"+data.FieldName+"[-]",p);
				// если пользователь приобрел все монополии одной фирмы - запишем их в монополию
				AliasData alias = GameFieldsManager.Manager.GetAliasFromField(f);
				GameField[] af = GameFieldsManager.Manager.GetFieldInAlias(alias);
				// но, для начала, обработаем случай, если пользователь приобрел город
				if (GameFieldsManager.Manager.IsCity(alias))
				{
					int ownerOf = 0;
					foreach (var field in af)
						if (field.Owner == manager.currentPlayer.OwnerID)
							ownerOf++;
					// проставим им правильно ранг и цены
					foreach (var field in af)
						if (field.Owner == manager.currentPlayer.OwnerID)
					{
						field.CurrentMonopolyRank = MonopolyRank.Monopoly+ownerOf;
						field.Price = (int)(GameFieldsManager.Manager.GetFieldData(field).GetCostByRank(field.CurrentMonopolyRank) *
							manager.GetClubCardTrueBussinesCoef(p.SocialID));
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
						foreach (var field in af)
						{
							field.CurrentMonopolyRank = MonopolyRank.Monopoly;
							field.Price = GameFieldsManager.Manager.GetFieldData(field).GetCostByRank(field.CurrentMonopolyRank);
						}
					}
				}
				if (manager.CurrentPostEffect != PostEffect.StepPlus)
					manager.EndStep(true);
				else
				{
					manager.SetState(IterationStep.Start);
					manager.MakeStepIteration(-1);
				}
			}
		}  catch (System.Exception e)
		{
			manager.LogToMainChat("Ошибка кнопки купить: "+e.Message+"\r\n"+e.StackTrace);
		}
	}

	public int CustomsFails = 0;

	public void OnThrowDiceClick()
	{
		SoundManager.PlayThrowDice();
		manager.UpdateButtons(false);
		var p = currentPlayer;
		int a,b;
		GameFieldsManager.ThrowDice(out a, out b);
		CubesController.DropDice(a,b);
		// выведем статистику в комнату
		manager.LogToSystemChat(string.Format("ThrowDice_{0}|{1}|{2}",p.SocialID,a,b));
		// и на экран пользователю 
		manager.LogToMainChat("{0} выбросил кубики [FFFFFF]"+a+":"+b+"[-]",p);

		switch (manager.CurrentPostEffect)
		{
		case PostEffect.Customs:
			if (a==b)
			{
				// везучий гад...
				manager.LogToMainChat("Игрок {0} выбрасывает дубль и выходит из таможни.",p);
				manager.LogToSystemChat(string.Format("CustomsEscape_{0}",p.SocialID));
				manager.SetPostEffect(PostEffect.None);
				CustomsFails = 0;
                manager.MakeStepIteration();
			} else
			{
				// ха-ха
                CustomsFails++;
				manager.LogToMainChat("Для выхода с таможни должен выпасть дубль, игрок {0} остается на таможне",p);
				manager.LogToSystemChat(string.Format("CustomsEscapeFail_{0}",p.SocialID));
				manager.EndStep(true);
			}
			break;

		case PostEffect.StepBack:
			// обратный польский ход
			manager.LogToMainChat("Игрок {0} совершил ход назад.",p);
			manager.LogToSystemChat(string.Format("StepBack_{0}|{1}",p.SocialID,a+b));
			manager.MakeStep(-(a+b));
			manager.SetPostEffect(PostEffect.None);
			break;

		case PostEffect.StepNone:
			// простой (в смысле не просто й ход, а пропуск хода)
			manager.LogToMainChat("Игрок {0} пропускает ход.",p);
			manager.LogToSystemChat(string.Format("StepNone_{0}",p.SocialID));
			manager.SetPostEffect(PostEffect.None);
			manager.EndStep(true);
			break;

		default:
			// похоже на обычный ход
			manager.SetPostEffect(PostEffect.None);
			manager.LogToSystemChat(string.Format("Step_{0}|{1}",p.SocialID,a+b));
			manager.MakeStep(a+b);
			break;
		}
		// отдуплим еще на один ход
		if (a==b)
			manager.SetPostEffect(PostEffect.StepPlus);
	}

	public void OnAuctionButton()
	{
		HideButtons();
		GameObject.FindObjectOfType<AuctionController>().InitAuction();
	}

	public void OnLayButton()
	{
		manager.GetComponent<GameModeManager>().StartDialog(GameMode.Lay);
	}

	public void OnToBuyButton()
	{
		manager.GetComponent<GameModeManager>().StartDialog(GameMode.ToBuy);
	}

	public void OnBuildButton()
	{
		manager.GetComponent<GameModeManager>().StartDialog(GameMode.Build);
	}

	public void OnSellButton()
	{
		manager.GetComponent<GameModeManager>().StartDialog(GameMode.Sell);
	}

	public void OnKeepCreditButton()
	{
		manager.CeepCredit();
	}

	public void OnReturnCreditButton()
	{
		manager.ReturnCredit();
	}

	private bool gameEscaped = false;

	public void OnEscapeButton()
	{
		if (!gameEscaped)
		{
            AlertWindow.Show("ПОДТВЕРЖДЕНИЕ", "Вы уверены, что хотите сдаться?", () =>
            {
                UIButton button = buttons.EscapeButton.GetComponent<UIButton>();
                if (button != null)
                {
                    button.normalSprite = "exit_button_na";
                    button.hoverSprite = "exit_button_hover";
                    button.pressedSprite = "exit_button_pressed";
                }

                manager.LogToMainChat("Вы сдались. Повторное нажатие кнопки совершит выход в лобби.");
                manager.SetBankrot(SocialManager.Instance.ViewerID);
                ServerData.AddGameToBlackList(manager.ServerDataManager.GetUserGameID().ToString());
                gameEscaped = true;
                buttons.SetAllInactive();
                buttons.EnableEscapeButton = true;
                buttons.UpdateButtons();
            }, () => { });
		}
		else
		{
			Application.LoadLevel(1);
		}
	}



	public void ShowButtons()
	{
		buttons.SetAllInactive();
		if (manager.currentPlayer.SocialID == SocialManager.Instance.ViewerID)
		{
			buttons.EnableAuctionButton = CanAuction();
			buttons.EnableBuildButton = CanBuild();
			buttons.EnableBuybutton = CanBuy();
			buttons.EnableLayButton = CanLay();
			buttons.EnablePayButton = CanPay();
			buttons.EnableSellButton = CanSell();
			buttons.EnableThrowDiceButton = CanThrowDice();
			buttons.EnableToBuyButton = CanToBuy();
			buttons.EnableKeepCreditButton = CanKeepCredit();
			buttons.EnableReturnCreditButton = CanReturnCredit();
		}
		buttons.EnableEscapeButton = CanEscape();
		buttons.UpdateButtons();
	}

	public void HideButtons()
	{
		buttons.SetAllInactive();
		buttons.UpdateButtons();
	}

	public bool CanChoise()
	{
		return buttons.CanChoise();
	}

	private bool CanLay()
	{
		var list = GameFieldsManager.Manager.GetPlayerFields(currentPlayer.OwnerID);
		foreach (var f in list)
			if (!f.Locked) 
				return true;
		return false;
	}

	private bool CanToBuy()
	{
		var p = currentPlayer;
		var list = GameFieldsManager.Manager.GetPlayerFields(p.OwnerID);
		foreach (var f in list)
			if (f.Locked) 
				return true;
		return false;
	}

	private bool CanBuild()
	{
		return GameFieldsManager.Manager.GetPlayerAliases(currentPlayer.OwnerID).Length != 0
            && currentState == IterationStep.Start;
	}

	private bool CanSell()
	{
		foreach (var alias in GameFieldsManager.Manager.GetPlayerAliases(currentPlayer.OwnerID))
		{
			foreach (var field in GameFieldsManager.Manager.GetFieldInAlias(alias))
			{
				if ((int)field.CurrentMonopolyRank > 1)
					return true;
			}
		}
		return false;
	}

	private bool CanKeepCredit()
	{
		return manager.IsCanCeepCredit && manager.GetClubCardCredit();
	}

	private bool CanReturnCredit()
	{
		return manager.IsCrediting;
	}

	private bool CanEscape()
	{
		return true;
	}

	private bool CanBuy()
	{
		if (currentState != IterationStep.FinishStep) return false;
		Player p = manager.currentPlayer;
		GameField field = manager.GameFieldsManager.GetFieldAtPlayer(p.OwnerID);
		if (field.Effect == GameField.FieldEffects.GameEffect && field.Owner == GameField.Owners.None)
		{
			FieldData data = manager.GameFieldsManager.GetFieldDataAtPlayer(p.OwnerID);
			if (p.Cash>=data.BuyPrice)
				return true;
		}
		return false;
	}

	private bool CanAuction()
	{
		if (currentState != IterationStep.FinishStep) return false;
		Player p = manager.currentPlayer;
		GameField field = manager.GameFieldsManager.GetFieldAtPlayer(p.OwnerID);
		if (field.Effect == GameField.FieldEffects.GameEffect && field.Owner == GameField.Owners.None)
			return true;
		return false;
	}

	private bool CanPay()
	{
		Player p = currentPlayer;
		GameField field = GameFieldsManager.GetFieldAtPlayer(p.OwnerID);

        if (field.Effect == GameField.FieldEffects.Customs && manager.CurrentPostEffect == PostEffect.Customs) return true;

		if (currentState != IterationStep.FinishStep) return false;
		if (field.Effect == GameField.FieldEffects.GameEffect && field.Owner != GameField.Owners.None)
		{
			if (manager.MyTeam(field.Owner) && manager.ServerDataManager.GetGameInfo().GameType == 1)
				return false;
			if (field.Owner != p.OwnerID && !field.Locked)
				return true;
		}
		if (field.Effect == GameField.FieldEffects.Tax) return true;
		return false;
	}

	private bool CanThrowDice()
	{
		if (manager.CurrentPostEffect == PostEffect.Customs && this.CustomsFails>2)
			return false;
		return currentState == IterationStep.Start;
	}
}
