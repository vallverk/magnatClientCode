using UnityEngine;
using System.Collections;
using System;

[RequireComponent(typeof(ChatConnector))]
public class LobbyChat : MonoBehaviour 
{
	private ChatConnector _chat;

	public UIInput ChatInput;

	private System.Collections.Generic.List<string> _newUsers;

	void Start()
	{
		_newUsers = new System.Collections.Generic.List<string>();
		_chat = GetComponent<ChatConnector>();
		_chat.OnConnectedSuccessful += Connected;
		_chat.OnMessageReacived += OnChatMessage;
		_chat.OnConnectFailed += OnConnectFailed;
		_chat.OnChatGetCount += OnChatCount;
	}

	void OnDestroy()
	{
		_chat.OnConnectedSuccessful -= Connected;
		_chat.OnMessageReacived -= OnChatMessage;
		_chat.OnConnectFailed -= OnConnectFailed;
		_chat.OnChatGetCount -= OnChatCount;
		_chat.ServerConnection.Disconnect();
	}

	public void OnSubmint()
	{
		if (ChatInput.value != "")
		{
			_chat.SendChatMessage(ChatInput.value);
			ChatInput.value = "";
		}
	}

	private bool showed = false;
	void OnChatCount (int obj)
	{
		if (!showed)
		{
			ServerInfo.Instance.GetGameAnnounceList((games)=>{
				NGUIDebugConsole.Log(string.Format("[ffd700][{0}] {1} : {2}[-]",
				                                   DateTime.Now.ToString("H:mm:ss"),
				                                   "Администрация",
				                                   "Привет "+SocialManager.User.FirstName+"."));
				NGUIDebugConsole.Log(string.Format("[ffd700][{0}] {1} : {2}[-]",
				                                   DateTime.Now.ToString("H:mm:ss"),
				                                   "Администрация",
				                                   "Рекомендуем сразу вступить " +
				                                   "в [url=http://vk.com/magnatgamegroup]наш клуб[/url]. " +
				                                   "Здесь ты можешь получить любые ответы по игре и не только. " + 
				                                   "Сейчас в игре "+obj+" пользователей и "+games.Length+" игр."));
			});
			showed = true;
		}
	}

	void Connected (string UserID)
	{
#if UNITY_EDITOR 
		if (UserID == GetComponent<ChatConnector>().user_id)
			_newUsers.Add(UserID);
#else
		if (UserID == SocialManager.Instance.UserId)
			_newUsers.Add(UserID);
#endif
	}

	void OnChatMessage (string Room, string UserID, string Message)
	{
	    if (Room == "Lobby")
	    {
	        StartCoroutine(LogMessage(Room, UserID, Message));
	    }
	    //NGUIDebugConsole.Log(string.Format("[{0}] {1} : {2}",DateTime.Now.ToString("H:mm:ss"),GetUserName(UserID),Message));
	}

    private IEnumerator LogMessage(string Room, string UserID, string Message)
    {

        string name = "";
        while (!SocialManager.Instance.SocialData.ContainsKey(UserID))
        {
            SocialManager.GetUserInfo(UserID);
            yield return null;
        }

        name = SocialManager.Instance.SocialData[UserID].FirstName;
        
        NGUIDebugConsole.Log(string.Format("[{0}] {1} : {2}", DateTime.Now.ToString("H:mm:ss"), name, Message));
    }

    void OnConnectFailed (string obj)
	{
		NGUIDebugConsole.Log("Не удалось подключиться к лобби-чату. Обновите страницу.");
		//NGUIDebugConsole.LogSystem("Ошибка подключения: "+obj);
	}
}
