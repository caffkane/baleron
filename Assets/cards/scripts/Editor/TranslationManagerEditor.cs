
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TranslationManager))]
public class TranslationManagerEditor : Editor {


	public override void OnInspectorGUI ()
	{

		base.OnInspectorGUI ();
		EditorGUILayout.HelpBox ("Generation of translation list is only possible ingame.",MessageType.Info);

		TranslationManager tm = (TranslationManager)target;
		if(GUILayout.Button("Generate Translation List"))
		{
			tm.generateTermList ();
		}

		//if savestring has content
		if( ! string.IsNullOrEmpty(tm.saveState)){
			EditorGUILayout.HelpBox (tm.saveState,MessageType.Info);
		}
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
	


