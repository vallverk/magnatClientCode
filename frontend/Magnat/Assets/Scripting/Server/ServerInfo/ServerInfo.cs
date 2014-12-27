using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[RequireComponent(typeof(ServerPoolSync))]
public partial class ServerInfo : Singleton<ServerInfo> 
{
	ServerPoolASync _p;
	ServerPoolASync Pool
	{
		get 
		{ 
			if (_p == null)
			{
				_p = gameObject.GetComponent<ServerPoolASync>();
				if (_p == null)
					_p = gameObject.AddComponent<ServerPoolASync>();
				_p.Host = "www.magnatgame.com";
				_p.AsyncReceive = false;
				_p.SendReceiveTimeout = 1000; 

				if (SocialManager.Platform == "VK")
					_p.Port = 3000;
				else
					_p.Port = 4000;
			}
			return _p;
		}
	}

	private string viewerID { get { return SocialManager.Instance.ViewerID; } }
	private string auth { get { return SocialManager.Instance.AuthKey; } }

	public void Init()
	{
		if (_p == null)
		{
			_p = gameObject.GetComponent<ServerPoolASync>();
			if (_p == null)
				_p = gameObject.AddComponent<ServerPoolASync>();
			_p.Host = "www.magnatgame.com";
			_p.AsyncReceive = false;
			_p.SendReceiveTimeout = 1000; 
			if (SocialManager.Platform == "VK")
				_p.Port = 3000;
			else
				_p.Port = 4000;
		}
	}

	void Start()
	{
		DontDestroyOnLoad(gameObject);

		if (SocialManager.Instance.IsLoaded)
			OnSocialDataLoaded(null);
		else
			SocialManager.Instance.OnUserDataLoaded += OnSocialDataLoaded;
	}

	public override void OnDestroy ()
	{
		base.OnDestroy ();
		SocialManager.Instance.OnUserDataLoaded -= OnSocialDataLoaded;
		DisconnectImmediatly();
	}

	void OnApplicationQuit()
	{
		DisconnectImmediatly();
	}

	public void DisconnectImmediatly()
	{
		if (_p!=null)
			_p.Disconnect();
		_p = null;
	}

	void OnSocialDataLoaded (SocialData data)
	{
		if (SocialManager.Instance.SocialData.ContainsKey(SocialManager.Instance.ViewerID))
		{
			SocialManager.Instance.OnUserDataLoaded -= OnSocialDataLoaded;
			var udata = SocialManager.Instance.SocialData[SocialManager.Instance.ViewerID];
			GetUserInfo(udata,(x)=>{},false);
		}
	}
}
