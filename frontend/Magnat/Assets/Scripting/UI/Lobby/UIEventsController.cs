using UnityEngine;
using System.Collections;

public class UIEventsController : MonoBehaviour 
{
	public void OnInviteFriendButtonClick()
	{
		if (Screen.fullScreen)
			Screen.SetResolution(800,800,false);
		SocialManager.InviteFriends();
	}
}
