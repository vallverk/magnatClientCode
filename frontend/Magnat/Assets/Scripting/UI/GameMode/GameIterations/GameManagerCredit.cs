using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public partial class GameManager : MonoBehaviour 
{
	private int stepsToCreditReturn = -1;

	public bool IsCrediting { get { return stepsToCreditReturn>=0; } }
	public bool IsCanCeepCredit { get { return stepsToCreditReturn==-1; } }

	public void CeepCredit()
	{
		currentPlayer.Cash += 3000000;
		UpdateUserData(currentPlayer,true);
		stepsToCreditReturn = 25;
        buttonsManager.HideButtons();
        buttonsManager.ShowButtons();
		LogToMainChat("Вы успешно взяли кредит. Если вы не вернете его в течении 25 ходов - вы автоматически становитесь банкротом.");
	}

	public void ReturnCredit()
	{
		if (currentPlayer.Cash>3000000)
		{
			currentPlayer.Cash -= 3000000;
			UpdateUserData(currentPlayer,false);
			stepsToCreditReturn = -2;
			LogToMainChat("Вы успешно вернули кредит.");
		}
		else
		{
			LogToMainChat("Не хватает денег для погашения кредита.");
		}
	}

	public void CreditStep()
	{
		if (currentPlayer.SocialID == SocialManager.User.ViewerId && IsCrediting && currentState == IterationStep.Start)
		{
			stepsToCreditReturn --;
			if (stepsToCreditReturn<=0)
			{
				LogToMainChat("Вы не успели вернуть кредит и признаны банкротом.");
				SetBankrot(currentPlayer.SocialID);
			}
            else
                LogToMainChat("Количество ходов до возвращения кредита: " + stepsToCreditReturn);
		}
	}
}
