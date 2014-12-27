using UnityEngine;
using System.Collections;

public class SendMessageWindow : WindowBehavoiur 
{
	public UITexture ReceiverAvatar;
	public UIInput TextLabel;

	private string receiver;
	private Texture startTexture;

	protected override void Start ()
	{
		base.Start ();
		startTexture = ReceiverAvatar.mainTexture;
	}

	public static void Show(string Receiver)
	{
		GameObject.FindObjectOfType<SendMessageWindow>().Init(Receiver);
	}

	private void Init(string Receiver)
	{
#if !UNITY_EDITOR
		ImageLoader.Instance.LoadAvatar(SocialManager.GetUserData(Receiver).Photo,(tex)=>{
			ReceiverAvatar.mainTexture = tex;
		});
#endif

		receiver = Receiver;
		ShowAtTop();
	}

	public void Send()
	{
		ServerInfo.Instance.SendUserMessage(receiver,TextLabel.value,()=>{
			Hide();
		});
	}

	public override void Hide ()
	{
		base.Hide ();

		TextLabel.value = "";
		ReceiverAvatar.mainTexture = startTexture;
	}
}
