using UnityEngine;
using System.Collections;

public class ClubCardField : MonoBehaviour 
{
	public UITexture CardIcon;
	public UILabel NameLabel;
	public UILabel DescritionLabel;

	protected ClubCard card;
	protected ClubInfo club;

	public virtual void Init(ClubInfo ClubID, ClubCard Card)
	{
		card = Card;
		club = ClubID;
		CardIcon.mainTexture = null;

		if (!string.IsNullOrEmpty(Card.image))
		{
			//Debug.LogError(card.image);
			ImageLoader.Instance.LoadAvatar(card.image,(tex)=>{
				Vector2 size = UITools.ResizeTo(tex,115);
				CardIcon.width = (int)size.x;
				CardIcon.height = (int)size.y;
				CardIcon.mainTexture = tex;
			});

			NameLabel.text = card.Name;
			DescritionLabel.text = card.Description;
		}
	}
}
