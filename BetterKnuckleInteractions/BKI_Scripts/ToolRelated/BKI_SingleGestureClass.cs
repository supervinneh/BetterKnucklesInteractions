using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BetterKnucklesInteractions
{
	[Serializable]
	public class BKI_SingleGestureClass : BKI_GestureClass
	{
		public BKI_Hand hand;
		public bool requiresClench;
		public BKI_FingerState[] fingerStates;

		public Vector2[] ranges;

		public BKI_SingleGestureClass() { }

		public void SetValues(BKI_Hand h, string id, bool reqClench, BKI_FingerState[] states)
		{
			hand = h;
			gestureIdentifier = id;
			requiresClench = reqClench;
			fingerStates = states;
			AssignValues(states);
		}

		// If the finger is ignored, set the range to 0 and 1 so it always returns true.
		// Else set to low or high values.
		private void AssignValues(BKI_FingerState[] states)
		{
			ranges = new Vector2[5];
			assignedGesturePriority = 0;

			// Thumb values are an exception.

			switch(states[0])
			{
				case BKI_FingerState.fingerIn:
					ranges[0] = new Vector2(0.4f, 1f);
					assignedGesturePriority++;
					break;
				case BKI_FingerState.fingerOut:
					ranges[0] = new Vector2(0f, 0.2f);
					break;
				case BKI_FingerState.FingerIgnored:
					ranges[0] = new Vector2(0, 1);
					assignedGesturePriority--;
					break;
			}

			for(int i = 1; i < ranges.Length; i++)
			{
				switch(states[i])
				{
					case BKI_FingerState.fingerIn:
						ranges[i] = new Vector2(0.65f, 1);
						assignedGesturePriority++;
						break;
					case BKI_FingerState.fingerOut:
						ranges[i] = new Vector2(0f, 0.5f);
						break;
					case BKI_FingerState.FingerIgnored:
						ranges[i] = new Vector2(0f, 1);
						assignedGesturePriority--;
						break;
				}
			}

			if(requiresClench)
				assignedGesturePriority++;
		}

		public override bool IsGestureValid(BKI_UIType t)
		{
			if(BKI_FingerCurler.BKI_Curler_Instance == null)
				return false;

			FingerValues values;

			switch(t)
			{
				case BKI_UIType.left:
					values = GetFingerValues(BKI_Hand.left);
					break;
				case BKI_UIType.right:
					values = GetFingerValues(BKI_Hand.right);
					break;
				default:
					return false;
			}

			if(requiresClench && !values.isGripping)
				return false;

			if( IsValueInAllowedRange(BKI_Finger.thumb, values.thumb) && 
				IsValueInAllowedRange(BKI_Finger.index, values.index) &&
				IsValueInAllowedRange(BKI_Finger.middle, values.middle) &&
				IsValueInAllowedRange(BKI_Finger.ring, values.ring) &&
				IsValueInAllowedRange(BKI_Finger.pinky, values.pinky))
				return true;

			return false;
		}

		private FingerValues GetFingerValues(BKI_Hand hand)
		{
			float thumbVal = BKI_FingerCurler.BKI_Curler_Instance.GetFingerValueMesh(hand, BKI_Finger.thumb);
			float indexVal = BKI_FingerCurler.BKI_Curler_Instance.GetFingerValueMesh(hand, BKI_Finger.index);
			float middleVal = BKI_FingerCurler.BKI_Curler_Instance.GetFingerValueMesh(hand, BKI_Finger.middle);
			float ringVal = BKI_FingerCurler.BKI_Curler_Instance.GetFingerValueMesh(hand, BKI_Finger.ring);
			float pinkyVal = BKI_FingerCurler.BKI_Curler_Instance.GetFingerValueMesh(hand, BKI_Finger.pinky);
			bool isClenching = BKI_FingerCurler.BKI_Curler_Instance.IsHandClenching(hand);

			return new FingerValues(thumbVal, indexVal, middleVal, ringVal, pinkyVal, isClenching);
		}

		public bool IsValueInAllowedRange(BKI_Finger finger, float value)
		{
			return (value >= ranges[(int)finger].x && value <= ranges[(int)finger].y);
		}

		public override int GetHashCode()
		{
			// Keep this number prime.
			int num = 3;
			num = num * hand.GetHashCode();
			num = num * 23 + ((gestureIdentifier == null) ? 0 : gestureIdentifier.GetHashCode());
			num = num * 23 + requiresClench.GetHashCode();
			num = num * 23 + ((fingerStates == null) ? 0 : fingerStates.GetHashCode());
			num = num * 23 + ((ranges == null) ? 0 : ranges.GetHashCode());

			return num;
		}

		public override bool Equals(object o)
		{
			if(o == null)
				return false;

			BKI_SingleGestureClass other = o as BKI_SingleGestureClass;

			if(other == null)
				return false;

			return this.GetHashCode() == other.GetHashCode();
		}

		public static bool operator ==(BKI_SingleGestureClass lhs, BKI_SingleGestureClass rhs)
		{
			return ReferenceEquals(lhs, null) || lhs.Equals(rhs);
		}

		public static bool operator !=(BKI_SingleGestureClass lhs, BKI_SingleGestureClass rhs)
		{
			return !(lhs == rhs);
		}
	}

	public struct FingerValues
	{
		public float thumb, index, middle, ring, pinky;
		public bool isGripping;
		public FingerValues(float t, float i, float m, float r, float p, bool c)
		{
			thumb = t;
			index = i;
			middle = m;
			ring = r;
			pinky = p;
			isGripping = c;
		}
	}
}
