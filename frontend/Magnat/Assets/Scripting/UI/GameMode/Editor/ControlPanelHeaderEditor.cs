using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(ControlPanelHeaderManager), true)]
public class ControlPanelHeaderEditor : Editor
{
	public override void OnInspectorGUI ()
	{
		serializedObject.Update();

		NGUIEditorTools.DrawProperty("Game ID label", serializedObject, "GameIDLabel");
		NGUIEditorTools.DrawProperty("Game time label", serializedObject, "GameTimeLabel");
		NGUIEditorTools.DrawProperty("Bet label", serializedObject, "BetLabel");
		NGUIEditorTools.DrawProperty("Game bank label", serializedObject, "GameBankLabel");

		ControlPanelHeaderManager t = target as ControlPanelHeaderManager;

		if (t.GameIDLabel!=null)
		{
			string val = EditorGUILayout.TextField("Game ID",t.GameID.ToString());
			long l;
			if (long.TryParse(val,out l))
				t.GameID = l;
		}

		if (t.GameTimeLabel!=null)
			t.GameTime = EditorGUILayout.IntField("Game time",t.GameTime);

		if (t.Bet!=null)
			t.Bet = EditorGUILayout.IntField("Bet",t.Bet);

		if (t.Bank!=null)
			t.Bank = EditorGUILayout.IntField("Bank",t.Bank);

		serializedObject.ApplyModifiedProperties();
	}
}
