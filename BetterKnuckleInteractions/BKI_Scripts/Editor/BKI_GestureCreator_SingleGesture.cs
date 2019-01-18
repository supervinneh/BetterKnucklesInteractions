using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System;

namespace BetterKnucklesInteractions
{
	// This partial is responsible for the combination gesture editor.
	public partial class BKI_GestureCreator
	{
		// Draws the appropriate title depending on the hand requested.
		private void DrawHandTitle(BKI_Hand hand)
		{
			string handString = (hand == BKI_Hand.right) ? "Right" : "Left";
			GUILayout.Space(12);
			EditorGUILayout.BeginHorizontal();
			{
				GUILayout.Label(handString + " hand \n gesture", subtitleStyle);
				if(GUILayout.Button("Reset values \n " + handString + " hand", GUILayout.Height(32), GUILayout.Width(96)))
				{
					if(hand == BKI_Hand.right)
						ResetRh();
					else
						ResetLh();
				}
				GUILayout.Space(32);
			}
			EditorGUILayout.EndHorizontal();
		}



		// Draws the check boxes and their labels as seen in the tool UI.
		private void DrawBooleans(BKI_Hand hand)
		{
			BKI_GestureMirrorClass values = (hand == BKI_Hand.right) ? rightHandValues : leftHandValues;

			Rect containerRect = (hand == BKI_Hand.right) ? handGUIContainer : handGUIContainer;

			Rect r = new Rect(DEFAULT_AREA_PADDING + 5, TITLE_FIELD_SIZE + 2, containerRect.width - DEFAULT_AREA_PADDING * 1.5f, containerRect.height - DEFAULT_AREA_PADDING * 1.5f - TITLE_FIELD_SIZE);
			GUILayout.BeginArea(r);
			{
				// Parenthesis to clarify GUI layout at code side.
				EditorGUILayout.BeginHorizontal();
				{
					EditorGUILayout.BeginVertical();
					{
						GUILayout.Label("Thumb: ");
						GUILayout.Space(3);
						GUILayout.Label("Index: ");
						GUILayout.Space(3);
						GUILayout.Label("Middle: ");
						GUILayout.Space(3);
						GUILayout.Label("Ring: ");
						GUILayout.Space(3);
						GUILayout.Label("Pinky: ");
					}
					EditorGUILayout.EndVertical();
					GUILayout.Space(-60);
					EditorGUILayout.BeginVertical();
					{
						values.fingerStates[0] = (BKI_FingerState)GUILayout.Toolbar((int)values.fingerStates[0], toolbarText, GUILayout.Width(160));
						values.fingerStates[1] = (BKI_FingerState)GUILayout.Toolbar((int)values.fingerStates[1], toolbarText, GUILayout.Width(160));
						values.fingerStates[2] = (BKI_FingerState)GUILayout.Toolbar((int)values.fingerStates[2], toolbarText, GUILayout.Width(160));
						values.fingerStates[3] = (BKI_FingerState)GUILayout.Toolbar((int)values.fingerStates[3], toolbarText, GUILayout.Width(160));
						values.fingerStates[4] = (BKI_FingerState)GUILayout.Toolbar((int)values.fingerStates[4], toolbarText, GUILayout.Width(160));
					}
					EditorGUILayout.EndVertical();

					//EditorGUILayout.BeginVertical();
					//{
					//	bool thumb = values.GetFingerPressed(BKI_Finger.thumb);
					//	thumb = EditorGUILayout.Toggle(values.GetFingerPressed(BKI_Finger.thumb));
					//	values.SetFingerPressed(BKI_Finger.thumb, thumb);

					//	bool index = values.GetFingerPressed(BKI_Finger.index);
					//	index = EditorGUILayout.Toggle(values.GetFingerPressed(BKI_Finger.index));
					//	values.SetFingerPressed(BKI_Finger.index, index);

					//	bool middle = values.GetFingerPressed(BKI_Finger.middle);
					//	middle = EditorGUILayout.Toggle(values.GetFingerPressed(BKI_Finger.middle));
					//	values.SetFingerPressed(BKI_Finger.middle, middle);

					//	bool ring = values.GetFingerPressed(BKI_Finger.ring);
					//	ring = EditorGUILayout.Toggle(values.GetFingerPressed(BKI_Finger.ring));
					//	values.SetFingerPressed(BKI_Finger.ring, ring);

					//	bool pinky = values.GetFingerPressed(BKI_Finger.pinky);
					//	pinky = EditorGUILayout.Toggle(values.GetFingerPressed(BKI_Finger.pinky));
					//	values.SetFingerPressed(BKI_Finger.pinky, pinky);
					//}
					//EditorGUILayout.EndVertical();

					//EditorGUILayout.BeginVertical();
					//{
					//	GUILayout.Label("Ignore thumb: ");
					//	GUILayout.Label("Ignore index: ");
					//	GUILayout.Label("Ignore middle: ");
					//	GUILayout.Label("Ignore ring: ");
					//	GUILayout.Label("Ignore pinky: ");
					//}
					//EditorGUILayout.EndVertical();

					//EditorGUILayout.BeginVertical();
					//{
					//	bool thumb = values.GetFingerIgnored(BKI_Finger.thumb);
					//	thumb = EditorGUILayout.Toggle(values.GetFingerIgnored(BKI_Finger.thumb));
					//	values.SetFingerIgnored(BKI_Finger.thumb, thumb);

					//	bool index = values.GetFingerIgnored(BKI_Finger.index);
					//	index = EditorGUILayout.Toggle(values.GetFingerIgnored(BKI_Finger.index));
					//	values.SetFingerIgnored(BKI_Finger.index, index);

					//	bool middle = values.GetFingerIgnored(BKI_Finger.middle);
					//	middle = EditorGUILayout.Toggle(values.GetFingerIgnored(BKI_Finger.middle));
					//	values.SetFingerIgnored(BKI_Finger.middle, middle);

					//	bool ring = values.GetFingerIgnored(BKI_Finger.ring);
					//	ring = EditorGUILayout.Toggle(values.GetFingerIgnored(BKI_Finger.ring));
					//	values.SetFingerIgnored(BKI_Finger.ring, ring);

					//	bool pinky = values.GetFingerIgnored(BKI_Finger.pinky);
					//	pinky = EditorGUILayout.Toggle(values.GetFingerIgnored(BKI_Finger.pinky));
					//	values.SetFingerIgnored(BKI_Finger.pinky, pinky);
					//}
					//EditorGUILayout.EndVertical();
				}
				EditorGUILayout.EndHorizontal();
			}
			GUILayout.EndArea();

			if(hand == BKI_Hand.right)
			{
				if(rightHandValues != values && values.gestureIdentifier != pickerWindowGNameRh)
				{
					ResetPickerWindow(false, true, false);
					rightHandValues = values;
				}
			}
			else if(leftHandValues != values && values.gestureIdentifier != pickerWindowGNameLh)
			{
				ResetPickerWindow(true, false, false);
				leftHandValues = values;
			}
		}

		// Draws one side of the tool.
		private void DrawHandGUI(BKI_Hand hand)
		{
			Rect r = handGUIContainer;
			Rect r2 = handRect;
			if(hand == BKI_Hand.right)
				r.x = r.x + (WINDOW_MAX_SIZE_X / 2) - 3;

			r.width += (DEFAULT_AREA_PADDING);
			r2.width += (DEFAULT_AREA_PADDING / 4);

			GUILayout.BeginArea(r);
			{
				GUI.DrawTexture(r2, containerHandTex2D);
				EditorGUILayout.BeginVertical();
				{
					DrawHandTitle(hand);

					DrawBooleans(hand);

					GUILayout.Space(108);

					EditorGUILayout.BeginHorizontal();
					{
						GUILayout.Space(12);
						GUILayout.Label("Requires clench:");
						((hand == BKI_Hand.right) ? rightHandValues : leftHandValues).requiresClench = EditorGUILayout.Toggle(((hand == BKI_Hand.right) ? rightHandValues : leftHandValues).requiresClench);
						GUILayout.Space(84);
					}
					EditorGUILayout.EndHorizontal();

					BKI_GestureMirrorClass values = (hand == BKI_Hand.right) ? rightHandValues : leftHandValues;
					DrawHandImages(hand, values, baseHandRect);

					GUILayout.Space(WINDOW_MAX_SIZE_Y * 0.37f);

					EditorGUILayout.BeginHorizontal();
					{
						GUILayout.Space(16);
						EditorGUILayout.BeginVertical();
						{
							EditorGUILayout.BeginHorizontal();
							{
								GUILayout.Label("Gesture identifier:");
								((hand == BKI_Hand.right) ? rightHandValues : leftHandValues).gestureIdentifier = EditorGUILayout.TextField(((hand == BKI_Hand.right) ? rightHandValues : leftHandValues).gestureIdentifier, GUILayout.Width(128));
								GUILayout.Space(128);
							}
							EditorGUILayout.EndHorizontal();

						}
						EditorGUILayout.EndVertical();
					}
					EditorGUILayout.EndHorizontal();
					
					EditorGUILayout.BeginHorizontal();
					{
						DrawButtonsSingle(hand);
					}
					EditorGUILayout.EndHorizontal();
				}
				EditorGUILayout.EndVertical();
			}
			GUILayout.EndArea();
		}

		private void DrawButtonsSingle(BKI_Hand hand)
		{
			string handString = (hand == BKI_Hand.right) ? "right" : "left";
			string handStringShorthand = (hand == BKI_Hand.right) ? "rh" : "lh";
			lhGesture = SerializeGesture(BKI_Hand.left, leftHandValues);
			rhGesture = SerializeGesture(BKI_Hand.right, rightHandValues);
			float buttonWidth = WINDOW_MAX_SIZE_X * 0.25f - (DEFAULT_AREA_PADDING * 2.5f);
			int xPos = DEFAULT_AREA_PADDING;

			BKI_UIType t = (hand == BKI_Hand.right) ? BKI_UIType.right : BKI_UIType.left;

			GUILayout.BeginArea(buttonStackRect);
			{
				GUILayout.BeginArea(new Rect(xPos, 0, WINDOW_MAX_SIZE_X * 0.45f - DEFAULT_AREA_PADDING, TITLE_FIELD_SIZE * 3));
				{
					EditorGUILayout.BeginHorizontal();
					DrawSubmitButtonSingle(hand, buttonWidth);
					EditorGUILayout.EndHorizontal();
				}
				GUILayout.EndArea();
				GUILayout.Space(DEFAULT_AREA_PADDING);
			}
			GUILayout.EndArea();
		}

		private void DrawSubmitButtonSingle(BKI_Hand hand, float buttonWidth)
		{
			bool rh = (rhGesture.gestureIdentifier == DEFAULT_GESTURE_NAME && hand == BKI_Hand.right);
			bool lh = (lhGesture.gestureIdentifier == DEFAULT_GESTURE_NAME && hand == BKI_Hand.left);
			bool both = (rhGesture.gestureIdentifier == DEFAULT_GESTURE_NAME && hand == BKI_Hand.right) || (lhGesture.gestureIdentifier == DEFAULT_GESTURE_NAME && hand == BKI_Hand.left);

			string defaultString = "Save current gesture";
			string tooltipWarning = "";

			if(hand == BKI_Hand.right)
			{
				DrawOpenGestureButton(BKI_UIType.right, buttonWidth);

				if((rhGesture.gestureIdentifier == DEFAULT_GESTURE_NAME))
					tooltipWarning = "Gesture name can't be [" + DEFAULT_GESTURE_NAME + "].";
				else if(rhGesture.gestureIdentifier.Length < 1)
					tooltipWarning = "Gesture name can't be empty.";
				else if(!isOpenedFromResourcesRh && gestureStorage.EntryAlreadyExists(BKI_Hand.right, rhGesture))
					tooltipWarning = "This gesture name is already used.";
				else
					tooltipWarning = defaultString;

				EditorGUI.BeginDisabledGroup(tooltipWarning != defaultString);
				{
					if(GUILayout.Button(new GUIContent("Save right hand \n gesture", tooltipWarning), GUILayout.Height(buttonHeight), GUILayout.Width(buttonWidth)))
					{
						Debug.Log("Save rh \n gesture");
						SubmitSingleHandGesture(BKI_Hand.right, rhGesture, isOpenedFromResourcesRh);
						currentGestureSavedRh = true;
						pickerWindowGNameRh = rhGesture.gestureIdentifier;
						isOpenedFromResourcesRh = true;
					}
				}
				EditorGUI.EndDisabledGroup();
			}
			else
			{
				DrawOpenGestureButton(BKI_UIType.left, buttonWidth);

				if((lhGesture.gestureIdentifier == DEFAULT_GESTURE_NAME))
					tooltipWarning = "Gesture name can't be [" + DEFAULT_GESTURE_NAME + "].";
				else if(lhGesture.gestureIdentifier.Length < 1)
					tooltipWarning = "Gesture name can't be empty.";
				else if(!isOpenedFromResourcesLh && gestureStorage.EntryAlreadyExists(BKI_Hand.left, lhGesture))
					tooltipWarning = "This gesture name is already used.";
				else
					tooltipWarning = defaultString;

				EditorGUI.BeginDisabledGroup(tooltipWarning != defaultString);
				{
					if(GUILayout.Button(new GUIContent("Save left hand \n gesture", tooltipWarning), GUILayout.Height(buttonHeight), GUILayout.Width(buttonWidth)))
					{
						Debug.Log("Save lh \n gesture");
						SubmitSingleHandGesture(BKI_Hand.left, lhGesture, isOpenedFromResourcesLh);
						currentGestureSavedLh = true;
						pickerWindowGNameLh = lhGesture.gestureIdentifier;
						isOpenedFromResourcesLh = true;
					}
				}
				EditorGUI.EndDisabledGroup();
			}
		}

		// Happens when the "Save Gesture" button is pressed. Starts the saving process. Returns the object referenced in the resources folder.
		private BKI_SingleGestureClass SubmitSingleHandGesture(BKI_Hand hand, BKI_SingleGestureClass ges, bool fromResources)
		{
			BKI_SingleGestureClass returnObj = null;
			if(gestureStorage == null)
			{
				Debug.LogError("Gesture storage could not be found. Exiting save function.");
				return null;
			}

			if(!gestureStorage.EntryAlreadyExists(hand, ges) || fromResources)
			{
				returnObj = SaveObjectToResources(ges, hand);
				gestureStorage.SaveToList(hand, returnObj, fromResources);
			}

			return returnObj;
		}
	}
}