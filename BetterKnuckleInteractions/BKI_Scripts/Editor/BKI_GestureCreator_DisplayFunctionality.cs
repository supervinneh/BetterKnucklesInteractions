using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System;

namespace BetterKnucklesInteractions
{
	// This partial is responsible for most of the visual/display calls.
	public partial class BKI_GestureCreator
	{
		private string[] toolbarText = new string[3] { "Out", "In", "Ignore" };
		private BKI_FingerState fingerState = BKI_FingerState.fingerOut;

		// Main draw function. Called in OnGUI.
		private void DrawGUI()
		{
			GUILayout.BeginArea(backgroundRect);
			{
				GUILayout.Space(14);
				EditorGUILayout.BeginHorizontal();
				{
					GUILayout.Space(10);
					GUI.DrawTexture(backgroundRect, backgroundTex2D);
					GUI.DrawTexture(titleContainer, titleBackgroundTex2D);
					GUILayout.Label("Gesture Creator", titleStyle);
					GUILayout.Space(20);
				}
				EditorGUILayout.EndHorizontal();

				GUILayout.Space(16);

				EditorGUILayout.BeginHorizontal();
				{
					DrawHandGUI(BKI_Hand.right);
					DrawHandGUI(BKI_Hand.left);
				}
				EditorGUILayout.EndHorizontal();
				GUI.DrawTexture(footerContainer, footerBackgroundTex2D);
				DrawFooter();
				//fingerState = (BKI_FingerState)GUILayout.Toolbar((int)fingerState, toolbarText);
			}
			GUILayout.EndArea();
		}

		private BKI_UIType lastSelected = BKI_UIType.right;

		// Display the "Load existing gesture" button and handles the input event coupled to it.
		private void DrawOpenGestureButton(BKI_UIType uiType, float width = 0)
		{
			int buttonHeight = 40;

			string buttonText = "";
			if(uiType == BKI_UIType.right)
				buttonText = "rh";
			if(uiType == BKI_UIType.left)
				buttonText = "lh";
			if(uiType == BKI_UIType.combi)
				buttonText = "combi";

			if(width > 0)
			{
				if(GUILayout.Button("Load existing \n gesture " + buttonText, GUILayout.Height(buttonHeight), GUILayout.Width(width)))
				{
					ShowPicker(uiType);
				}
			}
			else
			{
				if(GUILayout.Button("Load existing \n gesture " + buttonText, GUILayout.Height(buttonHeight)))
				{
					ShowPicker(uiType);
				}
			}

			if(Event.current.commandName == "ObjectSelectorUpdated" && EditorGUIUtility.GetObjectPickerControlID() == pickerWindow)
			{
				CatchPickerObject(EditorGUIUtility.GetObjectPickerObject(), lastSelected);
				
				pickerWindow = -1;
				this.Focus();
			}
		}

		private void CatchPickerObject(object o, BKI_UIType ls)
		{
			if(o == null)
				return;

			switch(ls)
			{
				case BKI_UIType.left:
					BKI_SingleGestureClass t = o as BKI_SingleGestureClass;

					if(t == null)
					{
						Debug.LogError("Object invalid. Stopping opening.");
						break;
					}
					if(t.hand != BKI_Hand.left)
					{
						Debug.LogError("You tried to open a right handed gesture into the left hand editor. Stopping opening.");
						break;
					}

					lhGesture = ScriptableObject.Instantiate(t);
					leftHandValues = DeserializeGesture(lhGesture);
					lhMirror = DeserializeGesture(lhGesture);
					isOpenedFromResourcesLh = true;
					currentGestureSavedLh = true;
					pickerWindowGNameLh = t.gestureIdentifier;
					break;
				case BKI_UIType.right:
					BKI_SingleGestureClass u = o as BKI_SingleGestureClass;

					if(u == null)
					{
						Debug.LogError("Object invalid. Stopping opening.");
						break;
					}
					if(u.hand != BKI_Hand.right)
					{
						Debug.LogError("You tried to open a left handed gesture into the left hand editor. Stopping opening.");
						break;
					}

					rhGesture = ScriptableObject.Instantiate(u);
					rightHandValues = DeserializeGesture(rhGesture);
					rhMirror = DeserializeGesture(rhGesture);
					isOpenedFromResourcesRh = true;
					currentGestureSavedRh = true;
					pickerWindowGNameRh = u.gestureIdentifier;
					break;
				case BKI_UIType.combi:
					BKI_CombiGestureClass ges = o as BKI_CombiGestureClass;

					if(ges == null)
					{
						Debug.LogError("Object invalid. Stopping opening.");
						break;
					}
					if(!ges.name.Contains("combi"))
					{
						Debug.LogError("You tried to open a gesture other than a combination gesture. Terminating.");
						break;
					}

					combiGesture = ScriptableObject.Instantiate(ges);
					CatchPickerObject(combiGesture.leftHandGesture, BKI_UIType.left);
					CatchPickerObject(combiGesture.rightHandGesture, BKI_UIType.right);
					pickerWindowGNameCombi = ges.gestureIdentifier;
					currentGestureSavedCombi = true;
					isOpenedFromResourcesCombi = true;
					break;
			}
		}

		// Draws the buttons at the bottom of the window.
		private void DrawFooter()
		{
			combiGesture.leftHandGesture = lhGesture;
			combiGesture.rightHandGesture = rhGesture;
			GUILayout.BeginArea(footerContainer);
			{
				GUILayout.Space(DEFAULT_AREA_PADDING / 2);
				EditorGUILayout.BeginHorizontal();
				{
					DrawCombiTitle();
					DrawButtonsAll();
				}
				EditorGUILayout.EndHorizontal();
			}
			GUILayout.EndArea();
		}

		// Draws the base hand and the individual fingers dependent on selected values.
		private void DrawHandImages(BKI_Hand hand, BKI_GestureMirrorClass values, Rect pos, float scale = 1)
		{
			float flipValueWidth, flipValuePos;
			float valueHeight = 1 / scale;
			Rect newPos = pos;
			if(hand == BKI_Hand.right)
			{
				flipValueWidth = 1;
				flipValuePos = 0;
				newPos.x += 15;
			}
			else
			{
				flipValueWidth = -1;
				flipValuePos = 1;
			}

			flipValueWidth /= scale;

			if(values.GetFingerState(BKI_Finger.thumb) == BKI_FingerState.FingerIgnored)
				GUI.DrawTextureWithTexCoords(newPos, thumbHandCrossText2D, new Rect(flipValuePos, 0, flipValueWidth, valueHeight));
			else if(!(values.GetFingerState(BKI_Finger.thumb) == BKI_FingerState.fingerIn))
				GUI.DrawTextureWithTexCoords(newPos, thumbHandText2D, new Rect(flipValuePos, 0, flipValueWidth, valueHeight));

			if(values.GetFingerState(BKI_Finger.index) == BKI_FingerState.FingerIgnored)
				GUI.DrawTextureWithTexCoords(newPos, indexHandCrossText2D, new Rect(flipValuePos, 0, flipValueWidth, valueHeight));
			else if(!(values.GetFingerState(BKI_Finger.index) == BKI_FingerState.fingerIn))
				GUI.DrawTextureWithTexCoords(newPos, indexHandText2D, new Rect(flipValuePos, 0, flipValueWidth, valueHeight));

			if(values.GetFingerState(BKI_Finger.middle) == BKI_FingerState.FingerIgnored)
				GUI.DrawTextureWithTexCoords(newPos, middleHandCrossText2D, new Rect(flipValuePos, 0, flipValueWidth, valueHeight));
			else if(!(values.GetFingerState(BKI_Finger.middle) == BKI_FingerState.fingerIn))
				GUI.DrawTextureWithTexCoords(newPos, middleHandText2D, new Rect(flipValuePos, 0, flipValueWidth, valueHeight));

			if(values.GetFingerState(BKI_Finger.ring) == BKI_FingerState.FingerIgnored)
				GUI.DrawTextureWithTexCoords(newPos, ringHandCrossText2D, new Rect(flipValuePos, 0, flipValueWidth, valueHeight));
			else if(!(values.GetFingerState(BKI_Finger.ring) == BKI_FingerState.fingerIn))
				GUI.DrawTextureWithTexCoords(newPos, ringHandText2D, new Rect(flipValuePos, 0, flipValueWidth, valueHeight));

			if(values.GetFingerState(BKI_Finger.pinky) == BKI_FingerState.FingerIgnored)
				GUI.DrawTextureWithTexCoords(newPos, pinkyHandCrossText2D, new Rect(flipValuePos, 0, flipValueWidth, valueHeight));
			else if(!(values.GetFingerState(BKI_Finger.pinky) == BKI_FingerState.fingerIn))
				GUI.DrawTextureWithTexCoords(newPos, pinkyHandText2D, new Rect(flipValuePos, 0, flipValueWidth, valueHeight));

			GUI.DrawTextureWithTexCoords(newPos, baseHandText2D, new Rect(flipValuePos, 0, flipValueWidth, valueHeight));
		}

		private void DrawCombiTitle()
		{
			EditorGUILayout.BeginHorizontal();
			{
				GUILayout.Label("Combination gesture", subtitleStyle);
			}
			EditorGUILayout.EndHorizontal();
		}
	}
}