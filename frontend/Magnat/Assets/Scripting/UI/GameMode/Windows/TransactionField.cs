using UnityEngine;
using System.Collections;

public class TransactionField : MonoBehaviour 
{
	public UILabel NameLabel;
	public UILabel CashLabel;

	private string fieldName;
	private int cash;

	public string FieldName
	{
		get { return fieldName; }
		set 
		{
			fieldName = value;
			NameLabel.text = value;
		}
	}

	public int Cash
	{
		get { return cash; }
		set 
		{
			cash = value;
			CashLabel.text = value.ToString("$### ### ##0");
		}
	}
}
