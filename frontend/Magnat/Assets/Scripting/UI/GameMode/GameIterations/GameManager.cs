using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public partial class GameManager : MonoBehaviour 
{
	public static int JackpotWinCash = 400000;

	public PlayersManager GameFieldsManager;
	public ControlPanelButtonManager ButtonsManager;
	public ChatTabsController ChatManager;
	public ControlPanelHeaderManager GameInfoManager;
	public PlayersGridControl PlayersGrid; 
	public ServerData ServerDataManager;
	public SystemChatConnector SystemChat;
	
	public int StepIterationTime = 70;
	
	private Player[] players;
	public Player[] Players { get { return players; } }
	private int currentPlayerID;
	public int CurrentPlayerID { get { return currentPlayerID; } }
	private IterationStep currentState;
	public IterationStep CurrentState { get { return currentState; } }
	private PostEffect currentPostEffect;
	public PostEffect CurrentPostEffect { get { return currentPostEffect; } }
	public Player currentPlayer { get { return players[currentPlayerID]; } }

	// менеджер кнопочек
	private ButtonsRuller buttonsManager;
	public ButtonsRuller BRuller { get { return buttonsManager; } }
	// время старта игры
	private double startTime = -1;
	// время таймера
	private double timerTime;
	// время последнего добавления сообщения для отправки в системный чат
	float lastSystemMessageAdded = 0;
	// стек сообщений для отправки в системный чат
	Stack<string> sendQueue = new Stack<string>();
	// пауза таймера 
	public bool TimerPause = false;

	public System.Action<Player> NextTurn = (p) => {
		if (p.SocialID == SocialManager.User.ViewerId)
			SoundManager.PlayMyStepStart();
	};

	void OnThrowStart ()
	{
		LogToMainChat("Игрок {0} проходит через старт и получает [FFFFFF]$400 000[-].",currentPlayer);
		currentPlayer.Cash += 400000;
		UpdateUserData(currentPlayer,false);
	}
	
	void OnPlayerEndStep()
	{
		// в оборот его!
		MakeStepIteration(currentPlayerID);
	}

	public bool MyTeam(GameField.Owners OwnerID)
	{
		if (players.Length!=4)
			return false;
		string UID = GetPlayerByOwnerID(OwnerID).SocialID;
		return players[0].SocialID == UID && players[1].SocialID == SocialManager.User.ViewerId ||
			players[1].SocialID == UID && players[0].SocialID == SocialManager.User.ViewerId ||
				players[2].SocialID == UID && players[3].SocialID == SocialManager.User.ViewerId ||
				players[3].SocialID == UID && players[2].SocialID == SocialManager.User.ViewerId;
	}
	
	public void SetState(IterationStep Step)
	{
		currentState = Step;
	}
	
	public void SetPostEffect(PostEffect Effect)
	{
		currentPostEffect = Effect;
	}
	
	public void UpdateButtons(bool Visible)
	{
		if (Visible)
			buttonsManager.ShowButtons();
		else
			buttonsManager.HideButtons();
	}
	
	
	void OnUserChatSubmint (string To, string Message)
	{
		// отправим сообщения в комнату
		LogToSystemChat("UserChatMessage_"+To+"|"+Message);
	}
	
	public void LogToMainChat(string Message, Player Player)
	{
		string text = string.Format(Message,ChatManager.MakeColoredBB(Player.OwnerID,Player.LongName));
		text = "[b1b0b0]"+text+"[-]";
		ChatManager.AddMessage(0,text);
	}

	public void LogToMainChat(string Message, Player Player1, Player Player2)
	{
		string text = string.Format(Message,ChatManager.MakeColoredBB(Player1.OwnerID,Player1.LongName),
		                            ChatManager.MakeColoredBB(Player2.OwnerID,Player2.LongName));
		text = "[b1b0b0]"+text+"[-]";
		ChatManager.AddMessage(0,text);
	}
	
	public void LogToSystemChat(string Message)
	{
		sendQueue.Push(Message);
		lastSystemMessageAdded = Time.time;
	}

	IEnumerator SystemChatSender()
	{
		while (true)
		{
			if ((Time.time - lastSystemMessageAdded >0.3f) && sendQueue.Count>0)
			{
				string sendData = "";
				while (sendQueue.Count>0)
				{
					sendData+=WWW.EscapeURL(sendQueue.Pop());
					if (sendQueue.Count>0) sendData+='&';
				}
				SystemChat.SendChatMessage(sendData);
				print ("merged message send = "+sendData);
			}
			yield return null;
		}
	}
	
	private void OnTimerEnd()
	{
		SetBankrot(currentPlayer.SocialID);
	}
	
	private void IncCurrentPlayer(string Player, bool SendSystem)
	{
		if (string.IsNullOrEmpty(Player))
		{
			do
			{
				currentPlayerID++;
				if (currentPlayerID>=players.Length)
					currentPlayerID=0;
			} while (currentPlayer.Bankrout);
            if (SendSystem)
            {
                UpdateUserData(currentPlayer, true);
                SystemChat.SendChatMessage("NextUser_" + currentPlayer.SocialID + "|" +
                                           TimeTools.GetUTCTimeStamp().ToString());
            }
		} else
		{
			do
			{
				currentPlayerID++;
				if (currentPlayerID>=players.Length)
					currentPlayerID=0;
			} while (currentPlayer.SocialID != Player);
		}
		RestartTimer();
	}
	
	private void RestartTimer()
	{
		StopCoroutine("StartTimer");
		SoundManager.StopStepOverSound();
		TimerPause = false;
		StartCoroutine("StartTimer");
		timerTime = TimeTools.GetUTCTimeStamp();
	}

	private void RestartTimer(double StartTime)
	{
		RestartTimer();
		timerTime = StartTime;
	}
	
	IEnumerator StartTimer()
	{
		int time = StepIterationTime;
		bool stepEnding = false;
		while (time>=0)
		{
			PlayersGrid.SetTime(currentPlayer.OwnerID,time);
			yield return null;
			if (TimerPause)
				timerTime = TimeTools.GetUTCTimeStamp() + time - StepIterationTime;
			else
			{
				if ( time < 10 && !stepEnding)
				{
					stepEnding = true;
					SoundManager.PlayStepOverSound();
				}
				time=(int)((timerTime+StepIterationTime) - TimeTools.GetUTCTimeStamp());
			}
		}
		OnTimerEnd();
	}
	
	public void ExitToLobbyButtonClick()
	{
		Application.LoadLevel(1);
	}
}