using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public partial class GameManager : MonoBehaviour 
{
	private List<UserAction> userActions = null;

	/// <summary>
	/// Gets the discount of monopoly.
	/// </summary>
	/// <returns> 0-1 range. 0 - 100% discount </returns>
	/// <param name="MonopolyID">Monopoly ID</param>
	public float GetDiscountOfMonopoly(int MonopolyID)
	{
		if (userActions == null) return 1;
		UserAction a = userActions.FirstOrDefault(action => int.Parse(action.monopoly) == MonopolyID);
		if (a == null) return 1;
		return int.Parse(a.discount)*0.01f;
	}
}
