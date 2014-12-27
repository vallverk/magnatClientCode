using UnityEngine;
using System.Collections;

public class ClubMembersInitiator : MonoBehaviour 
{
	public UIGrid MembersGrid;
	public GameObject[] MemberPrefabs;

	private ClubInfo club;

	private bool inited = true;

	public void Init(ClubInfo Club)
	{
		club = Club;
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
		UITools.RemoveChildrens(MembersGrid);
		
		ServerInfo.Instance.GetUserInfo( club.UserList.ToArray(), (users)=>{
			for (int i=0;i<users.Length;i++)
			{
				GameObject pref = MemberPrefabs[users[i].VIP!=0?0:1];
				GameObject field = NGUITools.AddChild(MembersGrid.gameObject,pref);
				MembersGrid.AddChild(field.transform);
				ClubMemberField cmf = field.GetComponent<ClubMemberField>();
				cmf.Init(club,users[i],i+1);
			}
			MembersGrid.Reposition();
		});
	}
}
