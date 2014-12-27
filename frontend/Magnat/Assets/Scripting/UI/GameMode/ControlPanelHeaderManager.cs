using UnityEngine;
using System.Collections;
using System;

public class ControlPanelHeaderManager : MonoBehaviour 
{
	public GameObject GameIDLabel;
	public GameObject GameTimeLabel;
	public GameObject BetLabel;
	public GameObject GameBankLabel;
	public float startTime=0;
	public bool showTime = false;
	public float TimerTime { get { return (Time.time - startTime); } }

	void Update()
	{
		if (showTime)
			GameTime = (int)(Time.time - startTime);
	}

	void Start()
	{
		StartGameTimer();
	}

	public void StartGameTimer()
	{
		startTime = Time.time;
		showTime = true;
	}


	[SerializeField] private long gameID;

	public long GameID
	{
		get { return gameID; }
		set
		{
			gameID = value;
			if (GameIDLabel!=null)
			{
				UILabel l = GameIDLabel.GetComponent<UILabel>();
				if (l!=null)
				{
					l.text = gameID.ToString("№ 0");
				}
			}
		}
	}

	[SerializeField] private int gameTime;

	public int GameTime
	{
		get { return gameTime; }
		set
		{
			gameTime = value;
			if (GameTimeLabel!=null)
			{
				UILabel l = GameTimeLabel.GetComponent<UILabel>();
				if (l!=null)
				{
					int s = gameTime;
					int m = 0;
					int h = 0;
					while (s>59)
					{
						s-=60;
						m++;
					}
					while (m>59)
					{
						m-=60;
						h++;
					}
					l.text = string.Format("В игре: {0}:{1,2:00}:{2,2:00}",h,m,s);
				}
			}
		}
	}

	[SerializeField] private int bet;

	public int Bet
	{
		get { return bet; }
		set
		{
			bet = value;
			if (BetLabel!=null)
			{
				UILabel l = BetLabel.GetComponent<UILabel>();
				if (l!=null)
				{
					l.text = bet.ToString("Ставка 0 кг");
				}
			}
		}
	}

	[SerializeField] private int bank;

	public int Bank
	{
		get { return bank; }
		set
		{
			bank = value;
			if (GameBankLabel!=null)
			{
				UILabel l = GameBankLabel.GetComponent<UILabel>();
				if (l!=null)
				{
					l.text = bank.ToString("Банк 0 кг");
				}
			}
		}
	}
}
