using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SelectGiftWindow : WindowBehavoiur 
{
	public UITable GiftTable;
	public GameObject GiftPrefab;

	private bool inited = false;
	private List<GiftField> fields = new List<GiftField>();

	public void Show (string ReceiverID)
	{
		ShowImmediately();

		if (!inited)
		{
			UITools.RemoveChildrens(GiftTable);

			ServerInfo.Instance.GetGiftsList((gifts)=>{
				foreach (var gift in gifts)
				{
					GameObject field = NGUITools.AddChild(GiftTable.gameObject,GiftPrefab);
					var afield = field.GetComponent<GiftField>();
					fields.Add(afield);
					afield.Init(gift);
					GiftTable.Reposition();
					inited = true;
				}
				UpdateRecaivers(ReceiverID);
			});
		} else
			UpdateRecaivers(ReceiverID);
	}

	private void UpdateRecaivers(string UserID)
	{
		foreach (var f in fields)
			f.RecaiverID = UserID;
	}
}
