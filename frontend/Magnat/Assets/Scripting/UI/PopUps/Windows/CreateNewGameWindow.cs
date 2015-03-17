using System;
using UnityEngine;
using System.Collections;

public class CreateNewGameWindow : WindowBehavoiur 
{
	public string GName = "Default gamename";
	public GameType GType = GameType.Standart;
	public int GPlayersCount = 1;
	public int GBet = 0;
	public string GPassword = "";

	public UIInput GNameInputField;
	public UIInput GPasswordInputField;
	public UIToggle StandartGameToggle;
	public CountOfPlayersSwitcher PlayersCountSwitcher;
	public BetSwitcher BetSwitcher;

	public void UpdateGameMode()
	{
		PlayersCountSwitcher.SetType(StandartGameToggle.value?GameType.Standart:GameType.TwoVSTwo);
	}

	private void GetData()
	{
		GName = GNameInputField.value;
		if (string.IsNullOrEmpty(GName))
		{
			var data = SocialManager.User;
			if (data != null)
				GName = data.FormatName;
			else
				GName = "DefaultGameName";
		}
		GPassword = GPasswordInputField.value;
		GPlayersCount = PlayersCountSwitcher.Players;
		GBet = BetSwitcher.GoldBet;
		GType = StandartGameToggle.value?GameType.Standart:GameType.TwoVSTwo;
	}

	public void CreateGame()
	{ 
        Debug.Log("Create game");
		GetData();
		try
		{
			ServerInfo.Instance.GetUserInfo(SocialManager.User,(u)=>{
				if (GBet<=u.Gold)
				{
                    if (u.GameCards > 0 || u.VIP != 0)
                    {
                        GameInfoController.DisconectFromAll(() =>
                        {
                            Debug.LogWarning("CALL BACK");

                            ServerInfo.Instance.CreateNewGameAnnounce(GName, GType, GPlayersCount, GBet, GPassword,
                                (res) =>
                                {
                                    Hide();
                                });
                        });

                        GameInfoController.ConnectedGameID = -1;

                    }
                    else
                        GameObject.FindObjectOfType<NoCards>().Show();
				}
				else
				{
					AlertWindow.Show("ОШИБКА","Нельзя ставить ставку больше, чем у вас есть золота");
				}
			},true);
		} 
		catch(Exception e)
		{ 
            Debug.Log(e);
			AlertWindow.Show("ОШИБКА","Ошибка создания игры. Перезагрузите страницу и попробуйте еще раз.");
		}
	}
}
