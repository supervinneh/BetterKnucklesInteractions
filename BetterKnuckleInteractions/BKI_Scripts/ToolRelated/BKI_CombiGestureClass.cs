using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace BetterKnucklesInteractions
{
	[Serializable]
	public class BKI_CombiGestureClass : BKI_GestureClass
	{
		public BKI_SingleGestureClass leftHandGesture;
		public BKI_SingleGestureClass rightHandGesture;

		public BKI_CombiGestureClass() { }

		public void SetValues(string id, BKI_SingleGestureClass lh, BKI_SingleGestureClass rh)
		{
			gestureIdentifier = id;
			leftHandGesture = lh;
			rightHandGesture = rh;
			assignedGesturePriority = leftHandGesture.GetGesturePriority() + rightHandGesture.GetGesturePriority();
		}

		public override bool IsGestureValid(BKI_UIType t)
		{
			if(leftHandGesture == null || rightHandGesture == null)
				return false;
			return leftHandGesture.IsGestureValid(BKI_UIType.left) && rightHandGesture.IsGestureValid(BKI_UIType.right);
		}

		public bool IsGestureFilled()
		{
			if(ReferenceEquals(leftHandGesture, null) || ReferenceEquals(rightHandGesture, null) || leftHandGesture.gestureIdentifier == DEFAULT_GESTURE_NAME || rightHandGesture.gestureIdentifier == DEFAULT_GESTURE_NAME)
				return false;
			else
				return true;
		}

		public override int GetHashCode()
		{
			// keep this a prime.
			int num = 3;
			num = num * 23 + ((gestureIdentifier == null) ? 0 : gestureIdentifier.GetHashCode());
			num = num * 23 + ((ReferenceEquals(leftHandGesture, null) ? 0 : leftHandGesture.GetHashCode()));
			num = num * 23 + ((ReferenceEquals(rightHandGesture, null) ? 0 : rightHandGesture.GetHashCode()));
			return num;
		}

		public override bool Equals(object o)
		{
			if(o == null)
				return false;

			BKI_CombiGestureClass other = o as BKI_CombiGestureClass;

			if(other == null)
				return false;

			return this.GetHashCode() == other.GetHashCode();
		}

		public static bool operator ==(BKI_CombiGestureClass lhs, BKI_CombiGestureClass rhs)
		{
			if(ReferenceEquals(lhs, null) || ReferenceEquals(rhs, null))
				return false;
			return lhs.Equals(rhs);
		}

		public static bool operator !=(BKI_CombiGestureClass lhs, BKI_CombiGestureClass rhs)
		{
			return !(lhs == rhs);
		}
	}
}