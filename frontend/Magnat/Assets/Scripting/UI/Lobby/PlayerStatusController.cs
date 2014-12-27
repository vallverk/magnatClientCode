using UnityEngine;
using System.Collections;

public struct PlayerStatusPair
{
	public long MinCash;
	public string StatusName;
}

public class PlayerStatusController : MonoBehaviour 
{
	public UILabel StatusLabel;
	public UIProgressBar ProgressComponent;
	public UILabel CashLabel;

	public void Init(ServerUserInfo uinfo)
	{
	 	CashLabel.text = uinfo.Capital.ToString("$ ###,###,###,##0");
		StatusLabel.text = uinfo.Title;
		ProgressComponent.value = 0;
		ServerInfo.Instance.GetStatuses((titles)=>{
			ProgressComponent.value = UserTools.GetLevelProgressByCapital(uinfo.Capital,titles);
		});
	}
}
