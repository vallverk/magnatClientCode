using UnityEngine;
using System.Collections;
using System.Threading;
using System.Collections.Generic;
using System;
using System.Linq;

public class ServerPoolASync:ServerPoolSync
{
	private Dictionary<Query,Action<Query>> requestPool = new Dictionary<Query, Action<Query>>();
	private List<KeyValuePair<Query,Action<Query>>> sendPool = 
		new List<KeyValuePair<Query, Action<Query>>>();
	private Thread runner = null;

	// триггер для синхронизации доступа к requestPool из 2х потоков
	private bool busy = false;

	void FixedUpdate()
	{
		if (requestPool.Count>0 && !busy)
		{
			busy = true;
			foreach (var pair in requestPool)
				pair.Value(pair.Key);
			requestPool.Clear();
			busy = false;
		}
	}

	public void SendPostRequestAsync(byte[] Bytes, Action<Query> Result)
	{
		Thread t = new Thread(()=>{
			if (!SocketClient.Send(_s,Bytes))
			{
				Connect();
				SocketClient.Send(_s,Bytes);
			}
			string r = SocketClient.Receive(_s);
			if (string.IsNullOrEmpty(r))
				Result(null);
			print ("Received form bytes : "+r);
			int bodyPos = r.IndexOf("\r\n\r\n") + 4;
			r = r.Substring(bodyPos);
			Result(Query.Parse(r));
		});
		t.Start();
	}

	public void SendPostRequestAsync(Query Send, Action<Query> Result, bool LowPriority)
	{
		if (LowPriority)
		{
			sendPool.Add(new KeyValuePair<Query, Action<Query>>(Send,Result));
			if (runner == null) 
			{
				runner = new Thread(GetQueryThread);
				runner.Start();
			}
		}
		else
			SendPostRequestAsync(Send,Result);
	}

	public void SendPostRequestAsync(Query Send, Action<Query> Result)
	{
		sendPool.Insert(0,new KeyValuePair<Query, Action<Query>>(Send,Result));
		
		if (runner == null) 
		{
			runner = new Thread(GetQueryThread);
			runner.Start();
		}
	}

	private void GetQueryThread()
	{
		while(true)
		{
			try
			{
				if (sendPool.Count>0)
				{
					var buffer = sendPool;
					sendPool = new List<KeyValuePair<Query, Action<Query>>>();
					buffer.Reverse();
					foreach (var pair in buffer)
					{
						Query request = SendPostRequest(pair.Key,10);
						while (request == null)
						{
							Thread.Sleep(100);
							request = SendPostRequest(pair.Key,10);
						}
						while (busy) { Thread.Sleep(10); }
						busy = true;
						if(pair.Key.Args!=null)
							requestPool.Add(request,pair.Value);
						busy = false;
					}
					buffer.Clear();
				}
				else
					Thread.Sleep(50);
			} catch { Thread.Sleep(50); }
		}
	}

	void OnDestroy()
	{
		if (runner!=null)
			runner.Abort();
	}
}
