using UnityEngine;
using System.Collections;
using System;

public class DepositClubWindow : WindowBehavoiur 
{
	public UIInput ClubNateText;
	public string Value 
	{ 
		get { return ClubNateText.value; } 
		private set { ClubNateText.value = value; }
	}
	
	private Action<int> Callback;
	
	public void Show(Action<int> Callback)
	{
		Value = "50";
		ShowAtTop();
		this.Callback = Callback;
	}
	
	public override void Hide ()
	{
		base.Hide ();
		if (Callback!=null)
		{
			//Callback(null);
			Callback = null;
		}
	}
	
	public void Accept()
	{
		Callback(int.Parse(Value));
		Callback = null;
		Hide();
	}
}
