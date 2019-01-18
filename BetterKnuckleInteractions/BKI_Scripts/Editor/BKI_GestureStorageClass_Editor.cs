using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace BetterKnucklesInteractions
{
	[CustomEditor(typeof(BKI_GestureStorageClass))]
	public class BKI_GestureStorageClass_Editor : Editor
	{
		private ReorderableList combiGestures;
		private ReorderableList lhGestures, rhGestures;

		private void OnEnable()
		{
			combiGestures = new ReorderableList(serializedObject, serializedObject.FindProperty("combiGesturesList"), true, true, false, true);
			combiGestures.drawHeaderCallback = (Rect rect) => {
				EditorGUI.LabelField(rect, "Combination gestures");
			};
			combiGestures.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
			{
				var element = combiGestures.serializedProperty.GetArrayElementAtIndex(index);
				rect.y += 2;
				EditorGUI.PropertyField(
					new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
					element.FindPropertyRelative("gesture"), GUIContent.none);
			};

			lhGestures = new ReorderableList(serializedObject, serializedObject.FindProperty("lhGesturesList"), true, true, false, true);
			lhGestures.drawHeaderCallback = (Rect rect) => {
				EditorGUI.LabelField(rect, "Left hand gestures");
			};
			lhGestures.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
			{
				var element = lhGestures.serializedProperty.GetArrayElementAtIndex(index);
				rect.y += 2;
				EditorGUI.PropertyField(
					new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
					element.FindPropertyRelative("gesture"), GUIContent.none);
			};

			rhGestures = new ReorderableList(serializedObject, serializedObject.FindProperty("rhGesturesList"), true, true, false, true);
			rhGestures.drawHeaderCallback = (Rect rect) => {
				EditorGUI.LabelField(rect, "Right hand gestures");
			};
			rhGestures.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
			{
				var element = rhGestures.serializedProperty.GetArrayElementAtIndex(index);
				rect.y += 2;
				EditorGUI.PropertyField(
					new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
					element.FindPropertyRelative("gesture"), GUIContent.none);
			};
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();
			combiGestures.DoLayoutList();
			lhGestures.DoLayoutList();
			rhGestures.DoLayoutList();
			serializedObject.ApplyModifiedProperties();
		}
	}
}