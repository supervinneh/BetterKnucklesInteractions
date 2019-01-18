using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

namespace BetterKnucklesInteractions
{
	// This is the steam controller script which you want to add to your in-scene controllers.
	public class BKI_SteamVR_Behaviour_Skeleton : SteamVR_Behaviour_Skeleton
	{
		public bool isHandFrozen = false;
		[SerializeField]
		private bool isThumbTipFrozen, isIndexTipFrozen, isMiddleTipFrozen, isRingTipFrozen, isPinkyTipFrozen;
		private bool isThumbProxFrozen, isIndexProxFrozen, isMiddleProxFrozen, isRingProxFrozen, isPinkyProxFrozen;

		private Vector3[] bonePositions;
		private Quaternion[] boneRotations;
		public Quaternion[] BoneRotations { get { return boneRotations; } }

		[SerializeField]
		private GameObject debugObj, collisionIndexPoint;

		private List<int> thumbRange, indexRange, middleRange, ringRange, pinkyRange;
		private int[] boneJointFrozenValues;
		private List<Collider> boneJointFrozenValuesRegisteredCols;

		[SteamVR_DefaultAction("Haptic")]
		public SteamVR_Action_Vibration hapticAction;
		[SerializeField]
		private float handUpdateBlend;
		[SerializeField]
		private float handUpdateBlendLow, handUpdateBlendHigh;

		protected override void Awake()
		{
			base.Awake();
			boneJointFrozenValues = new int[bones.Length];
			for(int i = 0; i < bones.Length; i++)
			{
				boneJointFrozenValues[i] = 0;
			}
			boneJointFrozenValuesRegisteredCols = new List<Collider>();
		}

		public void FreezeFingerInArray(BKI_Finger finger, BKI_FreezeTypes freezeType, Collider identifier)
		{
			// If the object that calls the freeze is already in the registry, return. We don't want duplicates to enter the array.
			if(boneJointFrozenValuesRegisteredCols.Contains(identifier))
				return;
			else
				boneJointFrozenValuesRegisteredCols.Add(identifier);

			switch(freezeType)
			{
				case BKI_FreezeTypes.toProximal:
					FreezeToProximal(finger);
					break;
				case BKI_FreezeTypes.toTip:
					FreezeToTip(finger);
					break;
				default:
					return;
			}
		}

		private void FreezeToTip(BKI_Finger finger)
		{
			int startNum;
			int exitNum;
			switch(finger)
			{
				case BKI_Finger.thumb:
					startNum = thumbRange[0];
					exitNum = startNum + thumbRange.Count;
					break;
				case BKI_Finger.index:
					startNum = indexRange[0];
					exitNum = startNum + indexRange.Count;
					break;
				case BKI_Finger.middle:
					startNum = middleRange[0];
					exitNum = startNum + middleRange.Count;
					break;
				case BKI_Finger.ring:
					startNum = ringRange[0];
					exitNum = startNum + ringRange.Count;
					break;
				case BKI_Finger.pinky:
					startNum = pinkyRange[0];
					exitNum = startNum + pinkyRange.Count;
					break;
				default:
					return;
			}
			FreezeFinger(finger, BKI_FreezeTypes.toTip);

			for(int i = startNum; i < exitNum; i++)
			{
				boneJointFrozenValues[i]++;
			}
		}

		private void FreezeToProximal(BKI_Finger finger)
		{
			int startNum;
			int exitNum;
			switch(finger)
			{
				case BKI_Finger.thumb:
					startNum = thumbRange[0];
					exitNum = startNum + 1;
					break;
				case BKI_Finger.index:
					startNum = indexRange[0];
					exitNum = startNum + 2;
					break;
				case BKI_Finger.middle:
					startNum = middleRange[0];
					exitNum = startNum + 2;
					break;
				case BKI_Finger.ring:
					startNum = ringRange[0];
					exitNum = startNum + 2;
					break;
				case BKI_Finger.pinky:
					startNum = pinkyRange[0];
					exitNum = startNum + 2;
					break;
				default:
					return;
			}
			FreezeFinger(finger, BKI_FreezeTypes.toProximal);

			for(int i = startNum; i < exitNum; i++)
			{
				boneJointFrozenValues[i]++;
			}
		}

		public  void UnfreezeFingerInArray(BKI_Finger finger, BKI_FreezeTypes freezeType, Collider identifier)
		{
			// If the object that calls the freeze is already in the registry, return. We don't want duplicates to enter the array.
			if(boneJointFrozenValuesRegisteredCols.Contains(identifier))
				boneJointFrozenValuesRegisteredCols.Remove(identifier);
			else
				return;

			switch(freezeType)
			{
				case BKI_FreezeTypes.toProximal:
					UnfreezeToProximal(finger);
					break;
				case BKI_FreezeTypes.toTip:
					UnfreezeToTip(finger);
					break;
				default:
					return;
			}
		}

		private void UnfreezeToTip(BKI_Finger finger)
		{
			int startNum;
			int exitNum;
			switch(finger)
			{
				case BKI_Finger.thumb:
					startNum = thumbRange[0];
					exitNum = startNum + thumbRange.Count;
					break;
				case BKI_Finger.index:
					startNum = indexRange[0];
					exitNum = startNum + indexRange.Count;
					break;
				case BKI_Finger.middle:
					startNum = middleRange[0];
					exitNum = startNum + middleRange.Count;
					break;
				case BKI_Finger.ring:
					startNum = ringRange[0];
					exitNum = startNum + ringRange.Count;
					break;
				case BKI_Finger.pinky:
					startNum = pinkyRange[0];
					exitNum = startNum + pinkyRange.Count;
					break;
				default:
					return;
			}
			UnfreezeFinger(finger, BKI_FreezeTypes.toTip);

			for(int i = startNum; i < exitNum; i++)
			{
				boneJointFrozenValues[i]--;
				if(boneJointFrozenValues[i] < 0)
					boneJointFrozenValues[i] = 0;
			}
		}

		private void UnfreezeToProximal(BKI_Finger finger)
		{
			int startNum;
			int exitNum;
			switch(finger)
			{
				case BKI_Finger.thumb:
					startNum = thumbRange[0];
					exitNum = startNum + 1;
					break;
				case BKI_Finger.index:
					startNum = indexRange[0];
					exitNum = startNum + 2;
					break;
				case BKI_Finger.middle:
					startNum = middleRange[0];
					exitNum = startNum + 2;
					break;
				case BKI_Finger.ring:
					startNum = ringRange[0];
					exitNum = startNum + 2;
					break;
				case BKI_Finger.pinky:
					startNum = pinkyRange[0];
					exitNum = startNum + 2;
					break;
				default:
					return;
			}
			UnfreezeFinger(finger, BKI_FreezeTypes.toProximal);

			for(int i = startNum; i < exitNum; i++)
			{
				boneJointFrozenValues[i]--;
				if(boneJointFrozenValues[i] < 0)
					boneJointFrozenValues[i] = 0;
			}
		}

		protected void Start()
		{
			// Metacarpal and Proximal for thumb are the same thing in knuckles controllers. To prevent array from overflowing the Metacarpal is excluded.
			thumbRange = new List<int> { /*SteamVR_Skeleton_JointIndexes.thumbMetacarpal,*/ SteamVR_Skeleton_JointIndexes.thumbProximal, SteamVR_Skeleton_JointIndexes.thumbMiddle, SteamVR_Skeleton_JointIndexes.thumbDistal, SteamVR_Skeleton_JointIndexes.thumbTip };
			indexRange = new List<int> { SteamVR_Skeleton_JointIndexes.indexMetacarpal, SteamVR_Skeleton_JointIndexes.indexProximal, SteamVR_Skeleton_JointIndexes.indexMiddle, SteamVR_Skeleton_JointIndexes.indexDistal, SteamVR_Skeleton_JointIndexes.indexTip };
			middleRange = new List<int> { SteamVR_Skeleton_JointIndexes.middleMetacarpal, SteamVR_Skeleton_JointIndexes.middleProximal, SteamVR_Skeleton_JointIndexes.middleMiddle, SteamVR_Skeleton_JointIndexes.middleDistal, SteamVR_Skeleton_JointIndexes.middleTip };
			ringRange = new List<int> { SteamVR_Skeleton_JointIndexes.ringMetacarpal, SteamVR_Skeleton_JointIndexes.ringProximal, SteamVR_Skeleton_JointIndexes.ringMiddle, SteamVR_Skeleton_JointIndexes.ringDistal, SteamVR_Skeleton_JointIndexes.ringTip };
			pinkyRange = new List<int> { SteamVR_Skeleton_JointIndexes.pinkyMetacarpal, SteamVR_Skeleton_JointIndexes.pinkyProximal, SteamVR_Skeleton_JointIndexes.pinkyMiddle, SteamVR_Skeleton_JointIndexes.pinkyDistal, SteamVR_Skeleton_JointIndexes.pinkyTip };

			bonePositions = GetBonePositions(inputSource);
			boneRotations = GetBoneRotations(inputSource);
		}
		protected override void UpdateSkeletonTransforms()
		{
			bonePositions = GetBonePositions(inputSource);
			boneRotations = GetBoneRotations(inputSource);
			for(int boneIndex = 0; boneIndex < bones.Length; boneIndex++)
			{
				if(bones[boneIndex] == null || boneJointFrozenValues[boneIndex] != 0)
					continue;

				SetBonePosition(boneIndex, Vector3.Lerp(bones[boneIndex].localPosition, bonePositions[boneIndex], skeletonBlend));
				SetBoneRotation(boneIndex, Quaternion.Lerp(bones[boneIndex].localRotation, boneRotations[boneIndex], skeletonBlend));
			}
		}

		// Updates the hand that mirrors the controller per finger. (if no fingers are frozen, it functions as the base controllers functions.)
		//protected override void UpdateSkeletonTransforms()
		//{
		//	bonePositions = GetBonePositions(inputSource);
		//	boneRotations = GetBoneRotations(inputSource);

		//	// Updates all five fingers seperately.
		//	SetFingerPositions(BKG_Finger.thumb);
		//	SetFingerPositions(BKG_Finger.index);
		//	SetFingerPositions(BKG_Finger.middle);
		//	SetFingerPositions(BKG_Finger.ring);
		//	SetFingerPositions(BKG_Finger.pinky);

		//	// Updates the wrist and root joint.
		//	for(int boneIndex = 0; boneIndex < 2; boneIndex++)
		//	{
		//		if(bones[boneIndex] == null)
		//			continue;

		//		SetBonePosition(boneIndex, Vector3.Lerp(bones[boneIndex].localPosition, bonePositions[boneIndex], skeletonBlend));
		//		SetBoneRotation(boneIndex, Quaternion.Lerp(bones[boneIndex].localRotation, boneRotations[boneIndex], skeletonBlend));
		//	}

		//	// Updates the excess bones in the palm of the hand.
		//	for(int boneIndex = 26; boneIndex < 31; boneIndex++)
		//	{
		//		if(bones[boneIndex] == null)
		//			continue;

		//		SetBonePosition(boneIndex, Vector3.Lerp(bones[boneIndex].localPosition, bonePositions[boneIndex], skeletonBlend));
		//		SetBoneRotation(boneIndex, Quaternion.Lerp(bones[boneIndex].localRotation, boneRotations[boneIndex], skeletonBlend));
		//	}
		//}

		//// Updates the individual finger given in its parameters.
		//private void SetFingerPositions(BKG_Finger finger)
		//{
		//	int startNum = 0;
		//	int exitNum = 0;
		//	float blendValue = 1;
		//	switch(finger)
		//	{
		//		case BKG_Finger.thumb:
		//			startNum = thumbRange[0];
		//			exitNum = startNum + thumbRange.Count;
		//			if(IsFingerFrozen(finger))
		//				blendValue = 0;
		//			break;
		//		case BKG_Finger.index:
		//			startNum = indexRange[0];
		//			exitNum = startNum + indexRange.Count;
		//			if(IsFingerFrozen(finger))
		//				blendValue = 0;
		//			break;
		//		case BKG_Finger.middle:
		//			startNum = middleRange[0];
		//			exitNum = startNum + middleRange.Count;
		//			if(IsFingerFrozen(finger))
		//				blendValue = 0;
		//			break;
		//		case BKG_Finger.ring:
		//			startNum = ringRange[0];
		//			exitNum = startNum + ringRange.Count;
		//			if(IsFingerFrozen(finger))
		//				blendValue = 0;
		//			break;
		//		case BKG_Finger.pinky:
		//			startNum = pinkyRange[0];
		//			exitNum = startNum + pinkyRange.Count;
		//			if(IsFingerFrozen(finger))
		//				blendValue = 0;
		//			break;
		//	}

		//	for(int boneIndex = startNum; boneIndex < exitNum; boneIndex++)
		//	{
		//		if(bones[boneIndex] == null)
		//			continue;

		//		SetBonePosition(boneIndex, Vector3.Lerp(bones[boneIndex].localPosition, bonePositions[boneIndex], blendValue));
		//		SetBoneRotation(boneIndex, Quaternion.Lerp(bones[boneIndex].localRotation, boneRotations[boneIndex], blendValue));
		//	}
		//}

		protected override void SetBonePosition(int boneIndex, Vector3 localPosition)
		{
			if(onlySetRotations == false) //ignore position sets if we're only setting rotations
				bones[boneIndex].localPosition = Vector3.Lerp(localPosition, bones[boneIndex].localPosition, handUpdateBlend);
		}

		public override void SetBoneRotation(int boneIndex, Quaternion localRotation)
		{
			bones[boneIndex].localRotation = Quaternion.Lerp(localRotation, bones[boneIndex].localRotation, handUpdateBlend);
		}

		// Simple callback that returns whether or not the finger is marked as frozen.
		public bool IsFingerFrozen(BKI_Finger finger)
		{
			switch(finger)
			{
				case BKI_Finger.thumb:
					return isThumbTipFrozen;
				case BKI_Finger.index:
					return isIndexTipFrozen;
				case BKI_Finger.middle:
					return isMiddleTipFrozen;
				case BKI_Finger.ring:
					return isRingTipFrozen;
				case BKI_Finger.pinky:
					return isPinkyTipFrozen;
				default:
					return false;
			}
		}

		// Flags the declared finger as frozen.
		public void FreezeFinger(BKI_Finger finger, BKI_FreezeTypes type)
		{
			switch(type)
			{
				case BKI_FreezeTypes.toProximal:
					switch(finger)
					{
						case BKI_Finger.thumb:
							isThumbProxFrozen = false;
							break;
						case BKI_Finger.index:
							isIndexProxFrozen = false;
							break;
						case BKI_Finger.middle:
							isMiddleProxFrozen = false;
							break;
						case BKI_Finger.ring:
							isRingProxFrozen = false;
							break;
						case BKI_Finger.pinky:
							isPinkyProxFrozen = false;
							break;
					}
					break;
				case BKI_FreezeTypes.toTip:
					switch(finger)
					{
						case BKI_Finger.thumb:
							isThumbTipFrozen = true;
							break;
						case BKI_Finger.index:
							isIndexTipFrozen = true;
							break;
						case BKI_Finger.middle:
							isMiddleTipFrozen = true;
							break;
						case BKI_Finger.ring:
							isRingTipFrozen = true;
							break;
						case BKI_Finger.pinky:
							isPinkyTipFrozen = true;
							break;
					}
					break;
			}
		}

		// Flags the declared finger as no longer frozen.
		public void UnfreezeFinger(BKI_Finger finger, BKI_FreezeTypes type)
		{
			switch(type)
			{
				case BKI_FreezeTypes.toProximal:
					switch(finger)
					{
						case BKI_Finger.thumb:
							isThumbProxFrozen = false;
							break;
						case BKI_Finger.index:
							isIndexProxFrozen = false;
							break;
						case BKI_Finger.middle:
							isMiddleProxFrozen = false;
							break;
						case BKI_Finger.ring:
							isRingProxFrozen = false;
							break;
						case BKI_Finger.pinky:
							isPinkyProxFrozen = false;
							break;
					}
					break;
				case BKI_FreezeTypes.toTip:
					switch(finger)
					{
						case BKI_Finger.thumb:
							isThumbTipFrozen = false;
							break;
						case BKI_Finger.index:
							isIndexTipFrozen = false;
							break;
						case BKI_Finger.middle:
							isMiddleTipFrozen = false;
							break;
						case BKI_Finger.ring:
							isRingTipFrozen = false;
							break;
						case BKI_Finger.pinky:
							isPinkyTipFrozen = false;
							break;
					}
					break;
			}
			
		}

		// Flags the hand as frozen.
		public void FreezeHand()
		{
			isHandFrozen = true;
			updatePose = false;
			TriggerHapticPulse(500, 0.2f);
		}

		// Flags the hand as no longer frozen.
		public void UnfreezeHand()
		{
			isHandFrozen = false;
			updatePose = true;
		}

		// handUpdateBlend is a variable that defines the speed along which the hand model updates according to the read values from the controller.
		// Higher values slow down the animation.
		public void SetHandUpdateBlend(bool isLowBlendValue)
		{
			float amt = (isLowBlendValue) ? handUpdateBlendLow : handUpdateBlendHigh;

			handUpdateBlend = Mathf.Clamp01(amt);
		}

		public void TriggerHapticPulse(ushort microSecondsDuration)
		{
			float seconds = (float)microSecondsDuration / 1000000f;
			if(hapticAction != null)
				hapticAction.Execute(0, seconds, 1f / seconds, 1, inputSource);
		}

		public void TriggerHapticPulse(ushort microSecondsDuration, float amplitude)
		{
			float seconds = (float)microSecondsDuration / 1000000f;
			if(hapticAction != null)
				hapticAction.Execute(0, seconds, 1f / seconds, amplitude, inputSource);
		}

		public void TriggerHapticPulse(float duration, float frequency, float amplitude)
		{
			if(hapticAction != null)
				hapticAction.Execute(0, duration, frequency, amplitude, inputSource);
		}
	}
}
