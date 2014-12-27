using UnityEngine;
using System.Collections;

public class GiftField : MonoBehaviour 
{
	public UITexture IconTexture;
	public UILabel NameLabel;
	public UILabel PriceLabel;
	public string RecaiverID = "";

	public UIButton SendButton;

	private Gift gift;

	public void Init(Gift data)
	{
		gift = data;
		if (!string.IsNullOrEmpty(data.image))
		{
			data.image = "http://moderator.magnatgame.com"+data.image;
			data.image = data.image.Replace("/action/","/gift/");
			//Debug.LogError(data.image);
			ImageLoader.Instance.LoadAvatar(data.image,(tex)=>{
				Vector2 size = UITools.ResizeTo(tex,120);
				IconTexture.width = (int)size.x;
				IconTexture.height = (int)size.y;
				IconTexture.mainTexture = tex;
			});
		}

		NameLabel.text = data.title;
		PriceLabel.text = data.price+" кг";

		SendButton.onClick.Clear();
		SendButton.onClick.Add(new EventDelegate(SendGift));
	}

	public void SendGift()
	{
		GetTextDialogWindow.Show("Введите описание подарка","Подарок",(text)=>{
			if (text!=null)
			{
				ServerInfo.Instance.TakeGift(gift._id,RecaiverID,text,()=>{
					LobbyHeaderLoader.ReInit();
					AlertWindow.Show("УВЕДОМЛЕНИЕ","Подарок успешно отправлен");
				});
				WindowBehavoiur.current.Hide();
			}
		});
	}
}
