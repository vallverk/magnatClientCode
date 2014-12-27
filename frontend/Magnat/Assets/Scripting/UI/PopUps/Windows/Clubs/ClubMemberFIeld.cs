using UnityEngine;
using System.Collections;

public class ClubMemberField : MonoBehaviour 
{
	public UILabel NumberLabel;
	public UITexture AvatarTexture;
	public UILabel NameLabel;
	public UILabel TitleLabel;
	public UILabel DepositLabel;

	protected ServerUserInfo user;
	protected ClubInfo club;

	public virtual void Init(ClubInfo Club, ServerUserInfo User, int Number)
	{
		user = User;
		club = Club;

		NumberLabel.text = Number.ToString();
		TitleLabel.text = user.Title;
		DepositLabel.text = user.Deposit.ToString("########0 кг");

		StartCoroutine(SetSocData());
	}

	protected IEnumerator SetSocData()
	{
		SocialData sdata = null;
#if UNITY_EDITOR
		yield return null;
		sdata = new SocialData()
		{
			FirstName = user.GUID,
			LastName = user.GUID,
			Photo = "",
			ViewerId = user.GUID
		};
#else
		sdata = SocialManager.GetUserData(user.GUID);
		while (sdata == null)
		{
			yield return new WaitForSeconds(1);
			sdata = SocialManager.GetUserData(user.GUID);
		}
#endif
		OnSocData(sdata);

		NameLabel.text = sdata.FormatName;
		if (!string.IsNullOrEmpty(sdata.Photo))
			ImageLoader.Instance.LoadAvatar(sdata.Photo,(tex)=>{
				AvatarTexture.mainTexture = tex;
			});
	}

	protected virtual void OnSocData(SocialData sdata) {}
}
