using UnityEngine;
using System.Collections;
using System;

public class ChangeClubNameWindow : WindowBehavoiur 
{
	public UIInput ClubNateText;
	public string Value 
	{ 
		get { return ClubNateText.value; } 
		private set { ClubNateText.value = value; }
	}

	private Action<string> Callback;

	public void Show(string CurrentName, Action<string> Callback)
	{
		Value = CurrentName;
		ShowAtTop();
		this.Callback = Callback;
	}

	public override void Hide ()
	{
		base.Hide ();
		if (Callback!=null)
		{
			Callback(null);
			Callback = null;
		}
	}

	public void Accept()
	{
		Callback(Value);
		Callback = null;
		Hide();
	}
}
