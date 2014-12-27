using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public partial class GameManager : MonoBehaviour 
{
	private Dictionary<string,List<ClubCard>> usersClubCards = new Dictionary<string, List<ClubCard>>();
	private int userClubLevel;

	private void InitClubCards()
	{
		ServerInfo.Instance.GetUserInfo(ServerDataManager.GetGameInfo().UserList.ToArray(),(users)=>{
			foreach (var u in users)
			{
				usersClubCards.Add(u.GUID,new List<ClubCard>());
				InitUserCards(u);
			}
		});
	}

	private void InitUserCards(ServerUserInfo u)
	{
		if (!string.IsNullOrEmpty(u.ClubId))
		{
			ServerInfo.Instance.GetClubCardList(u.ClubId,(cards)=>{
				// если есть карты - подтянем инфу клуба (чтоб сравнить уровни клуба и карт)
				if (cards.Length!=0)
				{
					ServerInfo.Instance.GetClub(u.ClubId,(club)=>{
						if (u.GUID == SocialManager.User.ViewerId)
							userClubLevel = club.Lavel;
						foreach (var card in cards)
                        {
                            if (int.Parse(card.status) <= (u.VIP == 0 ? 0 : 1) && card.Lavel.Trim() == club.Lavel.ToString())
                            {
                                // нужная карта!
                                usersClubCards[u.GUID].Add(card);
                                Debug.LogWarning(string.Format("У пользователя {0} найдена карта {1}", u.GUID, card.ToString()));
                            }
						}
					});
				}
			});
		}
	}

	public float GetClubCardTaxCoef()
	{
		if (!usersClubCards.ContainsKey(SocialManager.User.ViewerId)) return 1;
		ClubCard card = usersClubCards[SocialManager.User.ViewerId].FirstOrDefault(ecard => ecard.Effect == "nalIm");
		if (card == null) return 1;
		return 1 - userClubLevel*0.1f;
	}

	public bool GetClubCardActivePlayer()
	{
		if (!usersClubCards.ContainsKey(SocialManager.User.ViewerId)) return false;
		ClubCard card = usersClubCards[SocialManager.User.ViewerId].FirstOrDefault(ecard => ecard.Effect == "activ");
		return card != null;
	}

	public bool GetClubCardLeader(string GUID)
	{
		if (!usersClubCards.ContainsKey(GUID)) return false;
		ClubCard card = usersClubCards[GUID].FirstOrDefault(ecard => ecard.Effect == "lider");
		return card != null;
	}

	public bool GetClubCardCustomsFriend()
	{
		if (!usersClubCards.ContainsKey(SocialManager.User.ViewerId)) return false;
		ClubCard card = usersClubCards[SocialManager.User.ViewerId].FirstOrDefault(ecard => ecard.Effect == "friendT");
		return card != null;
	}

	public bool GetClubCardCredit()
	{
		if (!usersClubCards.ContainsKey(SocialManager.User.ViewerId)) return false;
		ClubCard card = usersClubCards[SocialManager.User.ViewerId].FirstOrDefault(ecard => ecard.Effect == "credit");
		return card != null;
	}

	public float GetClubCardTrueBussinesCoef(string GUID)
	{
		if (!usersClubCards.ContainsKey(GUID)) return 1;
		ClubCard card = usersClubCards[GUID].FirstOrDefault(ecard => ecard.Effect == "trueB");
		if (card == null) return 1;
		return 1 + userClubLevel*0.1f;
	}
}
