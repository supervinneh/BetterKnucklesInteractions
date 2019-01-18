using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

namespace BetterKnucklesInteractions
{
	public class BKI_FingerCollisionManager : MonoBehaviour
	{
		[SerializeField]
		[Tooltip("Should be a small collider without a mesh renderer for best results.")]
		private BKI_FingerCollider prefab;

		private BKI_SteamVR_Behaviour_Skeleton leftHandSkeleton, rightHandSkeleton;

		// Finger collider/positional values.
		[SerializeField]
		private List<BKI_FingerCollider> tipTriggersLeftHand, tipTriggersRightHand;
		private List<BKI_FingerCollider> proxTriggersLeftHand, proxTriggersRightHand;
		private List<BKI_FingerCollider> collidersLeftHand, collidersRightHand;
		private BKI_PickupableObject[] registeredPickupableRight, registeredPickupableLeft;
		private bool[] fingerFrozenLeftHand, fingerFrozenRightHand;
		private float[] fingerFrozenValueLeftHand, fingerFrozenValueRightHand;

		// Palm collider/positional values.
		[SerializeField]
		private BKI_HandPalmCollider leftHandPalmCollider, rightHandPalmCollider;
		private bool isLeftHandFrozen, isRightHandFrozen;

		private void Start()
		{
			leftHandSkeleton = BKI_FingerCurler.BKI_Curler_Instance.LeftHandSkeleton;
			rightHandSkeleton = BKI_FingerCurler.BKI_Curler_Instance.RightHandSkeleton;

			if(leftHandSkeleton == null || rightHandSkeleton == null)
			{
				Debug.LogError("Controllers unassigned. Quitting initialisation.");
				return;
			}

			fingerFrozenLeftHand = new bool[5];
			fingerFrozenRightHand = new bool[5];
			fingerFrozenValueLeftHand = new float[5];
			fingerFrozenValueRightHand = new float[5];

			SetupFingerTriggers();
			SetupPalmColliders();
			registeredPickupableRight = new BKI_PickupableObject[5];
			registeredPickupableLeft = new BKI_PickupableObject[5];
		}

		private void SetupPalmColliders()
		{
			System.Action onEnterCallbackLh = () => FreezeHand(BKI_Hand.left);
			System.Action onStayCallbackLh = () => RefreezeHand(BKI_Hand.left);
			System.Action onExitCallbackLh = () => UnfreezeHand(BKI_Hand.left);
			leftHandPalmCollider.Initialise(onEnterCallbackLh, onStayCallbackLh, onExitCallbackLh, BKI_Hand.left);

			System.Action onEnterCallbackRh = () => FreezeHand(BKI_Hand.right);
			System.Action onStayCallbackRh = () => RefreezeHand(BKI_Hand.right);
			System.Action onExitCallbackRh = () => UnfreezeHand(BKI_Hand.right);
			rightHandPalmCollider.Initialise(onEnterCallbackRh, onStayCallbackRh, onExitCallbackRh, BKI_Hand.right);
		}

		public bool IsValueOverFrozenValue(BKI_Hand h, BKI_Finger f)
		{
			switch(h)
			{
				case BKI_Hand.right:
					return fingerFrozenValueRightHand[(int)f] > BKI_FingerCurler.BKI_Curler_Instance.GetFingerValueRaw(BKI_Hand.right, f) || 
						BKI_FingerCurler.BKI_Curler_Instance.GetFingerValueRaw(BKI_Hand.right, f) < 0.1f;
				case BKI_Hand.left:
					return fingerFrozenValueLeftHand[(int)f] > BKI_FingerCurler.BKI_Curler_Instance.GetFingerValueRaw(BKI_Hand.left, f) ||
						BKI_FingerCurler.BKI_Curler_Instance.GetFingerValueRaw(BKI_Hand.left, f) < 0.1f;
				default:
					return false;
			}
		}

		// Instantiating and assigning the colliders of all 10 fingers.
		private void SetupFingerTriggers()
		{
			if(prefab == null)
			{
				Debug.LogError("Collider prefab is invalid. Quitting initialisation.");
				return;
			}

			// Left hand tip collider setup code.
			tipTriggersLeftHand = new List<BKI_FingerCollider>();
			for(int i = 0; i < 5; i++)
			{
				BKI_FingerCollider bcf = Instantiate(prefab.gameObject).GetComponent<BKI_FingerCollider>();
				Transform parentObj = null;
				BKI_Finger finger = (BKI_Finger)i;

				switch(finger)
				{
					case BKI_Finger.thumb:
						parentObj = leftHandSkeleton.thumbTip;
						break;
					case BKI_Finger.index:
						parentObj = leftHandSkeleton.indexTip;
						break;
					case BKI_Finger.middle:
						parentObj = leftHandSkeleton.middleTip;
						break;
					case BKI_Finger.ring:
						parentObj = leftHandSkeleton.ringTip;
						break;
					case BKI_Finger.pinky:
						parentObj = leftHandSkeleton.pinkyTip;
						break;
					default:
						Debug.LogError("Parent transform is invalid. Quitting initialisation.");
						return;
				}

				System.Action<Collider> onEnterCallback = (c) => FreezeFinger(BKI_Hand.left, finger, BKI_FreezeTypes.toTip, c);
				System.Action<Collider> onStayCallback = (c) => RefreezeFinger(BKI_Hand.left, finger, BKI_FreezeTypes.toTip, c);
				System.Action<Collider> onExitCallback = (c) => UnfreezeFinger(BKI_Hand.left, finger, BKI_FreezeTypes.toTip, c);
				System.Action<Collider> registerCall = (c) => RegisterPickupable(c, BKI_Hand.left, finger, BKI_FreezeTypes.toTip);
				System.Action<Collider> unregisterCall = (c) => UnregisterPickupable(c, BKI_Hand.left, finger, BKI_FreezeTypes.toTip);

				bcf.Initialise(this, parentObj, leftHandSkeleton, onEnterCallback, onStayCallback, onExitCallback, -0.005f, registerCall, unregisterCall);
				tipTriggersLeftHand.Add(bcf);
			}

			// Right hand tip collider setup code.
			tipTriggersRightHand = new List<BKI_FingerCollider>();
			for(int i = 0; i < 5; i++)
			{
				BKI_FingerCollider bcf = Instantiate(prefab.gameObject).GetComponent<BKI_FingerCollider>();
				Transform parentObj = null;
				BKI_Finger finger = (BKI_Finger)i;

				switch(finger)
				{
					case BKI_Finger.thumb:
						parentObj = rightHandSkeleton.thumbTip;
						break;
					case BKI_Finger.index:
						parentObj = rightHandSkeleton.indexTip;
						break;
					case BKI_Finger.middle:
						parentObj = rightHandSkeleton.middleTip;
						break;
					case BKI_Finger.ring:
						parentObj = rightHandSkeleton.ringTip;
						break;
					case BKI_Finger.pinky:
						parentObj = rightHandSkeleton.pinkyTip;
						break;
					default:
						Debug.LogError("Parent transform is invalid. Quitting initialisation.");
						return;
				}


				System.Action<Collider> onEnterCallback = (c) => FreezeFinger(BKI_Hand.right, finger, BKI_FreezeTypes.toTip, c);
				System.Action<Collider> onStayCallback = (c) => RefreezeFinger(BKI_Hand.right, finger, BKI_FreezeTypes.toTip, c);
				System.Action<Collider> onExitCallback = (c) => UnfreezeFinger(BKI_Hand.right, finger, BKI_FreezeTypes.toTip, c);
				System.Action<Collider> registerCall = (c) => RegisterPickupable(c, BKI_Hand.right, finger, BKI_FreezeTypes.toTip);
				System.Action<Collider> unregisterCall = (c) => UnregisterPickupable(c, BKI_Hand.right, finger, BKI_FreezeTypes.toTip);

				bcf.Initialise(this, parentObj, rightHandSkeleton, onEnterCallback, onStayCallback, onExitCallback, -0.005f, registerCall, unregisterCall);
				tipTriggersRightHand.Add(bcf);
			}

			// Left hand proximal trigger setup code.
			proxTriggersLeftHand = new List<BKI_FingerCollider>();
			for(int i = 0; i < 5; i++)
			{
				BKI_FingerCollider bcf = Instantiate(prefab.gameObject).GetComponent<BKI_FingerCollider>();
				Transform parentObj = null;
				BKI_Finger finger = (BKI_Finger)i;

				switch(finger)
				{
					case BKI_Finger.thumb:
						parentObj = leftHandSkeleton.thumbMiddle;
						break;
					case BKI_Finger.index:
						parentObj = leftHandSkeleton.indexMiddle;
						break;
					case BKI_Finger.middle:
						parentObj = leftHandSkeleton.middleMiddle;
						break;
					case BKI_Finger.ring:
						parentObj = leftHandSkeleton.ringMiddle;
						break;
					case BKI_Finger.pinky:
						parentObj = leftHandSkeleton.pinkyMiddle;
						break;
					default:
						Debug.LogError("Parent transform is invalid. Quitting initialisation.");
						return;
				}

				System.Action<Collider> onEnterCallback = (c) => FreezeFinger(BKI_Hand.left, finger, BKI_FreezeTypes.toProximal, c);
				System.Action<Collider> onStayCallback = (c) => RefreezeFinger(BKI_Hand.left, finger, BKI_FreezeTypes.toProximal, c);
				System.Action<Collider> onExitCallback = (c) => UnfreezeFinger(BKI_Hand.left, finger, BKI_FreezeTypes.toProximal, c);
				System.Action<Collider> registerCall = (c) => RegisterPickupable(c, BKI_Hand.left, finger, BKI_FreezeTypes.toProximal);
				System.Action<Collider> unregisterCall = (c) => UnregisterPickupable(c, BKI_Hand.left, finger, BKI_FreezeTypes.toProximal);

				bcf.Initialise(this, parentObj, leftHandSkeleton, onEnterCallback, onStayCallback, onExitCallback, -0.015f, registerCall, unregisterCall);
				proxTriggersLeftHand.Add(bcf);
			}

			// Right hand proximal trigger setup code.
			proxTriggersRightHand = new List<BKI_FingerCollider>();
			for(int i = 0; i < 5; i++)
			{
				BKI_FingerCollider bcf = Instantiate(prefab.gameObject).GetComponent<BKI_FingerCollider>();
				Transform parentObj = null;
				BKI_Finger finger = (BKI_Finger)i;

				switch(finger)
				{
					case BKI_Finger.thumb:
						parentObj = rightHandSkeleton.thumbMiddle;
						break;
					case BKI_Finger.index:
						parentObj = rightHandSkeleton.indexMiddle;
						break;
					case BKI_Finger.middle:
						parentObj = rightHandSkeleton.middleMiddle;
						break;
					case BKI_Finger.ring:
						parentObj = rightHandSkeleton.ringMiddle;
						break;
					case BKI_Finger.pinky:
						parentObj = rightHandSkeleton.pinkyMiddle;
						break;
					default:
						Debug.LogError("Parent transform is invalid. Quitting initialisation.");
						return;
				}

				System.Action<Collider> onEnterCallback = (c) => FreezeFinger(BKI_Hand.right, finger, BKI_FreezeTypes.toProximal, c);
				System.Action<Collider> onStayCallback = (c) => RefreezeFinger(BKI_Hand.right, finger, BKI_FreezeTypes.toProximal, c);
				System.Action<Collider> onExitCallback = (c) => UnfreezeFinger(BKI_Hand.right, finger, BKI_FreezeTypes.toProximal, c);
				System.Action<Collider> registerCall = (c) => RegisterPickupable(c, BKI_Hand.right, finger, BKI_FreezeTypes.toProximal);
				System.Action<Collider> unregisterCall = (c) => UnregisterPickupable(c, BKI_Hand.right, finger, BKI_FreezeTypes.toProximal);

				bcf.Initialise(this, parentObj, rightHandSkeleton, onEnterCallback, onStayCallback, onExitCallback, -0.015f, registerCall, unregisterCall);
				proxTriggersRightHand.Add(bcf);
			}
		}

		private void FreezeFinger(BKI_Hand hand, BKI_Finger finger, BKI_FreezeTypes freezeType, Collider c)
		{
			
			switch(hand)
			{
				case BKI_Hand.right:
					if(BKI_FingerCurler.BKI_Curler_Instance.GetFingerValueRaw(BKI_Hand.right, finger) <= 0.05f)
						break;
					fingerFrozenRightHand[(int)finger] = true;
					fingerFrozenValueRightHand[(int)finger] = Mathf.Clamp(BKI_FingerCurler.BKI_Curler_Instance.GetFingerValueRaw(BKI_Hand.right, finger) - 0.1f, 0.1f, 1f);
					rightHandSkeleton.FreezeFingerInArray(finger, freezeType, c);
					break;
				case BKI_Hand.left:
					if(BKI_FingerCurler.BKI_Curler_Instance.GetFingerValueRaw(BKI_Hand.left, finger) <= 0.05f)
						break;
					fingerFrozenLeftHand[(int)finger] = true;
					fingerFrozenValueLeftHand[(int)finger] = Mathf.Clamp(BKI_FingerCurler.BKI_Curler_Instance.GetFingerValueRaw(BKI_Hand.left, finger) - 0.1f, 0.1f, 1f);
					leftHandSkeleton.FreezeFingerInArray(finger, freezeType, c);
					break;
			}
			RegisterPickupable(c, hand, finger, freezeType);
		}

		private void RefreezeFinger(BKI_Hand hand, BKI_Finger finger, BKI_FreezeTypes freezeType, Collider c)
		{
			if(IsValueOverFrozenValue(hand, finger))
				UnfreezeFinger(hand, finger, freezeType, c);
			else
				FreezeFinger(hand, finger, freezeType, c);
		}

		private void UnfreezeFinger(BKI_Hand hand, BKI_Finger finger, BKI_FreezeTypes freezeType, Collider c)
		{
			switch(hand)
			{
				case BKI_Hand.right:
					fingerFrozenRightHand[(int)finger] = false;
					fingerFrozenValueRightHand[(int)finger] = 0;
					rightHandSkeleton.UnfreezeFingerInArray(finger, freezeType, c);
					break;
				case BKI_Hand.left:
					fingerFrozenLeftHand[(int)finger] = false;
					fingerFrozenValueLeftHand[(int)finger] = 0;
					leftHandSkeleton.UnfreezeFingerInArray(finger, freezeType, c);
					break;
			}
			UnregisterPickupable(c, hand, finger, freezeType);
		}

		private void FreezeHand(BKI_Hand hand)
		{
			switch(hand)
			{
				case BKI_Hand.right:
					rightHandSkeleton.FreezeHand();
					isRightHandFrozen = true;
					break;
				case BKI_Hand.left:
					leftHandSkeleton.FreezeHand();
					isLeftHandFrozen = true;
					break;
			}
		}

		private void RefreezeHand(BKI_Hand hand)
		{
			FreezeHand(hand);
		}

		private void UnfreezeHand(BKI_Hand hand)
		{
			switch(hand)
			{
				case BKI_Hand.right:
					rightHandSkeleton.UnfreezeHand();
					isRightHandFrozen = false;
					break;
				case BKI_Hand.left:
					leftHandSkeleton.UnfreezeHand();
					isLeftHandFrozen = false;
					break;
			}
		}

		public void RegisterPickupable(Collider c, BKI_Hand hand, BKI_Finger finger, BKI_FreezeTypes freezeType)
		{
			if(c.gameObject == null)
				return;

			switch(hand)
			{
				case BKI_Hand.right:
					if(freezeType == BKI_FreezeTypes.toProximal && registeredPickupableRight[(int)finger] != null)
						return;
					if(c.GetComponent<IBetterPickupable>() != null && registeredPickupableRight[(int)finger] == null)
						registeredPickupableRight[(int)finger] = c.GetComponent<IBetterPickupable>().GetPickupable();
					break;
				case BKI_Hand.left:
					if(freezeType == BKI_FreezeTypes.toProximal && registeredPickupableLeft[(int)finger] != null)
						return;
					if(c.GetComponent<IBetterPickupable>() != null && registeredPickupableLeft[(int)finger] == null)
						registeredPickupableLeft[(int)finger] = c.GetComponent<IBetterPickupable>().GetPickupable();
					break;
			}
		}

		public void UnregisterPickupable(Collider c, BKI_Hand hand, BKI_Finger finger, BKI_FreezeTypes freezeType)
		{
			if(c.GetComponent<IBetterPickupable>() == null)
				return;
			switch(hand)
			{
				case BKI_Hand.right:
					registeredPickupableRight[(int)finger] = null;
					break;
				case BKI_Hand.left:
					registeredPickupableLeft[(int)finger] = null;
					break;
			}
		}

		public BKI_PickupableObject GetRegisteredPickupable(BKI_Hand hand, BKI_Finger finger)
		{
			if(hand == BKI_Hand.right)
				return (IsHandGripping(BKI_Hand.right)) ? registeredPickupableRight[(int)finger] : null;
			else
				return (IsHandGripping(BKI_Hand.left)) ? registeredPickupableLeft[(int)finger] : null;
		}

		private bool IsHandGripping(BKI_Hand hand)
		{
			if(hand == BKI_Hand.left)
			{
				for(int i = 1; i < 5; i++)
				{
					if(registeredPickupableLeft[i] == registeredPickupableLeft[0])
						return true;
				}
			}
			else
			{
				for(int i = 1; i < 5; i++)
				{
					if(registeredPickupableRight[i] == registeredPickupableRight[0])
						return true;
				}
			}
			return false;
		}
	}
}
