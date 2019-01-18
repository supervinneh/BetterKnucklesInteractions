using System;
using UnityEngine;
using Valve.VR;

namespace BetterKnucklesInteractions
{
	// Based off of https://github.com/MayBeTall/MayBeKnuckles and built further upon.
	// Reads a values between 0 and 1 based on the Z rotation of the proximal joint transforms on the hand controller mesh.
	[Serializable]
	public class BKI_HandValues
	{
		private BKI_SteamVR_Behaviour_Skeleton skeleton;
		[SerializeField]
		[Tooltip("Reads a value between 0 and 1 from the mesh rotations of the proximal joints.")]
		private float meshThumbZ, meshIndexZ, meshMiddleZ, meshRingZ, meshPinkyZ;
		private float thumbAng, indexAng, middleAng, ringAng, pinkyAng;
		private float indexMinDistance = 10000f, middleMinDistance = 10000f, ringMinDistance = 10000f, pinkyMinDistance = 10000f, thumbMinDistance = 10000f;
		private float indexMaxDistance = 0f, middleMaxDistance = 0f, ringMaxDistance = 0f, pinkyMaxDistance = 0f, thumbMaxDistance = 0f;

		[SerializeField]
		[Tooltip("Reads a value between 0 and 1 from the rotational values before the rotations of the joints get assigned.")]
		private float rawThumbZ, rawIndexZ, rawMiddleZ, rawRingZ, rawPinkyZ;

		[SerializeField]
		private bool isClenching;
		[SerializeField]
		private Vector2[] magicNumbers;
		[SerializeField]
		private float[] proximalRotations;

		// Ctor.
		public BKI_HandValues(BKI_SteamVR_Behaviour_Skeleton skel, Vector2[] magicNums)
		{
			skeleton = skel;
			magicNumbers = magicNums;
			proximalRotations = new float[5];
		}

		public void Update()
		{
			UpdateFingersMesh();
			UpdateFingersRaw();
			isClenching = SteamVR_Input._default.inActions.Squeeze.GetAxis(skeleton.inputSource) >= 0.75f;
		}

		private void UpdateFingersRaw()
		{
			for(int i = 0; i < 5; i++)
			{
				proximalRotations[i] = skeleton.BoneRotations[BKI_FingerCurler.proximalArrayIndexes[i]].z;
			}
			rawThumbZ = GetFingerValueMapped(BKI_Finger.thumb);
			rawIndexZ = GetFingerValueMapped(BKI_Finger.index);
			rawMiddleZ = GetFingerValueMapped(BKI_Finger.middle);
			rawRingZ = GetFingerValueMapped(BKI_Finger.ring);
			rawPinkyZ = GetFingerValueMapped(BKI_Finger.pinky);
		}

		// Updates the finger rotations based on the Z rotation of the proximal joint transforms on the hand controller mesh.
		private void UpdateFingersMesh()
		{
			thumbAng = skeleton.GetBoneRotation(SteamVR_Skeleton_JointIndexes.thumbProximal, true).eulerAngles.z;
			thumbMinDistance = Mathf.Min(thumbMinDistance, thumbAng);
			thumbMaxDistance = Mathf.Max(thumbMaxDistance, thumbAng);
			meshThumbZ = Mathf.InverseLerp(thumbMaxDistance, thumbMinDistance, thumbAng);

			indexAng = skeleton.GetBoneRotation(SteamVR_Skeleton_JointIndexes.indexProximal, true).eulerAngles.z;
			indexMinDistance = Mathf.Min(indexMinDistance, indexAng);
			indexMaxDistance = Mathf.Max(indexMaxDistance, indexAng);
			meshIndexZ = Mathf.InverseLerp(indexMaxDistance, indexMinDistance, indexAng);

			middleAng = skeleton.GetBoneRotation(SteamVR_Skeleton_JointIndexes.middleProximal, true).eulerAngles.z;
			middleMinDistance = Mathf.Min(middleMinDistance, middleAng);
			middleMaxDistance = Mathf.Max(middleMaxDistance, middleAng);
			meshMiddleZ = Mathf.InverseLerp(middleMaxDistance, middleMinDistance, middleAng);

			ringAng = skeleton.GetBoneRotation(SteamVR_Skeleton_JointIndexes.ringProximal, true).eulerAngles.z;
			ringMinDistance = Mathf.Min(ringMinDistance, ringAng);
			ringMaxDistance = Mathf.Max(ringMaxDistance, ringAng);
			meshRingZ = Mathf.InverseLerp(ringMaxDistance, ringMinDistance, ringAng);

			pinkyAng = skeleton.GetBoneRotation(SteamVR_Skeleton_JointIndexes.pinkyProximal, true).eulerAngles.z;
			pinkyMinDistance = Mathf.Min(pinkyMinDistance, pinkyAng);
			pinkyMaxDistance = Mathf.Max(pinkyMaxDistance, pinkyAng);
			meshPinkyZ = Mathf.InverseLerp(pinkyMaxDistance, pinkyMinDistance, pinkyAng);
		}

		// Returns the float value of the specified finger.
		public float GetFingerValue(BKI_Finger finger)
		{
			switch(finger)
			{
				case BKI_Finger.thumb:
					return meshThumbZ;
				case BKI_Finger.index:
					return meshIndexZ;
				case BKI_Finger.middle:
					return meshMiddleZ;
				case BKI_Finger.ring:
					return meshRingZ;
				case BKI_Finger.pinky:
					return meshPinkyZ;
				default:
					return 0;
			}
		}

		// Returns the float value of the specified finger.
		public float GetFingerValueRaw(BKI_Finger finger)
		{
			switch(finger)
			{
				case BKI_Finger.thumb:
					return rawThumbZ;
				case BKI_Finger.index:
					return rawIndexZ;
				case BKI_Finger.middle:
					return rawMiddleZ;
				case BKI_Finger.ring:
					return rawRingZ;
				case BKI_Finger.pinky:
					return rawPinkyZ;
				default:
					return 0;
			}
		}

		public bool GetIsClenching()
		{
			return isClenching;
		}

		// Debugs the hand float values.
		public void DebugHandFloats(string identifier = "")
		{
			Debug.Log(identifier +
				" thumb: " + meshThumbZ +
				" index: " + meshIndexZ +
				" middle: " + meshMiddleZ +
				" ring: " + meshRingZ +
				" pinky: " + meshPinkyZ);
		}

		// Returns a value between 0 and 1 based on the raw rotation values before they get assigned to the hand mesh. 
		public float GetFingerValueMapped(BKI_Finger finger)
		{
			return ClampedMap(proximalRotations[(int)finger], magicNumbers[(int)finger].x, magicNumbers[(int)finger].y, 0, 1);
		}

		// Maps magic numbers to a more readable 0 to 1 range.
		public float ClampedMap(float value, float fromSource, float toSource, float fromTarget, float toTarget)
		{
			return Mathf.Clamp01((value - fromSource) / (toSource - fromSource) * (toTarget - fromTarget) + fromTarget);
		}
		// Maps magic numbers to a more readable 0 to 1 range.
		public float Map(float value, float fromSource, float toSource, float fromTarget, float toTarget)
		{
			return (value - fromSource) / (toSource - fromSource) * (toTarget - fromTarget) + fromTarget;
		}
	}
}
