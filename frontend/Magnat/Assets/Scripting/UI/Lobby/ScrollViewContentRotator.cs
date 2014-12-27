using UnityEngine;
using System.Collections;

[RequireComponent(typeof(UIScrollView))]
public class ScrollViewContentRotator : MonoBehaviour 
{
	UIScrollView viewer;

	void Awake()
	{
		viewer = GetComponent<UIScrollView>();
	}

	public void RotateLeft()
	{
		viewer.Scroll(-1);
	}

	public void RotateRight()
	{
		viewer.Scroll(1);
	}
}
