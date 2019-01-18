using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;

namespace BetterKnucklesInteractions
{
	public class BKI_GestureManager : MonoBehaviour
	{
		[SerializeField]
		private BKI_GestureStorageClass gestureStorage;

		[SerializeField]
		private List<BKI_FunctionalGesture> combinationGestures, leftHandGestures, rightHandGestures;
		private BKI_FunctionalGesture activeCombinationGesture, activeLhGesture, activeRhGesture;
		// Gets pulled from assigned value defined in the gesture. The priorities in the lists in this class aren't to this value.
		private bool combiActive, leftActive, rightActive;
		private int combinationPriorityValue = -10, leftHandPriorityValue = -10, rightHandPriorityValue = -10;
		private BKI_FunctionalGesture previousCombinationGesture, previousLhGesture, previousRhGesture;

		private int currentHandValueLh, currentHandValueRh;

		private void Awake()
		{
			if(gestureStorage == null)
			{
				Debug.LogError("Gesture storage is either null or invalid. Disabling BKI_GestureManager");
				enabled = false;
			}
		}

		private void Update()
		{
			currentHandValueRh = BKI_FingerCurler.BKI_Curler_Instance.GetFingerPriorityValue(BKI_Hand.right);
			currentHandValueLh = BKI_FingerCurler.BKI_Curler_Instance.GetFingerPriorityValue(BKI_Hand.left);

			if(combiActive)
			{
				if(!IsGestureValid(BKI_UIType.combi, activeCombinationGesture.gestureId))
				{
					ExitCombiGesture();
				}
				else
				{
					// Current gesture update function as it is a valid registered gesture.
					activeCombinationGesture.OnGestureStay();
				}
			}
			else if(!combiActive || 
				BKI_FingerCurler.BKI_Curler_Instance.GetFingerPriorityValue(BKI_Hand.right) +
				BKI_FingerCurler.BKI_Curler_Instance.GetFingerPriorityValue(BKI_Hand.left) >
				combinationPriorityValue)
			{
				CheckCombinationGestures();
			}

			if(rightActive)
			{
				if(!IsGestureValid(BKI_UIType.right, activeRhGesture.gestureId))
				{
					ExitRhGesture();
				}
				else
				{
					// Current gesture update function as it is a valid registered gesture.
					activeRhGesture.OnGestureStay();
				}
			}
			if(!combiActive && (!rightActive || currentHandValueRh > rightHandPriorityValue))
			{ 
				CheckRightHandGestures();
			}
			int n = BKI_FingerCurler.BKI_Curler_Instance.GetFingerPriorityValue(BKI_Hand.right);
			bool b = n > rightHandPriorityValue;
			if(leftActive)
			{
				if(!IsGestureValid(BKI_UIType.left, activeLhGesture.gestureId))
				{
					ExitLhGesture();
				}
				else
				{
					// Current gesture update function as it is a valid registered gesture.
					activeLhGesture.OnGestureStay();
				}
			}
			else if(!combiActive && (!leftActive || BKI_FingerCurler.BKI_Curler_Instance.GetFingerPriorityValue(BKI_Hand.left) > leftHandPriorityValue))
			{ 
				CheckLeftHandGestures();
			}
		}

		private void OnValidate()
		{
			if(combinationGestures != null && combinationGestures.Count > 1)
				combinationGestures = combinationGestures.OrderByDescending(o => o.priority).ToList();
			if(leftHandGestures != null && leftHandGestures.Count > 1)
				leftHandGestures = leftHandGestures.OrderByDescending(o => o.priority).ToList();
			if(rightHandGestures != null && rightHandGestures.Count > 1)
				rightHandGestures = rightHandGestures.OrderByDescending(o => o.priority).ToList();
		}

		// Current gesture Start function as it gets registered.
		private void EnterLeftHandGesture(BKI_FunctionalGesture gesture)
		{
			leftHandPriorityValue = gestureStorage.GetGesturePriorityAtKey(BKI_UIType.left, gesture.gestureId);
			previousLhGesture = activeLhGesture;
			activeLhGesture = gesture;
			activeLhGesture.OnGestureEnter();
			leftActive = true;
		}

		// Current gesture exit function right before it gets deregistered.
		private void ExitLhGesture()
		{
			if(activeLhGesture != null)
				activeLhGesture.OnGestureExit();
			activeLhGesture = null;
			leftHandPriorityValue = -10;
			leftActive = false;
		}

		// Current gesture Start function as it gets registered.
		private void EnterRightHandGesture(BKI_FunctionalGesture gesture)
		{
			rightHandPriorityValue = gestureStorage.GetGesturePriorityAtKey(BKI_UIType.right, gesture.gestureId);
			previousRhGesture = activeRhGesture;
			activeRhGesture = gesture;
			activeRhGesture.OnGestureEnter();
			rightActive = true;
		}

		// Current gesture exit function right before it gets deregistered.
		private void ExitRhGesture()
		{
			if(activeRhGesture != null)
				activeRhGesture.OnGestureExit();
			activeRhGesture = null;
			rightHandPriorityValue = -10;
			rightActive = false;
		}

		// Current gesture Start function as it gets registered.
		private void EnterCombiGesture(BKI_FunctionalGesture gesture)
		{
			combinationPriorityValue = gestureStorage.GetGesturePriorityAtKey(BKI_UIType.combi, gesture.gestureId);
			previousCombinationGesture = activeCombinationGesture;
			activeCombinationGesture = gesture;
			activeCombinationGesture.OnGestureEnter();
			combiActive = true;
		}

		// Current gesture exit function right before it gets deregistered.
		private void ExitCombiGesture()
		{
			if(activeCombinationGesture != null)
				activeCombinationGesture.OnGestureExit();
			activeCombinationGesture = null;
			combinationPriorityValue = -10;
			combiActive = false;
		}

		// Compares the combination gesture list if there is any valid gesture and sets the current active gesture to the valid gesture.
		private void CheckCombinationGestures()
		{ 
			foreach(BKI_FunctionalGesture gesture in combinationGestures)
			{
				if(gesture == null || gesture == activeCombinationGesture)
					return;
				if(IsGestureValid(BKI_UIType.combi, gesture.gestureId))
				{
					if(activeCombinationGesture == null || activeCombinationGesture != gesture)
					{
						EnterCombiGesture(gesture);
					}
					return;
				}
				else if(activeCombinationGesture != gesture)
				{
					ExitCombiGesture();
				}
			}

		}

		// Compares the left hand gesture list if there is any valid gesture and sets the current active gesture to the valid gesture.
		private void CheckLeftHandGestures()
		{
			if(combiActive)
				return;
			foreach(BKI_FunctionalGesture gesture in leftHandGestures)
			{
				if(gesture == null || gesture == activeLhGesture)
					return;
				if(IsGestureValid(BKI_UIType.left, gesture.gestureId))
				{
					if(activeLhGesture == null || activeLhGesture != gesture)
					{
						EnterLeftHandGesture(gesture);
					}
					return;
				}
				else if(activeLhGesture != gesture)
				{
					ExitLhGesture();
				}
			}

		}

		// Compares the right hand gesture list if there is any valid gesture and sets the current active gesture to the valid gesture.
		private void CheckRightHandGestures()
		{
			if(combiActive)
				return;
			BKI_HandValues vals = BKI_FingerCurler.BKI_Curler_Instance.rightHandValues;
			foreach(BKI_FunctionalGesture gesture in rightHandGestures)
			{
				if(gesture == null || gesture == activeRhGesture)
					break;

				if(IsGestureValid(BKI_UIType.right, gesture.gestureId))
				{
					if(activeRhGesture == null || activeRhGesture != gesture)
					{
						EnterRightHandGesture(gesture);
					}
					break;
				}
				else if(activeRhGesture != gesture)
				{
					ExitRhGesture();
				}
			}
		}


		public bool IsGestureValid(BKI_UIType hand, string id)
		{
			return id == null || gestureStorage.IsGestureAtKeyValid(hand, id);
		}
	}

	[Serializable]
	public class BKI_FunctionalGesture
	{
		public string gestureId;
		public int priority;
		[SerializeField]
		private UnityEvent OnEnterEvent, OnStayEvent, OnExitEvent;
		public BKI_FunctionalGesture() { }

		public void OnGestureEnter()
		{
			if(OnEnterEvent != null)
				OnEnterEvent.Invoke();
		}

		public void OnGestureStay()
		{
			if(OnStayEvent != null)
				OnStayEvent.Invoke();
		}

		public void OnGestureExit()
		{
			if(OnExitEvent != null)
				OnExitEvent.Invoke();
		}
	}
}
