using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public partial class GameManager : MonoBehaviour 
{
    public static GameManager Instance { get; private set; }

	void Awake()
	{
        Instance = this;
		#if UNITY_EDITOR
		if (FindObjectOfType<SocialManager>() == null)
			gameObject.AddComponent<SocialManager>();
		#endif
		buttonsManager = new ButtonsRuller(this);
	}
	
	void Start()
	{
		// с тартуем с присвоения обработчиков событий
		GameFieldsManager.OnPlayerEndStep += OnPlayerEndStep;
		ChatManager.OnChatSubmint += OnUserChatSubmint;
		GameFieldsManager.OnPlayerThrowStart += OnThrowStart;
		SubscribeButtonsEvents();
		// завози это мясо!
		Write("Запуск игрового менеджера..");
		try
		{
			Init();

			// подтянем инфу о акциях
			ServerInfo.Instance.GetUserActions(SocialManager.User.ViewerId,(actions)=>{
				this.userActions = new List<UserAction>(actions);
			});

			// подтянем инфу о клубных картах
			InitClubCards();
		} catch (System.Exception e)
		{
			LogToMainChat("Ошибка инициализации: "+e.Message+e.StackTrace);
		}
	}

	public void SubscribeButtonsEvents()
	{
		// *** кнопачки.... 11 *** кнопачек.... как они *** уже...
		ButtonsManager.OnAuctionButton += buttonsManager.OnAuctionButton;
		ButtonsManager.OnThrowDiceButton += buttonsManager.OnThrowDiceClick;
		ButtonsManager.OnBuyButton += buttonsManager.OnBuyButton;
		ButtonsManager.OnPayButton += buttonsManager.OnPayButton;
		ButtonsManager.OnLayButton += buttonsManager.OnLayButton;
		ButtonsManager.OnToBuyButton += buttonsManager.OnToBuyButton;
		ButtonsManager.OnBuildButton += buttonsManager.OnBuildButton;
		ButtonsManager.OnSellButton += buttonsManager.OnSellButton;
		ButtonsManager.OnEscapeButton += buttonsManager.OnEscapeButton;
        // и то 2 провтыкал...
        ButtonsManager.OnKeepCreditButton += buttonsManager.OnKeepCreditButton;
        ButtonsManager.OnReturnCreditButton += buttonsManager.OnReturnCreditButton;
	}
	
	public void UnsubscribeButtonsEvents()
	{
		ButtonsManager.OnAuctionButton -= buttonsManager.OnAuctionButton;
		ButtonsManager.OnThrowDiceButton -= buttonsManager.OnThrowDiceClick;
		ButtonsManager.OnBuyButton -= buttonsManager.OnBuyButton;
		ButtonsManager.OnPayButton -= buttonsManager.OnPayButton;
		ButtonsManager.OnLayButton -= buttonsManager.OnLayButton;
		ButtonsManager.OnToBuyButton -= buttonsManager.OnToBuyButton;
		ButtonsManager.OnBuildButton -= buttonsManager.OnBuildButton;
		ButtonsManager.OnSellButton -= buttonsManager.OnSellButton;
		ButtonsManager.OnEscapeButton -= buttonsManager.OnEscapeButton;
        ButtonsManager.OnKeepCreditButton -= buttonsManager.OnKeepCreditButton;
        ButtonsManager.OnReturnCreditButton -= buttonsManager.OnReturnCreditButton;
	}
	
	void OnDestroy()
	{
		StopCoroutine("SystemChatSender");
		// удалим обработчики перед удалением обьекта - чтоб беды не было потом
		GameFieldsManager.OnPlayerEndStep -= OnPlayerEndStep;
		if (ChatManager != null)
			ChatManager.OnChatSubmint -= OnUserChatSubmint;
		if (SystemChat != null)
			SystemChat.OnMessageReacived -= OnServerMessage;
		// и эти гребанные кнопочки тоже....
		UnsubscribeButtonsEvents();
	}

	private Player GetDefaultPlayer()
	{
		Player p = new Player()
		{
			SocialID = "",
			OwnerID = GameField.Owners.None,
			Cash = 2000000,
			Capital = 0
		};
		p.OnBankrotStart += SetBankrot;
		return p;
	}

	void Init()
	{
		// инициализируем комнатушку
		// начнем с прописывания данных в шапку панели управления игрой
		GameInfo info = ServerDataManager.GetGameInfo();
		GameInfoManager.Bet = info.Bet;
		GameInfoManager.Bank = info.Bet * info.PlayersCount;
		GameInfoManager.GameID = info.GUID;
		
		Write("Запускаю инициализацию..");
		players = GetPlayerList();	
		Write("Список пользователей игры загружен..");
		SystemChat.Init(ServerDataManager.GetUserGameID());
		StartCoroutine("SystemChatSender");
		SystemChat.OnMessageReacived += OnServerMessage;
		Write("Системный чат инициализирован..");
		Write("Инициализирую алгоритм определения очереди..");
		List<GameField.Owners> colors = new List<GameField.Owners>();
		colors.Add(GameField.Owners.Blue);
		colors.Add(GameField.Owners.Green);
		colors.Add(GameField.Owners.Orange);
		colors.Add(GameField.Owners.Purple);
		colors.Add(GameField.Owners.Red);
		for (int i =0;i<players.Length;i++)
		{
			string id = players[i].SocialID;
			players[i] = GetDefaultPlayer();
			players[i].SocialID = id;
			var data = SocialManager.GetUserData(id);
			#if UNITY_EDITOR
			// чтоб можно было с юнити тестить игровой режим - забьем хоть какие-нибудь данные
			data = new SocialData()
			{
				FirstName = "FName",
				LastName = "LName",
				Photo = ""
			};
			#else
			if (data == null) throw new System.Exception("Не найден пользователь "+id+" в БД игроков");
			#endif
			players[i].FirstName = data.FirstName;
			players[i].LastName = data.LastName;
			players[i].PhotoURL = data.Photo;
			int col = Random.Range(0,colors.Count);
			players[i].OwnerID = colors[col];
			colors.RemoveAt(col);
		}
		try
		{
			Write("Инициализация игровых модулей");
			if (info.GameType == 0)
				PlayersGrid.Init(players);
			else
				PlayersGrid.InitTwoVSTwo(players);
			Write("Доска пользователей инициализирована");
			GameFieldsManager.Init(players);
			Write("Игровые фишки инициализированны");
			
			// remove me from chat
			List<Player> other = (new List<Player>(players));
			for (int i=0;i<other.Count;i++)
				if (other[i].SocialID == SocialManager.Instance.ViewerID)
			{
				other.RemoveAt(i);
				break;
			}
			ChatManager.Init(other.ToArray());
			Write("Доска чатов инициализированна");	
		} catch (System.Exception e)
		{
			Write("Ошибка инициализации: "+e.Message+"\r\n"+e.StackTrace);
			LogToMainChat("Ошибка инициализации комнаты.");
		}
		LogToSystemChat(string.Format("Connected_{0}|{1}",SocialManager.Instance.ViewerID,TimeTools.GetUTCTimeStamp().ToString()));
		// инициализация окончена. в путь, первый игрок.
		SetState(IterationStep.Start);
		SetPostEffect(PostEffect.None);
		
		#if UNITY_EDITOR
		// тест диалогов
		for (int i=0;i<40;i++)
		{
			GameField f = GameFieldsManager.Manager.GamePoolSteps[i];
			if (f.Effect == GameField.FieldEffects.GameEffect)
			{
				f.Owner = Players[i%2].OwnerID;
				//f.CurrentMonopolyRank = MonopolyRank.Branch1;
				f.Price = GameFieldsManager.Manager.GetFieldData(f).GetCostByRank(f.CurrentMonopolyRank);
			}
		}
		#endif
		
		MakeStepIteration(0);
		RestartTimer();
		startTime = TimeTools.GetUTCTimeStamp();
	}
}