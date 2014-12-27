using UnityEngine;
using System.Collections;

public class VIPClubsWindow : WindowBehavoiur 
{
	public GameObject ClubFieldPrefab;
	public UIGrid TopClubsGrid;

	private bool inited = false;

	void InitTopClubs()
	{
		if (!inited)
		{
			ServerInfo.Instance.GetTop100Clubs((ids)=>{
				for (int i=0;i<ids.Length;i++)
				{
					GameObject field = NGUITools.AddChild(TopClubsGrid.gameObject,this.ClubFieldPrefab);
					TopClubsGrid.AddChild(field.transform);
					TopClubField tcf = field.GetComponent<TopClubField>();
					tcf.NumberLabel.text = (i+1).ToString();
					tcf.Init(ids[i]);
				}
				inited = true;
			});
		}
	}

	public override void Show ()
	{
		base.Show ();
		InitTopClubs();
	}
}
