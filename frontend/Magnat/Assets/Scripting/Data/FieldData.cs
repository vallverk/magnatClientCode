using UnityEngine;

[System.Serializable] public class FieldData
{
	[SerializeField] public int ID;
	[SerializeField] public string FieldName;
	[SerializeField] public UnityEngine.GameObject FieldGO;
	[SerializeField] public int BuyPrice;
	[SerializeField] public int MislayPrice;
	[SerializeField] public int BuyOutPrice;
	[SerializeField] public int BuildPrice;
	[SerializeField] public int MBuildPrice;
	[SerializeField] public int BasePrice;
	[SerializeField] public int MonopolyCost;
	[SerializeField] public int Branch1Cost;
	[SerializeField] public int Branch2Cost;
	[SerializeField] public int Branch3Cost;
	[SerializeField] public int Branch4Cost;
	[SerializeField] public int HoldingCost;

	public int GetCostByRank(MonopolyRank Rank)
	{
		switch (Rank)
		{
		case MonopolyRank.Base: return BasePrice;
		case MonopolyRank.Branch1: return Branch1Cost;
		case MonopolyRank.Branch2: return Branch2Cost;
		case MonopolyRank.Branch3: return Branch3Cost;
		case MonopolyRank.Branch4: return Branch4Cost;
		case MonopolyRank.Holding: return HoldingCost;
		case MonopolyRank.Monopoly: return MonopolyCost;
		default: return BasePrice;
		}
	}
}
