using UnityEngine;
using Valve.VR;
using System;
using System.Collections.Generic;

// Based off of https://github.com/MayBeTall/MayBeKnuckles and built further upon.
namespace BetterKnucklesInteractions
{
	public enum BKI_Finger { thumb = 0, index = 1, middle = 2, ring = 3, pinky = 4 }
	public enum BKI_Hand { right, left }
	public enum BKI_FreezeTypes { toProximal = 0, toTip = 1 }
	public enum BKI_UIType { combi, left, right }
	public enum BKI_FingerState { fingerOut = 0, fingerIn = 1, FingerIgnored = 2 }

	// The central script of the library. This stores the finger values of both hands and other relevant information.
	[Serializable]
	public class BKI_FingerCurler : MonoBehaviour
	{
		public const float DEFAULT_PRESSED_DEADZONE = 0.65f;

		public static BKI_FingerCurler BKI_Curler_Instance;
		public static int[] proximalArrayIndexes;
		[SerializeField]
		private BKI_FingerCollisionManager colliderManager;

		public List<string> freezeHandTags;

		[SerializeField]
		public BKI_HandValues leftHandValues, rightHandValues;

		[SerializeField]
		private BKI_SteamVR_Behaviour_Skeleton leftHandSkeleton, rightHandSkeleton;
		public BKI_SteamVR_Behaviour_Skeleton LeftHandSkeleton { get { return leftHandSkeleton; } }
		public BKI_SteamVR_Behaviour_Skeleton RightHandSkeleton { get { return rightHandSkeleton; } }

		private bool leftPreviousIsGripping, leftPreviousThumbDown, leftPreviousIndexDown, leftPreviousMiddleDown, leftPreviousRingDown, leftPreviousPinkyDown;
		private bool rightPreviousIsGripping, rightPreviousThumbDown, rightPreviousIndexDown, rightPreviousMiddleDown, rightPreviousRingDown, rightPreviousPinkyDown;

		private Vector2[] rhMagicNumbers, lhMagicNumbers;

		private BKI_PickupableObject rhGrippedObject, lhGrippedObject;

		private int priorityValueLh = 0, priorityValueRh = 0;

		// Lazy singleton init.
		private void Awake()
		{
			if(BKI_Curler_Instance == null)
				BKI_Curler_Instance = this;
			else
			{
				Debug.LogError("More than one BetterFingerCurler in scene. Disabling non-singleton BKI_FingerCurler instance on " + transform.name + ".");
				enabled = false;
			}

			if(freezeHandTags == null)
				freezeHandTags = new List<string>();

			SetupArrays();
			SetupMagicNumberArrays();
		}

		private void Start()
		{
			// Mandatory ValveVR setup.
			RightHandSkeleton.SetRangeOfMotion(EVRSkeletalMotionRange.WithoutController);
			LeftHandSkeleton.SetRangeOfMotion(EVRSkeletalMotionRange.WithoutController);

			// Sets up storage classes for both hands.
			leftHandValues = new BKI_HandValues(LeftHandSkeleton, lhMagicNumbers);
			rightHandValues = new BKI_HandValues(RightHandSkeleton, rhMagicNumbers);
		}

		private void Update()
		{
			rightHandValues.Update();
			leftHandValues.Update();

			SetFingerPriorities();
		}

		private void SetFingerPriorities()
		{
			int lhPrio = 0;
			int rhPrio = 0;

			for(int i = 0; i < 5; i++)
			{
				lhPrio += ((leftHandValues.GetFingerValue((BKI_Finger)i) > DEFAULT_PRESSED_DEADZONE) ? 1 : 0);
				rhPrio += ((rightHandValues.GetFingerValue((BKI_Finger)i) > DEFAULT_PRESSED_DEADZONE) ? 1 : 0);
			}

			lhPrio += (leftHandValues.GetIsClenching()) ? 1 : 0;
			rhPrio += (rightHandValues.GetIsClenching()) ? 1 : 0;

			priorityValueLh = lhPrio;
			priorityValueRh = rhPrio;
		}

		public int GetFingerPriorityValue(BKI_Hand hand)
		{
			return (hand == BKI_Hand.right) ? priorityValueRh : priorityValueLh;
		}

		// Sets the speed which the hand adjusts to finger values.
		public void SetHandUpdateBlend(BKI_Hand hand, bool isLowBlendPercentage)
		{
			switch(hand)
			{
				case BKI_Hand.right:
					rightHandSkeleton.SetHandUpdateBlend(isLowBlendPercentage);
					break;
				case BKI_Hand.left:
					leftHandSkeleton.SetHandUpdateBlend(isLowBlendPercentage);
					break;
			}
		}

		// Returns true or false depending on whether the read value exceeds the demanded deadzone.
		// Acts as a mirror of Unity.Input.GetButton() for the BetterKnuckleGestures library.
		public bool IsFingerDown(BKI_Hand hand, BKI_Finger finger)
		{
			BKI_HandValues handVals = null;
			switch(hand)
			{
				case BKI_Hand.right:
					handVals = rightHandValues;
					break;
				case BKI_Hand.left:
					handVals = leftHandValues;
					break;
			}
			switch(finger)
			{
				case BKI_Finger.thumb: return (handVals.GetFingerValue(finger) >= DEFAULT_PRESSED_DEADZONE);
				case BKI_Finger.index: return (handVals.GetFingerValue(finger) >= DEFAULT_PRESSED_DEADZONE);
				case BKI_Finger.middle: return (handVals.GetFingerValue(finger) >= DEFAULT_PRESSED_DEADZONE);
				case BKI_Finger.ring: return (handVals.GetFingerValue(finger) >= DEFAULT_PRESSED_DEADZONE);
				case BKI_Finger.pinky: return (handVals.GetFingerValue(finger) >= DEFAULT_PRESSED_DEADZONE);
				default: return false;
			}
		}

		// Returns a value between 0 and 1 based off of the rotational values of the proximal transform joints.
		// Acts as a mirror of Unity.Input.GetAxis() for the BetterKnuckleGestures library.
		public float GetFingerValueMesh(BKI_Hand hand, BKI_Finger finger)
		{
			BKI_HandValues handVals = null;
			switch(hand)
			{
				case BKI_Hand.right:
					handVals = rightHandValues;
					break;
				case BKI_Hand.left:
					handVals = leftHandValues;
					break;
			}
			switch(finger)
			{
				case BKI_Finger.thumb: return (handVals.GetFingerValue(finger));
				case BKI_Finger.index: return (handVals.GetFingerValue(finger));
				case BKI_Finger.middle: return (handVals.GetFingerValue(finger));
				case BKI_Finger.ring: return (handVals.GetFingerValue(finger));
				case BKI_Finger.pinky: return (handVals.GetFingerValue(finger));
				default: return 0;
			}
		}

		// Returns the rotational value read directly from the rotations before being applied to the mesh.
		public float GetFingerValueRaw(BKI_Hand hand, BKI_Finger finger)
		{
			BKI_HandValues handVals = null;
			switch(hand)
			{
				case BKI_Hand.right:
					handVals = rightHandValues;
					break;
				case BKI_Hand.left:
					handVals = leftHandValues;
					break;
			}
			switch(finger)
			{
				case BKI_Finger.thumb: return (handVals.GetFingerValueRaw(finger));
				case BKI_Finger.index: return (handVals.GetFingerValueRaw(finger));
				case BKI_Finger.middle: return (handVals.GetFingerValueRaw(finger));
				case BKI_Finger.ring: return (handVals.GetFingerValueRaw(finger));
				case BKI_Finger.pinky: return (handVals.GetFingerValueRaw(finger));
				default: return 0;
			}
		}

		// For centralised tag comparison purposes.
		public bool IsTagInTagList(string tag)
		{
			return freezeHandTags.Contains(tag);
		}

		// Returns current value.
		public bool IsHandGripping(BKI_Hand hand)
		{
			BKI_HandValues vals = (hand == BKI_Hand.left) ? leftHandValues : rightHandValues;
			BKI_SteamVR_Behaviour_Skeleton skel = (hand == BKI_Hand.left) ? leftHandSkeleton : rightHandSkeleton;

			bool canDoLoop = skel.IsFingerFrozen(BKI_Finger.thumb) && (skel.IsFingerFrozen(BKI_Finger.index) || skel.IsFingerFrozen(BKI_Finger.middle) || skel.IsFingerFrozen(BKI_Finger.ring) || skel.IsFingerFrozen(BKI_Finger.pinky));

			if(canDoLoop)
			{
				for(int i = 1; i < 5; i++)
				{
					if(!(skel.IsFingerFrozen((BKI_Finger)i) && !(vals.GetFingerValue((BKI_Finger)i) < 0.1f)))
						continue;
					if(
						(colliderManager.GetRegisteredPickupable(hand, BKI_Finger.thumb) != null && 
						(colliderManager.GetRegisteredPickupable(hand, (BKI_Finger)i)) != null) && 
						colliderManager.GetRegisteredPickupable(hand, BKI_Finger.thumb).GetPickupable() == colliderManager.GetRegisteredPickupable(hand, (BKI_Finger)i).GetPickupable())
					{
						if(hand == BKI_Hand.right)
							rhGrippedObject = colliderManager.GetRegisteredPickupable(hand, BKI_Finger.thumb);
						else
							lhGrippedObject = colliderManager.GetRegisteredPickupable(hand, BKI_Finger.thumb);
						return true;
					}
				}
			}
			else
			{
				if(hand == BKI_Hand.right)
					rhGrippedObject = null;
				else
					lhGrippedObject = null;
				return false;
			}
			return false;
		}

		public bool IsHandClenching(BKI_Hand hand)
		{
			return (hand == BKI_Hand.right) ? rightHandValues.GetIsClenching() : leftHandValues.GetIsClenching();
		}

		public BKI_PickupableObject GetGrippedObject(BKI_Hand hand)
		{
			return (hand == BKI_Hand.right) ? rhGrippedObject : lhGrippedObject;
		}

		// Only returns true on the frame grip is entered.
		public bool IsHandGrippingDown(BKI_Hand hand)
		{
			bool currentHandState = IsHandGripping(hand);
			bool prevGripping = (hand == BKI_Hand.left) ? leftPreviousIsGripping : rightPreviousIsGripping;

			bool returnVal = currentHandState && !prevGripping;

			if(hand == BKI_Hand.left)
				leftPreviousIsGripping = currentHandState;
			else
				rightPreviousIsGripping = currentHandState;

			return returnVal;
		}

		#region True finger rotation setup
		// Sets up the arrays.
		private void SetupArrays()
		{
			proximalArrayIndexes = new int[] {
				SteamVR_Skeleton_JointIndexes.thumbProximal,
				SteamVR_Skeleton_JointIndexes.indexProximal,
				SteamVR_Skeleton_JointIndexes.middleProximal,
				SteamVR_Skeleton_JointIndexes.ringProximal,
				SteamVR_Skeleton_JointIndexes.pinkyProximal };
		}

		// Min/max read out values of the proximal rotations. Determined by observing the values manually. (needs work)
		// TODO: Not magic numbers.
		private void SetupMagicNumberArrays()
		{
			lhMagicNumbers = new Vector2[5]
			{
				new Vector2(-0.270f, -0.070f),
				new Vector2(-0.180f, -0.726f),
				new Vector2(-0.153f, -0.755f),
				new Vector2(-0.186f, -0.695f),
				new Vector2(-0.092f, -0.666f)
			};
			rhMagicNumbers = new Vector2[5]
			{
				new Vector2(0.270f, 0.070f),
				new Vector2(-0.095f, -0.726f),
				new Vector2(-0.18f, -0.72f),
				new Vector2(-0.20f, -0.700f),
				new Vector2(-0.081f, -0.689f)
			};
		}
		#endregion
	}
}
