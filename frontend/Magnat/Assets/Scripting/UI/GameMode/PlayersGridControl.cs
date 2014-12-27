using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayersGridControl : MonoBehaviour 
{
	public UserPlane BluePlane;
	public UserPlane RedPlane;
	public UserPlane GreenPlane;
	public UserPlane PurplePlane;
	public UserPlane OrangePlane;
	public GameObject VSPlane;

	public List<UserPlane> planes;

	public System.Action<GameField.Owners> OnTransactionButtonClick = (a) => {};

	public void UpdateUserData(GameField.Owners Player, int Cash, int Capital)
	{
		var plane = GetPlaneByOwner(Player);
		plane.Capital = Capital;
		plane.Cash = Cash;
	}

	public void SetActive(GameField.Owners Player)
	{
		foreach (var plane in planes)
			plane.Size = UserPlane.PlaneSize.Small;
		var target = GetPlaneByOwner(Player);
		target.Size = UserPlane.PlaneSize.Big;
		target.Time = -1;
	}

	public void SetBankrout(GameField.Owners Player, bool Value)
	{
		var target = GetPlaneByOwner(Player);
		target.Bankrout = Value;
		target.Size = UserPlane.PlaneSize.Small;
	}

	public void SetTime(GameField.Owners Player, int time)
	{
		GetPlaneByOwner(Player).Time = time;
	}

	public void Init(Player[] Ps)
	{
		planes = new List<UserPlane>();
		UIGrid g = GetComponent<UIGrid>();
		g.GetChildList().Clear();
		g.enabled = true;
		g.maxPerLine = Ps.Length;
		VSPlane.SetActive(false);
		for (int i=0;i<Ps.Length;i++)
		{
			UserPlane p = GetPlaneByOwner(Ps[i].OwnerID);
			p.Name = Ps[i].FirstName;
			p.Status = "";
			p.EnableCrow = false;
			p.EnableGift = false;
			p.Size = UserPlane.PlaneSize.Small;
			p.gameObject.name = i.ToString();
#if !UNITY_EDITOR
			p.LoadAvatar(Ps[i].PhotoURL);
#endif

			g.AddChild(p.transform,true);
			planes.Add(p);

			UpdateUserData(Ps[i].OwnerID,Ps[i].Cash,Ps[i].Capital);
		}
		LoadUsersData(Ps,planes.ToArray());

		// hide unused
		if (!planes.Contains(BluePlane)) BluePlane.gameObject.SetActive(false);
		if (!planes.Contains(RedPlane)) RedPlane.gameObject.SetActive(false);
		if (!planes.Contains(GreenPlane)) GreenPlane.gameObject.SetActive(false);
		if (!planes.Contains(PurplePlane)) PurplePlane.gameObject.SetActive(false);
		if (!planes.Contains(OrangePlane)) OrangePlane.gameObject.SetActive(false);
	}

	public void InitTwoVSTwo(Player[] Ps)
	{
		planes = new List<UserPlane>();
		UIGrid g = GetComponent<UIGrid>();
		VSPlane.SetActive(true);
		var list = g.GetChildList();
		list.Clear();
		//g.enabled = true;
		g.maxPerLine = 5;
		for (int i=0;i<Ps.Length;i++)
		{
			UserPlane p = GetPlaneByOwner(Ps[i].OwnerID);
			p.Name = Ps[i].FirstName;
			p.Status = "";
			p.EnableCrow = false;
			p.EnableGift = false;
			p.Size = UserPlane.PlaneSize.Small;
			p.gameObject.name = (i*10).ToString();
			#if !UNITY_EDITOR
			p.LoadAvatar(Ps[i].PhotoURL);
			#endif

			if (i==2)
			{
				VSPlane.name = "15";
				list.Add(VSPlane.transform);
			}
			list.Add(p.transform);
			planes.Add(p);
			
			UpdateUserData(Ps[i].OwnerID,Ps[i].Cash,Ps[i].Capital);
		}
		LoadUsersData(Ps,planes.ToArray());
		g.Reposition();
		
		// hide unused
		if (!planes.Contains(BluePlane)) BluePlane.gameObject.SetActive(false);
		if (!planes.Contains(RedPlane)) RedPlane.gameObject.SetActive(false);
		if (!planes.Contains(GreenPlane)) GreenPlane.gameObject.SetActive(false);
		if (!planes.Contains(PurplePlane)) PurplePlane.gameObject.SetActive(false);
		if (!planes.Contains(OrangePlane)) OrangePlane.gameObject.SetActive(false);
	}

	private void LoadUsersData(Player[] Players, UserPlane[] Planes)
	{
		string[] uids = new string[Players.Length];
		for (int i=0;i<Players.Length;i++)
			uids[i] = Players[i].SocialID;
		ServerInfo.Instance.GetUserInfo(uids,(users)=>{
			for (int i=0;i<users.Length;i++)
			{
				Planes[i].Status = users[i].Title;
				Planes[i].EnableCrow = users[i].VIP!=0;
				Planes[i].LoadGift(users[i].GUID);
			}
		});
	}

	void Start()
	{
		BluePlane.TransactionButtonGO.GetComponent<UIButton>().onClick.Add(new EventDelegate(()=>{OnTransactionButtonClick(GameField.Owners.Blue);}));
		RedPlane.TransactionButtonGO.GetComponent<UIButton>().onClick.Add(new EventDelegate(()=>{OnTransactionButtonClick(GameField.Owners.Red);}));
		GreenPlane.TransactionButtonGO.GetComponent<UIButton>().onClick.Add(new EventDelegate(()=>{OnTransactionButtonClick(GameField.Owners.Green);}));
		PurplePlane.TransactionButtonGO.GetComponent<UIButton>().onClick.Add(new EventDelegate(()=>{OnTransactionButtonClick(GameField.Owners.Purple);}));
		OrangePlane.TransactionButtonGO.GetComponent<UIButton>().onClick.Add(new EventDelegate(()=>{OnTransactionButtonClick(GameField.Owners.Orange);}));
	}

	public void SetTransactionsEnable(bool enable)
	{
		BluePlane.EnableTransaction = enable;
		RedPlane.EnableTransaction = enable;
		GreenPlane.EnableTransaction = enable;
		PurplePlane.EnableTransaction = enable;
		OrangePlane.EnableTransaction = enable;
	}

	public void SetTransactionsEnable(GameField.Owners[] players)
	{
		SetTransactionsEnable(false);
		foreach (var p in players)
			GetPlaneByOwner(p).EnableTransaction = true;
	}

	private UserPlane GetPlaneByOwner(GameField.Owners Owner)
	{
		switch (Owner)
		{
		case GameField.Owners.Blue: return BluePlane;
		case GameField.Owners.Green: return GreenPlane;
		case GameField.Owners.Orange: return OrangePlane;
		case GameField.Owners.Purple: return PurplePlane;
		case GameField.Owners.Red: return RedPlane;
		}
		return null;
	}
}
