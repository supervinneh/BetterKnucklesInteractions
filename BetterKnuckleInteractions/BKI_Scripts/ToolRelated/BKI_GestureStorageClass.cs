using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

namespace BetterKnucklesInteractions
{
	[Serializable]
	//-------------------------------------------------------------Do not change this name if you don't know what you're doing!
	[CreateAssetMenuAttribute(fileName = "New gesture storage", menuName = "Better Knuckles Interactions/GeneralStorage")]
	public class BKI_GestureStorageClass : ScriptableObject
	{
		public List<BKI_CombiGestureStruct> combiGesturesList;
		private Dictionary<string, BKI_CombiGestureClass> combiDictionary;

		public List<BKI_SingleGestureStruct> lhGesturesList;
		private Dictionary<string, BKI_SingleGestureClass> lhDictionary;

		public List<BKI_SingleGestureStruct> rhGesturesList;
		private Dictionary<string, BKI_SingleGestureClass> rhDictionary;

		public void OnValidate()
		{
			SweepDictionaries();
		}

		public bool IsGestureAtKeyValid(BKI_UIType hand, string key)
		{
			switch(hand)
			{
				case BKI_UIType.combi:
					if(combiDictionary.ContainsKey(key))
						return combiDictionary[key].IsGestureValid(hand);
					else
						return false;
				case BKI_UIType.left:
					if(lhDictionary.ContainsKey(key))
						return lhDictionary[key].IsGestureValid(hand);
					else
						return false;
				case BKI_UIType.right:
					if(rhDictionary.ContainsKey(key))
						return rhDictionary[key].IsGestureValid(hand);
					else
						return false;
			}
			return false;
		}

		public int GetGesturePriorityAtKey(BKI_UIType hand, string key)
		{
			int returnVal = 0;

			switch(hand)
			{
				case BKI_UIType.combi:
					if(combiDictionary.ContainsKey(key))
						returnVal = combiDictionary[key].GetGesturePriority();
					break;
				case BKI_UIType.left:
					if(lhDictionary.ContainsKey(key))
						returnVal = lhDictionary[key].GetGesturePriority();
					break;
				case BKI_UIType.right:
					if(rhDictionary.ContainsKey(key))
						returnVal = rhDictionary[key].GetGesturePriority();
					break;
			}
			return returnVal;
		}

		// Saves individual hand
		public void SaveToList(BKI_Hand hand, BKI_SingleGestureClass ges, bool isOverwrite = false)
		{
			BKI_SingleGestureStruct struc = new BKI_SingleGestureStruct(ges);

			if(!EntryAlreadyExists(hand, ges) || isOverwrite)
			{
				if(isOverwrite)
				{
					foreach(var item in ((hand == BKI_Hand.right) ? rhGesturesList : lhGesturesList))
					{
						if(item.gesture.gestureIdentifier == ges.gestureIdentifier)
						{
							((hand == BKI_Hand.right) ? rhGesturesList : lhGesturesList).Remove(item);
							break;
						}
					}
					if(((hand == BKI_Hand.right) ? rhDictionary : lhDictionary).ContainsKey(ges.gestureIdentifier))
						((hand == BKI_Hand.right) ? rhDictionary : lhDictionary).Remove(ges.gestureIdentifier);
				}
				((hand == BKI_Hand.right) ? rhGesturesList : lhGesturesList).Add(struc);
				((hand == BKI_Hand.right) ? rhDictionary : lhDictionary).Add(ges.gestureIdentifier, ges);
			AssetDatabase.Refresh();
			EditorUtility.SetDirty(this);
			AssetDatabase.SaveAssets();
			}

		}

		public bool EntryAlreadyExists(BKI_Hand hand, BKI_SingleGestureClass ges)
		{
			SweepDictionaries();
			bool returnVal = false;

			if(!returnVal)
				returnVal = ((hand == BKI_Hand.right) ? rhDictionary : lhDictionary).ContainsKey(ges.gestureIdentifier);
			if(!returnVal)
			{
				if(hand == BKI_Hand.right)
				{
					foreach(var g in rhGesturesList)
					{
						if(g.gesture == ges)
							return true;
					}
				}
				else
				{
					foreach(var g in lhGesturesList)
					{
						if(g.gesture == ges)
							return true;
					}
				}
			}
			return returnVal;
		}

		// Saves combination
		public void SaveToList(BKI_CombiGestureClass bk, bool isOverwrite = false)
		{
			BKI_CombiGestureStruct struc = new BKI_CombiGestureStruct(bk);
			if(!EntryAlreadyExists(bk))
			{
				if(isOverwrite)
				{
					foreach(var item in combiGesturesList)
					{
						if(item.gesture.gestureIdentifier == bk.gestureIdentifier)
						{
							combiGesturesList.Remove(item);
							break;
						}
					}
					if(combiDictionary.ContainsKey(bk.gestureIdentifier))
						combiDictionary.Remove(bk.gestureIdentifier);
				}
				combiGesturesList.Add(struc);
				combiDictionary.Add(bk.gestureIdentifier, bk);
				AssetDatabase.Refresh();
				EditorUtility.SetDirty(this);
				AssetDatabase.SaveAssets();
			}
		}

		public bool EntryAlreadyExists(BKI_CombiGestureClass bk)
		{
			return combiDictionary.ContainsKey(bk.gestureIdentifier);
		}

		public void SweepDictionaries()
		{
			combiGesturesList = combiGesturesList.Distinct().ToList();
			combiGesturesList.RemoveAll(item => ReferenceEquals(item.gesture, null));
			
			lhGesturesList = lhGesturesList.Distinct().ToList();
			lhGesturesList.RemoveAll(item => ReferenceEquals(item.gesture, null));

			rhGesturesList = rhGesturesList.Distinct().ToList();
			rhGesturesList.RemoveAll(item => ReferenceEquals(item.gesture, null));

			if(combiDictionary == null)
				combiDictionary = new Dictionary<string, BKI_CombiGestureClass>();
			else
				combiDictionary.Clear();

			if(rhDictionary == null)
				rhDictionary = new Dictionary<string, BKI_SingleGestureClass>();
			else
				rhDictionary.Clear();

			if(lhDictionary == null)
				lhDictionary = new Dictionary<string, BKI_SingleGestureClass>();
			else
				lhDictionary.Clear();

			foreach(var gesStruct in combiGesturesList)
			{
				if(ReferenceEquals(gesStruct.gesture, null))
					continue;
				if(!combiDictionary.ContainsKey(gesStruct.gesture.gestureIdentifier))
					combiDictionary.Add(gesStruct.gesture.gestureIdentifier, gesStruct.gesture);
			}
			foreach(var gesStruct in lhGesturesList)
			{
				if(ReferenceEquals(gesStruct.gesture, null))
					continue;

				if(!lhDictionary.ContainsKey(gesStruct.gesture.gestureIdentifier))
					lhDictionary.Add(gesStruct.gesture.gestureIdentifier, gesStruct.gesture);
			}
			foreach(var gesStruct in rhGesturesList)
			{
				if(ReferenceEquals(gesStruct.gesture, null))
					continue;
				if(!rhDictionary.ContainsKey(gesStruct.gesture.gestureIdentifier))
					rhDictionary.Add(gesStruct.gesture.gestureIdentifier, gesStruct.gesture);
			}
		}
	}

	[Serializable]
	public struct BKI_SingleGestureStruct
	{
		public BKI_SingleGestureClass gesture;

		public BKI_SingleGestureStruct(BKI_SingleGestureClass gesture)
		{
			this.gesture = gesture;
		}
	}

	[Serializable]
	public struct BKI_CombiGestureStruct
	{
		public BKI_CombiGestureClass gesture;

		public BKI_CombiGestureStruct(BKI_CombiGestureClass gesture)
		{
			this.gesture = gesture;
		}
	}
}