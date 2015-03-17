using System;
using UnityEngine;
using System.Linq;
using System.Collections;

public partial class ServerInfo : Singleton<ServerInfo> 
{
	public void DeleteUserFromClub(string UserToDeleteID, Action<bool> Callback)
	{
		Query q = new QueryDeleteUserFromClub(viewerID,auth,UserToDeleteID);
		Pool.SendPostRequestAsync(q,(res)=>{
			Callback( JSONSerializer.Deserialize<QueryDeleteUserFromClub.Request>(res.Args[0].ToString()).Status == 200 );
		});
	}

	public void ExitFromClub(Action<bool> Callback)
	{
		Query q = new QueryExitClub(viewerID,auth);
		Pool.SendPostRequestAsync(q,(res)=>{
			Callback(true);
		});
	}

	public void ChangeClubOwner(string NewOwnerID, Action<bool> Callback)
	{
		Query q = new QueryChangeClubOwner(viewerID,auth,NewOwnerID);
		Pool.SendPostRequestAsync(q,(res)=>{
			Callback( JSONSerializer.Deserialize<QueryChangeClubOwner.Request>(res.Args[0].ToString()).Status == 200 );
		});
	}

	public void DepositToClub(ServerUserInfo User, ClubInfo Club, int Deposit, Action Callback)
	{
		Query q = new QuerySetClubDeposit(viewerID,auth,Club,User,Deposit);
		Pool.SendPostRequestAsync(q,(res)=>{ Callback(); });
	}

	public void InviteToClub(string TargetID, Action Callback)
	{
		Query q = new QueryInviteToClub(viewerID,auth,TargetID);
		Pool.SendPostRequestAsync(q,(res)=>{
			Callback();
		});
	}

	private string[] topClubsBuffer = null;
	public void GetTop100Clubs(Action<string[]> Callback)
	{
		if (topClubsBuffer!=null)
		{
			Callback(topClubsBuffer);
			return;
		}

		Query q = new QueryGetTop100Clubs(viewerID,auth);
		Pool.SendPostRequestAsync(q,(res)=>{
			string[] ids = JSONSerializer.Deserialize<string[]>(res.Args[0].ToString());
			topClubsBuffer = ids;
			Callback(ids);
		});
	}

	public void RemoveClubEvents(string ClubID, Action Callback)
	{
		Query q = new QueryRemoveClubEvents(viewerID,auth,ClubID);
		Pool.SendPostRequestAsync(q,(a)=>{Callback();});
	}

	public void AddClubEvent(string ClubID, string Description, Action Callback)
	{
		Query q = new QueryAddClubEvent(viewerID,auth,ClubID,Description,(long)TimeTools.GetLinuxTimeStamp());
		Pool.SendPostRequestAsync(q,(a)=>{ Callback(); });
	}

	public void GetClubEvents(string ClubID, Action<ClubEvent[]> Callback)
	{
		Query q = new QueryGetClubEvents(viewerID,auth,ClubID);
		Pool.SendPostRequestAsync(q,(res)=>{
			ClubEvent[] evs = new ClubEvent[res.Args.Count];
			for (int i=0;i<evs.Length;i++)
				evs[i]=JSONSerializer.Deserialize<ClubEvent>(res.Args[i].ToString());
			Callback(evs);
		});
	}

	public void GetClub(string ClubID, Action<ClubInfo> Callback)
	{
		Query q = new QueryGetClub(viewerID,auth,ClubID);
		Pool.SendPostRequestAsync(q,(res)=>{
			GetClubStatuses((statuses) => {
				ClubInfo club = JSONSerializer.Deserialize<ClubInfo>(res.Args[0].ToString());
				club.ID = ClubID;
				club.LevelName = statuses[club.Lavel-1].Title;
                club.Icon = club.Icon.Replace("http:", "http:");
				Callback(club);
			});
		});
	}

	public void SetClubDescription(string ClubID, string Description, Action<bool> Callback)
	{
		Query q = new QuerySetDescription(viewerID,auth,ClubID,Description);
		Pool.SendPostRequestAsync(q,(res)=>{
			Callback(true);
		});
	}

	public void SetClubName(string ClubID, string ClubName, Action<bool> Callback)
	{
		Query q = new QuerySetClubName(viewerID,auth,ClubID,ClubName);
		Pool.SendPostRequestAsync(q,(res)=>{
			Callback(true);
		});
	}

	public void SetClubGold(string ClubID, int Gold, Action<bool> Callback)
	{
		Query q = new QuerySetGold(viewerID,auth,ClubID,Gold);
		Pool.SendPostRequestAsync(q,(res)=>{
			Callback(true);
		});
	}

	public void SetClubDateOfDeath(string ClubID, long DateOfDeath, Action<bool> Callback)
	{
		Query q = new QuerySetDateOfDeath(viewerID,auth,ClubID,DateOfDeath);
		Pool.SendPostRequestAsync(q,(res)=>{
			Callback(true);
		});
	}

	public void SetClubMinEnterPrice(string ClubID, int MinEnterPrice, Action<bool> Callback)
	{
		Query q = new QuerySetMinEnterPrice(viewerID,auth,ClubID,MinEnterPrice);
		Pool.SendPostRequestAsync(q,(res)=>{
			Callback(true);
		});
	}
	
	public void CreateClub(string ClubName,Action<bool> Callback)
	{
		Query q = new QueryCreateClub(viewerID,auth,ClubName);
		Pool.SendPostRequestAsync(q,(res)=>{
			Callback(JSONSerializer.Deserialize<QueryCreateClub.Request>(res.Args[0].ToString()).Status == 200);
		});
	}

	public void SetClubFileIcon(string ClubID, string ImageName, Action<bool> Callback)
	{
		Query q = new QuerySetClubImage(viewerID,auth,ClubID,ImageName);
		Pool.SendPostRequestAsync(q,(res)=>{
			Callback(true);
		});
	}
}
