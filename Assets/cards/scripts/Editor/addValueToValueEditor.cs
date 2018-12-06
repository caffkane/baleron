using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(addValueToValue))]
public class addValueToValueEditor : Editor {
	public override void OnInspectorGUI ()
	{
		base.OnInspectorGUI ();
	}
}

// Modifier Drawer
[CustomPropertyDrawer(typeof(addValueToValue.resultModifierForAddingValueToValue))]
public class ModifierValueToValueDrawer : PropertyDrawer
{
	// Draw the property inside the given rect
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		//don't alter
		EditorGUI.BeginProperty(position, label, property);
		position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
		var indent = EditorGUI.indentLevel;
		EditorGUI.indentLevel = 0;

		// Calculate rects
		var rectLArgument = new Rect(position.x, position.y, position.width * 0.40f, position.height);
		var rectAdd = new Rect(position.x + position.width * 0.41f  , position.y, position.width * 0.09f , position.height);
		var rectMult = new Rect(position.x + position.width * 0.51f  , position.y, position.width * 0.19f , position.height);
		var rectRArgument = new Rect(position.x + position.width * 0.71f  , position.y, position.width * 0.29f , position.height);

		// Draw fields - passs GUIContent.none to each so they are drawn without labels
		EditorGUI.PropertyField(rectLArgument, property.FindPropertyRelative("lArgument"), GUIContent.none);
		EditorGUI.LabelField (rectAdd, "+=");
		EditorGUI.PropertyField(rectMult, property.FindPropertyRelative("multiplier"), GUIContent.none);
		EditorGUI.PropertyField(rectRArgument, property.FindPropertyRelative("rArgument"), GUIContent.none);

		//don't alter
		EditorGUI.indentLevel = indent;
		EditorGUI.EndProperty();
	}
}
	
	