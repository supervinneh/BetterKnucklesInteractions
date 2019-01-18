using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BetterKnucklesInteractions
{
	[RequireComponent(typeof(Collider))]
	public class BKI_HandPalmCollider : MonoBehaviour
	{
		// These Actions are the callbacks for the collision events.
		private Action onColEnter, onColStay, onColExit;

		private BKI_Hand hand;

		// This list acts as a buffer system for preventing premature unfreezing of a finger when the object collided with multiple objects.
		private List<Collider> collidedObjects;

		// Initialises the object to get parented to the designated finger on the designated hand.
		// Sets the callbacks for when the object enters, stays or exits collision. 
		public void Initialise(Action onColEnterCallback, Action onColStayCallback, Action onColExitCallback, BKI_Hand h)
		{
			collidedObjects = new List<Collider>();
			onColEnter = onColEnterCallback;
			onColStay = onColStayCallback;
			onColExit = onColExitCallback;

			hand = h;

			// Makes sure the collider is set to isTrigger.
			GetComponent<Collider>().isTrigger = true;
		}

		public void OnTriggerEnter(Collider other)
		{
			if(other.tag == "freezeSurface")
			{
				collidedObjects.Add(other);
				onColEnter();
			}
			if(collidedObjects.Count > 0)
				BKI_FingerCurler.BKI_Curler_Instance.SetHandUpdateBlend(hand, false);
			else
				BKI_FingerCurler.BKI_Curler_Instance.SetHandUpdateBlend(hand, true);
		}

		public void OnTriggerStay(Collider other)
		{
			if(other.tag == "freezeSurface")
			{
				if(!collidedObjects.Contains(other))
					collidedObjects.Add(other);
				onColStay();
			}

			if(collidedObjects.Count > 0)
				BKI_FingerCurler.BKI_Curler_Instance.SetHandUpdateBlend(hand, false);
			else
				BKI_FingerCurler.BKI_Curler_Instance.SetHandUpdateBlend(hand, true);
		}

		private void OnTriggerExit(Collider other)
		{
			if(other.tag == "freezeSurface")
			{
				if(collidedObjects.Contains(other))
					collidedObjects.Remove(other);
				else
					return;

				if(collidedObjects.Count > 0)
					BKI_FingerCurler.BKI_Curler_Instance.SetHandUpdateBlend(hand, false);
				else
					BKI_FingerCurler.BKI_Curler_Instance.SetHandUpdateBlend(hand, true);

				onColExit();
			}
		}
	}
}