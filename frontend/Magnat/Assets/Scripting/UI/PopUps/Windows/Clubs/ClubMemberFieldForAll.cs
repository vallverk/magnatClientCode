using UnityEngine;
using System.Collections;

public class ClubMemberFieldForAll : ClubMemberField 
{
	public UILabel CapitalLabel;

	public override void Init (ClubInfo Club, ServerUserInfo User, int Number)
	{
		base.Init (Club, User, Number);
		CapitalLabel.text = user.Capital.ToString("$###,###,###,##0k");
	}
}
