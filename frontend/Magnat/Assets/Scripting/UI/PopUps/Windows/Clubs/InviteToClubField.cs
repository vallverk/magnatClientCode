using UnityEngine;
using System.Collections;

public class InviteToClubField : MonoBehaviour 
{
	public UITexture ClubTexture;
	public UILabel ClubName;
	public UIButton AcceptButton;
	public UIButton RefuseButton;

	private string clubID;
	private int minEnterPrice;

	public void Init(string ClubID)
	{
		ClubName.text = "";
		clubID = ClubID;
		ServerInfo.Instance.GetClub(ClubID,(club)=>{
			ClubName.text = club.ClubName;
			minEnterPrice = club.MinEnterPrice;
			if (!string.IsNullOrEmpty(club.Icon))
			{
				ImageLoader.Instance.LoadAvatar(club.Icon,(tex)=>{
					Vector2 size = UITools.ResizeTo(tex,66);
					ClubTexture.width = (int)size.x;
					ClubTexture.height = (int)size.y;
					ClubTexture.mainTexture = tex;
				});
			}
		});
		AcceptButton.onClick.Clear();
		AcceptButton.onClick.Add(new EventDelegate(()=>{Accept();}));
		RefuseButton.onClick.Clear();
		RefuseButton.onClick.Add(new EventDelegate(()=>{Refuse();}));
	}

	public void OpenClub()
	{
		ClubWindow.Show(clubID);
	}

	private void Accept()
	{
		if (MyAccountWindow.MyProfile.Gold<minEnterPrice)
			AlertWindow.Show("ОШИБКА","У вас недостаточно средств для вступления в клуб. " +
				"Минимальный первоначальный взнос в клубе "+minEnterPrice+"кг. золота.",null,null);
		else
		ServerInfo.Instance.AddUserToClub(clubID,(accepted)=>{
				if (accepted)
				{
					AlertWindow.Show("УВЕДОМЛЕНИЕ","Вы успешно вступили в клуб",null,null);
					LobbyHeaderLoader.ReInit();
				}
				else
					AlertWindow.Show("ОШИБКА","Ошибка при зачислении в клуб (возможно, в клубе привышен лемит членов клуба)",null,null);
				GameObject.FindObjectOfType<MyAccountWindow>().UpdateWindow();
		});
	}

	private void Refuse()
	{
		ServerInfo.Instance.RefuseClubInvation(clubID,()=>{
			GameObject.FindObjectOfType<MyAccountWindow>().UpdateWindow();
		});
	}
}
