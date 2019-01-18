using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System;

namespace BetterKnucklesInteractions
{
	public class BKI_GestureMirrorClass
	{
		public string gestureIdentifier = BKI_GestureCreator.DEFAULT_GESTURE_NAME;
		public BKI_FingerState[] fingerStates;
		public bool requiresClench = false;

		// Default ctor
		public BKI_GestureMirrorClass()
		{
			fingerStates = new BKI_FingerState[5];
		}

		public void SetFingerState(BKI_Finger f, BKI_FingerState v)
		{
			fingerStates[(int)f] = v;
		}

		public BKI_FingerState GetFingerState(BKI_Finger f)
		{
			return fingerStates[(int)f];
		}

		public void SetValues(string id, bool reqClench, BKI_FingerState[] states)
		{
			gestureIdentifier = id;
			requiresClench = reqClench;
			fingerStates = states;
		}

		public override bool Equals(object obj)
		{
			if(obj == null)
				return false;

			BKI_GestureMirrorClass other = obj as BKI_GestureMirrorClass;

			if(other == null)
				return false;

			int dupeFingers = 0;

			for(int i = 0; i < 5; i++)
			{
				if(this.fingerStates[i] == other.fingerStates[i])
					dupeFingers++;
			}
			return (dupeFingers >= 5 && (this.requiresClench && other.requiresClench));
		}

		public static bool operator ==(BKI_GestureMirrorClass lhs, BKI_GestureMirrorClass rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(BKI_GestureMirrorClass lhs, BKI_GestureMirrorClass rhs)
		{
			return !(lhs == rhs);
		}
	}
}