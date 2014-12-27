using UnityEngine;
using System.Collections;

public class UpdateGrid : MonoBehaviour 
{
	void OnEnabled()
	{
		GetComponent<UIGrid>().repositionNow = true;
	}
}
