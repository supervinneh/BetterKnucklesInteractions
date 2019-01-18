using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System;

namespace BetterKnucklesInteractions
{
	// This partial is responsible for the saving of the gesture functionality.
	public partial class BKI_GestureCreator
	{
		private void DeserializeSavedGestureToTool(BKI_CombiGestureClass cGes)
		{
			lhGesture = cGes.leftHandGesture;
			rhGesture = cGes.rightHandGesture;

			lhMirror = DeserializeGesture(lhGesture);
			rhMirror = DeserializeGesture(rhGesture);
		}

		private BKI_CombiGestureClass SerializeToolToGesture(string identifier, BKI_SingleGestureClass lhGes, BKI_SingleGestureClass rhGes)
		{
			BKI_CombiGestureClass returnVal = new BKI_CombiGestureClass();
			returnVal.SetValues(identifier, lhGes, rhGes);
			return returnVal;
		}

		private BKI_CombiGestureClass SaveObjectToResources(BKI_CombiGestureClass o)
		{
			string path = RESOURCES_COMBI_PATH + "/combiGes_" + o.gestureIdentifier + ".asset";
			AssetDatabase.CreateAsset(o, path);
			//AssetDatabase.AddObjectToAsset(o.leftHandGesture, o);
			//AssetDatabase.AddObjectToAsset(o.rightHandGesture, o);
			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();

			BKI_CombiGestureClass returnVal = AssetDatabase.LoadAssetAtPath<BKI_CombiGestureClass>(path);

			return returnVal;
		}

		private BKI_SingleGestureClass SaveObjectToResources(BKI_SingleGestureClass o, BKI_Hand hand)
		{
			string path = "";

			switch(hand)
			{
				case BKI_Hand.left:
					{
						path = RESOURCES_LEFTHAND_PATH + "/lhGes_" + o.gestureIdentifier + ".asset";
					}
					break;
				case BKI_Hand.right:
					{
						path = RESOURCES_RIGHTHAND_PATH + "/rhGes_" + o.gestureIdentifier + ".asset";
					}
					break;
			}

			AssetDatabase.CreateAsset(o, path);
			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();

			return AssetDatabase.LoadAssetAtPath<BKI_SingleGestureClass>(path);
		}

		/// <summary>
		/// Function that converts the mirror class to the class that will be used at runtime.
		/// Cannot return null. If the input class is null, it returns a freshly generated BKI_GestureClass.
		/// </summary>
		private BKI_SingleGestureClass SerializeGesture(BKI_Hand h, BKI_GestureMirrorClass mirror)
		{
			BKI_SingleGestureClass returnVal = ScriptableObject.CreateInstance<BKI_SingleGestureClass>();
			if(mirror == null)
				return returnVal;

			returnVal.SetValues(h, mirror.gestureIdentifier, mirror.requiresClench, mirror.fingerStates);

			return returnVal;
		}

		/// <summary>
		/// Function that converts the mirror class to the class that will be used at runtime.
		/// Cannot return null. If the input class is null, it returns a freshly generated BKI_GestureMirrorClass.
		/// </summary>
		private BKI_GestureMirrorClass DeserializeGesture(BKI_SingleGestureClass gesture)
		{
			BKI_GestureMirrorClass returnVal = new BKI_GestureMirrorClass();
			if(gesture == null)
				return returnVal;

			returnVal.SetValues(gesture.gestureIdentifier, gesture.requiresClench, gesture.fingerStates);

			return returnVal;
		}
	}
}
