using UnityEngine;
using System.Collections;

public class BankWindow : WindowBehavoiur 
{
	public UILabel kg1;
	public UILabel kg5;
	public UILabel kg10;
	public UILabel kg20;
	public UILabel kg50;

	private string good = null;

    private ServerInfo.GoldStatus buf = null;

	public override void Show ()
	{
		base.Show ();

		ServerInfo.Instance.GetGoldCurses((stats)=>{
            buf = stats;
			kg1.text = stats.kg1+" голосов";
			kg5.text = stats.kg5+" голосов";
			kg10.text = stats.kg10+" голосов";
			kg20.text = stats.kg20+" голосов";
			kg50.text = stats.kg50+" голосов";
		});
	}

	public void Buy7VIP()
	{
		good = "vip7";
		Application.ExternalEval("order('vip7x15')");
		if (Screen.fullScreen)
			Screen.SetResolution(800,800,false);
	}

	public void Buy30VIP()
	{
		good = "vip30";
		Application.ExternalEval("order('vip30x30')");
		if (Screen.fullScreen)
			Screen.SetResolution(800,800,false);
	}

	public void Buy1()
	{
		good = "kg1";
		Application.ExternalEval("order('kg1x"+buf.kg1.ToString()+"')");
		if (Screen.fullScreen)
			Screen.SetResolution(800,800,false);
	}

	public void Buy5()
	{
		good = "kg5";
        Application.ExternalEval("order('kg5x" + buf.kg5.ToString() + "')");
		if (Screen.fullScreen)
			Screen.SetResolution(800,800,false);
	}

	public void Buy10()
	{
		good = "kg10";
        Application.ExternalEval("order('kg10x" + buf.kg10.ToString() + "')");
		if (Screen.fullScreen)
			Screen.SetResolution(800,800,false);
	}

	public void Buy20()
	{
		good = "kg20";
        Application.ExternalEval("order('kg20x" + buf.kg20.ToString() + "')");
		if (Screen.fullScreen)
			Screen.SetResolution(800,800,false);
	}

	public void Buy50()
	{
		good = "kg50";
        Application.ExternalEval("order('kg50x" + buf.kg50.ToString() + "')");
		if (Screen.fullScreen)
			Screen.SetResolution(800,800,false);
	}

	public void OnPaymentSuccessful(string order_id)
	{
		if (good != null)
		{
			switch (good)
			{
			case "vip7":
				ServerInfo.Instance.SetUserVIP(7,()=>{
					AlertWindow.Show("УВЕДОМЛЕНИЕ","Статус VIP приобретен на 7 дней");
					LobbyHeaderLoader.ReInit();
				});
				break;

			case "vip30":
				ServerInfo.Instance.SetUserVIP(30,()=>{
					AlertWindow.Show("УВЕДОМЛЕНИЕ","Статус VIP приобретен на 30 дней");
					LobbyHeaderLoader.ReInit();
				});
				break;

			case "kg1":
				ServerInfo.Instance.UpdateUserGold(1,()=>{
					AlertWindow.Show("УВЕДОМЛЕНИЕ","Вы приобрели 1 кг золота");
					LobbyHeaderLoader.ReInit();
				});
				break;

			case "kg5":
				ServerInfo.Instance.UpdateUserGold(5,()=>{
					AlertWindow.Show("УВЕДОМЛЕНИЕ","Вы приобрели 5 кг золота");
					LobbyHeaderLoader.ReInit();
				});
				break;
				
			case "kg10":
				ServerInfo.Instance.UpdateUserGold(10,()=>{
					AlertWindow.Show("УВЕДОМЛЕНИЕ","Вы приобрели 10 кг золота");
					LobbyHeaderLoader.ReInit();
				});
				break;
				
			case "kg20":
				ServerInfo.Instance.UpdateUserGold(20,()=>{
					AlertWindow.Show("УВЕДОМЛЕНИЕ","Вы приобрели 20 кг золота");
					LobbyHeaderLoader.ReInit();
				});
				break;
				
			case "kg50":
				ServerInfo.Instance.UpdateUserGold(50,()=>{
					AlertWindow.Show("УВЕДОМЛЕНИЕ","Вы приобрели 50 кг золота");
					LobbyHeaderLoader.ReInit();
				});
				break;

			default:
				AlertWindow.Show("ОШИБКА","Ошибка при совершении транзацкии");
				break;
			}
			good = null;
		}
	}

	public void OnPaymentError(object temp)
	{
		AlertWindow.Show("ОШИБКА","Ошибка при совершении транзацкии");
	}

	public void OnPaymentCancel(object temp)
	{
		//AlertWindow.Show("ОШИБКА","Ошибка при совершении транзацкии");
	}
}
