using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System;

namespace BetterKnucklesInteractions
{
	// This partial is responsible for the base EditorWindow functionality.
	public partial class BKI_GestureCreator : EditorWindow
	{
		[MenuItem("Window/Better Knuckles Interactions Gesture Creator")]
		static public void OpenAllWindow()
		{
			BKI_GestureCreator window = (BKI_GestureCreator)GetWindow(typeof(BKI_GestureCreator));
			window.minSize = new Vector2(WINDOW_MIN_SIZE_X, WINDOW_MIN_SIZE_Y);
			window.maxSize = new Vector2(WINDOW_MIN_SIZE_X, WINDOW_MAX_SIZE_Y);
			window.Show();
		}

		// Start functionality
		private void OnEnable()
		{
			InitialiseHandImages();
			ResetTool();
			InitComboGesture();

			titleStyle = new GUIStyle();
			titleStyle.fontSize = 24;
			titleStyle.alignment = TextAnchor.UpperCenter;

			subtitleStyle = new GUIStyle();
			subtitleStyle.fontSize = 16;
			subtitleStyle.alignment = TextAnchor.UpperCenter;
			subsubtitleStyle = new GUIStyle();

			subsubtitleStyle.fontSize = 12;
			subsubtitleStyle.alignment = TextAnchor.UpperLeft;
			subsubtitleStyle.fontStyle = FontStyle.Italic;

			backgroundRect = new Rect(0, 0, WINDOW_MAX_SIZE_X + DEFAULT_AREA_PADDING, WINDOW_MAX_SIZE_Y + DEFAULT_AREA_PADDING);

			handGUIContainer = new Rect(4, TITLE_FIELD_SIZE, WINDOW_MAX_SIZE_X / 2 - 4, WINDOW_MAX_SIZE_Y - TITLE_FIELD_SIZE * 2);

			titleContainer = new Rect(DEFAULT_AREA_PADDING, DEFAULT_AREA_PADDING, WINDOW_MAX_SIZE_X - 2 * DEFAULT_AREA_PADDING, TITLE_FIELD_SIZE - DEFAULT_AREA_PADDING);

			footerContainer = new Rect(DEFAULT_AREA_PADDING, WINDOW_MAX_SIZE_Y - FOOTER_FIELD_SIZE - DEFAULT_AREA_PADDING, WINDOW_MAX_SIZE_X - 2 * DEFAULT_AREA_PADDING, FOOTER_FIELD_SIZE + DEFAULT_AREA_PADDING - 4f);

			footerContainerSmall = new Rect(DEFAULT_AREA_PADDING, WINDOW_MAX_SIZE_Y - FOOTER_FIELD_SIZE - DEFAULT_AREA_PADDING, WINDOW_MAX_SIZE_X - 2 * DEFAULT_AREA_PADDING, FOOTER_FIELD_SIZE - DEFAULT_AREA_PADDING);

			handRect = new Rect(4, DEFAULT_AREA_PADDING, handGUIContainer.width - DEFAULT_AREA_PADDING * 1.5f, handGUIContainer.height - DEFAULT_AREA_PADDING * 1.5f - TITLE_FIELD_SIZE - 3);

			float w = (WINDOW_MAX_SIZE_X * 0.5f);
			float x = WINDOW_MAX_SIZE_Y - (TITLE_FIELD_SIZE * 4) - DEFAULT_AREA_PADDING;
			buttonStackRect = new Rect(new Vector2(DEFAULT_AREA_PADDING, x), new Vector2(w, TITLE_FIELD_SIZE * 2f));

			Color c1 = new Color(0.85f, 0.85f, 0.85f);
			Color c2 = new Color(0.65f, 0.65f, 0.65f);

			backgroundTex2D = new Texture2D(1, 1);
			backgroundTex2D.SetPixel(0, 0, c2);
			backgroundTex2D.Apply();

			containerHandTex2D = new Texture2D(1, 1);
			containerHandTex2D.SetPixel(0, 0, c1);
			containerHandTex2D.Apply();

			titleBackgroundTex2D = new Texture2D(1, 1);
			titleBackgroundTex2D.SetPixel(0, 0, c1);
			titleBackgroundTex2D.Apply();

			footerBackgroundTex2D = new Texture2D(1, 1);
			footerBackgroundTex2D.SetPixel(0, 0, c1);
			footerBackgroundTex2D.Apply();

			gestureStorage = Resources.Load<BKI_GestureStorageClass>(RESOURCES_GESTURESTORAGE_PATH) as BKI_GestureStorageClass;
			Debug.Log("isStorageFound = " + (gestureStorage != null));
		}

		private void InitComboGesture()
		{
			lhGesture = ScriptableObject.CreateInstance<BKI_SingleGestureClass>();
			rhGesture = ScriptableObject.CreateInstance<BKI_SingleGestureClass>();

			lhMirror = new BKI_GestureMirrorClass();
			rhMirror = new BKI_GestureMirrorClass();

			combiGesture = ScriptableObject.CreateInstance<BKI_CombiGestureClass>();
			combiGesture.SetValues(DEFAULT_GESTURE_NAME, lhGesture, rhGesture);
		}

		// Sets up the UI for displaying contracted fingers.
		private void InitialiseHandImages()
		{
			Vector2 pos = new Vector2(HAND_POS_X, HAND_POS_Y);
			Vector2 size = new Vector2(HAND_SIZE_W, HAND_SIZE_H);
			baseHandRect = new Rect(pos, size);

			baseHandText2D = new Texture2D(HAND_SIZE_W, HAND_SIZE_H);
			thumbHandText2D = new Texture2D(HAND_SIZE_W, HAND_SIZE_H);
			indexHandText2D = new Texture2D(HAND_SIZE_W, HAND_SIZE_H);
			middleHandText2D = new Texture2D(HAND_SIZE_W, HAND_SIZE_H);
			ringHandText2D = new Texture2D(HAND_SIZE_W, HAND_SIZE_H);
			pinkyHandText2D = new Texture2D(HAND_SIZE_W, HAND_SIZE_H);

			thumbHandCrossText2D = new Texture2D(HAND_SIZE_W, HAND_SIZE_H);
			indexHandCrossText2D = new Texture2D(HAND_SIZE_W, HAND_SIZE_H);
			middleHandCrossText2D = new Texture2D(HAND_SIZE_W, HAND_SIZE_H);
			ringHandCrossText2D = new Texture2D(HAND_SIZE_W, HAND_SIZE_H);
			pinkyHandCrossText2D = new Texture2D(HAND_SIZE_W, HAND_SIZE_H);

			string imagePath = "Assets/BetterKnuckleInteractions/BKI_Sprites";

			baseHandText2D = AssetDatabase.LoadAssetAtPath<Texture2D>(imagePath + "/baseHand.png");
			indexHandText2D = AssetDatabase.LoadAssetAtPath<Texture2D>(imagePath + "/indexHand.png");
			thumbHandText2D = AssetDatabase.LoadAssetAtPath<Texture2D>(imagePath + "/thumbHand.png");
			middleHandText2D = AssetDatabase.LoadAssetAtPath<Texture2D>(imagePath + "/middleHand.png");
			ringHandText2D = AssetDatabase.LoadAssetAtPath<Texture2D>(imagePath + "/ringHand.png");
			pinkyHandText2D = AssetDatabase.LoadAssetAtPath<Texture2D>(imagePath + "/pinkyHand.png");

			thumbHandCrossText2D = AssetDatabase.LoadAssetAtPath<Texture2D>(imagePath + "/thumbHandCross.png");
			indexHandCrossText2D = AssetDatabase.LoadAssetAtPath<Texture2D>(imagePath + "/indexHandCross.png");
			middleHandCrossText2D = AssetDatabase.LoadAssetAtPath<Texture2D>(imagePath + "/middleHandCross.png");
			ringHandCrossText2D = AssetDatabase.LoadAssetAtPath<Texture2D>(imagePath + "/ringHandCross.png");
			pinkyHandCrossText2D = AssetDatabase.LoadAssetAtPath<Texture2D>(imagePath + "/pinkyHandCross.png");

			baseHandText2D.Apply();
			thumbHandText2D.Apply();
			indexHandText2D.Apply();
			middleHandText2D.Apply();
			ringHandText2D.Apply();
			pinkyHandText2D.Apply();

			thumbHandCrossText2D.Apply();
			indexHandCrossText2D.Apply();
			middleHandCrossText2D.Apply();
			ringHandCrossText2D.Apply();
			pinkyHandCrossText2D.Apply();
		}

		// Opens the object selection window when the "Load existing Gesture" is pressed.
		private void ShowPicker(BKI_UIType uiType)
		{
			lastSelected = uiType;

			pickerWindow = EditorGUIUtility.GetControlID(FocusType.Passive) + 100;

			string buttonText = "";
			if(uiType == BKI_UIType.right)
				buttonText = "rhGes_";
			if(uiType == BKI_UIType.left)
				buttonText = "lhGes_";
			if(uiType == BKI_UIType.combi)
			{
				buttonText = "combiGes_";
				EditorGUIUtility.ShowObjectPicker<BKI_CombiGestureClass>(null, false, buttonText, pickerWindow);
			}
			else
				EditorGUIUtility.ShowObjectPicker<BKI_SingleGestureClass>(null, false, buttonText, pickerWindow);
		}

		// Update functionality
		private void OnGUI()
		{
			DrawGUI();
		}

		public void OnInspectorUpdate()
		{
			if(gestureStorage != null)
				gestureStorage.OnValidate();

			CheckIfValueUpdated();
		}

		private void CheckIfValueUpdated()
		{
			if(pickerWindowGNameCombi == "" || !isOpenedFromResourcesCombi)
				return;
			if(combiGesture.gestureIdentifier != pickerWindowGNameCombi)
				ResetPickerWindow(false, false, true);

			if(pickerWindowGNameLh == "" || !isOpenedFromResourcesLh)
				return;
			if(leftHandValues.gestureIdentifier != pickerWindowGNameLh)
				ResetPickerWindow(true, false, false);

			if(pickerWindowGNameRh == "" || !isOpenedFromResourcesRh)
				return;
			if(rightHandValues.gestureIdentifier != pickerWindowGNameRh)
				ResetPickerWindow(false, true, false);
		}

		private void ResetTool()
		{
			ResetRh();
			ResetLh();
			ResetCombinationTool();
		}

		// Defined as loose functions because the buttons in the tool also are able to call this function.
		private void ResetLh()
		{
			leftHandValues = new BKI_GestureMirrorClass();
			lhMirror = new BKI_GestureMirrorClass();
			lhGesture = ScriptableObject.CreateInstance<BKI_SingleGestureClass>();
			ResetPickerWindow(true, false, false);
			GUI.FocusControl(null);
			currentGestureSavedLh = false;
		}

		private void ResetRh()
		{
			rightHandValues = new BKI_GestureMirrorClass();
			rhMirror = new BKI_GestureMirrorClass();
			rhGesture = ScriptableObject.CreateInstance<BKI_SingleGestureClass>();
			ResetPickerWindow(false, true, true);
			GUI.FocusControl(null);
			currentGestureSavedRh = false;
		}

		private void ResetCombinationTool()
		{
			combiGesture = ScriptableObject.CreateInstance<BKI_CombiGestureClass>();
			ResetLh();
			ResetRh();
			ResetPickerWindow(true, true, true);
			GUI.FocusControl(null);
			currentGestureSavedCombi = false;
		}

		private void ResetPickerWindow(bool resetLh, bool resetRh, bool resetCombi)
		{
			if(resetLh)
			{
				isOpenedFromResourcesLh = false;
				pickerWindowGNameLh = "";
			}
			if(resetRh)
			{
				isOpenedFromResourcesRh = false;
				pickerWindowGNameRh = "";
			}
			if(resetCombi)
			{
				isOpenedFromResourcesCombi = false;
				pickerWindowGNameCombi = "";
			}
		}
	}
}