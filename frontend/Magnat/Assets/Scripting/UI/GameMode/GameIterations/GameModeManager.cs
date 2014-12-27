using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class GameModeManager : MonoBehaviour 
{
	public GameDataManager GameData;
	public GameManager GManager;
	private ControlPanelButtonManager buttons;
	public AuctionWindow Auction;
	public TransactionWindow Transaction;

	public Color TintNormalColor = Color.white;
	public Color TintShadowColor = Color.gray;
	public Color TintBuildColor = new Color(0.7f,0.7f,0.7f);

	private GameMode currentMode;
	private List<UIButton> inactiveButtons = new List<UIButton>();
	private List<GameObject> inactiveObjects = new List<GameObject>();

	private int startCash = -1;
	private List<GameField> fieldBuffer = new List<GameField>();

	public System.Action OnAuctionAccepted = () => {};
	public System.Action OnAuctionRefused = () => {};
	private GameField auctionField;

	private int buildStepIteration=-1;
    private List<int> toBuildMonopolyBuffer = new List<int>();

	void Start()
	{
		buttons = GManager.ButtonsManager;
		buttons.OnCancelButton += OnCancelClick;
		Auction.AcceptButton.onClick.Clear();
		Auction.CancelButton.onClick.Clear();
		Auction.AcceptButton.onClick.Add(new EventDelegate(()=>{OnAuctionAccepted();}));
		Auction.CancelButton.onClick.Add(new EventDelegate(()=>{OnAuctionRefused();}));
		OnAuctionRefused += () => { StopCoroutine("AuctionTimer"); };
		OnAuctionAccepted += () => { StopCoroutine("AuctionTimer"); };
		GManager.PlayersGrid.OnTransactionButtonClick += ShowTransactionWindow;
		Transaction.AcceptButton.onClick.Add(new EventDelegate(()=>{FinishDialog(true);}));
		Transaction.CancelButton.onClick.Add(new EventDelegate(()=>{FinishDialog(false);}));
	}

	public void ShowTransactionWindow(GameField.Owners TargetOwner)
	{
		GManager.UnsubscribeButtonsEvents();
		Player p1 = GManager.currentPlayer;
		Player p2 = GManager.GetPlayerByOwnerID(TargetOwner);
		// инициализируем окно обмена
		Transaction.Init(p1,p2);
		Transaction.SetSliderActive(true);
		Transaction.ShowDoalog();

		// инициализируем фшки
		// "спрячем" лишние фишки
		// сначала фишки не игроков
		List<GameField> fields = new List<GameField>((new List<GameField>(GameData.GetNoPlayerFields(p1.OwnerID))).Intersect(new List<GameField>(GameData.GetNoPlayerFields(p2.OwnerID))));
		foreach (var b in fields)
			TurnLight(b,false);
		// потом не игровые поля
		foreach (var o in GameData.GetNonGameField())
			TurnLight(o,false);
		fields = new List<GameField>(GameData.GetPlayerFields(p1.OwnerID));
		fields.AddRange(GameData.GetPlayerFields(p2.OwnerID));
		// потом игровые поля, которые не подходят по условиям (с постройками)
		foreach (var b in fields)
			if (b.CurrentMonopolyRank != MonopolyRank.Base && b.CurrentMonopolyRank != MonopolyRank.Monopoly && !GameData.IsCity(b))
				TurnLight(b,false);
		currentMode = GameMode.Transaction;
		InfoPanelController.CanShow = false;
		startCash = p1.Cash;
	}

	public void ShowTransactionWindow(TransactionInfo info)
	{
		GManager.UnsubscribeButtonsEvents();
		Player p1 = GManager.GetPlayerBySocialID(info.Player1);
		Player p2 = GManager.GetPlayerBySocialID(info.Player2);
		// инициализируем окно обмена

		Transaction.Init(p1,p2);
		Transaction.SetSliderActive(false);
		Transaction.ShowDoalog();
		List<GameField> fields = new List<GameField>();
		foreach (int id in info.FieldInds)
		{
			FieldData data = GameData.Fields[id-1];
			GameField field = GameData.GetGameField(data);
			Transaction.AddFieldToList(p1.OwnerID == field.Owner?1:0,field);
			fields.Add(field);
			fieldBuffer.Add(field);
		}
		Transaction.SetSupplement(info.Supplement);
		List<GameField> all = new List<GameField>(GameData.GamePoolSteps);
		List<GameField> hide = new List<GameField>();
		all.ForEach((f)=>{if(!fields.Contains(f))hide.Add(f);});
		foreach (var o in hide)
			TurnLight(o,false);

		currentMode = GameMode.TransactionView;
		InfoPanelController.CanShow = false;
		startCash = p2.Cash;
	}

	public void ShowAuctionDialog(Player Target, FieldData Field, int CurrentPrice, int StepPrice)
	{
		Auction.PlayerName = Target.LongName;
		Auction.FieldName = Field.FieldName;
		Auction.CurrentPrice = CurrentPrice;
		Auction.IncStep = StepPrice;
		StartCoroutine("AuctionTimer");
		Auction.OnValidate();
		auctionField = GameData.GetGameField(Field);
		auctionField.targetSprite.GetComponent<UIWidget>().depth = 10;
		auctionField.targetPriceField.GetComponent<UIWidget>().depth = 11;
		auctionField.targetPriceField.transform.parent.GetComponent<UIWidget>().depth = 10;
		Auction.Show();
	}

	public void HideAuctionDialog()
	{
		StopCoroutine("AuctionTimer");
		StartCoroutine(PutFieldsBack());
		Auction.Hide();
	}

	private IEnumerator PutFieldsBack()
	{
		yield return new WaitForSeconds(0.3f);
		auctionField.targetSprite.GetComponent<UIWidget>().depth = 2;
		auctionField.targetPriceField.GetComponent<UIWidget>().depth = 3;
		auctionField.targetPriceField.transform.parent.GetComponent<UIWidget>().depth = 2;
	}

	private IEnumerator AuctionTimer()
	{
		float time = Time.time;
		for (float delta = 0; delta < 15; delta = Time.time - time)
		{
			Auction.TimerTime = 15 - delta;
			Auction.OnValidate();
			yield return new WaitForEndOfFrame();
		}
		OnAuctionRefused();
	}

	public void OnFieldClisck()
	{
		UIButton targetBut = UIButton.current;
		GameField targetField = targetBut.transform.parent.GetComponent<GameField>();
		FieldData targetData = GameData.GetFieldData(targetField);
		if (!inactiveButtons.Contains(targetBut))
		switch (currentMode)
		{
		case GameMode.Lay:
			{
				targetField.Locked = !targetField.Locked;
				GManager.currentPlayer.Cash += targetData.MislayPrice * (targetField.Locked?1:-1);
				GManager.UpdateUserData(GManager.currentPlayer,false);
				if (targetField.Locked)
					fieldBuffer.Add(targetField);
				else
					fieldBuffer.Remove(targetField);
			}
			break;
		case GameMode.ToBuy:
			{
				if (targetData.BuyOutPrice <= GManager.currentPlayer.Cash)
				{
					targetField.Locked = !targetField.Locked;
					GManager.currentPlayer.Cash += targetData.BuyOutPrice * (targetField.Locked?1:-1);
					GManager.UpdateUserData(GManager.currentPlayer,false);
					if (!targetField.Locked)
						fieldBuffer.Add(targetField);
					else
						fieldBuffer.Remove(targetField);
				}
				else
					GManager.LogToMainChat("Поле "+targetData.FieldName+ " не может быть выкуплено, т. к. не хватает денег на выкуп.");
			}
			break;

		case GameMode.Build:
			{
                var alias = GManager.GameFieldsManager.Manager.GetAliasFromField(targetField);
				if (fieldBuffer.Contains(targetField))
				{
                    if (toBuildMonopolyBuffer.Contains(alias.ID))
                        toBuildMonopolyBuffer.Remove(alias.ID);
					fieldBuffer.Remove(targetField);
					ChangeColor(targetField,Color.white);
					if (targetField.CurrentMonopolyRank == MonopolyRank.Holding)
						GManager.currentPlayer.Cash += targetData.MBuildPrice;
					else
						GManager.currentPlayer.Cash += targetData.BuildPrice;
					targetField.CurrentMonopolyRank--;
					targetField.Price = targetData.GetCostByRank(targetField.CurrentMonopolyRank);
				} else 
				{
					int price = 0;
					if (targetField.CurrentMonopolyRank+1 == MonopolyRank.Holding)
						price = targetData.MBuildPrice;
					else
						price = targetData.BuildPrice;
					if (GManager.currentPlayer.Cash >= price)
					{
                        if (toBuildMonopolyBuffer.Contains(alias.ID))
                        {
                            GManager.LogToMainChat("Вы уже выбрали поле из этой монополии для постройки.");
                        }
                        else
                        {
                            toBuildMonopolyBuffer.Add(alias.ID);
                            fieldBuffer.Add(targetField);
                            ChangeColor(targetField, TintBuildColor);
                            targetField.CurrentMonopolyRank++;
                            if (targetField.CurrentMonopolyRank == MonopolyRank.Holding)
                                GManager.currentPlayer.Cash -= targetData.MBuildPrice;
                            else
                                GManager.currentPlayer.Cash -= targetData.BuildPrice;
                            targetField.Price = targetData.GetCostByRank(targetField.CurrentMonopolyRank);
                        }
					} else
						GManager.LogToMainChat("Не достаточно денег для совершения данной операции.");
				}
				GManager.UpdateUserData(GManager.currentPlayer,false);
			}
			break;

			case GameMode.Sell:
			{
				if (fieldBuffer.Contains(targetField))
				{
					fieldBuffer.Remove(targetField);
					ChangeColor(targetField,Color.white);
					if (targetField.CurrentMonopolyRank+1 == MonopolyRank.Holding)
						GManager.currentPlayer.Cash -= targetData.MBuildPrice;
					else
						GManager.currentPlayer.Cash -= targetData.BuildPrice;
					targetField.CurrentMonopolyRank++;
					targetField.Price = targetData.GetCostByRank(targetField.CurrentMonopolyRank);
				} else 
				{
					int price = 0;
					if (targetField.CurrentMonopolyRank == MonopolyRank.Holding)
						price = targetData.MBuildPrice;
					else
						price = targetData.BuildPrice;

					fieldBuffer.Add(targetField);
					ChangeColor(targetField,TintBuildColor);
					targetField.CurrentMonopolyRank--;
					GManager.currentPlayer.Cash += price;
					targetField.Price = targetData.GetCostByRank(targetField.CurrentMonopolyRank);
				}
				GManager.UpdateUserData(GManager.currentPlayer,false);
			}
			break;

			case GameMode.Transaction:
			{
				if (fieldBuffer.Contains(targetField))
				{
					fieldBuffer.Remove(targetField);
					ChangeColor(targetField,Color.white);
					Transaction.RemoveFromList(targetField.Owner == GManager.currentPlayer.OwnerID?0:1,targetField);
				} else 
				{
                    Player p1 = GManager.GetPlayerBySocialID(Transaction.Player1.ID);
                    Player p2 = GManager.GetPlayerBySocialID(Transaction.Player2.ID);
                    int sup = Transaction.Supplement + (int)(targetData.BuyPrice * Mathf.Sign(Transaction.Supplement));
                    // если у обоих игроков хватит денег на сделку
                    if (!(sup < 0 && sup < -p1.Cash) && !(sup > 0 && sup > p2.Cash))
                    {
                        fieldBuffer.Add(targetField);
                        ChangeColor(targetField, TintBuildColor);
                        Transaction.AddFieldToList(targetField.Owner == GManager.currentPlayer.OwnerID ? 0 : 1, targetField);
                    }
				}
			}
			break;
		}
	}

	/// <summary>
	/// Инициализирует диалоговое окно соответствующего режима
	/// </summary>
	/// <param name="Mode">Mode.</param>
	public void StartDialog(GameMode Mode)
	{
		if (Mode == GameMode.Build && GManager.StepIteration == buildStepIteration)
		{
			GManager.LogToMainChat("Можно строить только один раз за ход.");
			return;
		} else
			buildStepIteration = GManager.StepIteration;

		InfoPanelController.CanShow = false;
		currentMode = Mode;
		startCash = GManager.currentPlayer.Cash;
		// отошьем обработку кнопочек игровым менеджером, чтоб самим их контролить
		GManager.UnsubscribeButtonsEvents();
		Player cplayer = GManager.currentPlayer;
		switch (Mode)
		{
			// если мы закладываем фишки...
		case GameMode.Lay:
			// покажем кнопки "зложить" и "отмена"
			buttons.SetAllInactive();
			buttons.EnableLayButton = true;
			buttons.OnLayButton += OnAllowClick;
			buttons.EnableCancelButton = true;
			buttons.UpdateButtons();
			// "спрячем" лишние фишки
			// сначала фишки не игрока
			foreach (var b in GameData.GetNoPlayerFields(cplayer.OwnerID))
				TurnLight(b,false);
			// потом не игровые поля
			foreach (var o in GameData.GetNonGameField())
				TurnLight(o,false);
			// потом игровые поля, которые не подходят по условиям
			foreach (var b in GameData.GetPlayerFields(cplayer.OwnerID))
				if (b.Locked || (b.CurrentMonopolyRank != MonopolyRank.Base && b.CurrentMonopolyRank != MonopolyRank.Monopoly))
					if (!GameData.IsCity(b) || b.Locked)
						TurnLight(b,false);
			break;
			// выкуп
		case GameMode.ToBuy:
			// покажем кнопки "выкупить" и "отмена"
			buttons.SetAllInactive();
			buttons.EnableToBuyButton = true;
			buttons.OnToBuyButton += OnAllowClick;
			buttons.EnableCancelButton = true;
			buttons.UpdateButtons();
			// "спрячем" лишние фишки
			// сначала фишки не игрока
			foreach (var b in GameData.GetNoPlayerFields(cplayer.OwnerID))
				TurnLight(b,false);
			// потом не игровые поля
			foreach (var o in GameData.GetNonGameField())
				TurnLight(o,false);
			// потом игровые поля, которые не подходят по условиям
			foreach (var b in GameData.GetPlayerFields(cplayer.OwnerID))
				if (!b.Locked)
					TurnLight(b,false);
			break;

		case GameMode.Build:
            toBuildMonopolyBuffer.Clear();
			// покажем кнопки "выкупить" и "отмена"
			buttons.SetAllInactive();
			buttons.EnableBuildButton = true;
			buttons.OnBuildButton += OnAllowClick;
			buttons.EnableCancelButton = true;
			buttons.UpdateButtons();
			// "спрячем" лишние фишки
			// сначала фишки не игрока
			foreach (var b in GameData.GetNoPlayerFields(cplayer.OwnerID))
				TurnLight(b,false);
			// потом не игровые поля
			foreach (var o in GameData.GetNonGameField())
				TurnLight(o,false);
			// потом игровые поля, которые не подходят по условиям
			List<GameField> userFields = new List<GameField>(GameData.GetPlayerFields(cplayer.OwnerID));
			var aliases = GameData.GetPlayerAliases(cplayer.OwnerID);
			// обшарим каждую монополию, которая есть у пользователя
			foreach (var alias in aliases)
			{
				if (GameData.IsCity(alias)) continue; // монополию "мегаполис" нельзя апгрейдить
				GameField[] fields = GameData.GetFieldInAlias(alias);
				// доступны могут быть только поля с минимальным рангом
				int minRank = 50;
				foreach (var f in fields)
					if ((int)(f.CurrentMonopolyRank)<minRank)
						minRank = (int)(f.CurrentMonopolyRank);
				if ((MonopolyRank)minRank == MonopolyRank.Holding) continue; // куда уж дальше строить то?
				foreach (var f in fields)
				{
					if ((int)(f.CurrentMonopolyRank) == minRank && userFields.Contains(f) && !f.Locked)
					{
						// а еще если у юзера хватает бабосов на постройку
						int price = 0;
						if (f.CurrentMonopolyRank+1 == MonopolyRank.Holding)
							price = GameData.GetFieldData(f).MBuildPrice;
						else
							price = GameData.GetFieldData(f).BuildPrice;
						if (GManager.currentPlayer.Cash >= price)
							userFields.Remove(f);
					}
				}
			}
			// ну а все остальные поля можно лочить
			foreach (var f in userFields)
				TurnLight(f,false);
			break;

		case GameMode.Sell:
			// покажем кнопки "выкупить" и "отмена"
			buttons.SetAllInactive();
			buttons.EnableSellButton = true;
			buttons.OnSellButton += OnAllowClick;
			buttons.EnableCancelButton = true;
			buttons.UpdateButtons();
			// "спрячем" лишние фишки
			// сначала фишки не игрока
			foreach (var b in GameData.GetNoPlayerFields(cplayer.OwnerID))
				TurnLight(b,false);
			// потом не игровые поля
			foreach (var o in GameData.GetNonGameField())
				TurnLight(o,false);
			// потом игровые поля, которые не подходят по условиям
			List<GameField> userFieldsSell = new List<GameField>(GameData.GetPlayerFields(cplayer.OwnerID));
			var aliasesSell = GameData.GetPlayerAliases(cplayer.OwnerID);
			// обшарим каждую монополию, которая есть у пользователя
			foreach (var alias in aliasesSell)
			{
				if (GameData.IsCity(alias)) continue; // монополию "мегаполис" нельзя продавать (как и построить)
				GameField[] fields = GameData.GetFieldInAlias(alias);
				// доступны могут быть только поля с минимальным рангом
				int maxRank = 0;
				foreach (var f in fields)
					if ((int)(f.CurrentMonopolyRank)>maxRank)
						maxRank = (int)(f.CurrentMonopolyRank);
				if ((MonopolyRank)maxRank == MonopolyRank.Monopoly) continue; // куда уж дальше продавать?
				foreach (var f in fields)
				{
					if ((int)(f.CurrentMonopolyRank) == maxRank && userFieldsSell.Contains(f))
						userFieldsSell.Remove(f);
				}
			}
			// ну а все остальные поля можно лочить
			foreach (var f in userFieldsSell)
				TurnLight(f,false);
			break;
		}
	}

	void OnCancelClick ()
	{
		FinishDialog(false);
	}

	void OnAllowClick()
	{
		FinishDialog(true);
	}

	/// <summary>
	/// Заканчивает текущий диалог
	/// </summary>
	/// <param name="Allow"> Применить изменения? </param>
	public void FinishDialog(bool Allow)
	{
		InfoPanelController.CanShow = true;
		// обязательные обработчики
		GManager.currentPlayer.Cash = startCash;
		switch (currentMode)
		{
		case GameMode.Lay:
			buttons.OnLayButton -= OnAllowClick;
			if (Allow && fieldBuffer.Count>0)
			{
				string q = "LayFields_";    
				string text = "{0} заложил поля ";
				foreach (var f in fieldBuffer)
				{
					FieldData data = GameData.GetFieldData(f);
					GManager.currentPlayer.Cash+=data.MislayPrice;
					q+=(data.ID-1).ToString()+"|";
					text += "[FFFFFF]"+data.FieldName+"[-],";
				}
				q = q.Substring(0,q.Length-1);
				text = text.Substring(0,text.Length-1)+".";
				GManager.LogToMainChat(text,GManager.currentPlayer);
				GManager.LogToSystemChat(q);
				GManager.UpdateUserData(GManager.currentPlayer,true);
			} else
			{
				foreach (var f in fieldBuffer)
					f.Locked = false;
			}
			break;

		case GameMode.ToBuy:
			buttons.OnToBuyButton -= OnAllowClick;
			if (Allow && fieldBuffer.Count>0)
			{
				string q = "ToBuyFields_";
				string text = "{0} выкупил поля ";
				foreach (var f in fieldBuffer)
				{
					FieldData data = GameData.GetFieldData(f);
					GManager.currentPlayer.Cash-=data.BuyOutPrice;
					q+=(data.ID-1).ToString()+"|";
					text += "[FFFFFF]"+data.FieldName+"[-],";
				}
				q = q.Substring(0,q.Length-1);
				text = text.Substring(0,text.Length-1)+".";
				GManager.LogToMainChat(text,GManager.currentPlayer);
				GManager.LogToSystemChat(q);
				GManager.UpdateUserData(GManager.currentPlayer,true);
			} else
			{
				foreach (var f in fieldBuffer)
					f.Locked = true;
			}
			break;

		case GameMode.Build:
			buttons.OnBuildButton -= OnAllowClick;
			if (Allow && fieldBuffer.Count>0)
			{
				string q = "BuildFields_";
				string text = "{0} построил филиалы на полях ";
				foreach (var f in fieldBuffer)
				{
					FieldData data = GameData.GetFieldData(f);
					if (f.CurrentMonopolyRank==MonopolyRank.Holding)
						GManager.currentPlayer.Cash-=data.MBuildPrice;
					else
						GManager.currentPlayer.Cash-=data.BuildPrice;
					q+=(data.ID-1).ToString()+"|";
					text += "[FFFFFF]"+data.FieldName+"[-],";
					ChangeColor(f,Color.white);
				}
				q = q.Substring(0,q.Length-1);
				text = text.Substring(0,text.Length-1)+".";
				GManager.LogToMainChat(text,GManager.currentPlayer);
				GManager.LogToSystemChat(q);
				GManager.UpdateUserData(GManager.currentPlayer,true);
			} else
			{
				foreach (var f in fieldBuffer)
				{
					f.CurrentMonopolyRank--;
					ChangeColor(f,Color.white);
					f.Price = GameData.GetFieldData(f).GetCostByRank(f.CurrentMonopolyRank);
				}
			}
			break;

		case GameMode.Sell:
			buttons.OnSellButton -= OnAllowClick;
			if (Allow && fieldBuffer.Count>0)
			{
				string q = "SellFields_";
				string text = "{0} продал филиалы на полях ";
				foreach (var f in fieldBuffer)
				{
					FieldData data = GameData.GetFieldData(f);
					if (f.CurrentMonopolyRank+1==MonopolyRank.Holding)
						GManager.currentPlayer.Cash+=data.MBuildPrice;
					else
						GManager.currentPlayer.Cash+=data.BuildPrice;
					q+=(data.ID-1).ToString()+"|";
					text += "[FFFFFF]"+data.FieldName+"[-],";
					ChangeColor(f,Color.white);
				}
				q = q.Substring(0,q.Length-1);
				text = text.Substring(0,text.Length-1)+".";
				GManager.LogToMainChat(text,GManager.currentPlayer);
				GManager.LogToSystemChat(q);
				GManager.UpdateUserData(GManager.currentPlayer,true);
			} else
			{
				foreach (var f in fieldBuffer)
				{
					f.CurrentMonopolyRank++;
					ChangeColor(f,Color.white);
					f.Price = GameData.GetFieldData(f).GetCostByRank(f.CurrentMonopolyRank);
				}
			}
			break;

		case GameMode.Transaction:
			Transaction.HideDialog();
			var tinfo = Transaction.GetInfo();
			if (Allow && tinfo.FieldInds.Count!=0)
				GManager.LogToSystemChat("Transaction_"+JSONSerializer.Serialize(tinfo));
			foreach (var f in fieldBuffer)
				ChangeColor(f,Color.white);
			break;

		case GameMode.TransactionView:
			Transaction.HideDialog();
			var toutinfo = Transaction.GetInfo();
			toutinfo.Status = Allow?1:2;
			GManager.LogToSystemChat("Transaction_"+JSONSerializer.Serialize(toutinfo));
			GManager.EndStep(true);
			foreach (var f in fieldBuffer)
				ChangeColor(f,Color.white);
			break;
		}
		currentMode = GameMode.None;
		GManager.UpdateUserData(GManager.currentPlayer,false);
		fieldBuffer.Clear();
		// Вернем управление кнопочками игровому менеджеру
		GManager.SubscribeButtonsEvents();
		GManager.UpdateButtons(true);
		// сбросим данные полей
		currentMode = GameMode.None;
		while (inactiveButtons.Count>0)
		{
			TurnLightButton(inactiveButtons[0],true);
			inactiveButtons.RemoveAt(0);
		}
		while (inactiveObjects.Count>0)
		{
			TurnLightObject(inactiveObjects[0],true);
			inactiveObjects.RemoveAt(0);
		}
	}

	/// <summary>
	/// Включить или включить затемнение. Потушенные поля не будут давать отчеты о нажатии.
	/// </summary>
	/// <param name="Field">Field.</param>
	/// <param name="TurnOn">If set to <c>true</c> turn on.</param>
	private void TurnLight(GameField Field, bool TurnOn)
	{
		if (Field.Effect == GameField.FieldEffects.GameEffect)
		{
			UIButton targetBut = Field.targetSprite.GetComponent<UIButton>();
			TurnLightButton(targetBut,TurnOn);
			inactiveButtons.Add(targetBut);
			TurnLightObject(Field.targetPriceField,TurnOn);
			inactiveObjects.Add(Field.targetPriceField);
			TurnLightObject(Field.targetPriceField.transform.parent.gameObject,TurnOn);
			inactiveObjects.Add(Field.targetPriceField.transform.parent.gameObject);
			TurnLightObject(Field.lockerSptite,TurnOn);
			inactiveObjects.Add(Field.lockerSptite);
		}
		else
		{
			TurnLightObject(Field.targetSprite,TurnOn);
			inactiveObjects.Add(Field.targetSprite);
		}
	}

	/// <summary>
	/// Сменить цвет поля
	/// </summary>
	/// <param name="Field">Field.</param>
	/// <param name="TargetColor">Target color.</param>
	private void ChangeColor(GameField Field, Color TargetColor)
	{
		if (Field.Effect == GameField.FieldEffects.GameEffect)
		{
			UIButton targetBut = Field.targetSprite.GetComponent<UIButton>();
			TurnLightButton(targetBut,TargetColor);
			TurnLightObject(Field.targetPriceField,TargetColor);
			TurnLightObject(Field.targetPriceField.transform.parent.gameObject,TargetColor);
			TurnLightObject(Field.lockerSptite,TargetColor);
		}
	}

	// архитектура рулит, нужно делать 2 отдельных аниматора на затемнение 
	// отдельно тела и отдельно лейбы с ценой у каждой фишки
	private void TurnLightButton(GameObject Target, bool TurnOn)
	{
		Color tcol = TurnOn?TintNormalColor:TintShadowColor;
		UIButton but = Target.GetComponent<UIButton>();
		but.disabledColor = tcol;
		but.SetState(TurnOn?UIButtonColor.State.Normal:UIButtonColor.State.Disabled,true);
		but.defaultColor = tcol;
		but.hover = tcol;
		but.pressed = tcol;
	}

	private void TurnLightButton(UIButton Target, bool TurnOn)
	{
		Color tcol = TurnOn?TintNormalColor:TintShadowColor;
		Target.disabledColor = tcol;
		Target.SetState(TurnOn?UIButtonColor.State.Normal:UIButtonColor.State.Disabled,true);
		Target.defaultColor = tcol;
		Target.hover = tcol;
		Target.pressed = tcol;
	}

	private void TurnLightButton(UIButton Target, Color Col)
	{
		Color tcol = Col;
		Target.disabledColor = tcol;
		Target.SetState(Target.state == UIButtonColor.State.Disabled ?UIButtonColor.State.Normal:UIButtonColor.State.Disabled,true);
		Target.defaultColor = tcol;
		Target.hover = tcol;
		Target.pressed = tcol;
	}

	private void TurnLightObject(GameObject Target, bool TurnOn)
	{
		TweenColor tween = NGUITools.AddMissingComponent<TweenColor>(Target);
		tween.from = TurnOn?TintShadowColor:TintNormalColor;
		tween.to = TurnOn?TintNormalColor:TintShadowColor;
		tween.style = UITweener.Style.Once;
		tween.duration = 0.2f;
		tween.Play();
	}

	private void TurnLightObject(GameObject Target, Color col)
	{
		TweenColor tween = NGUITools.AddMissingComponent<TweenColor>(Target);
		tween.from = tween.value;
		tween.to = col;
		tween.style = UITweener.Style.Once;
		tween.duration = 0.2f;
		tween.Play();
	}
}
