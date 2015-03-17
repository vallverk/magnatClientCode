using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public class GameInfoController : MonoBehaviour 
{
	public enum GameMode
	{
		Normal,
		TwoVSTwo
	}

	public UILabel GameNameLabel = null;
	public UILabel GameIDLabel = null;
	public UILabel BetLabel = null;
	public UISprite VSSprite = null;
	public UISprite LockerSprite = null;
	public GameObject VipPlayerPrefab = null;
	public GameObject PlayerPrefab = null;
	public GameObject AddButtonPrefab = null;

	public GameMode GameType;
	public string GameName;
	public long GameID;
	public List<string> Players = null;
	public int Bet;
	public string Password;
	public int PlayersCount;

	public float FirstIconStartX = 0;
	public float IconOffsetX = 100;

	private GameObject[] icons;


    private static long _connectedGameID = -1;
    public static long ConnectedGameID {
        get
        {
            return _connectedGameID;
        }
        set
        {
            _connectedGameID = value;
        }
    } 

    public static List<GameInfoController> GameInfoControllers = new List<GameInfoController>();


    private static Action DisconectCallback; 

    private void OnEnable()
    {
        GameInfoControllers.Add(this);
    }

    private void OnDisable()
    {
        GameInfoControllers.Remove(this);
    }

    public static void DisconectFromAll(Action disconectCallback = null)
    {
        DisconectCallback = disconectCallback;

        foreach (GameInfoController controller in GameInfoControllers)
        {
            if (controller.Players.Contains(SocialManager.Instance.ViewerID))
            {
                controller.Disconnect();
                return;
            }
        }

        GameInfoController.ConnectedGameID = -1;

        if (DisconectCallback != null)
            DisconectCallback();

    }
     
    public void SetInfo(GameInfo info, bool andUpdate)
	{
		GameType = info.GameType == 0? GameMode.Normal:GameMode.TwoVSTwo;
		GameName = info.GameName;
		PlayersCount = info.PlayersCount;
		Bet = info.Bet;
		Password = info.Password;
		Players = info.UserList;
		foreach (string uid in Players)
			if (uid == SocialManager.Instance.ViewerID)
				ConnectedGameID = info.GUID;
		GameID = info.GUID;
		if (icons == null)
			icons = new GameObject[PlayersCount];
		if (andUpdate)
			UpdateVisual();
	}

	public void Connect()
	{
	    GameInfoController.DisconectFromAll(() =>
	    {
	        ServerInfo.Instance.GetUserInfo(SocialManager.User, (u) =>
	        {
	            if (u.Gold < Bet)
	                AlertWindow.Show("ОШИБКА",
	                    "У вас не достаточно золота для вступления в игру");
	            else
	            {
	                if (u.GameCards <= 0 && u.VIP == 0)
	                    GameObject.FindObjectOfType<NoCards>().Show();
	                else
	                {
	                    if (Password == "-")
	                    {
	                        // пароля нет, зягружаем инфу...
	                        RlyConnect();
	                    }
	                    else
	                    {
	                        // проверка на пароль
	                        GetTextDialogWindow.Show("ВВЕДИТЕ ПАРОЛЬ", "", (pass) =>
	                        {
	                            if (pass == null)
	                            {
	                            }
	                            //AlertWindow.Show("ОШИБКА","Пароль для приватной игры введен не правильно");
	                            else if (Password != MD5Convertor.getMd5Hash(pass))
	                                AlertWindow.Show("ОШИБКА",
	                                    "Пароль для приватной игры введен не правильно");
	                            else
	                                RlyConnect();
	                        });
	                    }
	                }
	            }
	        }, false);
	    });
	}

	private void RlyConnect()
	{
		if (this.GameType == GameMode.TwoVSTwo)
		{
			RlyConnect(int.Parse(UIButton.current.name)>1?2:1);
			return;
		}
		if (ConnectedGameID!=-1)
			Disconnect();
		if (ConnectedGameID == -1)
			ServerInfo.Instance.ConnectToGameAnnounce(GameID,"",(res)=>{if (res) ConnectedGameID = GameID;});
	}

	private void RlyConnect(int Team)
	{
		if (ConnectedGameID!=-1)
			Disconnect();
		if (ConnectedGameID == -1)
			ServerInfo.Instance.ConnectTo2x2GameAnnounce(GameID,"",Team,(res)=>{
				//if (res) ConnectedGameID = GameID;
				if (res == 200)
					ConnectedGameID = GameID;
			});
	}
	
	public void Disconnect()
	{
	    //DisconectCallback = disconectCallback;
        Debug.Log(ConnectedGameID);
		if (ConnectedGameID!=-1)
		{
            
			if (GameType == GameMode.Normal)
				ServerInfo.Instance.DisconnectFromGameAnnounce(ConnectedGameID, (res) =>
				{
				    if (res)
				    {
                        if (DisconectCallback != null)
                            DisconectCallback();
				        ConnectedGameID=-1;
				    }


				});
			else
				ServerInfo.Instance.DisconnectFrom2x2GameAnnounce(ConnectedGameID, (res) =>
				{
				    if (res)
				    {
                        if (DisconectCallback != null)
                            DisconectCallback();
				        ConnectedGameID=-1;
				    }
				});
			ConnectedGameID = -1;
		}
	}


	void UpdateVisual()
	{
		if (GameNameLabel != null)
			GameNameLabel.text = GameName;

		if (GameIDLabel != null)
			GameIDLabel.text = "№"+GameID.ToString();

		if (BetLabel != null)
			BetLabel.text = Bet.ToString();

		LockerSprite.enabled = !string.IsNullOrEmpty(this.Password) && this.Password != "-";

		VSSprite.enabled = GameType == GameMode.TwoVSTwo;

		for (int i=0;i<this.PlayersCount;i++)
		{
			// если иконка не инициализированна - создадим ее
			if (icons[i] == null)
				CreateIcon(i);
			else
				// если иконца уже инициализированна
			{
				GameListPlayerAvatar glpa = icons[i].GetComponent<GameListPlayerAvatar>();
				// если это иконка добавления в игру
				if (glpa == null)
				{
					if (i < Players.Count && !string.IsNullOrEmpty(Players[i]))
					{
						// а юзер то уже подключился...
						GameObject.Destroy(icons[i]);
						CreateIcon(i);
					}
				} else
				{
					if ((i < Players.Count && glpa.GUID!=Players[i]) ||
					    (i >= Players.Count))
					{
						// что-то пошло не так, перезальем его
						GameObject.Destroy(icons[i]);
						CreateIcon(i);
					}
				}
			}
		}
	}

	private void CreateIcon(int iconInd)
	{
		if (iconInd<Players.Count && !string.IsNullOrEmpty(Players[iconInd]))
			// игрок уже определен
		{
			// узнаем ни вип ли он
			ServerInfo.Instance.GetUserInfo(new string[1] {Players[iconInd]},(u)=>{
				var user = u[0];
				int ind = Players.IndexOf(user.GUID);
				
				GameObject go=null;
				if (user.VIP == 0)
					go = NGUITools.AddChild(gameObject,PlayerPrefab);
				else
					go = NGUITools.AddChild(gameObject,VipPlayerPrefab);
				
				float x = FirstIconStartX + IconOffsetX * ind;
				if (GameType == GameMode.TwoVSTwo && ind>1)
					x+=IconOffsetX;
				go.transform.localPosition = new Vector3(x,0,0);
				
				GameListPlayerAvatar glpa = go.GetComponent<GameListPlayerAvatar>();
				
				glpa.Init(user.GUID);
				if (Players[iconInd] == SocialManager.Instance.ViewerID)
				{
					UIButton but = go.GetComponent<UIButton>();
					but.onClick.Clear();
					but.onClick.Add(new EventDelegate(Disconnect));
				}
				
				icons[ind] = go;
			});
		}
		else
		{
			// иконка добавления нового юзера
			GameObject go = NGUITools.AddChild(gameObject,AddButtonPrefab);
			go.name = iconInd.ToString();
			float x = FirstIconStartX + IconOffsetX * iconInd;
			if (GameType == GameMode.TwoVSTwo && iconInd>1)
				x+=IconOffsetX;
			go.transform.localPosition = new Vector3(x,0,0);
			
			if (ConnectedGameID != GameID)
			{
				UIButton but = go.GetComponent<UIButton>();
				but.onClick.Clear();
				but.name=iconInd.ToString();
				but.onClick.Add(new EventDelegate(Connect));
			}
			
			icons[iconInd] = go;
		}
	}
}



























