using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BetSwitcher : MonoBehaviour 
{
	public ButtonEx[] Buttons;
	private List<ButtonEx> buttons = null;
	public int Bet = 1;
	public int GoldBet = 0;
	
	void Start()
	{
		buttons = new List<ButtonEx>(Buttons);
		for(int i=0;i<buttons.Count;i++)
		{
			buttons[i].EventClick+=OnClick;
			buttons[i].EventHover+=OnHover;
			buttons[i].EventNormal+=OnNormal;
		}
		UpdateTexs();
	}
	
	void UpdateTexs()
	{
		if (buttons == null) return;
		
		for (int i=0;i<buttons.Count;i++)
			buttons[i].UpdateSprite(i+1==Bet?UIButtonColor.State.Pressed:UIButtonColor.State.Disabled);
	}
	
	void UpdateTexs(int currentHover)
	{
		if (buttons == null) return;
		
		for (int i=0;i<buttons.Count;i++)
		{
			if (i+1==Bet)
				buttons[i].UpdateSprite(UIButtonColor.State.Pressed);
			else
			{
				if (i+1==currentHover)
					buttons[i].UpdateSprite(UIButtonColor.State.Hover);
				else
					buttons[i].UpdateSprite(UIButtonColor.State.Disabled);
			}
		}
	}
	
	private void OnClick(ButtonEx b)
	{
		Bet = buttons.IndexOf(b)+1;
		GoldBet = (new int[5]{0,5,10,20,50})[Bet-1];
		UpdateTexs();
	}
	
	private void OnNormal(ButtonEx b)
	{
		UpdateTexs();
	}
	
	private void OnHover(ButtonEx b)
	{
		UpdateTexs(buttons.IndexOf(b)+1);
	}
}
