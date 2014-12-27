using UnityEngine;
using System.Collections;

public class UserActionsInitiator : MonoBehaviour 
{
	public UIGrid ActionsGrid;

	public GameObject BuyedActionPrefab;
	public GameObject EmptyActionPrefab;

	private bool inited = true;
	private string userID;

	public void Init(string UserID)
	{
		userID = UserID;
		inited = false;
	}

	void OnEnable()
	{
		if (!inited)
		{
			UITools.RemoveChildrens(ActionsGrid);

			ServerInfo.Instance.GetUserActions(userID,(actions)=>{
				for (int i=0;i<5;i++)
				{
					if (i<actions.Length)
					{
						// поле акции
						GameObject field = NGUITools.AddChild(ActionsGrid.gameObject,BuyedActionPrefab);
						ActionsGrid.AddChild(field.transform);
						var afield = field.GetComponent<ActionFieldBuyed>();
						afield.Init(actions[i]);
					}
					else
					{
						// поле добавления акции
						if (EmptyActionPrefab!=null)
						{
							GameObject field = NGUITools.AddChild(ActionsGrid.gameObject,EmptyActionPrefab);
							ActionsGrid.AddChild(field.transform);
						}
					}
				}
				ActionsGrid.Reposition();
				inited = true;
			});
		}
	}
}
