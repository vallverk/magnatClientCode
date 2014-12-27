using UnityEngine;
using System.Collections;

public class CLubCardFieldFree : ClubCardField 
{
	public UILabel InfoLabel;
	public UIButton BuyButton;
	public UILabel PriceLabel;

	[HideInInspector] public bool ReBuy = false;

	public override void Init (ClubInfo Club, ClubCard Card)
	{
		base.Init (Club, Card);

		PriceLabel.text = card.price+" кг";

		ServerInfo.Instance.GetClubStatuses((statuses)=>{
			InfoLabel.text = string.Format("[fe5151]Уровень:[-] {0}" +
			                               "\r\n[fe5151]Срок действия:[-] {1} дней" +
			                               "\r\n[fe5151]Действует для:[-] [f3ff73]{2}[-]",
			                               statuses[int.Parse(card.Lavel)-1].Title,
			                               card.term,
			                               card.status == "0"?"Стандарт":"VIP");
		});
		
		BuyButton.onClick.Clear();
		BuyButton.onClick.Add(new EventDelegate(Buy));
	}

	public void Buy()
	{
        Debug.LogWarning("BUY THIS!");

		ServerInfo.Instance.GetUserInfo(SocialManager.User,(u)=>{
			if (club.Gold<int.Parse(card.price))
				AlertWindow.Show("ОШИБКА","В казне клуба не достаточно золота");
			else
			{
                if (ReBuy)
                    AlertWindow.Show("УВЕДОМЛЕНИЕ", "Предыдущая карта этого типа будет заменена на новую. Купить?", () =>
                    {
                        if (u.VIP == 0 && card.status == "1")
                            AlertWindow.Show("ОШИБКА", "Ваш статус не соответствует статусу карты");
                        else
                        {
                            if (int.Parse(card.Lavel) > club.Lavel)
                                AlertWindow.Show("ОШИБКА", "Нельзя покупать карты, уровень которых привышает уровень клана");
                            else
                            {
                                if (ServerInfo.Instance.GetLevelByStatus(u.Title.Trim().ToLower()) <
                                    ServerInfo.Instance.GetLevelByStatus(card.Lavel.Trim().ToLower()))
                                {
                                    // ошибка титула
                                    AlertWindow.Show("ОШИБКА", "Ваш титул не соответствует тебуемому для данной карты");
                                }
                                else
                                {
                                    ServerInfo.Instance.BuyClubCard(club.ID, card._id, (res) =>
                                    {
                                        transform.GetComponentInParent<VipClubFor>().UpdateWindow();
                                    });
                                }
                            }
                        }
                    },
                    () => { });
                else
                {
                    if (u.VIP == 0 && card.status == "1")
                        AlertWindow.Show("ОШИБКА", "Ваш статус не соответствует статусу карты");
                    else
                    {
                        if (int.Parse(card.Lavel) > club.Lavel)
                            AlertWindow.Show("ОШИБКА", "Нельзя покупать карты, уровень которых привышает уровень клана");
                        else
                        {
                            if (ServerInfo.Instance.GetLevelByStatus(u.Title.Trim().ToLower()) <
                                ServerInfo.Instance.GetLevelByStatus(card.Lavel.Trim().ToLower()))
                            {
                                // ошибка титула
                                AlertWindow.Show("ОШИБКА", "Ваш титул не соответствует тебуемому для данной карты");
                            }
                            else
                            {
                                ServerInfo.Instance.BuyClubCard(club.ID, card._id, (res) =>
                                {
                                    transform.GetComponentInParent<VipClubFor>().UpdateWindow();
                                });
                            }
                        }
                    }
                }
			}
		},false);

	}
}
