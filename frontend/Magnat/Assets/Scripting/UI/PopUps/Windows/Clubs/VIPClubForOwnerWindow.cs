using UnityEngine;
using System.Collections;

public class VIPClubForOwnerWindow : VipClubFor
{
	public UIInput MinEnterPrice;

	public override void Show (ClubInfo club)
	{
		base.Show (club);
		MinEnterPrice.value = club.MinEnterPrice.ToString();
	}

	public void RemoveClubEvents()
	{
		AlertWindow.Show("УВЕДОМЛЕНИЕ","Подтвердите удаление истории операций клуба.",()=>{
			ServerInfo.Instance.RemoveClubEvents(currentinfo.ID,()=>{
				UpdateWindow();
			});
		},()=>{});
	}

	public void SetClubDescription()
	{
		GetTextDialogWindow.Show("ОПИСАНИЕ ГРУППЫ",currentinfo.Description,(desc)=>{
			if (!string.IsNullOrEmpty(desc))
			{
				ServerInfo.Instance.SetClubDescription(currentinfo.ID,desc,(res)=>{

					string udata = SocialManager.User.FormatName;
					string msg = "[9b9b9b][ffffff]"+udata+"[-] изменил(а) информацию о клубе[-]";
					ServerInfo.Instance.AddClubEvent(currentinfo.ID,msg,()=>{});
					LobbyHeaderLoader.ReInit();
					UpdateWindow();
				});
			}
		});
	}

	public void SetMinEnterPrice()
	{
		int mep =  0;
		if (string.IsNullOrEmpty(MinEnterPrice.value) || !int.TryParse(MinEnterPrice.value,out mep))
			AlertWindow.Show("ОШИБКА","Минимальная цена не указана, либо указана не правильно.",null,null);
		else
		{
			ServerInfo.Instance.SetClubMinEnterPrice(currentinfo.ID,mep,(res)=>{

				string udata = SocialManager.User.FormatName;
				string msg = "[9b9b9b][ffffff]"+udata+"[-] изменил(а) цену вступления в клуб[-]";
				ServerInfo.Instance.AddClubEvent(currentinfo.ID,msg,()=>{});
				LobbyHeaderLoader.ReInit();
				UpdateWindow();
			});
		}
	}

	public void SetNewIcon()
	{
        ServerInfo.Instance.GetGoldCurses((prices) =>
        {
            int price = 0;
            if (int.TryParse(prices.avatar, out price))
            {
                if (price <= currentinfo.Gold)
                {
                    AlertWindow.Show("УВЕДОМЛЕНИЕ", "Стоимость данной операции " + price.ToString() + " кг золота."+
                        "Подтвердите операцию. Золото будет снято с казны клуба.", () =>
                    {
                        Application.ExternalCall("OpenFileDialog");
                    }, () => { });
                }
                else
                    AlertWindow.Show("Ошибка", "В вашем клубе не достаточно средств для выполнения данной операции");
            }
            else
                AlertWindow.Show("Ошибка", "Не удалось получить стоимость операции от сервера");
        });
	}

	public void OnFileSelected(string file)
	{
		if (file!="404" && file!="503")
        {
            ServerInfo.Instance.GetGoldCurses((prices) =>
            {
                int price = 0;
                if (int.TryParse(prices.avatar, out price))
                {
                    if (price <= currentinfo.Gold)
                    {
                        ServerInfo.Instance.SetClubFileIcon(currentinfo.ID, file, (res) =>
                        {
                            ServerInfo.Instance.SetClubGold(currentinfo.ID, currentinfo.Gold -price, (res2) =>
                            {
                                string udata = SocialManager.User.FormatName;
                                string msg = "[9b9b9b][ffffff]" + udata + "[-] изменил(а) иконку клуба.[-]";
                                ServerInfo.Instance.AddClubEvent(currentinfo.ID, msg, () => { });
                                LobbyHeaderLoader.ReInit();
                                UpdateWindow();
                            });
                        });
                    }
                    else
                        AlertWindow.Show("Ошибка", "В клубе не достаточно средств для выполнения данной операции");
                }
                else
                    AlertWindow.Show("Ошибка", "Не удалось получить стоимость операции от сервера");
            });
        }
		else
			NGUIDebugConsole.Log("Ошибка при загрузке аватарки клуба");
	}

	public void OnChangeName()
	{
        ServerInfo.Instance.GetGoldCurses((prices) => {
            int price = 0;
            if (int.TryParse(prices.name, out price))
            {
                if (price <= currentinfo.Gold)
                {
                    AlertWindow.Show("УВЕДОМЛЕНИЕ", "Стоимость данной операции "+price.ToString()+" кг золота (из казны клуба). Подтвердите операцию.", () => {
                        GameObject.FindObjectOfType<ChangeClubNameWindow>().Show(currentinfo.ClubName, (res) =>
                        {
                            if (!string.IsNullOrEmpty(res))
                            {
                                ServerInfo.Instance.SetClubName(currentinfo.ID, res, (result) => {
                                    ServerInfo.Instance.SetClubGold(currentinfo.ID, currentinfo.Gold - price, (res2) =>
                                    {
                                        string udata = SocialManager.User.FormatName;
                                        string msg = "[9b9b9b][ffffff]" + udata + "[-] изменил(а) название клуба.[-]";
                                        ServerInfo.Instance.AddClubEvent(currentinfo.ID, msg, () => { });
                                        LobbyHeaderLoader.ReInit();
                                        UpdateWindow();
                                    });
                                });
                            }
                        });
                    }, () => { });
                }
                else
                    AlertWindow.Show("Ошибка", "В казне вашего клуба не достаточно средств для выполнения данной операции");
            }
            else
                AlertWindow.Show("Ошибка", "Не удалось получить стоимость операции от сервера");
        });
	}

	public void OnExtend()
	{
        ServerInfo.Instance.GetGoldCurses((prices) =>
        {
            int price = 0;
            if (int.TryParse(prices.term, out price))
            {
                if (price <= currentinfo.Gold)
                {
                    AlertWindow.Show("УВЕДОМЛЕНИЕ", "Стоимость данной операции " + price.ToString() + " кг золота (из казны клуба). Подтвердите операцию.", () =>
                    {
                        ServerInfo.Instance.SetClubDateOfDeath(currentinfo.ID, currentinfo.DateOfDeath + 2592000000, (res1) =>
                        {
                            ServerInfo.Instance.SetClubGold(currentinfo.ID, currentinfo.Gold -price, (res2) =>
                            {
                                string udata = SocialManager.User.FormatName;
                                string msg = "[9b9b9b][ffffff]" + udata + "[-] продлил(а) срок действия клуба.[-]";
                                ServerInfo.Instance.AddClubEvent(currentinfo.ID, msg, () => { });
                                LobbyHeaderLoader.ReInit();
                                UpdateWindow();
                            });
                        });
                    }, () => { });
                }
                else
                    AlertWindow.Show("Ошибка", "В казне вашего клуба не достаточно средств для выполнения данной операции");
            }
            else
                AlertWindow.Show("Ошибка", "Не удалось получить стоимость операции от сервера");
        });
	}
}
