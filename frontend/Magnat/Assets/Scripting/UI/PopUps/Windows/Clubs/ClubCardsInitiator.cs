using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class ClubCardsInitiator : MonoBehaviour 
{
	public UIGrid ActionsGrid;
	
	public GameObject BuyedCardPrefab;
	public GameObject FreeCardPrefab;
	
	private bool inited = true;
	private ClubInfo club;

	private static List<string> cardsIDs = null;

	void Start()
	{
		if (cardsIDs == null)
		{
			cardsIDs = new List<string>();
			ServerInfo.Instance.GetClubCardsIDList((ids)=>{cardsIDs = new List<string>(ids);});
		}
	}

	public void Init(ClubInfo ClubID)
	{
		club = ClubID;
		inited = false;
	}
	
	void OnEnable()
	{
		if (!inited)
		{
			UITools.RemoveChildrens(ActionsGrid);
			//Debug.LogError(ActionsGrid.GetChildList().Count);
			ServerInfo.Instance.GetClubCardList(club.ID,(cards)=>{
				foreach (var id in cardsIDs)
				{
					GameObject prefab;
					ClubCard card = cards.FirstOrDefault(p => p._id == id);
					if (card!=null)
					{
						prefab = BuyedCardPrefab;
						GameObject field = NGUITools.AddChild(ActionsGrid.gameObject,prefab);
						ActionsGrid.AddChild(field.transform);
						var afield = field.GetComponent<ClubCardField>();
						afield.Init(club,card);
					}
					else
					{
						prefab = FreeCardPrefab;
						if (prefab!=null)
						{
							GameObject field = NGUITools.AddChild(ActionsGrid.gameObject,prefab);
							ActionsGrid.AddChild(field.transform);
							var afield = field.GetComponent<ClubCardField>();
							ServerInfo.Instance.GetClubCard(id,(oldCard)=>{
								(afield as CLubCardFieldFree).ReBuy = cards.Any(targetcard=>targetcard.Effect == oldCard.Effect);
								afield.Init(club, oldCard);
							});
						}
					}
				}
				
				inited = true;
			});

		}
	}
}
