using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.ComponentModel;

[RequireComponent(typeof(ServerPoolSync))]
public class ChatConnector : MonoBehaviour 
{
	public string user_id = "12345678";
	public string room_name = "Lobby";
	public string viewer_id = "15361226";
	public string auth_key = "415fb34a78515e52ccd1c4ab25454111";

	public bool ChatConnected { get; private set; }

	public Action<string> OnConnectFailed = (x) => {};
	public Action<string> OnConnectedSuccessful = (x) => {};
	public Action<string,string,string> OnMessageReacived = (a,b,c) => {};
	public Action<int> OnChatGetCount = (a)=>{};

	ServerPoolSync _server;
	public ServerPoolSync ServerConnection { get { return _server; } }

	void Awake()
	{
		ChatConnected = false;
	}

	void Start()
	{
		_server = GetComponent<ServerPoolSync>();
		_server.OnServerResponse -= OnServerMessage;
		_server.OnError -= OnServerError;
		_server.OnServerResponse += OnServerMessage;
		_server.OnError += OnServerError;


#if UNITY_EDITOR
		OnSocialDataLoaded(null);
		ConnectChat();
#else
		user_id = "";
		viewer_id = "";
		auth_key = "";
		if (SocialManager.Instance.IsLoaded)
			OnSocialDataLoaded(null);
		else
		{
			SocialManager.Instance.OnBaseDataLoaded -= OnBaseSocialDataLoaded;
			SocialManager.Instance.OnBaseDataLoaded += OnBaseSocialDataLoaded;
		}
#endif
	}

	public void Disconnect()
	{
		_server.Disconnect();
		_server.OnServerResponse -= OnServerMessage;
		_server.OnError -= OnServerError;
		ChatConnected = false;
	}

	void OnDestroy()
	{
		Disconnect();
		SocialManager.Instance.OnUserDataLoaded -= OnSocialDataLoaded;
	}

	void OnBaseSocialDataLoaded ()
	{
		SocialManager.Instance.OnUserDataLoaded -= OnSocialDataLoaded;
		NGUIDebugConsole.LogSystem("OnSocialDataLoaded");
		user_id = SocialManager.Instance.UserId;
		viewer_id = SocialManager.Instance.UserId;
		auth_key = SocialManager.Instance.AuthKey;
		ConnectChat();
	}

	void OnSocialDataLoaded (SocialData data)
	{
		SocialManager.Instance.OnUserDataLoaded -= OnSocialDataLoaded;
		NGUIDebugConsole.LogSystem("OnSocialDataLoaded");
		user_id = SocialManager.Instance.ViewerID;
		viewer_id = SocialManager.Instance.ViewerID;
		auth_key = SocialManager.Instance.AuthKey;
		ConnectChat();
	}

	void OnServerError (string obj)
	{
		if (!_server.Connected)
			StartCoroutine("WaitAndConnect");
	}

	IEnumerator WaitAndConnect()
	{
		yield return new WaitForSeconds(1);
		ConnectChat();
	}

	void ConnectChat()
	{
		if (!ChatConnected)
		{
			if (SocialManager.Platform == "VK")
				_server.Port = 3001;
			else
				_server.Port = 4001;
			QueryConnectChat q = new QueryConnectChat(user_id,auth_key,viewer_id,room_name);
			_server.SendQuery(q);
		}
	}

	public void SendChatMessage(string Message)
	{
		Query qa = new QuerySendChatMessage(user_id,auth_key,viewer_id,room_name,Message);
		_server.SendQuery(qa);
	}

	private class StatusReq
	{
		public int Status { get; set; }
	}

	void OnServerMessage (string obj)
	{
		print("Recieved: "+obj);
		if (string.IsNullOrEmpty(obj))
			return;

		Query q;
		if (Query.TryParse(obj,out q) && q.Type != "")
		{
			switch (q.Type)
			{
			case "connect.chat":
				try 
				{
					int status = JSONSerializer.Deserialize<StatusReq>(q.Args[0].ToString()).Status;
					if (status == 200)
					{
						ChatConnected = true;
						OnConnectedSuccessful(q.UserID.Clone() as String);
						if (q.UserID == SocialManager.User.ViewerId)
							SendChatMessage("getPeopleCount");
					}
					else
						OnConnectFailed("Error: Can't connect to chat; Bad server request (503).");
				} catch (Exception ex) 
				{
					OnConnectFailed("Error: "+ex.Message);
				}
				break;
			}
		} 
		else
		{
			try
			{
				QueryChatMessage qm;
				qm = JSONSerializer.Deserialize<QueryChatMessage>(obj);
				if (qm.Message == "getPeopleCount" && qm.SenderID == SocialManager.User.ViewerId)
					OnChatGetCount(qm.count);
				else
					if (qm.Message != "getPeopleCount")
						OnMessageReacived(qm.RoomName,qm.SenderID,qm.Message);
			} catch (Exception exc) 
			{
				Debug.Log("Can't parse query: "+exc.Message);
			}
		}
	}
}
