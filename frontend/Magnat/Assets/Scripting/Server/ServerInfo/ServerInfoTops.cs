using System;
using System.Collections.Generic;

public partial class ServerInfo : Singleton<ServerInfo> 
{
	private string[] top10buffer = null;
	public void GetTop10Data(Action<string[]> Callback)
	{
		if (top10buffer != null)
		{
			Callback(top10buffer);
			return;
		}

		Query q = new QueryGetTop10(viewerID,auth,viewerID);
		Pool.SendPostRequestAsync(q,(a)=>{
			List<string> res = new List<string>();
			for (int i=0;i<a.Args.Count;i++)
				res.Add(a.Args[i].ToString());
			top10buffer = res.ToArray();
			Callback(res.ToArray());
		});
	}

	private string[] top100buffer = null;
	public void GetTop100Data(Action<string[]> Callback)
	{
		if (top100buffer != null)
		{
			Callback(top100buffer);
			return;
		}

		Query q = new QueryGetTop100(viewerID,auth,viewerID);
		Pool.SendPostRequestAsync(q,(a)=>{
			List<string> res = new List<string>();
			for (int i=0;i<a.Args.Count;i++)
				res.Add(a.Args[i].ToString());
			top100buffer = res.ToArray();
			Callback(res.ToArray());
		});
	}

	private string[] weekTop10buffer = null;
	public void GetWeekTop10Data(Action<string[]> Callback)
	{
		if (weekTop10buffer != null)
		{
			Callback(weekTop10buffer);
			return;
		}

		Query q = new QueryGetWeekTop10(viewerID,auth,viewerID);
		Pool.SendPostRequestAsync(q,(a)=>{
			List<string> res = new List<string>();
			for (int i=0;i<a.Args.Count;i++)
				res.Add(a.Args[i].ToString());
			weekTop10buffer = res.ToArray();
			Callback(res.ToArray());
		});
	}

	private string[] weekTop100buffer = null;
	public void GetWeekTop100Data(Action<string[]> Callback)
	{
		if (weekTop100buffer != null)
		{
			Callback(weekTop100buffer);
			return;
		}

		Query q = new QueryGetWeekTop100(viewerID,auth,viewerID);
		Pool.SendPostRequestAsync(q,(a)=>{
			List<string> res = new List<string>();
			for (int i=0;i<a.Args.Count;i++)
				res.Add(a.Args[i].ToString());
			weekTop100buffer = res.ToArray();
			Callback(res.ToArray());
		});
	}
}
