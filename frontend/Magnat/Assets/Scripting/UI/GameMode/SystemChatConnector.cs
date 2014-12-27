using UnityEngine;
using System.Collections;
using System;

[RequireComponent(typeof(ServerPoolSync))]
public class SystemChatConnector : MonoBehaviour 
{
	public string user_id;
	public string room_name;
	public string viewer_id;
	public string auth_key;
	
	public bool ChatConnected { get; private set; }
	
	public Action<string> OnConnectFailed = (x) => {};
	public Action<string> OnConnectedSuccessful = (x) => {};
	public Action<string,string,string> OnMessageReacived = (a,b,c) => {};
	
	ServerPoolSync _server;
	public ServerPoolSync ServerConnection { get { return _server; } }

	void Awake()
	{
		ChatConnected = false;
	}

	public void Init(long GameID)
	{
		if (!ChatConnected)
		{
			user_id = SocialManager.Instance.ViewerID;
			viewer_id = SocialManager.Instance.ViewerID;
			auth_key = SocialManager.Instance.AuthKey;
			room_name = GameID.ToString();

			_server = GetComponent<ServerPoolSync>();
			_server.OnServerResponse -= OnServerMessage;
			_server.OnError -= OnServerError;
			_server.OnServerResponse += OnServerMessage;
			_server.OnError += OnServerError;

			ConnectChat();
		}
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
	}
	
	void OnServerError (string obj)
	{
		if (!_server.Connected)
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
				OnMessageReacived(qm.RoomName,qm.SenderID,qm.Message);
			} catch (Exception exc) 
			{
				Debug.Log("Can't parse query: "+exc.Message);
			}
		}
	}
}