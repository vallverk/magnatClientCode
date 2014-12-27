using UnityEngine;
using System.Collections;

public class UserMessagesContainer : MonoBehaviour 
{
	public GameObject MessagePrefab;

	private UIGrid grid;

	void OnEnable()
	{
		grid = GetComponent<UIGrid>();
		var gridFields = grid.GetChildList();

		// очистим
		if (gridFields.Count!=0)
		{
			foreach (var ch in gridFields)
				GameObject.Destroy(ch.gameObject);
			gridFields.Clear();
		}

		// заполним
		ServerInfo.Instance.GetUserMessagesIDs((ids)=>{
			for (int i=0;i<ids.Length;i++)
			{
				GameObject mo = NGUITools.AddChild(grid.gameObject,MessagePrefab);
				gridFields.Add(mo.transform);
				MessageField field = mo.GetComponent<MessageField>();
				field.Init(ids[i]);
			}
			grid.repositionNow = true;
		});
	}
}
