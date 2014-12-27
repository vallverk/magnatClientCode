using UnityEngine;
using System.Collections;

public class GameListPlayerAvatar : ProfileShower
{
	public UITexture AvatarTexture;
	public GameObject MinusGO;

	public string GUID;

	private ServerUserInfo userInfo;

	public void Init(string UID)
	{
		GUID = UID;

		if (AvatarTexture!=null && UID != SocialManager.Instance.ViewerID)
			SetOnClickEvent(UID);

		MinusGO.SetActive(UID == SocialManager.Instance.ViewerID);
		StartCoroutine(LoadAvatar());
	}

	IEnumerator LoadAvatar()
	{
		SocialData data = SocialManager.GetUserData(GUID);
		while (data == null)
		{
			yield return new WaitForSeconds(0.3f);
			data = SocialManager.GetUserData(GUID);
		}
		if (!string.IsNullOrEmpty(data.Photo))
			ImageLoader.Instance.LoadAvatar(data.Photo,(tex) => {AvatarTexture.mainTexture = tex;});
	}
}
