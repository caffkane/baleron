using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ValueScript))]
public class ValueScriptEditor : Editor {

}


[CustomPropertyDrawer(typeof(ValueScript.valueToIcon))]
public class ValueToIconDrawer : PropertyDrawer
{
	// Draw the property inside the given rect
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		// Using BeginProperty / EndProperty on the parent property means that
		// prefab override logic works on the entire property.
		EditorGUI.BeginProperty(position, label, property);

		// Draw label
		position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

		// Don't make child fields be indented
		var indent = EditorGUI.indentLevel;
		EditorGUI.indentLevel = 0;

		// Calculate rects
		var textRect = new Rect(position.x, position.y, position.width * 0.5f, position.height);
		var iconRect = new Rect(position.x + position.width * 0.52f  , position.y, position.width * 0.48f , position.height);

		// Draw fields - passs GUIContent.none to each so they are drawn without labels
		EditorGUI.PropertyField(textRect, property.FindPropertyRelative("minValue"), GUIContent.none);
		EditorGUI.PropertyField(iconRect, property.FindPropertyRelative("icon"), GUIContent.none);


		// Set indent back to what it was
		EditorGUI.indentLevel = indent;

		EditorGUI.EndProperty();
	}
}