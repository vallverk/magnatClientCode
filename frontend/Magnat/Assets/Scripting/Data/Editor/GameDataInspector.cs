using UnityEngine;
using System.Collections;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.IO;

public class GameDataInspector : EditorWindow
{
	public GameDataManager Manager;

	bool showFields;
	Vector2 scrollPosition;

	bool showAlias;
	Vector2 aliasScrollPosition;

	bool showRelations;
	Vector2 relationsPosition;

	bool showPoolWay;
	Vector2 poolWayPosition;

	[MenuItem("Window/Game Data Inspector")]
	public static void ShowWindow()
	{
		EditorWindow.GetWindow(typeof(GameDataInspector));
	}

	List<string> errors = new List<string>();

	void TryData()
	{
		if (GUILayout.Button("Проверить целостность данных"))
		{
			errors.Clear();
			
			for (int i =0;i<Manager.Fields.Length;i++)
			{
				if (Manager.Fields[i].FieldGO==null)
					errors.Add(string.Format("Нет привязки поля [{0}:{1}] к GO",Manager.Fields[i].ID,Manager.Fields[i].FieldName));
				else
				{
					GameField gf = Manager.Fields[i].FieldGO.GetComponent<GameField>();
					if (gf == null)
						errors.Add(string.Format("Неверно задан GO для [{0}:{1}] (не найден компонент GameField)",Manager.Fields[i].ID,Manager.Fields[i].FieldName));
					else
					{
						if (gf.targetSprite == null)
							errors.Add(string.Format("Не найден основной спрайт у [{0}:{1}]",Manager.Fields[i].ID,Manager.Fields[i].FieldName));
						if (gf.targetPriceField == null)
							errors.Add(string.Format("Не найден label у [{0}:{1}]",Manager.Fields[i].ID,Manager.Fields[i].FieldName));
						if (gf.lockerSptite == null)
							errors.Add(string.Format("Не найден locker у [{0}:{1}]",Manager.Fields[i].ID,Manager.Fields[i].FieldName));
						if (gf.ChipFirstPosition == null)
							errors.Add(string.Format("Не найден chip first position у [{0}:{1}]",Manager.Fields[i].ID,Manager.Fields[i].FieldName));
						if (gf.ChipLastPosition == null)
							errors.Add(string.Format("Не найден last chip position у [{0}:{1}]",Manager.Fields[i].ID,Manager.Fields[i].FieldName));

						if (string.IsNullOrEmpty(gf.normalSprite))
							errors.Add(string.Format("Не задан стандартный спрайт для [{0}:{1}]",Manager.Fields[i].ID,Manager.Fields[i].FieldName));
						if (string.IsNullOrEmpty(gf.blueSprite))
							errors.Add(string.Format("Не задан синий спрайт для [{0}:{1}]",Manager.Fields[i].ID,Manager.Fields[i].FieldName));
						if (string.IsNullOrEmpty(gf.greenSprite))
							errors.Add(string.Format("Не задан зеленый спрайт для [{0}:{1}]",Manager.Fields[i].ID,Manager.Fields[i].FieldName));
						if (string.IsNullOrEmpty(gf.orangeSprite))
							errors.Add(string.Format("Не задан оранжевый спрайт для [{0}:{1}]",Manager.Fields[i].ID,Manager.Fields[i].FieldName));
						if (string.IsNullOrEmpty(gf.redSprite))
							errors.Add(string.Format("Не задан красный спрайт для [{0}:{1}]",Manager.Fields[i].ID,Manager.Fields[i].FieldName));
						if (string.IsNullOrEmpty(gf.purpleSprite))
							errors.Add(string.Format("Не задан фиолетовый спрайт для [{0}:{1}]",Manager.Fields[i].ID,Manager.Fields[i].FieldName));
					}
				}
			}
		}
		
		// show errors
		if (errors != null && errors.Count>0)
			for (int i =0;i<errors.Count;i++)
				GUILayout.Label(errors[i]);
	}

	void SetDefaults()
	{
		if (GUILayout.Button("Выставить базовые настройки"))
		{
			for (int i=0;i<Manager.Fields.Length;i++)
			{
				FieldData fd = Manager.Fields[i];
				GameField gf = fd.FieldGO.GetComponent<GameField>();
				gf.Price = fd.BuyPrice;
				gf.Effect = GameField.FieldEffects.GameEffect;
				gf.Locked = false;
				gf.Owner = GameField.Owners.None;
			}
		}
	}

    void SerializeManager()
    {
        if (GUILayout.Button("Сохранить менеджер"))
        {
            File.WriteAllText(Application.dataPath + "/DB/GameData.json", JSONSerializer.Serialize(Manager));
            AssetDatabase.Refresh();
        }
    }
	
	void OnGUI()
	{
		Manager = EditorGUILayout.ObjectField("Game data manager",Manager,typeof(GameDataManager),true) as GameDataManager;
		if (Manager==null) return;

		TryData();
		SetDefaults();
        //SerializeManager();

		SerializedObject obj = new SerializedObject(Manager);
		obj.Update();

		showFields = EditorGUILayout.ToggleLeft("Показать данные о картах", showFields);

		if (showFields)
		{
			scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

			// header
			EditorGUILayout.BeginHorizontal();
			GUILayout.Label("ID",GUILayout.Width(20));
			GUILayout.Label("Название",GUILayout.Width(100));
			GUILayout.Label("GO",GUILayout.Width(70));
			GUILayout.Label("Покупка",GUILayout.Width(70));
			GUILayout.Label("Залог",GUILayout.Width(50));
			GUILayout.Label("Выкуп",GUILayout.Width(50));
			GUILayout.Label("Постройка",GUILayout.Width(70));
			GUILayout.Label("MПостройка",GUILayout.Width(70));
			GUILayout.Label("Базовая цена",GUILayout.Width(70));
			GUILayout.Label("Монополия",GUILayout.Width(70));
			GUILayout.Label("1 филиал",GUILayout.Width(60));
			GUILayout.Label("2 филиала",GUILayout.Width(60));
			GUILayout.Label("3 филиала",GUILayout.Width(60));
			GUILayout.Label("4 филиала",GUILayout.Width(60));
			GUILayout.Label("Холдинг",GUILayout.Width(60));
			EditorGUILayout.EndHorizontal();
			
			// body
			for (int i =0;i<Manager.Fields.Length;i++)
			{
				EditorGUILayout.BeginHorizontal();
				GUILayout.Label(Manager.Fields[i].ID.ToString(),GUILayout.Width(20));
				Manager.Fields[i].FieldName = EditorGUILayout.TextField(Manager.Fields[i].FieldName,GUILayout.Width(100));
				Manager.Fields[i].FieldGO = EditorGUILayout.ObjectField(Manager.Fields[i].FieldGO,typeof(GameObject),true,GUILayout.Width(70)) as GameObject;
				Manager.Fields[i].BuyPrice = EditorGUILayout.IntField(Manager.Fields[i].BuyPrice,GUILayout.Width(70));
				Manager.Fields[i].MislayPrice = EditorGUILayout.IntField(Manager.Fields[i].MislayPrice,GUILayout.Width(50));
				Manager.Fields[i].BuyOutPrice = EditorGUILayout.IntField(Manager.Fields[i].BuyOutPrice,GUILayout.Width(50));
				Manager.Fields[i].BuildPrice = EditorGUILayout.IntField(Manager.Fields[i].BuildPrice,GUILayout.Width(70));
				Manager.Fields[i].MBuildPrice = EditorGUILayout.IntField(Manager.Fields[i].MBuildPrice,GUILayout.Width(70));
				Manager.Fields[i].BasePrice = EditorGUILayout.IntField(Manager.Fields[i].BasePrice,GUILayout.Width(70));
				Manager.Fields[i].MonopolyCost = EditorGUILayout.IntField(Manager.Fields[i].MonopolyCost,GUILayout.Width(70));
				Manager.Fields[i].Branch1Cost = EditorGUILayout.IntField(Manager.Fields[i].Branch1Cost,GUILayout.Width(70));
				Manager.Fields[i].Branch2Cost = EditorGUILayout.IntField(Manager.Fields[i].Branch2Cost,GUILayout.Width(60));
				Manager.Fields[i].Branch3Cost = EditorGUILayout.IntField(Manager.Fields[i].Branch3Cost,GUILayout.Width(60));
				Manager.Fields[i].Branch4Cost = EditorGUILayout.IntField(Manager.Fields[i].Branch4Cost,GUILayout.Width(60));
				Manager.Fields[i].HoldingCost = EditorGUILayout.IntField(Manager.Fields[i].HoldingCost,GUILayout.Width(60));
				EditorGUILayout.EndHorizontal();
			}

			EditorGUILayout.BeginHorizontal();

			if (GUILayout.Button("Добавить игровое поле"))
				Array.Resize(ref Manager.Fields,Manager.Fields.Length+1);

			if (GUILayout.Button("Обновить идентификаторы"))
				if (EditorUtility.DisplayDialog("Обновить идентификаторы","Уверен, что хочешь это сделать?!","+","-"))
					for (int i=0;i<Manager.Fields.Length;i++)
						Manager.Fields[i].ID = i+1;

			EditorGUILayout.EndHorizontal();

			EditorGUILayout.EndScrollView();
		}

		GUILayout.Space(10f);

		showAlias = EditorGUILayout.ToggleLeft("Показать данные о монополиях", showAlias);
		
		if (showAlias)
		{
			aliasScrollPosition = EditorGUILayout.BeginScrollView(aliasScrollPosition);

			// header
			EditorGUILayout.BeginHorizontal();
			GUILayout.Label("ID",GUILayout.Width(20));
			GUILayout.Label("Название",GUILayout.Width(150));
			EditorGUILayout.EndHorizontal();

			// body
			for (int i =0;i<Manager.Alias.Length;i++)
			{
				EditorGUILayout.BeginHorizontal();
				GUILayout.Label(Manager.Alias[i].ID.ToString(),GUILayout.Width(20));
				Manager.Alias[i].AliasName = EditorGUILayout.TextField(Manager.Alias[i].AliasName,GUILayout.Width(150));
				EditorGUILayout.EndHorizontal();
			}

			EditorGUILayout.BeginHorizontal();

			if (GUILayout.Button("Добавить монополию"))
				Array.Resize(ref Manager.Alias,Manager.Alias.Length+1);

			if (GUILayout.Button("Обновить идентификаторы"))
				if (EditorUtility.DisplayDialog("Обновить идентификаторы","Уверен, что хочешь это сделать?!","+","-"))
					for (int i=0;i<Manager.Alias.Length;i++)
						Manager.Alias[i].ID = i+1;

			EditorGUILayout.EndHorizontal();

			EditorGUILayout.EndScrollView();
		}

		GUILayout.Space(10f);

		showRelations = EditorGUILayout.ToggleLeft("Показать данные о полях в монополиях", showRelations);
		
		if (showRelations)
		{
			relationsPosition = EditorGUILayout.BeginScrollView(relationsPosition);
			
			// header
			EditorGUILayout.BeginHorizontal();
			GUILayout.Label("ID поля",GUILayout.Width(20));
			GUILayout.Label("Название поля",GUILayout.Width(150));
			GUILayout.Label("Монополия",GUILayout.Width(150));
			EditorGUILayout.EndHorizontal();

			System.Collections.Generic.List<string> a = new System.Collections.Generic.List<string>();
			for (int i=0;i<Manager.Alias.Length;i++)
				a.Add(Manager.Alias[i].AliasName);

			// body
			if (Manager.FieldInAlias!=null)
			{
					for (int i =0;i<Manager.FieldInAlias.Length;i++)
					{
						EditorGUILayout.BeginHorizontal();
						GUILayout.Label(Manager.FieldInAlias[i].FieldID.ToString(),GUILayout.Width(20));
						GUILayout.Label(Manager.Fields[Manager.FieldInAlias[i].FieldID-1].FieldName,GUILayout.Width(150));
						Manager.FieldInAlias[i].AliasID = EditorGUILayout.Popup(Manager.FieldInAlias[i].AliasID,a.ToArray(),GUILayout.Width(150));
						EditorGUILayout.EndHorizontal();
					}
			}

			EditorGUILayout.BeginHorizontal();

			if (GUILayout.Button("Сбросить данные о полях"))
				if (EditorUtility.DisplayDialog("Сбросить данные о полях в монополиях","Уверен, что хочешь это сделать?!","+","-"))
			{
				Manager.FieldInAlias = new GameDataManager.FieldInAliasData[Manager.Fields.Length];
				for (int i=0;i<Manager.FieldInAlias.Length;i++)
				{
					Manager.FieldInAlias[i].FieldID = i+1;
					Manager.FieldInAlias[i].AliasID = 1;
				}
			}
			
			EditorGUILayout.EndHorizontal();
			
			EditorGUILayout.EndScrollView();
		}

		GUILayout.Space(10f);
		
		showPoolWay = EditorGUILayout.ToggleLeft("Показать данные о очередности клеток на поле", showPoolWay);
		
		if (showPoolWay)
		{
			poolWayPosition = EditorGUILayout.BeginScrollView(poolWayPosition);
			
			// header
			EditorGUILayout.BeginHorizontal();
			GUILayout.Label("№ поля",GUILayout.Width(20));
			GUILayout.Label("Название поля",GUILayout.Width(150));
			//GUILayout.Label("Монополия",GUILayout.Width(150));
			EditorGUILayout.EndHorizontal();
			
			// body
			if (Manager.GamePoolSteps!=null)
			{
				for (int i =0;i<Manager.GamePoolSteps.Length;i++)
				{
					EditorGUILayout.BeginHorizontal();
					GUILayout.Label((i+1).ToString(),GUILayout.Width(20)); 
					Manager.GamePoolSteps[i] = EditorGUILayout.ObjectField(Manager.GamePoolSteps[i],typeof(GameField),true,GUILayout.Width(150)) as GameField;

					string text = "null";
					if (Manager.GamePoolSteps[i]!=null)
					switch (Manager.GamePoolSteps[i].Effect)
					{
					case GameField.FieldEffects.Customs:
						text = "Таможня";
						break;
					case GameField.FieldEffects.GameEffect:
						text = "Неизвестное поле";
						for (int j=0;j<Manager.Fields.Length;j++)
							if (Manager.GamePoolSteps[i] == Manager.Fields[j].FieldGO.GetComponent<GameField>())
						{
							text = Manager.Fields[j].FieldName;
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
					GUILayout.Label(text,GUILayout.Width(150));

					EditorGUILayout.EndHorizontal();
				}
			}
			
			EditorGUILayout.BeginHorizontal();
			
			if (GUILayout.Button("Сбросить данные о полях"))
				if (EditorUtility.DisplayDialog("Сбросить данные о полях в монополиях","Уверен, что хочешь это сделать?!","+","-"))
			{
				Manager.GamePoolSteps = new GameField[44];
			}
			
			EditorGUILayout.EndHorizontal();
			
			EditorGUILayout.EndScrollView();
		}

		obj.ApplyModifiedProperties();
	}
}










































