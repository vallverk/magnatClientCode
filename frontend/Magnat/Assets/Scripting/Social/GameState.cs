using UnityEngine;

public class GameState : MonoBehaviour
{
	private SocialManager SM { get { return SocialManager.Instance; } }
	
	void OnGUI()
	{
		if (!SM.IsLoaded)
		{
			GUI.Label(new Rect(Screen.width / 2 - 50, Screen.height / 2, 200, 20), "Loading...");
			return;
		}
		
		if (SM.SocialData.ContainsKey(SM.UserId))
		{
			var data = SM.SocialData[SM.UserId];
			
			GUI.Label(new Rect(Screen.width / 2 - 100, 10, Screen.width, 20), data.FormatName);
		}

		/*
		if (GUI.Button(new Rect(Screen.width / 2 - 50, Screen.height / 2, 100, 20), "Invite"))
		{
			SocialManager.InviteFriends();
		}
		
		if (GUI.Button(new Rect(Screen.width / 2 - 50, Screen.height / 2 + 30, 100, 20), "Post"))
		{
			SocialManager.PostToWall("Hello");
		}
		*/
	}
}