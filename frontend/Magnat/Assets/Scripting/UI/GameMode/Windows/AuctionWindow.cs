using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AuctionWindow : MonoBehaviour 
{
	public GameObject TextInfo;
	private UILabel info;
	public GameObject TextTime;
	UILabel time;
	public GameObject Shadow;

	public GameObject BackGround;
	public UIButton AcceptButton;
	public UIButton CancelButton;

	public string PlayerName = "Виталий";
	public string FieldName = "Судостроительная компания";

	public int CurrentPrice = 320000;
	public int IncStep = 50000;
	public float TimerTime=15;

	private TweenAlpha bgTween;
	private TweenAlpha shadowTween;

	void Start()
	{
		bgTween = NGUITools.AddMissingComponent<TweenAlpha>(BackGround);
		shadowTween = NGUITools.AddMissingComponent<TweenAlpha>(Shadow);
		Shadow.GetComponent<UIWidget>().alpha = 0;
		BackGround.GetComponent<UIWidget>().alpha = 0;
	}

	public void OnValidate()
	{
		if (info == null)
			info = TextInfo.GetComponent<UILabel>();
		info.text = string.Format("[999999]Игрок:[-] {0}\r\n[999999]выставил на аукцион:[-] {1}\r\n\r\n\r\n" +
			"[999999]Текущая цена покупки:[-] {2}\r\n[999999]Шаг повышения цены:[-] {3}", 
		                          PlayerName, FieldName, CurrentPrice.ToString("$### ### ##0"), IncStep.ToString("$### ### ##0"));
		if (time == null)
			time = TextTime.GetComponent<UILabel>();
		time.text = string.Format("Автоматический отказ через {0} сек",(int)TimerTime);
	}

	public void Show()
	{
		SoundManager.PlayAuctionWindow();
		bgTween.PlayForward();
		shadowTween.PlayForward();
	}

	public void Hide()
	{
		bgTween.PlayReverse();
		shadowTween.PlayReverse();
	}
}
