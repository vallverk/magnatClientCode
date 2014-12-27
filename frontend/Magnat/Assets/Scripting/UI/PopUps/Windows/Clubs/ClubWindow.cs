using UnityEngine;

public class ClubWindow
{
	public static void Show(string ClubID)
	{
		if (WindowBehavoiur.current!=null)
		{
			WindowBehavoiur.current.Hide();
			WindowBehavoiur.current = null;
		}

		ServerInfo.Instance.GetClub(ClubID,(club)=>{
			if (club.CreatorID == SocialManager.Instance.ViewerID)
				GameObject.FindObjectOfType<VIPClubForOwnerWindow>().Show(club);
			else
			{
				if (club.UserList.Contains(SocialManager.Instance.ViewerID))
					GameObject.FindObjectOfType<VIPClubForMembersWindow>().Show(club);
				else
					GameObject.FindObjectOfType<VIPClubForOtherWindow>().Show(club);
			}
		});
	}
}
