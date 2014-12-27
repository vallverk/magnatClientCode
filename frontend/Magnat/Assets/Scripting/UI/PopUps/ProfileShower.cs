using UnityEngine;
using System.Collections;

public class ProfileShower : MonoBehaviour 
{
	private string uid;
	
	public void SetOnClickEvent(string UID)
	{
		UIButton but = GetComponent<UIButton>();
		but.onClick.Clear();
		but.onClick.Add(new EventDelegate(()=>{OnGoToProfile();}));
		uid = UID;
	}
	
	void OnGoToProfile()
	{
		UserProfileWindow.Show(uid);
	}
}
