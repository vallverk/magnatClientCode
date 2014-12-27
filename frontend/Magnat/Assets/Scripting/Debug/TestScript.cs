using UnityEngine;
using System.Collections;

public class TestScript : MonoBehaviour 
{
	public UITextList Log;

	private SystemChatConnector SystemChat;

	public void OnInit()
	{
		Log.Add("OnInit");
		SystemChat.Init(123);
	}

	public void OnStop()
	{
		Log.Add("OnStop");
		SystemChat.Disconnect();
	}

	public void OnSendMessage()
	{
		Log.Add("OnSendMessage");
		SystemChat.SendChatMessage("Hello world");
	}

	void Awake()
	{
		SystemChat = GetComponent<SystemChatConnector>();
		SystemChat.OnConnectedSuccessful += (message) => {Log.Add(string.Format("Connected to chat successfuly, [{0}]",message));};
		SystemChat.OnConnectFailed += (message) => {Log.Add(string.Format("Connection failed, [{0}]",message));};
		SystemChat.OnMessageReacived += (room,uid,message) => {Log.Add(string.Format("Recieved -> Room [{0}] UID [{1}] Message [{2}]",room,uid,message));};
		SocialManager.Instance.OnBaseDataLoaded += () => Log.Add("Base social data loaded");
	}
}
