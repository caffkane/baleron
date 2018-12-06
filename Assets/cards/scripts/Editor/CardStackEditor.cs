
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CardStack))]
public class CardStackEditor : Editor {

	public bool verbose = false;

	public override void OnInspectorGUI ()
	{


		//begin our drawing

		verbose = EditorGUILayout.Toggle ("Verbose", verbose);
		if (verbose == true) {
			base.OnInspectorGUI ();
		} else {
			showScriptField ();

			showSerializedElement ("allCards");
			showSerializedElement ("fallBackCard");
			showSerializedElement ("swipe");
			showSerializedElement ("moveBackSpeed");
			showSerializedElement ("moveOutSpeed");
			showSerializedElement ("CardParent");
			showSerializedElement ("moveOutMax");
			showSerializedElement ("onCardSwipe");
		}
			
		CardStack cs = (CardStack)target;
		if(GUILayout.Button("Test duplicate/missing cards"))
		{
			cs.testForDuplicateCards();
		}
	}

	void showSerializedSubElement(string class1, string class2){
		SerializedProperty c1 = serializedObject.FindProperty (class1);
		SerializedProperty c2 = c1.FindPropertyRelative (class2);
		EditorGUI.BeginChangeCheck();
		EditorGUILayout.PropertyField(c2, true);
		if(EditorGUI.EndChangeCheck())
			serializedObject.ApplyModifiedProperties();
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
	


