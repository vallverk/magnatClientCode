using UnityEngine;
using System.Collections;

public class ClubContainer: MonoBehaviour
{
	public UILabel ClubName;
	public UILabel ClubStatus;
	public UITexture ClubLogo;

	public GameObject NoClubGO;

	private string clubID;

	public static string OwnerOf = "";

	public void Init(string ClubID)
	{
		OwnerOf = "";
		clubID = ClubID;

        NoClubGO.SetActive(string.IsNullOrEmpty(clubID));
        if (string.IsNullOrEmpty(clubID))
        {
            ClubName.text = "";
            ClubStatus.text = "";
            ClubLogo.mainTexture = null;
        }

		if (!string.IsNullOrEmpty(clubID))
		ServerInfo.Instance.GetClub(clubID,(club)=>{

				if (club.CreatorID == SocialManager.Instance.ViewerID)
					OwnerOf = club.ID;

				ClubName.text = club.ClubName;
				ClubStatus.text = "";
				if (!string.IsNullOrEmpty(club.Icon))
				{
					ClubLogo.mainTexture = null;
					ImageLoader.Instance.LoadAvatar(club.Icon,(tex)=>{
						if (tex!=null)
						{
							ClubLogo.mainTexture = tex;
							Vector2 size = UITools.ResizeTo(tex,100);
							ClubLogo.width = (int)size.x;
							ClubLogo.height = (int)size.y;
						}
						else
							AlertWindow.Show("ОШИБКА","Ошибка загрузки аватарки клуба",null,null);
					});
				}
			});
	}

	void Start()
	{
		GetComponent<UIButton>().onClick.Add(new EventDelegate(OnClick));
		ClubName.text = "";
		ClubStatus.text = "";
	}

	public void OnClick()
	{
		if (!string.IsNullOrEmpty(clubID))
			ClubWindow.Show(clubID);
	}
}
