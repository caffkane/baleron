using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(HighScoreNameLinker))]
public class HighScoreNameLinkerEditor : Editor {

	public override void OnInspectorGUI (){
		showScriptField ();

		HighScoreNameLinker hsnl = (HighScoreNameLinker)target;

        GUI.enabled = false;
		showSerializedElement ("hsnPairs");
        GUI.enabled = true;

		showSerializedElement ("highScoreSource");
		if (hsnl.highScoreSource == HighScoreNameLinker.hsnSelection.scoreCounter) {
			showSerializedElement ("sc");
		} else {
			showSerializedElement ("valueType");
		}

        showSerializedElement("highScoreSort");

        showSerializedElement("highScoreFields");

        //showSerializedElement ("countryNameText");
        //showSerializedElement ("highScoreText");

        //base.OnInspectorGUI ();
    }

	void showSerializedElement(string class1){
		SerializedProperty c1 = serializedObject.FindProperty (class1);
		EditorGUI.BeginChangeCheck();
		EditorGUILayout.PropertyField(c1, true);
		if(EditorGUI.EndChangeCheck())
			serializedObject.ApplyModifiedProperties();
	}

	void showScriptField(){
		//show the script field
		serializedObject.Update();
		SerializedProperty prop = serializedObject.FindProperty("m_Script");
		GUI.enabled = false;
		EditorGUILayout.PropertyField(prop, true, new GUILayoutOption[0]);
		GUI.enabled = true;
		serializedObject.ApplyModifiedProperties();

	}
}
