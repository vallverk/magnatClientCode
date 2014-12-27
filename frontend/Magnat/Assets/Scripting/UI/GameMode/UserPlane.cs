using UnityEngine;
using System.Collections;

public class UserPlane : MonoBehaviour 
{
	public GameObject BackBigSprite;
	public GameObject BackSmallSprite;
	public GameObject NameLabel;
	public GameObject StatusLabel;
	public GameObject CashLabel;
	public GameObject CapitalLabel;
	public GameObject CrownGO;
	public GameObject GiftGO;
	public GameObject TimeLabel;
	public GameObject BankroutSprite;
	public GameObject TransactionButtonGO;

	[SerializeField] public string bigBackSprite;
	[SerializeField] public string smallBackSprite;
	[SerializeField] private string name;
	[SerializeField] private string status;
	[SerializeField] private int cash;
	[SerializeField] private int capital;
	[SerializeField] private int time;
	[SerializeField] private bool enableCrown;
	[SerializeField] private bool enableGift;

	public void LoadAvatar(string AvatarURL)
	{
		ImageLoader.Instance.LoadAvatar(AvatarURL, (tex) => { transform.GetComponentInChildren<UITexture>().mainTexture = tex; } );
	}

	public void LoadGift(string UserID)
	{
		EnableGift = false;
		ServerInfo.Instance.GetUserGifts(UserID,(gifts)=>{
			if (gifts.Length!=0)
			{
				Gift g = gifts[gifts.Length-1];
				if (!string.IsNullOrEmpty(g.image))
					ImageLoader.Instance.LoadAvatar(g.image,(tex)=>{
						UITexture gtex = GiftGO.GetComponent<UITexture>();
						if (gtex!=null)
						{
							Vector2 size = UITools.ResizeTo(tex,50);
							gtex.width = (int)size.x;
							gtex.height = (int)size.y;
							gtex.mainTexture = tex;
							EnableGift = true;
						}
					});
			}
		});
	}

	public bool EnableCrow
	{
		get { return enableCrown; }
		set 
		{
			enableCrown = value;
			if (CrownGO!=null)
				CrownGO.SetActive(value);
		}
	}

	private bool enableTransaction;
	public bool EnableTransaction
	{
		get { return enableTransaction; }
		set
		{
			enableTransaction = value;
			if (TransactionButtonGO != null)
				TransactionButtonGO.SetActive(value && size == PlaneSize.Small);
		}
	}
		

	private bool bankrout;
	public bool Bankrout
	{
		get {return bankrout;}
		set
		{
			bankrout = value;
			if (BankroutSprite!=null)
				BankroutSprite.SetActive(value);
		}
	}

	public bool EnableGift
	{
		get { return enableGift; }
		set 
		{
			enableGift = value;
			if (GiftGO!=null)
				GiftGO.SetActive(value);
		}
	}

	public int Time
	{
		get { return this.time; }
		set
		{
			time = value;
			if (TimeLabel!=null)
			{
				UILabel l = TimeLabel.GetComponent<UILabel>();
				if (l!=null)
				{
					if (time!=-1)
						l.text = time.ToString("0 сек");
					else
						l.text = "";
				}
			}
		}
	}

	public enum PlaneSize
	{
		Small,
		Big
	}

	[SerializeField] private PlaneSize size;

	public PlaneSize Size
	{
		get { return this.size; }
		set
		{
			size = value;
			if (BackBigSprite != null && BackSmallSprite != null)
			{
				BackBigSprite.SetActive(value == PlaneSize.Big);
				BackSmallSprite.SetActive(value == PlaneSize.Small);
				if (TransactionButtonGO != null)
					TransactionButtonGO.SetActive(value == PlaneSize.Small && enableTransaction);
			}
			if (TimeLabel!=null)
				TimeLabel.SetActive(value == PlaneSize.Big);
		}
	}

	public string Name
	{
		get { return this.name; }
		set
		{
			name = value;
			if (NameLabel!=null)
			{
				UILabel l = NameLabel.GetComponent<UILabel>();
				if (l!=null)
				{
					l.text = name;
				}
			}
		}
	}

	public string Status
	{
		get { return this.status; }
		set
		{
			status = value;
			if (StatusLabel!=null)
			{
				UILabel l = StatusLabel.GetComponent<UILabel>();
				if (l!=null)
				{
					l.text = status;
				}
			}
		}
	}

	public int Cash
	{
		get { return this.cash; }
		set
		{
			cash = value;
			if (CashLabel!=null)
			{
				UILabel l = CashLabel.GetComponent<UILabel>();
				if (l!=null)
				{
					l.text = cash.ToString("###,###,###,##0");
				}
			}
		}
	}

	public int Capital
	{
		get { return this.capital; }
		set
		{
			capital = value;
			if (CapitalLabel!=null)
			{
				UILabel l = CapitalLabel.GetComponent<UILabel>();
				if (l!=null)
				{
					l.text = capital.ToString("###,###,###,##0");
				}
			}
		}
	}
}
