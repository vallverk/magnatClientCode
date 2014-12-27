using UnityEngine;
using System.Collections;

public class ClubMemberFieldForOwner : ClubMemberField 
{
	public GameObject SetNewLeaderButton;
	public GameObject RemoveFromClubButton;

	public override void Init (ClubInfo Club, ServerUserInfo User, int Number)
	{
		base.Init (Club, User, Number);
		bool ownerField = club.CreatorID == user.GUID;
		SetNewLeaderButton.SetActive(!ownerField);
		RemoveFromClubButton.SetActive(!ownerField);
	}

	public void SetNewLeader()
	{
		AlertWindow.Show("ПОДТВЕРЖДЕНИЕ","Подтвердите смену владельца клуба",()=>{
			ServerInfo.Instance.ChangeClubOwner(user.GUID,(res)=>{
                if (res)
                {
                    AlertWindow.Show("УВЕДОМЛЕНИЕ", "Смена владельца клуба прошла успешно");
                    LobbyHeaderLoader.ReInit();
                    MyAccountWindow w = GameObject.FindObjectOfType<MyAccountWindow>();
                    if (w != null)
                        w.UpdateWindow();
                }
                else
                    AlertWindow.Show("УВЕДОМЛЕНИЕ", "Ошибка при смене владельца клуба");
				ClubWindow.Show(club.ID);
			});
		},()=>{});
	}

	public void RemoveFromClub()
	{
		AlertWindow.Show("ПОДТВЕРЖДЕНИЕ","Исключить человека из клуба?",()=>{
			ServerInfo.Instance.DeleteUserFromClub(user.GUID,(res)=>{
				if (res)
					AlertWindow.Show("УВЕДОМЛЕНИЕ","Пользователь исключен из клуба");
				else
					AlertWindow.Show("УВЕДОМЛЕНИЕ","Ошибка во время исключения человека из клуба");
				ClubWindow.Show(club.ID);
			});
		},()=>{});
	}
}
