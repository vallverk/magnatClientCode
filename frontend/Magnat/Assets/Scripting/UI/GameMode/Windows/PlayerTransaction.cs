using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class PlayerTransaction
{
	public UILabel NameLabel;
	public UISprite MainSprite;
	public UISprite SliderSprite;
	public UITexture Avatar;
	public UILabel CashLabel;
	public UIGrid FieldsGrid;
	public GameObject CompanFieldPrefab;

	private string socialID;
	public string PlayerName;
	public GameField.Owners Owner;
	public Texture AvatarTexture;
	public int Cash;
	public GameField[] Fields;
	public string ID;
	
	private Dictionary<GameField,Transform> gridObjects = new Dictionary<GameField, Transform>();
	
	private GameDataManager GData;
	
	public void UpdateFields()
	{
		NameLabel.text = PlayerName;
		Avatar.mainTexture = AvatarTexture;
		CashLabel.text = Cash.ToString("Сумма активов: $### ### ##0");
		// обновим список полей, только в режиме игры
		if (Application.isPlaying)
		{
			if (GData == null) GData = GameObject.FindObjectOfType<GameDataManager>();
			
			List<GameField> dell = new List<GameField>();
			List<GameField> been = new List<GameField>(Fields);
			foreach (var o in gridObjects)
				if (!been.Contains(o.Key))
					dell.Add(o.Key);
			foreach (var f in dell)
			{
				Transform t = gridObjects[f];
				FieldsGrid.RemoveChild(t);
				gridObjects.Remove(f);
				GameObject.Destroy(t.gameObject);
			}
			foreach (GameField field in Fields)
			{
				if (!gridObjects.ContainsKey(field))
				{
					GameObject go = NGUITools.AddChild(FieldsGrid.gameObject,CompanFieldPrefab) as GameObject;
					FieldsGrid.AddChild(go.transform);
					TransactionField tf = go.GetComponent<TransactionField>();
					FieldData data = GData.GetFieldData(field);
					tf.FieldName = data.FieldName;
					tf.Cash = data.BuyPrice;
					gridObjects.Add(field,go.transform);
				}
			}
		}
	}

	public void LoadAvatar(string URL)
	{
		ImageLoader.Instance.LoadAvatar(URL,(tex)=>{
			AvatarTexture= tex;
			Avatar.mainTexture = AvatarTexture;
		});
	} 
	
	public void SetSpriteNames(string Main, string Slider)
	{
		SliderSprite.spriteName = Slider;
		MainSprite.spriteName = Main;
	}
}