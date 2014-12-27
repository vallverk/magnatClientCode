using UnityEngine;
using System.Collections;
using System;

public class GetTextDialogWindow : WindowBehavoiur 
{
	public static void Show(string WindowTitle, string CurrentText, Action<string> Callback)
	{
		var window = GameObject.FindObjectOfType<GetTextDialogWindow>();
		window.WindowTitle = WindowTitle;
		window.Show(CurrentText,Callback);
	}

	public UILabel WindowTitleLabel;
	public string WindowTitle
	{
		get { return WindowTitleLabel.text; }
		set { WindowTitleLabel.text = value; }
	}

	public UIInput ClubDescriptionText;
	public string Value 
	{ 
		get { return ClubDescriptionText.value; } 
		private set { ClubDescriptionText.value = value; }
	}

	private Action<string> Callback;

	public void Show(string CurrentDescription, Action<string> Callback)
	{
		Value = CurrentDescription;
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
