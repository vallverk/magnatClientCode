using UnityEngine;
using System.Collections;

public class ActionFieldBuyed : MonoBehaviour 
{
	public UITexture ActionIcon;
	public int TargetIconSize = 133;
	public UILabel NameLabel;
	public UILabel FieldsLabel;
	public UILabel DiscountLabel;
	public UILabel LifeTimeLabel;
	
	public void Init(UserAction data)
	{

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

		DiscountLabel.text = "[b8b8b8]Оплата [ffffff]" + data.discount + "%[-] аренды при попадании на чужую фирму из текущего пакета акций[-]";
		LifeTimeLabel.text = "[fe5151]Действует до [-] " + TimeTools.FormatUTSTime(long.Parse(data.term));
		
		ImageLoader.Instance.LoadAvatar(data.image,(tex)=>{
			Vector2 size = UITools.ResizeTo(tex,TargetIconSize);
			ActionIcon.width = (int)size.x;
			ActionIcon.height = (int)size.y;
			ActionIcon.mainTexture = tex;
		});
	}
}
