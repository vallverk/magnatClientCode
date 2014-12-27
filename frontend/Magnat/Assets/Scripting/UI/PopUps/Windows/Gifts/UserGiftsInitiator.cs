using UnityEngine;
using System.Collections;

public class UserGiftsInitiator : MonoBehaviour 
{
	public UITable GiftsTable;
	public GameObject UserGiftPrefab;
	
	private string userID;
	
	private bool inited = true;
	
	public void Init(string UserID)
	{
		userID = UserID;
		inited = false;
		if (gameObject.activeInHierarchy)
			OnEnable();
	}
	
	void OnEnable()
	{
		if (!inited)
		{
			Show();
			inited = true;
		}
	}
	
	private void Show()
	{
		UITools.RemoveChildrens(GiftsTable);
		
		ServerInfo.Instance.GetUserGifts( userID, (gifts)=>{
			for (int i=0;i<gifts.Length;i++)
			{
				GameObject field = NGUITools.AddChild(GiftsTable.gameObject,UserGiftPrefab);
				//GiftsTable.AddChild(field.transform);
				UserGiftField cmf = field.GetComponent<UserGiftField>();
				cmf.Init(gifts[i]);
			}
			GiftsTable.Reposition();
		});
	}
}
