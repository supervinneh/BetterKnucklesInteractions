using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BetterKnucklesInteractions {
	// Don't make an instance of this class. It won't be functional.
	[Serializable]
	public class BKI_GestureClass : ScriptableObject
	{
		public const string DEFAULT_GESTURE_NAME = "Name";
		public string gestureIdentifier = "Name";
		// Validation function that compares values from the controllers to the preset values.
		public virtual bool IsGestureValid(BKI_UIType t) { return false; }
		// Assigned priority is determined by the finger values. The higher the required input is, the higher the assigned priority will be.
		// A fully ignored hand will return a priority of -5. A fully contracted hand (fist) with the clench boolean will become a 6.
		protected int assignedGesturePriority;
		public int GetGesturePriority() { return assignedGesturePriority; }
	}
}