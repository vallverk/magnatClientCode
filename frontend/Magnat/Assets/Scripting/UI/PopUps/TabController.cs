using UnityEngine;
using System.Collections;

public class TabController : MonoBehaviour 
{
	[System.Serializable]
	public class Tab
	{
		[SerializeField] public GameObject TabGO;
		[SerializeField] public UIButton ActivationButton;
	}

	public Tab[] Tabs;


	public void SetActiveTabViaButton()
	{
		foreach (var t in Tabs)
			t.TabGO.SetActive(t.ActivationButton == UIButton.current);
	}

	public void SetActiveTab(int TabID)
	{
		if (TabID>=0 && TabID<Tabs.Length)
			SetActiveTab(Tabs[TabID]);
	}

	public void SetActiveTab(Tab tab)
	{
		foreach (var t in Tabs)
			t.TabGO.SetActive (t == tab);
	}

	public void Start()
	{
		foreach (var tab in Tabs)
			if (tab.ActivationButton != null)
				tab.ActivationButton.onClick.Add(new EventDelegate(SetActiveTabViaButton));
		SetActiveTab(Tabs[0]);
	}
}
