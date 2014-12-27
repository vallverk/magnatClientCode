using UnityEngine;
using System.Collections;

public class ActionField : MonoBehaviour 
{
	public UITexture ActionIcon;
	public int TargetIconSize = 142;
	public UILabel NameLabel;
	public UILabel FieldsLabel;
	public UILabel DiscountLabel;
	public UILabel LifeTimeLabel;
	public UILabel StatusLabel;
	public UILabel TitleLabel;
	public UIButton BuyButton;
	[HideInInspector] public bool ReBuy = false;
	public UILabel PriceLabel;

	private UserAction action = null;

	public void Init(UserAction data)
	{
		UITools.FadeOut(gameObject,0.01f);

		this.action = data;

		string[] ff = data.name.Split(',');
		if (ff.Length == 4)
			FieldsLabel.text = ff[0]+"\t"+ff[1]+"\r\n"+ff[2]+"\t"+ff[3];
		else
		{
			FieldsLabel.text = " ";
			for (int i=0;i<ff.Length;i++)
			{
				FieldsLabel.text += ff[i];
				if (i!=ff.Length-1)
					FieldsLabel.text+="\r\n";
			}
		}

		string[] monopolyNames = new string[11]{
			"Общепит",
			"Мегаполис",
			"Агенство",
			"Табак и алкоголь",
			"Медиа",
			"Развлечения",
			"Спорт",
			"Транспорт",
			"Острова",
			"Добыча топлива",
			"Драгресурсы"
		};

		NameLabel.text = monopolyNames[int.Parse(data.monopoly)-1]+" ("+(int)(10-int.Parse(data.discount)/10)+" уровень)";
		
		DiscountLabel.text = "[b8b8b8]Оплата [ffffff]" + data.discount + "%[-] аренды при попадании на чужую фирму[-]";
		LifeTimeLabel.text = "[fe5151]Срок действия:[-] " + data.term + " дней";
		StatusLabel.text = "[fe5151]Статус:[-] "+(data.status=="0"?"Стандарт":"VIP");

		if (data.lavel == "1")
			data.lavel = "Гранд";
		TitleLabel.text = "[fe5151]Титул:[-] "+data.lavel;

		PriceLabel.text = data.price+" кг";
		//Debug.LogError(string.Format("Action {0}, image {1}",data._id,data.image));
		// упс, костыльчик... 

		ImageLoader.Instance.LoadAvatar(data.image,(tex)=>{
			Vector2 size = UITools.ResizeTo(tex,TargetIconSize);
			ActionIcon.width = (int)size.x;
			ActionIcon.height = (int)size.y;
			ActionIcon.mainTexture = tex;
			UITools.FadeIn(gameObject,0.3f);
		});

		BuyButton.onClick.Clear();
		BuyButton.onClick.Add(new EventDelegate(() => { OnBuy(); }));
	}

	public void OnBuy()
	{
		ServerInfo.Instance.GetUserInfo(SocialManager.User,(u)=>{
            Debug.LogError(string.Format("Покупаем акцию. Титулы: {0} {1}",ServerInfo.Instance.GetLevelByStatus(u.Title.Trim().ToLower()) ,
			    ServerInfo.Instance.GetLevelByStatus(action.lavel.Trim().ToLower())));
			if (ServerInfo.Instance.GetLevelByStatus(u.Title.Trim().ToLower()) <
			    ServerInfo.Instance.GetLevelByStatus(action.lavel.Trim().ToLower()))
			{
				// ошибка титула
				AlertWindow.Show("ОШИБКА","Ваш титул не соответствует тебуемому для данной карты");
			}
			else
			{
				if (int.Parse(action.price)>u.Gold)
					AlertWindow.Show("ОШИБКА","Недостаточно золота для совершения операции.");
				else
				{
					if (ReBuy)
						AlertWindow.Show("ПОДТВЕРЖДЕНИЕ","У вас уже есть пакет акций данного типа. " +
							"Вы действительно хотите сменить пакет акций?",Buy,()=>{});
					else
						Buy();
				}
			}
		},false);
	}

	private void Buy()
	{
		ServerInfo.Instance.BuyAction(action._id,(res)=>{
			switch (res)
			{
			case 200:
				AlertWindow.Show("УВЕДОМЛЕНИЕ","Акция успешно приобретена");
				LobbyHeaderLoader.ReInit();
				break;
			case 504:
				AlertWindow.Show("ОШИБКА","Недостаточно слотов для акций");
				break;
			case 505:
				AlertWindow.Show("ОШИБКА","Невозможно приобрести карту, т. к. ее статус не соответсвует вашему");
				break;
			case 506:
				AlertWindow.Show("ОШИБКА","Невозможно приобрести карту, т. к. ее титул не соответсвует вашему");
				break;
			default:
				AlertWindow.Show("УВЕДОМЛЕНИЕ","Ошибка при покупке акции");
				break;
			}
		});
	}
}
