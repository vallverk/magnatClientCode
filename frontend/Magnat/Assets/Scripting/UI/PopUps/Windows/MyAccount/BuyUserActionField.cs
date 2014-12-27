using UnityEngine;
using System.Collections;

public class BuyUserActionField : MonoBehaviour 
{
	public void ShowShop()
	{
		if (WindowBehavoiur.current!=null)
		{
			WindowBehavoiur.current.Hide();
			WindowBehavoiur.current = null;
		}
		GameObject.FindObjectOfType<ShopWindow>().Show();
	}
}
