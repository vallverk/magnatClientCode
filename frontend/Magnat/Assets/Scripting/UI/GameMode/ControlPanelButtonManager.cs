using UnityEngine;
using System.Collections;
using System;

public class ControlPanelButtonManager : MonoBehaviour 
{
	public GameObject ThrowDiceButton;
	public GameObject BuyButton;
	public GameObject AuctionButton;
	public GameObject PayButton;

	public GameObject LayButton;
	public GameObject ToBuyButton;
	public GameObject BuildButton;
	public GameObject SellButton;
	public GameObject CancelButton;

	public GameObject KeepCreditButton;
	public GameObject ReturnCreditButton;
	public GameObject EscapeButton;
	

	public bool EnableThrowDiceButton;
	public bool EnableBuybutton;
	public bool EnableAuctionButton;
	public bool EnablePayButton;
	public bool EnableLayButton;
	public bool EnableToBuyButton;
	public bool EnableBuildButton;
	public bool EnableSellButton;
	public bool EnableCancelButton;

	public bool EnableKeepCreditButton;
	public bool EnableReturnCreditButton;
	public bool EnableEscapeButton;


	public Action OnThrowDiceButton = () => { };
	public Action OnBuyButton = () => {};
	public Action OnAuctionButton = () => {};
	public Action OnPayButton = () => {};
	public Action OnLayButton = () => {};
	public Action OnToBuyButton = () => {};
	public Action OnBuildButton = () => {};
	public Action OnSellButton = () => {};
	public Action OnCancelButton = () => {};

	public Action OnKeepCreditButton = () => {};
	public Action OnReturnCreditButton = () => {};
	public Action OnEscapeButton = () => {};


	void Start()
	{
		if (ThrowDiceButton!=null)
			ThrowDiceButton.GetComponent<UIButton>().onClick.Add(new EventDelegate(() => { OnThrowDiceButton(); }));
		
		if (BuyButton!=null)
			BuyButton.GetComponent<UIButton>().onClick.Add(new EventDelegate(() => {OnBuyButton();}));
		
		if (AuctionButton!=null)
			AuctionButton.GetComponent<UIButton>().onClick.Add(new EventDelegate(()=>{OnAuctionButton();}));
		
		if (PayButton!=null)
			PayButton.GetComponent<UIButton>().onClick.Add(new EventDelegate(()=>{OnPayButton();}));
		
		if (LayButton!=null)
			LayButton.GetComponent<UIButton>().onClick.Add(new EventDelegate(()=>{OnLayButton();}));
		
		if (ToBuyButton!=null)
			ToBuyButton.GetComponent<UIButton>().onClick.Add(new EventDelegate(()=>{OnToBuyButton();}));
		
		if (BuildButton!=null)
			BuildButton.GetComponent<UIButton>().onClick.Add(new EventDelegate(()=>{OnBuildButton();}));
		
		if (SellButton!=null)
			SellButton.GetComponent<UIButton>().onClick.Add(new EventDelegate(()=>{OnSellButton();}));

		if (KeepCreditButton!=null)
			KeepCreditButton.GetComponent<UIButton>().onClick.Add(new EventDelegate(()=>{OnKeepCreditButton();}));

		if (ReturnCreditButton!=null)
			ReturnCreditButton.GetComponent<UIButton>().onClick.Add(new EventDelegate(()=>{OnReturnCreditButton();}));

		if (EscapeButton!=null)
			EscapeButton.GetComponent<UIButton>().onClick.Add(new EventDelegate(()=>{OnEscapeButton();}));

		if (CancelButton!=null)
			CancelButton.GetComponent<UIButton>().onClick.Add(new EventDelegate(()=>{OnCancelButton();}));
	}

	void OnValidate()
	{
		if (ThrowDiceButton!=null)
			UpdateInGrid(ThrowDiceButton,EnableThrowDiceButton);

		if (BuyButton!=null)
			UpdateInGrid(BuyButton,EnableBuybutton);

		if (AuctionButton!=null)
			UpdateInGrid(AuctionButton,EnableAuctionButton);

		if (PayButton!=null)
			UpdateInGrid(PayButton,EnablePayButton);

		if (LayButton!=null)
			UpdateInGrid(LayButton,EnableLayButton);

		if (ToBuyButton!=null)
			UpdateInGrid(ToBuyButton,EnableToBuyButton);

		if (BuildButton!=null)
			UpdateInGrid(BuildButton,EnableBuildButton);

		if (SellButton!=null)
			UpdateInGrid(SellButton,EnableSellButton);

		if (KeepCreditButton!=null)
			UpdateInGrid(KeepCreditButton,EnableKeepCreditButton);
		
		if (ReturnCreditButton!=null)
			UpdateInGrid(ReturnCreditButton,EnableReturnCreditButton);
		
		if (EscapeButton!=null)
			UpdateInGrid(EscapeButton,EnableEscapeButton);

		if (CancelButton!=null)
			UpdateInGrid(CancelButton,EnableCancelButton);
	}

	public void SetAllInactive()
	{
		EnableThrowDiceButton = false;
		EnableBuybutton = false;
		EnableAuctionButton = false;
		EnablePayButton = false;
		EnableLayButton = false;
		EnableToBuyButton = false;
		EnableBuildButton = false;
		EnableSellButton = false;
		EnableKeepCreditButton = false;
		EnableReturnCreditButton = false;
		//EnableEscapeButton = false;
		EnableCancelButton = false;
	}

	public bool CanChoise()
	{
		return EnableThrowDiceButton || EnableBuybutton || EnableAuctionButton || 
			EnablePayButton;
	}

	public void UpdateButtons()
	{
		OnValidate();
	}

	private void UpdateInGrid(GameObject GO, bool Enabled)
	{
		// добавлять/удалять нужно только активный ГО, в противном случае таблица не обновится
		if (Enabled)
			GO.SetActive(true);
		if (Enabled)
			AddToGrid(GO);
		else
			RemoveFromChild(GO);
		if (!Enabled)
			GO.SetActive(false);
	}

	private void RemoveFromChild(GameObject GO)
	{
		GO.transform.parent.GetComponent<UIGrid>().RemoveChild(GO.transform);
	}

	private void AddToGrid(GameObject GO)
	{
		GO.transform.parent.GetComponent<UIGrid>().AddChild(GO.transform,true);
	}
}
