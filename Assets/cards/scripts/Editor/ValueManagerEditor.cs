using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(valueManager))]
public class valueMangerEditor : Editor {

	public override void OnInspectorGUI ()
	{
		base.OnInspectorGUI ();

		valueManager myScript = (valueManager)target;

		EditorGUILayout.HelpBox ("Test of duplicates and missing values are only possible ingame.",MessageType.Info);

		if(GUILayout.Button("Test duplicate/missing"))
		{
			myScript.testForDuplicatesAndMissingValues();
		}
	}
}
