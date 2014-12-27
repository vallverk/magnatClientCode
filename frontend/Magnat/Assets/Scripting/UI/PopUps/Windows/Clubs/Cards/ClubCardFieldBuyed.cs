using UnityEngine;
using System.Collections;

public class ClubCardFieldBuyed : ClubCardField 
{
	public UILabel InfoLabel;

	public override void Init (ClubInfo Club, ClubCard Card)
	{
		base.Init (Club, Card);

		InfoLabel.text = string.Format("[fe5151]До:[-] {0}" +
		                               "\r\n[fe5151]Действует для:[-] [ffffff]{1}[-]",
		                               TimeTools.FormatUTSTime(long.Parse(card.term)),
		                               card.status == "0"?"Стандарт":"VIP");
	}
}
