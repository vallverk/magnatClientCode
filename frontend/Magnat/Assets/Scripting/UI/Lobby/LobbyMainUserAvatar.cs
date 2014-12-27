using UnityEngine;
using System.Collections;

[RequireComponent(typeof(UITexture))]
public class LobbyMainUserAvatar : MonoBehaviour 
{
	UITexture texture;

	void Start()
	{
		texture = GetComponent<UITexture>();
		texture.mainTexture = null;
		StartCoroutine("Load");
	}

	IEnumerator Load()
	{
		// wait for social data loaded
		while (!SocialManager.Instance.IsLoaded)
			yield return new WaitForSeconds(0.5f);

		var udata = SocialManager.GetUserData(SocialManager.Instance.ViewerID);
		// whait while udata loaded
		while (udata == null)
		{
			yield return new WaitForSeconds(0.5f);
			udata = SocialManager.GetUserData(SocialManager.Instance.ViewerID);
		}

		ImageLoader.Instance.LoadAvatar(udata.Photo,
		                                (tex) => {this.texture.mainTexture = tex;});
	}
}
