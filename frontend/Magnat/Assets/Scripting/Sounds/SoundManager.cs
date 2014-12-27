using UnityEngine;
using System.Collections;

public class SoundManager : MonoBehaviour 
{
	public AudioClip OnButtonHover;
	public AudioClip OnButtonPress;
	public AudioClip OnMyStepStartSound;
	public AudioClip OnOpenAuctionWindowSound;
	public AudioClip OnPayComeInSound;
	public AudioClip OnTransactionWindowSound;
	public AudioClip OnThrowDiceSound;
	public AudioClip OnPrivatMessageSound;
	public AudioClip OnPrivateMessageSendSound;
	public AudioClip OnTransactionSuccessSound;
	public AudioClip OnTransactionFailSound;
	public AudioClip OnPayOutSound;

	public AudioClip OnStepOverSound;

	public UIButton[] OnClick;
	public UIButton[] OnHover;

	private static SoundManager instance;

	private static AudioSource stepOwerSound = null;
	private static bool canPlay = true;

	void OnApplicationFocus(bool focus) 
	{
		canPlay = focus;
	}

	public static void StopStepOverSound()
	{
		if (stepOwerSound!=null)
			stepOwerSound.volume = 0;
	}

	public static void PlayStepOverSound()
	{
		if (stepOwerSound==null)
			stepOwerSound = instance.gameObject.AddComponent<AudioSource>();

		stepOwerSound.clip = instance.OnStepOverSound;
		stepOwerSound.volume = 1;
		stepOwerSound.Play();
	}

	public static void PlayPrivatMessageReceive()
	{
		if (Settings.SoundOn)
			NGUITools.PlaySound(instance.OnPrivatMessageSound);
	}

	public static void PlayPrivatMessageSend()
	{
		if (Settings.SoundOn)
			NGUITools.PlaySound(instance.OnPrivateMessageSendSound);
	}

	public static void PlayThrowDice()
	{
		if (Settings.SoundOn)
			NGUITools.PlaySound(instance.OnThrowDiceSound);
	}

	public static void PlayTransactionSucces()
	{
		if (Settings.SoundOn)
			NGUITools.PlaySound(instance.OnTransactionSuccessSound);
	}

	public static void PlayTransactionFail()
	{
		if (Settings.SoundOn)
			NGUITools.PlaySound(instance.OnTransactionFailSound);
	}

	public static void PlayTransactionWindow()
	{
		if (Settings.SoundOn)
			NGUITools.PlaySound(instance.OnTransactionWindowSound);
	}

	public static void PlayPayInSound()
	{
		if (Settings.SoundOn)
			NGUITools.PlaySound(instance.OnPayOutSound);
	}

	public static void PlayPayOutSound()
	{
		if (Settings.SoundOn)
			NGUITools.PlaySound(instance.OnPayComeInSound);
	}

	public static void PlayAuctionWindow()
	{
		if (Settings.SoundOn)
			NGUITools.PlaySound(instance.OnOpenAuctionWindowSound);
	}

	public static void PlayMyStepStart()
	{
		if (Settings.SoundOn)
			NGUITools.PlaySound(instance.OnMyStepStartSound);
	}

	public static void PlayPressSound()
	{
		if (Settings.SoundOn&& canPlay)
			NGUITools.PlaySound(instance.OnButtonPress);
	}

	public static void PlayHoverSound()
	{
		if (Settings.SoundOn&& canPlay)
			NGUITools.PlaySound(instance.OnButtonHover);
	}

	void Start()
	{
		instance = this;

		foreach (var b in OnHover)
			b.onHover.Add(new EventDelegate(()=>{
				if (Settings.SoundOn && canPlay) NGUITools.PlaySound(OnButtonHover);
			}));

		foreach (var b in OnClick)
			b.onClick.Add(new EventDelegate(()=>{
				if (Settings.SoundOn && canPlay) NGUITools.PlaySound(OnButtonPress);
			}));
	}
}
