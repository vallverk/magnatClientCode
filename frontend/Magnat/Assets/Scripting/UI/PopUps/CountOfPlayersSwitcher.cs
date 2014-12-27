using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CountOfPlayersSwitcher : MonoBehaviour 
{
	public ButtonEx[] Buttons;
	private List<ButtonEx> buttons = null;
	public int Players = 2;
	public int MinPlayersCount = 2;

	private bool normalGame;

	void Start()
	{
		buttons = new List<ButtonEx>(Buttons);
		for(int i=0;i<buttons.Count;i++)
		{
			buttons[i].EventClick+=this.OnClick;
			buttons[i].EventHover+=this.OnHover;
			buttons[i].EventNormal+=this.OnNormal;
		}
		UpdateTexs();
	}

	public void SetCount(int Count)
	{
		Players = Count;
		UpdateTexs();
	}

	public void SetType(GameType Type)
	{
		switch (Type)
		{
		case GameType.Standart:
			normalGame = true;
			UpdateTexs();
			break;
		case GameType.TwoVSTwo:
			normalGame = false;
			UpdateTexs();
			break;
		}
	}

	void UpdateTexs()
	{
		if (buttons == null) return;
		if (!normalGame)
			Players = 4;
		else
			Players = Mathf.Max(Players,MinPlayersCount);
		for (int i=0;i<buttons.Count;i++)
			buttons[i].UpdateSprite(i<Players?UIButtonColor.State.Pressed:UIButtonColor.State.Hover);
	}

	void UpdateTexs(int currentHover)
	{
		if (buttons == null) return;
		if (!normalGame)
			Players = 4;
		else
			Players = Mathf.Max(Players,MinPlayersCount);
		for (int i=0;i<buttons.Count;i++)
		{
			if (i<Players)
				buttons[i].UpdateSprite(UIButtonColor.State.Pressed);
			else
			{
				if (i<currentHover)
					buttons[i].UpdateSprite(UIButtonColor.State.Disabled);
				else
					buttons[i].UpdateSprite(UIButtonColor.State.Hover);
			}
		}
	}

	private void OnClick(ButtonEx b)
	{
		if (!normalGame) return;
		int n = buttons.IndexOf(b)+1;
		if (n>=MinPlayersCount)
		{
			Players = Mathf.Max(n,MinPlayersCount);
			UpdateTexs();
		}
	}

	private void OnNormal(ButtonEx b)
	{
		UpdateTexs();
	}

	private void OnHover(ButtonEx b)
	{
		if (normalGame)
			UpdateTexs(buttons.IndexOf(b)+1);
	}
}
