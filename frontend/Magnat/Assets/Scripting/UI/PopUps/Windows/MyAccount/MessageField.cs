using UnityEngine;
using System.Collections;

public class MessageField : MonoBehaviour 
{
	public UITexture AvatarTexture;
	public UILabel MessageTextLabel;
	public UILabel MessageTimeLabel;

	public void Init(long MessageID)
	{
		ServerInfo.Instance.GetUserMessageByID(MessageID,(message)=>{
			MessageTextLabel.text = message.Text;
			MessageTimeLabel.text = TimeTools.FormatUTSTime(message.Time);
#if !UNITY_EDITOR
			StartCoroutine(LoadAvatart(message.Sender));
#endif
		});
	}

	private IEnumerator LoadAvatart(string UID)
	{
		SocialData udata = null;
		while ((udata = SocialManager.GetUserData(UID)) == null)
			yield return new WaitForSeconds(0.3f);
		ImageLoader.Instance.LoadAvatar(udata.Photo,(tex)=>{
			AvatarTexture.mainTexture = tex;
		});
	}
}
