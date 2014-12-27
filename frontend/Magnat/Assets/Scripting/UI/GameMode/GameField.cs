using UnityEngine;
using System.Collections;

[System.Serializable]
public enum MonopolyRank
{
	Base = 0,
	Monopoly = 1,
	Branch1 = 2,
	Branch2 = 3,
	Branch3 = 4,
	Branch4 = 5,
	Holding = 6
}

[System.Serializable]
public class GameField : MonoBehaviour 
{
	[System.Serializable]
	public enum Owners
	{
		None = 0,
		Blue = 1,
		Green = 2,
		Orange = 3,
		Purple = 4,
		Red = 5
	}

	[System.Serializable]
	public enum FieldEffects
	{
		GameEffect,
		Start,
		Customs,
		Jackpot,
		Vacation,
		Tax,
		Lottery,
		SkipStep,
		StepBack
	}

	[SerializeField] public GameObject targetSprite;

	[SerializeField] public string normalSprite;
	[SerializeField] public string blueSprite;
	[SerializeField] public string greenSprite;
	[SerializeField] public string orangeSprite;
	[SerializeField] public string purpleSprite;
	[SerializeField] public string redSprite;

	[SerializeField] public GameObject lockerSptite;
	[SerializeField] public GameObject monopolyStateSprite;

	private MonopolyRank currentMonopolyRank;
	public MonopolyRank CurrentMonopolyRank
	{
		get { return currentMonopolyRank; }
		set
		{
			currentMonopolyRank = value;
			if (Effect != FieldEffects.GameEffect) return;
			// создадим GO для монополии, если такого нету
			if (monopolyStateSprite == null)
			{
				monopolyStateSprite = NGUITools.AddChild(gameObject);
				monopolyStateSprite.name = "MonopolyIcon";
			}
			// найдем спрайт
			UISprite sprite = monopolyStateSprite.GetComponent<UISprite>();
			if (sprite == null)
			{
				sprite = NGUITools.AddMissingComponent<UISprite>(monopolyStateSprite);
				sprite.atlas = targetSprite.GetComponent<UISprite>().atlas;
				sprite.depth = 2;
			}
			// зададим данные для спрайта
			string sname = "";
			switch (value)
			{
			case MonopolyRank.Branch1: sname = "house_1"; break;
			case MonopolyRank.Branch2: sname = "house_2"; break;
			case MonopolyRank.Branch3: sname = "house_3"; break;
			case MonopolyRank.Branch4: sname = "house_4"; break;
			case MonopolyRank.Holding: sname = "house_monopoly"; break;
			}
			// если нужен спрайт, поставим его
			sprite.spriteName = sname;
			// и отпозиционируем
			if (sname != "")
			{
				UISpriteData sdata = targetSprite.GetComponent<UISprite>().atlas.GetSprite(sname);
				sprite.width = (int)(sdata.width*0.7);
				sprite.height = (int)(sdata.height*0.7);
				UIWidget w = targetSprite.GetComponent<UIWidget>();
				if (w.panel == null) w.panel = w.transform.GetComponentInParent<UIPanel>();
				Vector3 pos = w.panel.transform.InverseTransformPoint(transform.position);
				if (Mathf.Abs(pos.x) > Mathf.Abs(pos.y))
				{
					if (pos.x<0)
					{
						// левая колонка
						pos.x += w.width/2+sprite.height/2;
						monopolyStateSprite.transform.rotation = Quaternion.Euler(0,0,270);
					}
					else
					{
						// правая колонка
						pos.x -= w.width/2+sprite.height/2;
						monopolyStateSprite.transform.rotation = Quaternion.Euler(0,0,90);
					}
				}
				else
				{
					// поле в верхней/нижней колонке 
					if (pos.y<0)
					{
						// низ
						pos.y += w.height/2+sprite.height/2;
						monopolyStateSprite.transform.rotation = Quaternion.Euler(0,0,0);
					}
					else
					{
						// верх
						pos.y -= w.height/2+sprite.height/2;
						monopolyStateSprite.transform.rotation = Quaternion.Euler(0,0,180);
					}
				}
				monopolyStateSprite.transform.position = w.panel.transform.TransformPoint(pos);
			}
		}
	}

	private Owners owner;
	public Owners Owner
	{
		get { return owner; }
		set 
		{
			owner = value;
			if (targetSprite!=null)
			{
				UISprite sprite = targetSprite.GetComponent<UISprite>();
				UIButton button = targetSprite.GetComponent<UIButton>();
				if (sprite!=null)
				{
					switch (owner)
					{
					case Owners.None:
						sprite.spriteName = normalSprite;
						button.normalSprite = normalSprite;
						break;

					case Owners.Blue:
						sprite.spriteName = blueSprite;
						button.normalSprite = blueSprite;
						break;
						
					case Owners.Green:
						sprite.spriteName = greenSprite;
						button.normalSprite = greenSprite;
						break;
						
					case Owners.Orange:
						sprite.spriteName = orangeSprite;
						button.normalSprite = orangeSprite;
						break;
						
					case Owners.Purple:
						sprite.spriteName = purpleSprite;
						button.normalSprite = purpleSprite;
						break;
						
					case Owners.Red:
						sprite.spriteName = redSprite;
						button.normalSprite = redSprite;
						break;

					default:
						sprite.spriteName = normalSprite;
						button.normalSprite = normalSprite;
						break;
					}
				}
			}
		}
	}

	public GameObject targetPriceField;
	private int price = 180;
	public int Price 
	{
		get { return price; }
		set
		{
			price = value;
			if (targetPriceField!=null)
			{
				UILabel l = targetPriceField.GetComponent<UILabel>();
				if (l!=null)
				{
					string k = "";
					int tp = value;
					if (tp>1000)
					{
						tp/=1000;
						k+="k";
					}
					l.text = tp+k;
				}
			}
		}
	}

	private bool locked;
	public bool Locked
	{
		get { return locked; }
		set
		{
			locked = value;
			if (lockerSptite!=null)
				lockerSptite.SetActive(locked);
		}
	}

	[SerializeField] public Transform ChipFirstPosition;
	[SerializeField] public Transform ChipLastPosition;

	[SerializeField] public FieldEffects Effect;
}
