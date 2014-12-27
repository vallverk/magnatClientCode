using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class PlayersManager : MonoBehaviour 
{
	public GameDataManager Manager;

	public Action OnInit = () => {};

	public Action OnPlayerThrowStart = () => {};
	private Action<PlayerInfo> OnPlayerAtStart = (a) => {};
	public Action OnPlayerEndStep = () => {};

	private enum PlayerState
	{
		Wait,
		Idle,
		Turn,
		Bankrut,
		Disabled
	}

	private class PlayerInfo
	{
		public PlayerState State;
		public Transform Chip;
		public int CurrentCardID;
		public GameField.Owners OwnerID;
	}

	public Transform BlueChip;
	public Transform GreenChip;
	public Transform OrangeChip;
	public Transform PurpleChip;
	public Transform RedChip;

	private PlayerInfo[] players = null;
	private int initedPlayers;

	public void ThrowDice(out int ValueA, out int ValueB)
	{
		// [1,7)
		int a = UnityEngine.Random.Range(1,7);
		// [1,7)
		int b = UnityEngine.Random.Range(1,7);

#if UNITY_EDITOR
        a = 1;
        b = 1;
#endif

		ValueA = a;
		ValueB = b;
	}

	public void SetInactive(GameField.Owners OwnerID)
	{
		// спрячем фишку
		Transform chip = GetChipByOwnerID(OwnerID);
		chip.GetComponent<TweenScale>().Play();
		chip.GetComponent<TweenAlpha>().Play();
	}

	public void SetPosition(int PlayerID, int Position)
	{
		if (players[PlayerID].CurrentCardID!=Position)
			StartCoroutine(PlayerGoToTarget(PlayerID,Position));
		players[PlayerID].CurrentCardID = Position;
	}

	public void MakeStep(int PlayerID, int Steps)
	{
		int next = players[PlayerID].CurrentCardID+Steps;
		if (next >= Manager.GamePoolSteps.Length)
			next -= Manager.GamePoolSteps.Length;
		StartCoroutine(PlayerGoTo(players[PlayerID],next,(int)Mathf.Sign(Steps)));
	}

	public int GetPosition(int PlayerID)
	{
		return players[PlayerID].CurrentCardID;
	}

	private PlayerInfo[] GetPlayersAtField(int FieldID)
	{
		List<PlayerInfo> res = new List<PlayerInfo>();
		for (int i =0;i<players.Length;i++)
			if (players[i].CurrentCardID == FieldID)
				res.Add(players[i]);
		return res.ToArray();
	}

	public void GoToVacation(int PlayerID)
	{
		StartCoroutine(PlayerGoToTarget(PlayerID,33));
	}

	public void GoToCustoms(int PlayerID)
	{
		StartCoroutine(PlayerGoToTarget(PlayerID,11));
	}

	private IEnumerator PlayerGoToTarget(int PlayerID, int TargetID)
	{
		PlayerInfo player = players[PlayerID];
		int prew = player.CurrentCardID;
		int next = TargetID;
		player.CurrentCardID = -1;
		yield return StartCoroutine(MakeStep(player.Chip,prew,next,0.3f));
		player.CurrentCardID= next;
	}

	private IEnumerator PlayerGoTo(PlayerInfo player,int nextCardID, int Way)
	{
		while (player.CurrentCardID!=nextCardID)
		{
			int next = player.CurrentCardID+Way;
			if (next >= Manager.GamePoolSteps.Length)
			{
				next = 0;
				OnPlayerAtStart(player);
				OnPlayerThrowStart();
			}
			int prew = player.CurrentCardID;
			player.CurrentCardID = -1;
			yield return StartCoroutine(MakeStep(player.Chip,prew,next,0.3f));
			player.CurrentCardID= next;
		}
		OnPlayerEndStep();
	}

	Vector3 GetPositionInField(int FieldID)
	{
		Vector3 f = Manager.GamePoolSteps[FieldID].ChipFirstPosition.position;
		Vector3 e = Manager.GamePoolSteps[FieldID].ChipLastPosition.position;
		return (f+e)/2;
	}

	Vector3[] GetPositionInField(int FieldID, int PlayersCount)
	{
		try
		{
			Vector3 f = Manager.GamePoolSteps[FieldID].ChipFirstPosition.position;
			Vector3 e = Manager.GamePoolSteps[FieldID].ChipLastPosition.position;
			Vector3[] res = new Vector3[PlayersCount];
			for (int i=0;i<PlayersCount;i++)
				res[i] = Vector3.Lerp(f,e,(i*1.0f)/(PlayersCount*1.0f)) + (e-f)/(PlayersCount+1.0f);
			return res;
		}
		catch
		{
			return new Vector3[0];
		}
	}

	IEnumerator MakeStep(Transform Chip,int FromID, int ToID, float MoveTime)
	{
		//
		if (ToID>=Manager.GamePoolSteps.Length)
			ToID -= Manager.GamePoolSteps.Length;
		if (FromID>=Manager.GamePoolSteps.Length)
			FromID -= Manager.GamePoolSteps.Length;

		// check old pos
		PlayerInfo[] playersAtPrewCard = this.GetPlayersAtField(FromID);
		if (playersAtPrewCard !=null && playersAtPrewCard.Length>0)
		{
			// return they to normal pos
			Vector3[] pos = GetPositionInField(FromID,playersAtPrewCard.Length);
			for (int i=0;i<playersAtPrewCard.Length;i++)
				try
			{
				if (playersAtPrewCard != null && pos != null)
					StartCoroutine(AnimMove(playersAtPrewCard[i].Chip,pos[i],MoveTime));
				else
					yield break;
			} catch 
			{
				Debug.LogError("Ошибка в итерации прыжка фишки. "+i);
			}
		}

		Vector3 spos = Chip.position;
		Vector3 epos = Vector3.zero;

		// chect new pos
		PlayerInfo[] playersAtNewCard = GetPlayersAtField(ToID);
		if (playersAtNewCard.Length>0)
		{
			// push thay to new pos
			Vector3[] pos = GetPositionInField(ToID,playersAtNewCard.Length+1);
			for (int i=0;i<playersAtNewCard.Length;i++)
				StartCoroutine(AnimMove(playersAtNewCard[i].Chip,pos[i],MoveTime));
			epos = pos[pos.Length-1];
		} 
		else 
			epos = GetPositionInField(ToID);

		float stime = Time.time;
		for (float delta = 0;delta<MoveTime;delta = Time.time-stime)
		{
			Chip.position = Vector3.Lerp(spos,epos,Mathf.Log((delta/MoveTime)*9+1));
			yield return new WaitForFixedUpdate();
		}
		Chip.position = epos;
	}

	IEnumerator AnimMove(Transform Target, Vector3 To, float MoveTime)
	{
		Vector3 startPos = Target.position;
		float stime = Time.time;
		for (float delta = 0;delta<MoveTime;delta = Time.time-stime)
		{
			Target.position = Vector3.Lerp(startPos,To,delta/MoveTime);
			yield return new WaitForFixedUpdate();
		}
	}

	private Transform GetChipByOwnerID(GameField.Owners Owner)
	{
		switch (Owner)
		{
		case GameField.Owners.Blue: return BlueChip;
		case GameField.Owners.Green: return GreenChip;
		case GameField.Owners.Orange: return OrangeChip;
		case GameField.Owners.Purple: return PurpleChip;
		case GameField.Owners.Red: return RedChip;
		default: return null;
		}
	}

	private PlayerInfo GetPlayerByOwnerID(GameField.Owners Owner)
	{
		foreach (var p in players)
			if (p.OwnerID == Owner) return p;
		return null;
	}

	public GameField.FieldEffects GetEffectAtPlayer(GameField.Owners Owner)
	{
		return Manager.GamePoolSteps[GetPlayerByOwnerID(Owner).CurrentCardID].Effect;
	}

	public GameField GetFieldAtPlayer(GameField.Owners Owner)
	{
		return Manager.GamePoolSteps[GetPlayerByOwnerID(Owner).CurrentCardID];
	}

	public int GetPlayerCurrentCardID(GameField.Owners Owner)
	{
		return GetPlayerByOwnerID(Owner).CurrentCardID;
	}

	public FieldData GetFieldDataAtPlayer(GameField.Owners Owner)
	{
		GameObject fieldGO = GetFieldAtPlayer(Owner).gameObject;
		foreach (var data in Manager.Fields)
		{
			if (data.FieldGO == fieldGO)
				return data;
		}
		return null;
	}

	public void Init(Player[] Ps)
	{
		if (players == null)
			players = new PlayerInfo[Ps.Length];

		List<Transform> cc = new List<Transform>();
		for (int i =0;i<Ps.Length;i++)
		{
			Transform c = GetChipByOwnerID(Ps[i].OwnerID);
			cc.Add(c);
			players[i] = new PlayerInfo()
			{
				Chip = c,
				State = PlayerState.Wait,
				OwnerID = Ps[i].OwnerID,
				CurrentCardID = 0
			};
		}

		if (!cc.Contains(BlueChip)) BlueChip.gameObject.SetActive(false);
		if (!cc.Contains(GreenChip)) GreenChip.gameObject.SetActive(false);
		if (!cc.Contains(OrangeChip)) OrangeChip.gameObject.SetActive(false);
		if (!cc.Contains(PurpleChip)) PurpleChip.gameObject.SetActive(false);
		if (!cc.Contains(RedChip)) RedChip.gameObject.SetActive(false);

		OnInit();
	}

}
