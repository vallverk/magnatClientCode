using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TransactionWindow : MonoBehaviour 
{
	public Texture2D DefaultAvatar;

	public PlayerTransaction Player1;
	public PlayerTransaction Player2;

	public GameObject PanelGO;
	private TweenAlpha panelTween;
	public GameObject ShadowGO;
	private TweenAlpha shadowTween;
	public UIButton AcceptButton;
	public UIButton CancelButton;

	public string[] MainSpriteNames;
	public string[] SliderSpriteNames;

	private GameDataManager GData;
	private int supplement;

    public int Supplement { get { return supplement; } }


	public UISlider Slider;
	public UILabel LeftPrice;
	public UILabel RightPrice;

	void Start()
	{
		panelTween = NGUITools.AddMissingComponent<TweenAlpha>(PanelGO);
		shadowTween = NGUITools.AddMissingComponent<TweenAlpha>(ShadowGO);
		PanelGO.GetComponent<UIPanel>().alpha = 0;
		ShadowGO.GetComponent<UITexture>().color = new Color(1,1,1,0);
		GData = GameObject.FindObjectOfType<GameDataManager>();
	}

	public TransactionInfo GetInfo()
	{
		TransactionInfo info = new TransactionInfo();
		info.Player1 = Player1.ID;
		info.Player2 = Player2.ID;
		info.Supplement = supplement;
		info.Status = 0;
		info.FieldInds = new List<int>();
		foreach (var field in Player1.Fields)
			info.FieldInds.Add(GData.GetFieldData(field).ID);
		foreach (var field in Player2.Fields)
			info.FieldInds.Add(GData.GetFieldData(field).ID);

		//FindObjectOfType<GameManager>().LogToMainChat(JSONSerializer.Serialize(info));
		return info;
	}

	public void ShowDoalog()
	{
		SoundManager.PlayTransactionWindow();
		panelTween.PlayForward();
		shadowTween.PlayForward();
	}

	public void HideDialog()
	{
		panelTween.PlayReverse();
		shadowTween.PlayReverse();
	}

	public void Init(Player P1, Player P2)
	{
		SetFielData(P1,Player1);
		SetFielData(P2,Player2);
		UpdateMinMaxSupplement();
		OnValidate();
	}

	public void SetSliderActive(bool Active)
	{
		Slider.gameObject.SetActive(Active);
	}

	public void UpdateMinMaxSupplement()
	{
		int p = Player1.Cash-Player2.Cash;
        float minSupplement = p * 0.7f;
        float maxSupplement = p * 1.3f;
        if (p == 0)
        {
            minSupplement = Player1.Cash * 0.7f;
            maxSupplement = Player1.Cash * 1.3f;
        }
		float sliderPos = Slider.value;
		supplement = (int)Mathf.Lerp(minSupplement,maxSupplement,sliderPos);
		if (supplement>0)
		{
			LeftPrice.text = "$ 0";
			RightPrice.text = supplement.ToString("$### ### ##0");
		} else
		{
			RightPrice.text = "$ 0";
			LeftPrice.text = (-supplement).ToString("$### ### ##0");
		}
	}

	public void SetSupplement(int sup)
	{
		supplement = sup;
		if (supplement>0)
		{
			LeftPrice.text = "$ 0";
			RightPrice.text = supplement.ToString("$### ### ##0");
		} else
		{
			RightPrice.text = "$ 0";
			LeftPrice.text = (-supplement).ToString("$### ### ##0");
		}
	}

	private void SetFielData(Player P, PlayerTransaction PT)
	{
		PT.PlayerName = P.FirstName;
		PT.Owner = P.OwnerID;
		PT.Cash = 0;
		PT.Fields = new GameField[0];
		PT.ID = P.SocialID;
		PT.Avatar.mainTexture = DefaultAvatar;
#if !UNITY_EDITOR
		PT.LoadAvatar(P.PhotoURL);
#endif
	}

	public void AddFieldToList(int List, GameField Field)
	{
		PlayerTransaction p = List == 0? Player1 : Player2;
		var list = new List<GameField>(p.Fields);
		list.Add(Field);
		p.Fields = list.ToArray();
		p.Cash += GData.GetFieldData(Field).BuyPrice;
		OnValidate();
	}

	public void RemoveFromList(int List, GameField Field)
	{
		PlayerTransaction p = List == 0? Player1 : Player2;
		var list = new List<GameField>(p.Fields);
		if (list.Contains(Field)) list.Remove(Field);
		p.Fields = list.ToArray();
		p.Cash -= GData.GetFieldData(Field).BuyPrice;
		OnValidate();
	}

	void OnValidate()
	{
		Player1.UpdateFields();
		if (Player1.Owner!= GameField.Owners.None)
			Player1.SetSpriteNames(MainSpriteNames[(int)Player1.Owner-1],SliderSpriteNames[(int)Player1.Owner-1]);
		Player2.UpdateFields();
		if (Player2.Owner!= GameField.Owners.None)
			Player2.SetSpriteNames(MainSpriteNames[(int)Player2.Owner-1],SliderSpriteNames[(int)Player2.Owner-1]);
		UpdateMinMaxSupplement();
	}
}
