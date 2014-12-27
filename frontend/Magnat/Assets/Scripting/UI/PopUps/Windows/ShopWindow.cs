using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class ShopWindow : WindowBehavoiur
{
	public UIGrid ActionsGrid;
	public GameObject ActionFieldPrefab;

	private bool actionsInited = false;

	private List<UserAction> actions = null;

	protected override void Start ()
	{
		base.Start ();

		actions = new List<UserAction>();
#if UNITY_EDITOR
		PreLoad();
#else
		if (SocialManager.Instance.IsLoaded)
			PreLoad();
		else
			SocialManager.Instance.OnBaseDataLoaded += PreLoad;
#endif
	}

	private void PreLoad()
	{
		SocialManager.Instance.OnBaseDataLoaded -= PreLoad;
		ServerInfo.Instance.GetActionIDList((ids)=>{
			foreach (var id in ids)
			{
				ServerInfo.Instance.GetActionByID(id,(action)=>{
					action.image = action.image.Replace("moder","moderator");
					actions.Add(action);
					ImageLoader.Instance.LoadAvatar(action.image,(tex)=>{});
				});
			}
		});
	}

	public override void Show ()
	{
		base.Show ();

		if (!actionsInited)
		{
			UITools.RemoveChildrens(ActionsGrid);

			// sort actions
			actions.Sort((UserAction x, UserAction y) => {
				if (x.monopoly == y.monopoly)
					return int.Parse(y.discount) - int.Parse(x.discount);
				return int.Parse(x.monopoly) - int.Parse(y.monopoly);
			});

			ServerInfo.Instance.GetUserActions(SocialManager.User.ViewerId,(ua)=>{
				foreach (var act in actions)
				{
					GameObject fieldGO = NGUITools.AddChild(ActionsGrid.gameObject,ActionFieldPrefab);
					ActionsGrid.AddChild(fieldGO.transform);
					ActionField afield = fieldGO.GetComponent<ActionField>();
					afield.Init(act);
					afield.ReBuy = ua.Any(a => a.monopoly == act.monopoly);
				}
				ActionsGrid.Reposition();
				actionsInited = true;
			});
		}
	}
}
