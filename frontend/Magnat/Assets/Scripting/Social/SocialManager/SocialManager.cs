#define PlatformVK
//#define PlatformOK

using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SocialManager : MonoBehaviour
{
#if PlatformVK
	public static string Platform = "VK";
#else
	public static string Platform = "OK";
#endif

	public static SocialManager Instance {get; private set; }

	public bool IsLoaded { get; private set; }
	public bool IsFriendsLoaded { get; private set; }
	public string UserId { get; private set; }
	public string ViewerID { get; private set; }
	public string AuthKey { get; private set; }
	public string Protocol { get; private set; }
	public string ApiURL { get; private set; }
	public string ApiID { get; private set; }
	public string ViewerType { get; private set; }
	public string SID { get; private set; }
	public string AccesToken { get; private set; }
	public string ApiSettings { get; private set; }
	public string Secret { get; private set; }
	public List<string> Friends { get; private set; }

	public Action<SocialData> OnUserDataLoaded = (a) => {};
	public Action OnBaseDataLoaded = () => {};
	public Action OnFriendsDataLoaded = () => {};
	
	public readonly Dictionary<string, SocialData> SocialData = new Dictionary<string, global::SocialData>();

	private List<string> loading = new List<string>();

	public static SocialData User
	{
		get
		{
			if (!Instance.IsLoaded) return null;
			return GetUserData(Instance.ViewerID);
		}
	}

	public void OnDestroy()
	{
		OnUserDataLoaded = (a) => {};
		OnBaseDataLoaded = () => {}; 
		OnFriendsDataLoaded = () => {}; 
	}

	public static SocialData GetUserData(string UID)
	{
		if (Instance == null || string.IsNullOrEmpty(UID)) return null;
		if (!Instance.SocialData.ContainsKey(UID)) 
		{
			GetUserInfo(UID);
			return null;
		}
		return Instance.SocialData[UID];
	}
	
	void Awake()
	{
		// не будим плодить сущности...
		if (FindObjectOfType<SocialManager>() != this)
		{
			Destroy(gameObject);
			return;
		}

		IsLoaded = false;
		Friends = new List<string>();
		DontDestroyOnLoad(gameObject);
		Instance = this;
		Debug.LogError("SOC INITED");
		#if UNITY_EDITOR
		#if PlatformVK
		//this.ViewerID = "249156791"; // FG test
		this.ViewerID = "15361226"; // My 
		//this.ViewerID = "42048261"; // Alex
		//this.ViewerID = "25417055"; // Oleg
		this.AuthKey = MD5Convertor.getMd5Hash("4496266_"+this.ViewerID+"_E4YICIo0hwlML3eJoGiZ");
		IsLoaded = true;
		OnBaseDataLoaded();
		#endif
		
		#if PlatformOK
		this.ViewerID = "566673434861"; // My
		this.AuthKey = MD5Convertor.getMd5Hash("1085217280_"+this.ViewerID+"_7A2B1E96BE88DC84A2A15BBB");
		#endif
		
		SocialData.Add(this.ViewerID,new SocialData() { ViewerId = this.ViewerID, FirstName = "FName",LastName = "LName", Photo = ""});
		IsLoaded = true;
		//ServerInfo.Instance.GetUserInfo(new string("0"));
		#endif
	}
	
	// Use this for initialization
	void Start()
	{
		Application.ExternalCall("GetParams");
		Write("Create SocialManager instance!");
	}
	
	private static void Write(string mes)
	{
		//NGUIDebugConsole.LogSystem("SM->" + mes);
	}
	
	#region CALLBACKS
	
	public void RecvParams(string a)
	{
		
		if (a.StartsWith("https"))
			Protocol = "https";
		else
			Protocol = "http";
		
		a = a.Split('?')[1];
		
		string[] mas = a.Split('&');

#if PlatformVK
		foreach (string s in mas)
		{
			string[] k = s.Split('=');
			switch (k[0])
			{
			case "viewer_id":
				ViewerID = k[1];
				Write("ViewerID set to '" + ViewerID+"'");
				break;
			case "user_id":
				UserId = k[1];
				Write("UserID set to '"+UserId+"'");
				break;
			case "auth_key":
				AuthKey = k[1];
				Write("AuthKey set to '" + AuthKey+"'");
				break;
			case "api_id":
				ApiID = k[1];
				Write("ApiID set to '"+ApiID+"'");
				break;
			case "api_url":
				ApiURL = k[1];
				Write("ApiURL set to '"+ApiURL+"'");
				break;
			case "viewer_type":
				ViewerType = k[1];
				Write("ViewerType set to '"+ViewerType+"'");
				break;
			case "sid":
				SID = k[1];
				Write("SID set to '"+SID+"'");
				break;
			case "access_token":
				AccesToken = k[1];
				Write("AccessToken set to '"+AccesToken+"'");
				break;
			case "api_settings":
				ApiSettings = k[1];
				Write("ApiSettings set to '"+ApiSettings+"'");
				break;
			case "secret":
				Secret = k[1];
				Write("Secret set to '"+Secret+"'");
				break;

			}
		}
#endif

#if PlatformOK

		foreach (string s in mas)
		{
			string[] k = s.Split('=');
			switch (k[0])
			{
			case "logged_user_id":
				ViewerID = k[1];
				Write("ViewerID set to '" + ViewerID+"'");
				UserId = k[1];
				Write("UserID set to '"+UserId+"'");
				break;
			case "apiconnection":
				ApiID = k[1].Split('_')[0];
				Write("ApiID set to '"+ApiID+"'");
				break;
			case "sig":
				SID = k[1];
				Write("SID set to '"+SID+"'");
				break;
				
			}
		}

		Secret = "7A2B1E96BE88DC84A2A15BBB";
		AuthKey = MD5Convertor.getMd5Hash(ApiID+"_"+this.ViewerID+"_"+Secret);
		ApiURL = "http://www.odnoklassniki.ru/game/"+ApiID;
		ViewerType = "";
		AccesToken = "";
		ApiSettings = "";

#endif

		Write("RecvParams: " + a);
		
		GetUserInfo(UserId);

		Application.ExternalCall("GetFriends", UserId);
		
		IsLoaded = true;
		OnBaseDataLoaded();
	}

	public void OnGetFriends(string str)
	{
		string[] obj = str.Split(']');
		for (int i=0;i<obj.Length-1;i++)
		{
			string[] data = obj[i].Substring(1).Split(',');
			var socData = new SocialData
			{
				ViewerId = data[0],
				FirstName = data[1],
				LastName = data[2],
				Photo = data[3]
			};
			if (SocialData.ContainsKey(socData.ViewerId))
				SocialData[socData.ViewerId] = socData;
			else
				SocialData.Add(socData.ViewerId, socData);
			Friends.Add(socData.ViewerId);
		}
		Write("Loaded "+Friends.Count+" friends");
		OnFriendsDataLoaded();
		IsFriendsLoaded = true;
	}
	
	public void OnGetPlayer(string str)
	{
		var mas = str.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
		
		var socData = new SocialData
		{
			ViewerId = mas[0],
			FirstName = mas[1],
			LastName = mas[2],
			Photo = mas[3]
		};

		if (loading.Contains(socData.ViewerId))
			loading.Remove(socData.ViewerId);
		
		if (SocialData.ContainsKey(socData.ViewerId))
		{
			SocialData[socData.ViewerId] = socData;
		}
		else
		{
			SocialData.Add(socData.ViewerId, socData);
		}
		
		Write("OnGetPlayer: '" + str + "'");

		OnUserDataLoaded(socData);
	}
	
	#endregion
	
	public static void PostToWall(string text)
	{
		Application.ExternalCall("PostToWall", text);
	}

	public static void GetUserInfo(string viewer_id)
	{
#if UNITY_EDITOR 
		return;
#endif

		if (Instance.SocialData.ContainsKey(viewer_id))
			return;
		
		if (Instance.loading.Contains(viewer_id))
			return;
		else
			Instance.loading.Add(viewer_id);
		
		Application.ExternalCall("GetProfile", viewer_id);
	}

    public static void GetUserInfo(string[] viewer_ids)
    {
#if UNITY_EDITOR
        return;
#endif
        viewer_ids = viewer_ids.Where(id => !(Instance.SocialData.ContainsKey(id)) ).ToArray();

        foreach (var viewer_id in viewer_ids)
            if (!Instance.loading.Contains(viewer_id))
                Instance.loading.Add(viewer_id);

        if (viewer_ids.Length != 0)
        {
            string ids = "";
            foreach (var id in viewer_ids)
                ids += id + ',';
            ids = ids.Substring(0, ids.Length - 1);
            Application.ExternalCall("GetProfile", ids);
        }
    }

	public static void InviteFriends()
	{
		Application.ExternalCall("ShowInvite");
	}
}