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
		private void DrawSubmitButtonsBoth()
		{
			GUILayout.BeginArea(buttonStackRect);
			{
				EditorGUILayout.BeginVertical();
				{
					if(GUILayout.Button("Save combined \n gesture", GUILayout.Height(55)))
					{
						SubmitCombinationGesture(combiGesture, isOpenedFromResourcesCombi);
						ResetCombinationTool();
					}
				}
				EditorGUILayout.EndVertical();
			}
			GUILayout.EndArea();
		}

		// Starts the saving sequence of the combination gesture.
		private void SubmitCombinationGesture(BKI_CombiGestureClass ges, bool fromResources)
		{
			if(gestureStorage == null)
			{
				Debug.LogError("Gesture storage could not be found. Exiting save function.");
				return;
			}

			if(!gestureStorage.EntryAlreadyExists(ges) || fromResources)
			{
				//ges.leftHandGesture = 
				//ges.rightHandGesture = 
				BKI_SingleGestureClass lh = SubmitSingleHandGesture(BKI_Hand.left, ges.leftHandGesture, isOpenedFromResourcesLh);
				BKI_SingleGestureClass rh = SubmitSingleHandGesture(BKI_Hand.right, ges.rightHandGesture, isOpenedFromResourcesRh);

				BKI_CombiGestureClass g = SaveObjectToResources(ges);

				g.leftHandGesture = lh;
				g.rightHandGesture = lh;

				gestureStorage.SaveToList(g, fromResources);
			}
		}
		

		private void DrawButtonsAll()
		{

			int rectWidth = (int)(WINDOW_MAX_SIZE_X * 0.95f);
			Rect combiGestureContainerRect = new Rect(DEFAULT_AREA_PADDING, 16, rectWidth, 80);
			int buttonWidth = (int)(rectWidth / 6 * 1.45f);
			Rect buttonRect = new Rect(WINDOW_MAX_SIZE_X * 0.25f - DEFAULT_AREA_PADDING, 15, buttonWidth * 3f +	DEFAULT_AREA_PADDING, 70);


			GUILayout.BeginArea(combiGestureContainerRect);
			{
				EditorGUILayout.BeginVertical();
				{
					GUILayout.Space(12);
					GUILayout.Label("Gesture identifier: ");
					combiGesture.gestureIdentifier = EditorGUILayout.TextField(combiGesture.gestureIdentifier, GUILayout.Width(buttonWidth));
				}
				EditorGUILayout.EndVertical();
				GUILayout.Space(DEFAULT_AREA_PADDING);
				//EditorGUILayout.BeginVertical();
				//{
				EditorGUILayout.BeginHorizontal();
				{
					
					GUILayout.BeginArea(buttonRect);
					{
						EditorGUILayout.BeginHorizontal();
						{
							if(GUILayout.Button("Reset combination \n gesture", GUILayout.Height(40), GUILayout.Width(buttonWidth)))
							{
								ResetCombinationTool();
							}
							DrawOpenGestureButton(BKI_UIType.combi, buttonWidth);

							DrawSubmitButtonAll(buttonWidth);
						}
						EditorGUILayout.EndHorizontal();
					}
					GUILayout.EndArea();
				}
				EditorGUILayout.EndHorizontal();
				//}
				//EditorGUILayout.EndVertical();
			}
			GUILayout.EndArea();
		}

		private void DrawSubmitButtonAll(float buttonWidth)
		{
			string defaultString = "Save current gesture";
			string tooltipWarning = "";
			if(!combiGesture.IsGestureFilled())
				tooltipWarning = "One of the gesture parts is either null of has a default name.";
			else if((combiGesture.gestureIdentifier == DEFAULT_GESTURE_NAME))
				tooltipWarning = "Gesture name can't be [" + DEFAULT_GESTURE_NAME + "].";
			else if(combiGesture.gestureIdentifier.Length < 1)
				tooltipWarning = "Gesture name can't be empty.";
			else if(!isOpenedFromResourcesCombi && gestureStorage.EntryAlreadyExists(combiGesture))
				tooltipWarning = "This gesture name is already used.";
			else
				tooltipWarning = defaultString;

			EditorGUI.BeginDisabledGroup(tooltipWarning != defaultString);
			{
				if(GUILayout.Button(new GUIContent("Save combination \n gesture", tooltipWarning), GUILayout.Height(buttonHeight), GUILayout.Width(buttonWidth)))
				{
					Debug.Log("Save combi \n gesture");
					SubmitCombinationGesture(combiGesture, isOpenedFromResourcesCombi);
					currentGestureSavedCombi = true;
					pickerWindowGNameCombi = combiGesture.gestureIdentifier;
					isOpenedFromResourcesCombi = true;
				}
			}
			EditorGUI.EndDisabledGroup();
		}
	}
}
