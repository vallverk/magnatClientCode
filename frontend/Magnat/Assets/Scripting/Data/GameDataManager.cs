using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class GameDataManager : MonoBehaviour
{
    [Serializable]
	public struct FieldInAliasData
	{
		[SerializeField] public int FieldID;
		[SerializeField] public int AliasID;
	}

	[SerializeField] public FieldData[] Fields;
	[SerializeField] public AliasData[] Alias;
	[SerializeField] public FieldInAliasData[] FieldInAlias;

	[SerializeField] public GameField[] GamePoolSteps;

	// буферизаторы данных
	private List<GameField> cities;
	private List<GameField> companies;
	private List<GameField> noGameFields;
	private List<GameField> gameFields;
	private Dictionary<GameField, FieldData> gameFieldDataBuffer;
	private Dictionary<FieldData, GameField> dataGameFieldBuffer;
	private Dictionary<int, List<FieldData>> fieldInAlias;
	private Dictionary<GameField, AliasData> aliasInField;

	private bool buffersInited = false;

	void Start()
	{
		InitBuffers();

		foreach (var f in gameFields)
			f.Price = gameFieldDataBuffer[f].BuyPrice;
	}

	private void InitBuffers()
	{
		if (buffersInited) return;
		// инициализируем буфера
		cities = new List<GameField>();
		companies = new List<GameField>();
		gameFieldDataBuffer = new Dictionary<GameField, FieldData>();
		dataGameFieldBuffer = new Dictionary<FieldData, GameField>();
		noGameFields = new List<GameField>();
		gameFields = new List<GameField>();
		fieldInAlias = new Dictionary<int, List<FieldData>>();
		aliasInField = new Dictionary<GameField, AliasData>();
		foreach (var d in Fields)
		{
			GameField f = d.FieldGO.GetComponent<GameField>();
			gameFieldDataBuffer.Add(f,d);
			dataGameFieldBuffer.Add(d,f);
			gameFields.Add(f);
		}
		foreach (var f in FieldInAlias)
		{
			GameField field = dataGameFieldBuffer[Fields[f.FieldID-1]];
			if (f.AliasID == 1)
			{
				cities.Add(dataGameFieldBuffer[Fields[f.FieldID-1]]);
			}
			else
				companies.Add(dataGameFieldBuffer[Fields[f.FieldID-1]]);
			if (!fieldInAlias.ContainsKey(f.AliasID))
				fieldInAlias.Add(f.AliasID,new List<FieldData>());
			fieldInAlias[f.AliasID].Add(Fields[f.FieldID-1]);
			aliasInField.Add(field,Alias[f.AliasID]);
		}
		foreach (var f in GamePoolSteps)
		{
			if (!gameFields.Contains(f)) noGameFields.Add(f);
		}
		buffersInited = true;
	}

	public bool IsCity(AliasData alias)
	{
		return alias.ID == 2;
	}

	public bool IsCity(FieldData field)
	{
		if (!buffersInited) InitBuffers();
		return cities.Contains(dataGameFieldBuffer[field]);
	}

	public bool IsCity(GameField field)
	{
		if (!buffersInited) InitBuffers();
		return cities.Contains(field);
	}
	
	public AliasData GetAliasFromField(GameField Field)
	{
		return aliasInField[Field];
	}

	public FieldData GetFieldData(GameField Field)
	{
		if (!buffersInited) InitBuffers();
		return gameFieldDataBuffer[Field];
	}

	public GameField GetGameField(FieldData Field)
	{
		if (!buffersInited) InitBuffers();
		return dataGameFieldBuffer[Field];
	}

	public GameField[] GetNonGameField()
	{
		if (!buffersInited) InitBuffers();
		return noGameFields.ToArray();
	}

	public GameField[] GetNoPlayerFields(GameField.Owners OwnerID)
	{
		if (!buffersInited) InitBuffers();
		// а че у игрока за поля во владениях?
		List<GameField> res = new List<GameField>();
		foreach (var f in gameFields)
			if (f.Owner != OwnerID)
				res.Add(f);
		return res.ToArray();
	}

	public GameField[] GetPlayerFields(GameField.Owners OwnerID)
	{
		if (!buffersInited) InitBuffers();
		// а че у игрока за поля во владениях?
		List<GameField> res = new List<GameField>();
		foreach (var f in gameFields)
			if (f.Owner == OwnerID)
				res.Add(f);
		return res.ToArray();
	}

	public FieldData[] GetPlayerFieldsData(GameField.Owners OwnerID)
	{
		if (!buffersInited) InitBuffers();
		// а че у игрока за поля во владениях?
		List<FieldData> res = new List<FieldData>();
		foreach (var f in gameFields)
			if (f.Owner == OwnerID)
				res.Add(GetFieldData(f));
		return res.ToArray();
	}

	public AliasData[] GetPlayerAliases(GameField.Owners OwnerID)
	{
		if (!buffersInited) InitBuffers();
		// а че за монополии?
		List<AliasData> res = new List<AliasData>();
		// неведомая хрень, но, видимо, рабочая
		// P.S.: как тестирование показало - действительно рабочая...
		int[] all = new int[Alias.Length];
		int[] own = new int[Alias.Length];
		foreach (var fia in FieldInAlias)
		{
			all[fia.AliasID]++;
			GameField gf = Fields[fia.FieldID-1].FieldGO.GetComponent<GameField>();
			if (gf.Owner == OwnerID)
				own[fia.AliasID]++;
		}
		for (int i=0;i<all.Length;i++)
			if (all[i]==own[i])
				res.Add(Alias[i]);
		return res.ToArray();
	}

	public FieldData[] GetFieldDataInAlias(AliasData Alias)
	{
		if (!buffersInited) InitBuffers();
		return fieldInAlias[Alias.ID-1].ToArray();
	}

	public GameField[] GetFieldInAlias(AliasData Alias)
	{
		if (!buffersInited) InitBuffers();
		List<GameField> res = new List<GameField>();
		foreach (var f in GetFieldDataInAlias(Alias))
			res.Add(dataGameFieldBuffer[f]);
		return res.ToArray();
	}

	[ContextMenu("Test")]
	void Test()
	{
		var list = GetFieldInAlias(Alias[3]);
		string t = "";
		foreach (var item in list)
			t+=item.name+" ";
		print(t);
	}

	public string GetFieldName(int PoolStepID)
	{
		string text = "";
		switch (GamePoolSteps[PoolStepID].Effect)
		{
		case GameField.FieldEffects.Customs:
			text = "Таможня";
			break;
		case GameField.FieldEffects.GameEffect:
			text = "Неизвестное поле";
			for (int j=0;j<Fields.Length;j++)
				if (GamePoolSteps[PoolStepID] == Fields[j].FieldGO.GetComponent<GameField>())
			{
				text = Fields[j].FieldName;
				break;
			}
			break;
		case GameField.FieldEffects.Jackpot:
			text = "Джекпот";
			break;
		case GameField.FieldEffects.Lottery:
			text = "Лотерея";
			break;
		case GameField.FieldEffects.SkipStep:
			text = "Пропуск ход";
			break;
		case GameField.FieldEffects.Start:
			text = "Старт";
			break;
		case GameField.FieldEffects.StepBack:
			text = "Шаг назад";
			break;
		case GameField.FieldEffects.Tax:
			text = "Налог 20%";
			break;
		case GameField.FieldEffects.Vacation:
			text = "Отпуск";
			break;
		default:
			text = "---";
			break;
		}
		return text;
	}
}