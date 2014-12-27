using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(UserPlane), true)]
public class UserPlaneEditor : Editor
{
	public override void OnInspectorGUI ()
	{
		serializedObject.Update();

		NGUIEditorTools.DrawProperty("Big BG target", serializedObject, "BackBigSprite");
		NGUIEditorTools.DrawProperty("Small BG target", serializedObject, "BackSmallSprite");
		NGUIEditorTools.DrawProperty("Name label target", serializedObject, "NameLabel");
		NGUIEditorTools.DrawProperty("Status label target", serializedObject, "StatusLabel");
		NGUIEditorTools.DrawProperty("Cash label target", serializedObject, "CashLabel");
		NGUIEditorTools.DrawProperty("Capital label target", serializedObject, "CapitalLabel");
		NGUIEditorTools.DrawProperty("Crown GO", serializedObject, "CrownGO");
		NGUIEditorTools.DrawProperty("Gift GO", serializedObject, "GiftGO");
		NGUIEditorTools.DrawProperty("Time label target", serializedObject, "TimeLabel");
		NGUIEditorTools.DrawProperty("Bankrout sprite target", serializedObject, "BankroutSprite");
		NGUIEditorTools.DrawProperty("Transaction button GO", serializedObject, "TransactionButtonGO");

		UserPlane up = target as UserPlane;

		if (up.BackBigSprite!=null && up.BackSmallSprite!=null)
		{
			UISprite bs = up.BackBigSprite.GetComponent<UISprite>();
			UISprite ss = up.BackSmallSprite.GetComponent<UISprite>();
			if (bs!=null && ss!=null)
			{
				SerializedObject obj = new SerializedObject(bs);
				SerializedProperty atlas = obj.FindProperty("mAtlas");
				NGUITools.SetDirty(obj.targetObject);
				NGUIEditorTools.DrawSpriteField("BigSprite", serializedObject, atlas, serializedObject.FindProperty("bigBackSprite"), true);
				bs.spriteName = up.bigBackSprite;
				obj.ApplyModifiedProperties();
				obj = new SerializedObject(ss);
				NGUITools.SetDirty(obj.targetObject);
				NGUIEditorTools.DrawSpriteField("SmallSprite", serializedObject, atlas, serializedObject.FindProperty("smallBackSprite"), true);
				ss.spriteName = up.smallBackSprite;
				obj.ApplyModifiedProperties();
			}
		}

		if (up.NameLabel!=null)
			up.Name = EditorGUILayout.TextField("Name",up.Name);

		if (up.StatusLabel!=null)
			up.Status = EditorGUILayout.TextField("Status",up.Status);

		if (up.CashLabel!=null)
			up.Cash = EditorGUILayout.IntField("Cash",up.Cash);

		if (up.Capital != null)
			up.Capital = EditorGUILayout.IntField("Capital",up.Capital);

		if (up.TimeLabel!=null)
			up.Time = EditorGUILayout.IntField("Time",up.Time);

		if (up.CrownGO!=null)
			up.EnableCrow = EditorGUILayout.Toggle("Enable Crown",up.EnableCrow);

		if (up.GiftGO!=null)
			up.EnableGift = EditorGUILayout.Toggle("Enable Gift",up.EnableGift);

		if (up.BankroutSprite!=null)
			up.Bankrout = EditorGUILayout.Toggle("Is bankrout",up.Bankrout);

		if (up.TransactionButtonGO!=null)
			up.EnableTransaction = EditorGUILayout.Toggle("Enable transaction button",up.EnableTransaction);

		up.Size = (UserPlane.PlaneSize)EditorGUILayout.EnumPopup("Size",up.Size);

		serializedObject.ApplyModifiedProperties();
	}
}
