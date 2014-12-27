using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class ServerGameList : MonoBehaviour 
{
	public GameObject GridGO;
	public GameObject GameLabelPrefab;

	void Start()
	{
		StartCoroutine(PoolData());
	}

	void OnDestroy()
	{
		StopAllCoroutines();
	}

	// забор данных с сервера о играх
	private IEnumerator PoolData()
	{
		while(true)
		{
			ServerInfo.Instance.GetGameAnnounceList(UpdateInfo);
			yield return new WaitForSeconds(0.9f);
		}
	}

	private void UpdateInfo(GameInfo[] gis)
	{
		foreach (var g in gis)
		{
			if (g.UserList.Contains(SocialManager.Instance.ViewerID) && g.Status == 1 && 
			    !ServerData.IsGameAtBlackList(g.GUID.ToString()))
			{
				// ready to start!... maybe....
				#if !UNITY_EDITOR
				bool canLoad = true;
				for (int i=0;i<g.UserList.Count;i++)
					if (SocialManager.GetUserData(g.UserList[i]) == null)
				{
					canLoad = false;
					break;
				}
				//  rly ready to start!
				if (canLoad)
					#endif
				{
					PlayerPrefs.SetString("LoadGame",JSONSerializer.Serialize(g));
					Application.LoadLevel(2);
				}
			}
		}

		var players = GridGO.transform.GetComponentsInChildren<GameInfoController>();
		if (players == null) return;
		List<GameInfoController> games = new List<GameInfoController>(players);
		
		foreach (GameInfo gi in gis)
		{
			if (ServerData.IsGameAtBlackList(gi.GUID.ToString())) continue;
			GameInfoController con = null;
			for (int i = 0;i<games.Count;i++)
				if (games[i].GameID == gi.GUID)
			{
				con = games[i];
				break;
			}

            if (con != null) games.Remove(con);//if (!con.Players.All(p => p == SocialManager.Instance.UserId))
			
			if (con == null && gi.Status==0)
			{
				GameObject go = NGUITools.AddChild(GridGO,GameLabelPrefab) as GameObject;
				go.GetComponent<GameInfoController>().SetInfo(gi,true);
				GridGO.GetComponent<UIGrid>().AddChild(go.transform);
			} 
			
			if (con!=null && gi.Status != 0)
			{
				Destroy(con);
			}
			
			if (con!=null && gi.Status == 0)
			{
				con.SetInfo(gi,true);
			}
		}
		
		while (games.Count>0)
		{
			GridGO.GetComponent<UIGrid>().RemoveChild(games[0].transform);
			Destroy(games[0].gameObject);
			games.RemoveAt(0);
		}
	}
}
