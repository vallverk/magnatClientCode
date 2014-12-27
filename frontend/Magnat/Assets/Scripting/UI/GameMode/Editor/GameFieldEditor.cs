using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(GameField), true)]
public class GameFieldEditor : Editor 
{
	public override void OnInspectorGUI ()
	{
		GUIStyle headers = new GUIStyle(GUI.skin.label);
		headers.alignment = TextAnchor.MiddleCenter;

		GUILayout.Space(3f);
		GUILayout.Label("--- Objects ---",headers);

		serializedObject.Update();
		NGUIEditorTools.DrawProperty("Field target", serializedObject, "targetSprite");

		GameField gf = target as GameField;

		if (gf.targetSprite!=null)
		{
			UISprite sprite = gf.targetSprite.GetComponent<UISprite>();
			if (sprite!=null)
			{
				SerializedObject obj = new SerializedObject(sprite);
				SerializedProperty atlas = obj.FindProperty("mAtlas");
				NGUIEditorTools.DrawSpriteField("Free field", serializedObject, atlas, serializedObject.FindProperty("normalSprite"), true);
				NGUIEditorTools.DrawSpriteField("Blue owner", serializedObject, atlas, serializedObject.FindProperty("blueSprite"), true);
				NGUIEditorTools.DrawSpriteField("Green owner", serializedObject, atlas, serializedObject.FindProperty("greenSprite"), true);
				NGUIEditorTools.DrawSpriteField("Orange owner", serializedObject, atlas, serializedObject.FindProperty("orangeSprite"), true);
				NGUIEditorTools.DrawSpriteField("Purple owner", serializedObject, atlas, serializedObject.FindProperty("purpleSprite"), true);
				NGUIEditorTools.DrawSpriteField("Red owner", serializedObject, atlas, serializedObject.FindProperty("redSprite"), true);
				obj.ApplyModifiedProperties();
			}
		}

		NGUIEditorTools.DrawProperty("Locker target", serializedObject, "lockerSptite");

		NGUIEditorTools.DrawProperty("Price target", serializedObject, "targetPriceField");

		NGUIEditorTools.DrawProperty("First chip position", serializedObject, "ChipFirstPosition");
		NGUIEditorTools.DrawProperty("Last chip position", serializedObject, "ChipLastPosition");
		serializedObject.ApplyModifiedProperties();

		GUILayout.Space(10f);
		GUILayout.Label("--- Settings ---",headers);

		if (gf.targetPriceField!=null)
			gf.Price = EditorGUILayout.IntField("Price",gf.Price);

		if (gf.targetSprite!=null)
		{
			UISprite sprite = gf.targetSprite.GetComponent<UISprite>();
			if (sprite!=null)
			{
				var o =new SerializedObject(sprite);
				NGUITools.SetDirty(o.targetObject);
				gf.Owner = (GameField.Owners)EditorGUILayout.EnumPopup("Owner",gf.Owner);
				o.ApplyModifiedProperties(); 
			}
		}

		gf.Effect = (GameField.FieldEffects)EditorGUILayout.EnumPopup("Effect",gf.Effect);
		gf.CurrentMonopolyRank = (MonopolyRank)EditorGUILayout.EnumPopup("Rank",gf.CurrentMonopolyRank);

		gf.Locked = EditorGUILayout.Toggle("Locked",gf.Locked);
	}
}
