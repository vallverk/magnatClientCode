using UnityEngine;
using System.Collections;
using System;

public class ClubEventLabel : MonoBehaviour 
{
	public UILabel TextLabel;
	public UILabel DateLabel;

	public void Init(string Text, string TimeStamp)
	{
		TextLabel.text = Text;
		DateLabel.text = TimeStamp;
	}
}
