using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public partial class GameManager : MonoBehaviour 
{
	void OnServerMessage (string Room, string UID, string Message)
	{
		string[] data = Message.Split('&');
		for (int i =0;i<data.Length;i++)
			ServerMessageProc(Room,UID,WWW.UnEscapeURL(data[i]));
	}
	
	void ServerMessageProc(string Room, string UID, string Message)
	{
		try
		{
			//Write("[Системное сообщение] "+Message);
			
			string[] mm = Message.Split('_');
			string[] args;
			switch (mm[0])
			{
			case "UserChatMessage":
			{
				// если сообщение из чата - выведем соощение в чат...
				string[] MessageData = mm[1].Split('|');
				if (MessageData[0] == "Main")
					LogToMainChat("{0}: "+MessageData[1],GetPlayerBySocialID(UID));
				else
					if (SocialManager.Instance.ViewerID == MessageData[0]) 
				{
					// нам пришли сообщения
					if (MessageData[0] != "Main")
						SoundManager.PlayPrivatMessageReceive();

					Player senderData = GetPlayerBySocialID(UID);
					string UserChatMessage = string.Format("{0}: "+MessageData[1],ChatManager.MakeColoredBB(senderData.OwnerID,senderData.LongName));
					ChatManager.AddMessageByChatName(UID,UserChatMessage);
				} else
					if (SocialManager.Instance.ViewerID == UID)
				{
					// мы отправили сообщение
					if (MessageData[0] != "Main")
						SoundManager.PlayPrivatMessageSend();

					Player senderData = GetPlayerBySocialID(UID);
					string UserChatMessage = string.Format("{0}: "+MessageData[1],ChatManager.MakeColoredBB(senderData.OwnerID,senderData.LongName));
					ChatManager.AddMessageByChatName(MessageData[0],UserChatMessage);
				}
			} 
				break;
			case "Auction":
			{
				AuctionInfo ainfo = JSONSerializer.Deserialize<AuctionInfo>(mm[1]);
				GetComponent<AuctionController>().MakeAuctionIteration(ainfo);
			} 
				break; 
			case "Transaction":
			{
				TransactionInfo tinfo = JSONSerializer.Deserialize<TransactionInfo>(mm[1]);
				switch (tinfo.Status)
				{
				case 0:
					TimerPause = true;
					buttonsManager.HideButtons();
					if (tinfo.Player2 == SocialManager.Instance.ViewerID)
                    {
                        GetComponent<GameModeManager>().ShowTransactionWindow(tinfo);
                        
                    }
					else
						LogToMainChat("Игрок {0} предложил сделку игроку {1}.",GetPlayerBySocialID(tinfo.Player1),
						              GetPlayerBySocialID(tinfo.Player2));
					break;
				case 1:
				{
					string p1fields = "";
					string p2fields = "";
					
					// если успешно завершен диалог - поменяем фишки и деньги
					Player p1 = GetPlayerBySocialID(tinfo.Player1);
					Player p2 = GetPlayerBySocialID(tinfo.Player2);
					List<GameField> toUpdate = new List<GameField>();

					if (SocialManager.User.ViewerId == p1.SocialID ||
					    SocialManager.User.ViewerId == p2.SocialID)
						SoundManager.PlayTransactionSucces();
					
					foreach (int id in tinfo.FieldInds)
					{
						FieldData tdata = GameFieldsManager.Manager.Fields[id-1];
						GameField tfield = GameFieldsManager.Manager.GetGameField(tdata);
						toUpdate.Add(tfield);
						if (tfield.Owner == p1.OwnerID)
						{
							tfield.Owner = p2.OwnerID;
							p1.Capital -= tdata.BuyPrice;
							p2.Capital += tdata.BuyPrice;
							p1fields += "[FFFFFF]"+tdata.FieldName+"[-],";
						}
						else
						{
							tfield.Owner = p1.OwnerID;
							p1.Capital += tdata.BuyPrice;
							p2.Capital -= tdata.BuyPrice;
							p2fields += "[FFFFFF]"+tdata.FieldName+"[-],";
						}
						
						tfield.CurrentMonopolyRank = MonopolyRank.Base;
						tfield.Price = tdata.GetCostByRank(tfield.CurrentMonopolyRank);
					}
					string message = "Игрок {1} принял сделку. ";
					if (p1fields.Length>0)
					{
						p1fields = p1fields.Substring(0,p1fields.Length-1);
						message += "Игрок {1} получил поля "+p1fields+". ";
					}
					if (p2fields.Length>0)
					{
						p2fields = p2fields.Substring(0,p2fields.Length-1);
						message += "Игрок {0} получил поля "+p2fields+". ";
					}
					message += " Доплата со стороны "+(tinfo.Supplement>0?"{0}":"{1}")+": "+Mathf.Abs(tinfo.Supplement).ToString("$### ### ##0");
					LogToMainChat(message,p1,p2);
					
					p1.Cash += tinfo.Supplement;
					p2.Cash -= tinfo.Supplement;
					
					UpdateUserData(p1,false);
					UpdateUserData(p2,false);
					
					// и обновим данные монополий
					while (toUpdate.Count>0)
					{
						AliasData talias = GameFieldsManager.Manager.GetAliasFromField(toUpdate[0]);
						GameField[] taf = GameFieldsManager.Manager.GetFieldInAlias(talias);
						foreach (var del in taf)
							toUpdate.Remove(del);
						// но, для начала, обработаем случай, если пользователь приобрел город
						if (GameFieldsManager.Manager.IsCity(talias))
						{
							int owner1Of = 0;
							int owner2Of = 0;
							foreach (var field in taf)
							{
								if (field.Owner == p1.OwnerID)
									owner1Of++;
								if (field.Owner == p2.OwnerID)
									owner2Of++;
							}
							// проставим им правильно ранг и цены
							foreach (var field in taf)
							{
								if (field.Owner == p1.OwnerID)
									field.CurrentMonopolyRank = MonopolyRank.Monopoly+owner1Of;
								if (field.Owner == p2.OwnerID)
									field.CurrentMonopolyRank = MonopolyRank.Monopoly+owner2Of;
								if (field.Owner != GameField.Owners.None)
									field.Price = GameFieldsManager.Manager.GetFieldData(field).GetCostByRank(field.CurrentMonopolyRank);
								else
									field.Price = GameFieldsManager.Manager.GetFieldData(field).BuyPrice;
							}
						}
						else
						{
							bool oneOwner = true;
							for (int i=1;i<taf.Length;i++)
								if (taf[0].Owner != taf[i].Owner)
							{
								oneOwner = false;
								break;
							}
							if (oneOwner)
							{
								foreach (var field in taf)
								{
									field.CurrentMonopolyRank = MonopolyRank.Monopoly;
									field.Price = GameFieldsManager.Manager.GetFieldData(field).GetCostByRank(field.CurrentMonopolyRank);
								}
							}
						}
					}
				}
					TimerPause = false;
					break;
				case 2:
					if (SocialManager.User.ViewerId == tinfo.Player1 ||
					    SocialManager.User.ViewerId == tinfo.Player2)
						SoundManager.PlayTransactionSucces();
					LogToMainChat("Игрок {0} отказался от сделки.",GetPlayerBySocialID(tinfo.Player2));
					if (tinfo.Player1 == SocialManager.Instance.ViewerID)
						buttonsManager.ShowButtons();
					TimerPause = false;
					break;
				}
			}
				break;
				
			case "MakeEffect":
			{
				int effectPlayerID = GetPlayerIDBySocialID(UID);
				Player effectPlayer = Players[effectPlayerID];
				string text = "{0} попал на поле ";
				if (mm[1].Contains("|"))
					args = mm[1].Split('|');
				else
				args = new string[1] {mm[1]};
				switch (args[0])
				{
				case "Customs":
					text += "таможня. ";
					GameFieldsManager.GoToVacation(effectPlayerID);
					break;
					
				case "Jackpot":
					text+="джекпот. Выигрышь: [FFFFFF]"+JackpotWinCash.ToString("### ### ##0")+" $[-]";
					break;
					
				case "Lottery":
					text+="лотерея. Выигрышь: [FFFFFF]"+(int.Parse(args[1])).ToString("### ###")+" $[-]";
					break;
					
				case "SkipStep":
					text+="пропуска хода. Следующий ход будет пропущен.";
					break;
					
				case "StepBack":
					text+="хода назад. Следующий ход будет в обратной направленности.";
					break;
					
				case "Vacation":
					text+="отпуск.";
					GameFieldsManager.GoToCustoms(effectPlayerID);
					break;

				case "Step":
					break;
				}
				LogToMainChat(text,effectPlayer);
				break;
			    }

                case "CustomsFriend":
                {
                    LogToMainChat("Игрок {0} воспользовался картой \"Друг таможенника\".", GetPlayerBySocialID(UID));
                }
                break;
			}
			// в противном случае если пакет пришел от нас - смысла обрабатывать его нету
			if (UID == SocialManager.Instance.ViewerID) return;
			// до нас дошел твой месседж, mr. FreeMen
			switch (mm[0])
			{
                case "NoneStep":
                    LogToMainChat("{0} пропускает ход",GetPlayerBySocialID(mm[1]));
                    break;

                case "PayForField":
                    args = mm[1].Split('|');
                    Player angry = GetPlayerBySocialID(args[0]);
                    Player happy = GetPlayerBySocialID(args[1]);
                    int price = int.Parse(args[2]);

                    string taxText = string.Format("[FFFFFF]{0} $[-] игроку {1}.", price.ToString("### ### ##0"),
                                                   ChatManager.MakeColoredBB(happy.OwnerID, happy.LongName));
				    LogToMainChat("{0} оплатил аренду фирмы [FFFFFF]"+args[3]+"[-] - "+taxText,angry);
                break;

            case "PayForFieldWithAction":
                args = mm[1].Split('|');
                Player angryA = GetPlayerBySocialID(args[0]);
                Player happyA = GetPlayerBySocialID(args[1]);
                int priceA = int.Parse(args[2]);

                //  Игорь, имея пакет акций «Общепит 1», оплачивает аренду игроку Марина – $4,500
                LogToMainChat("{0}, имея пакет акций «" + args[4]+ "», оплатил аренду фирмы [FFFFFF]" + args[3] + "[-] игроку {1}- " +
                    priceA.ToString("### ### ##0 $"), angryA, happyA);
            break;
				
			case "SetBankrout":
				SetBankrot(mm[1]);
				break;
				
			case "NextUser":
				args = mm[1].Split('|');
				IncCurrentPlayer(args[0],false);
				RestartTimer(double.Parse(args[1]));
				MakeStepIteration(-1);
				break;
				
			case "UpdateUser":
				Player p = JSONSerializer.Deserialize<Player>(WWW.UnEscapeURL(mm[1]));
				Player main = GetPlayerBySocialID(p.SocialID);
				main.Cash = p.Cash;
				main.Capital = p.Capital;
				if (!main.Bankrout && p.Bankrout) SetBankrot(main.SocialID);
				GameFieldsManager.SetPosition(GetPlayerIDBySocialID(p.SocialID), p.TablePosition);
				PlayersGrid.UpdateUserData(main.OwnerID,main.Cash,main.Capital);
				break;
				
			case "Connected":
				// отправим напрямую, минуя конвертацию в WEB URL формат
				SystemChat.SendChatMessage("GameInfo_"+JSONSerializer.Serialize(GetCurrentGameData()).Replace('"','\''));
				break;
				
			case "GameInfo":
				// хаха! новенькие данные о игре!
				StartedGameData info = JSONSerializer.Deserialize<StartedGameData>(mm[1].Replace('\'','"'));
				// обновим, если они более свежие, чем наши
				if (info.STT < startTime)
					SetCurrentGameData(info);
				
				break;
				
			case "Step":
				args = mm[1].Split('|');
				IncCurrentPlayer(args[0],false);
				GameFieldsManager.MakeStep(currentPlayerID,int.Parse(args[1]));
				break;
				
			case "StepNone":
				LogToMainChat("Игрок {0} пропускает ход.",GetPlayerBySocialID(mm[1]));
				break;
				
			case "StepBack":
				args = mm[1].Split('|');
				Player setpPlayer = GetPlayerBySocialID(args[0]);
				LogToMainChat("Игрок {0} совершил ход назад.",setpPlayer);
				GameFieldsManager.MakeStep(GetPlayerIDBySocialID(args[0]),-int.Parse(args[1]));
				break;
				
			case "CustomEscapeFail":
				LogToMainChat("Для выхода с таможни должен выпасть дубль, игрок {0} остается на таможне",GetPlayerBySocialID(mm[1]));
				break;
				
			case "CustomEscape":
				LogToMainChat("Игрок {0} выбрасывает дубль и выходит из таможни.",GetPlayerBySocialID(mm[1]));
				break;
				
			case "ThrowDice":
				args = mm[1].Split('|');
				int throwPlayerID = GetPlayerIDBySocialID(args[0]);
				LogToMainChat("{0} выбросил кубики [FFFFFF]"+args[1]+":"+args[2]+"[-]",Players[throwPlayerID]);
				CubesController.DropDice(int.Parse(args[1]),int.Parse(args[2]));
				break;
				
			case "BuyField":
				args = mm[1].Split('|');
				int buyPlayerInd = GetPlayerIDBySocialID(args[0]);
				GameFieldsManager.SetPosition(buyPlayerInd,int.Parse(args[1]));
				Player buyPlayer = Players[buyPlayerInd];
				FieldData data = GameFieldsManager.GetFieldDataAtPlayer(buyPlayer.OwnerID);
				GameField gf = GameFieldsManager.Manager.GetGameField(data);
				gf.Owner = buyPlayer.OwnerID;
				gf.CurrentMonopolyRank = MonopolyRank.Base;
				gf.Price = data.GetCostByRank(gf.CurrentMonopolyRank);
				LogToMainChat("{0} приобрел фирму [FFFFFF]"+data.FieldName+"[-]",buyPlayer);
				// если пользователь приобрел все монополии одной фирмы - запишем их в монополию
				AliasData alias = GameFieldsManager.Manager.GetAliasFromField(gf);
				GameField[] af = GameFieldsManager.Manager.GetFieldInAlias(alias);
				// но, для начала, обработаем случай, если пользователь приобрел город
				if (GameFieldsManager.Manager.IsCity(alias))
				{
					int ownerOf = 0;
					foreach (var field in af)
						if (field.Owner == buyPlayer.OwnerID)
							ownerOf++;
					// проставим им правильно ранг и цены
					foreach (var field in af)
						if (field.Owner == buyPlayer.OwnerID)
					{
						field.CurrentMonopolyRank = MonopolyRank.Monopoly+ownerOf;
						field.Price = (int)( GameFieldsManager.Manager.GetFieldData(field).GetCostByRank(field.CurrentMonopolyRank) * 
						                    GetDiscountOfMonopoly(GameFieldsManager.Manager.GetAliasFromField(field).ID) *
						                    GetClubCardTrueBussinesCoef(args[0]));
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
				break;
				
			case "LayFields":
				args = mm[1].Split('|');
				string layedMessage = "{0} заложил поля ";
				foreach (var id in args)
				{
					FieldData layedData = GameFieldsManager.Manager.Fields[int.Parse(id)];
					layedMessage += "[FFFFFF]"+layedData.FieldName+"[-],";
					layedData.FieldGO.GetComponent<GameField>().Locked = true;
				}
				layedMessage = layedMessage.Substring(0,layedMessage.Length-1)+".";
				LogToMainChat(layedMessage,GetPlayerBySocialID(UID));
				break;
				
			case "ToBuyFields":
				args = mm[1].Split('|');
				string buyOutMessage = "{0} выкупил поля ";
				foreach (var id in args)
				{
					FieldData layedData = GameFieldsManager.Manager.Fields[int.Parse(id)];
					buyOutMessage += "[FFFFFF]"+layedData.FieldName+"[-],";
					layedData.FieldGO.GetComponent<GameField>().Locked = false;
				}
				buyOutMessage = buyOutMessage.Substring(0,buyOutMessage.Length-1)+".";
				LogToMainChat(buyOutMessage,GetPlayerBySocialID(UID));
				break;
				
			case "BuildFields":
				string buildFieldText = "{0} построил филиалы на полях ";
				args = mm[1].Split('|');
				foreach (var id in args)
				{
					FieldData buildData = GameFieldsManager.Manager.Fields[int.Parse(id)];
					GameField gfb = GameFieldsManager.Manager.GetGameField(buildData);
					gfb.CurrentMonopolyRank++;
					gfb.Price = buildData.GetCostByRank(gfb.CurrentMonopolyRank);
					buildFieldText += "[FFFFFF]"+buildData.FieldName+"[-],";
				}
				buildFieldText = buildFieldText.Substring(0,buildFieldText.Length-1)+".";
				LogToMainChat(buildFieldText,GetPlayerBySocialID(UID));
				break;
				
			case "SellFields":
				string sellFieldText = "{0} продал филиалы на полях ";
				args = mm[1].Split('|');
				foreach (var id in args)
				{
					FieldData buildData = GameFieldsManager.Manager.Fields[int.Parse(id)];
					GameField gfb = GameFieldsManager.Manager.GetGameField(buildData);
					gfb.CurrentMonopolyRank--;
					gfb.Price = buildData.GetCostByRank(gfb.CurrentMonopolyRank);
					sellFieldText += "[FFFFFF]"+buildData.FieldName+"[-],";
				}
				sellFieldText = sellFieldText.Substring(0,sellFieldText.Length-1)+".";
				LogToMainChat(sellFieldText,GetPlayerBySocialID(UID));
				break;
				
			case "AuctionFinish":
				if (mm[1] == "NoPlayers")
				{
					LogToMainChat("Аукцион не состоялся");
					EndStep(false);
				}
				break;
			}
		} catch (System.Exception e)
		{
			Debug.Log("Ошибка распознавания сообщения: "+e.Message+"\r\n"+e.StackTrace);
		}
	}
}
