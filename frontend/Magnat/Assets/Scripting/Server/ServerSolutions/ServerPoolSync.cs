using UnityEngine;
using System.Collections;
using System.Net.Sockets;
using System;
using System.Threading;
using System.Collections.Generic;
using System.Net;

public class ServerPoolSync : MonoBehaviour 
{
	public string Host;
	private IPAddress ip;
	public int Port;

	public int SendReceiveTimeout = 1000;

	public bool Connected { get { return this._s != null && this._s.Connected; } }

	public bool AsyncReceive = true;

	public Action<string> OnServerResponse = (x) => {};
	public Action OnConnected = () => {};
	public Action<string> OnError = (x) => {};

	protected Socket _s = null;
	private Thread _rt = null;

	private List<string> _received = new List<string>();

	void GetIP()
	{
		ip = SocketClient.GetIP(Host);
	}

	protected Socket Connect()
	{
		if (Connected) return _s;
		if (ip == null)
			GetIP();
		_s = null;
		_s = SocketClient.Connect(ip,Port);
		if (_s == null)
			OnError("Can't connect to server");
		else
		{
			if (Application.platform == RuntimePlatform.OSXWebPlayer)
			{
				_s.SendTimeout = this.SendReceiveTimeout;
				_s.ReceiveTimeout = this.SendReceiveTimeout;
			}
			_s.ReceiveBufferSize = 51200;
			_s.SendBufferSize = 51200;
			if (AsyncReceive)
				StartRecieve();
			OnConnected();
		}
		return _s;
	}

	public Query SendPostRequest(Query Q, int stackSize)
	{
		if (!Connected)
			Connect();

		string data = "data=";
		if (Q.UseUREncode)
			data += System.Uri.EscapeDataString(Q.GetJSON());
		else
			data += Q.GetJSON();
		// create post request
		string header = "POST /{4} HTTP/1.1\r\n"+
				"Host: {2}:{3}\r\n"+
						"Connection: keep-alive\r\n"+
						"Content-Length: {0}\r\n"+
						"Pragma: no-cache\r\n"+
						"Cache-Control: no-cache\r\n"+
						"Accept: text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8\r\n"+
						"Accept-Charset: utf-8\r\n"+
						"Origin: null\r\n"+
						"Content-Type: application/x-www-form-urlencoded\r\n\r\n{1}";
		string q = string.Format(header,
		                         data.Length,data,Host,Port,Q.Type);
		print("Send (POST) = "+q);

		if (!SocketClient.Send(_s,q))
		{
			Connect();
			SocketClient.Send(_s,q);
		}

		string r = SocketClient.Receive(_s);
		print("Recieved (POST) = "+r);

		if (string.IsNullOrEmpty(r))
			return null;

		//Content-Type: text/html; charset=utf-8
		//Content-Length: 2779
		//Date: Sat, 08 Nov 2014 11:03:45 GMT
		//Connection: keep-alive

		// высчитаем длинну контента
		int lenStartInd = r.IndexOf("Content-Length: ")+"Content-Length:".Length;
		string len = r.Substring(lenStartInd);
		int contentLength = int.Parse(len.Substring(0,len.IndexOf("\r\n")));

		int bodyPos = r.IndexOf("\r\n\r\n") + 4;
		string content = r.Substring(bodyPos);

		while (DataTransfer.GetBytes(content).Length != contentLength)
		{
			// значит сейчас будет еще несколько пакетов, нужно их сосплитить...
			string additional = SocketClient.Receive(_s);
			//print("Recieved (POST additional) = "+additional);
			content += additional;
		}

		return Query.Parse(content);
	}

	void StartRecieve()
	{
		StartCoroutine("Loop");
		_rt = new Thread(Recieve);
		_rt.Start();
	}

	void Recieve()
	{
		while (Connected)
		{
			try 
			{
				string text = SocketClient.Receive(_s);
				if (!string.IsNullOrEmpty(text))
				{
					_received.Add(text);
				}
			} catch {}
		}
		Thread.CurrentThread.Abort();
	}

	IEnumerator Loop()
	{
		while (true)
		{
			if (_received.Count!=0)
			{
				OnServerResponse(_received[0]);
				_received.RemoveAt(0);
			}
			yield return null;
		}
	}

	public void Disconnect()
	{
		//SocketClient.CloseConnection(_s);
		if (_s!=null)
		{
			if (_s.Connected)
				_s.Shutdown(SocketShutdown.Both);
			_s.Close();
		} 
		_s = null;
		_rt = null;
		StopCoroutine("Loop");
	}

	public void SendQuery(Query Q)
	{
		print("Send = "+Q.GetJSON());
		if (Connected)
		{
			SocketClient.Send(_s,Q.GetJSON());
		}
		else
		{
			//_startQuerues.Add(Q);
			Connect();
			if (Connected)
				SocketClient.Send(_s,Q.GetJSON());
		}
	}
}
