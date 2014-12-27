using UnityEngine;
using System.Collections;
using System;

public class AlertWindow : WindowBehavoiur
{
	public UILabel AlertText;
	public UILabel WindowName;
	
	private Action AcceptCallback;
	private Action CancelCallback;

	public UIButton AcceptButton;
	public UIButton CancelButton;
	
	public void ShowAlert(string WindowName, string AlertText, Action Ok, Action Cancel)
	{
		this.AlertText.text = AlertText;
		this.WindowName.text = WindowName;
		ShowAtTop();
		AcceptCallback = Ok;
		AcceptButton.gameObject.SetActive(Ok!=null);
		CancelCallback = Cancel;
		CancelButton.gameObject.SetActive(Cancel!=null);
		Show();
	}
	
	public override void Hide ()
	{
		base.Hide ();
	}

	public void Cancel()
	{
		if (CancelCallback != null)
			CancelCallback();
		Hide();
	}
	
	public void Accept()
	{
		if (AcceptCallback != null)
			AcceptCallback();
		Hide();
	}

	public static void Show(string WindowName, string AlertText, Action Ok, Action Cancel)
	{
		GameObject.FindObjectOfType<AlertWindow>().ShowAlert(WindowName, AlertText, Ok, Cancel);
	}

	public static void Show(string WindowName, string AlertText)
	{
		Show(WindowName,AlertText,null,null);
	}
}
