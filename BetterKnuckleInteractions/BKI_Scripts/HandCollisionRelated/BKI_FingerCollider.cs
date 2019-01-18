using System;
using System.Collections.Generic;
using UnityEngine;

namespace BetterKnucklesInteractions
{
	[RequireComponent(typeof(Collider))]
	public class BKI_FingerCollider : MonoBehaviour
	{
		private BKI_FingerCollisionManager manager;
		public BKI_SteamVR_Behaviour_Skeleton hand;

		// This list acts as a buffer system for preventing premature unfreezing of a finger when the object collided with multiple objects.
		private List<GameObject> collidedObjects;

		// These Actions are the callbacks for the collision events.
		private Action<Collider> onColEnter, onColStay, onColExit, registerCallback, unregisterCallback;

		// Initialises the object to get parented to the designated finger on the designated hand.
		// Sets the callbacks for when the object enters, stays or exits collision. 
		public void Initialise(BKI_FingerCollisionManager m, Transform parentOb, BKI_SteamVR_Behaviour_Skeleton h, Action<Collider> onColEnterCallback, Action<Collider> onColStayCallback, Action<Collider> onColExitCallback, float offset, Action<Collider> reg, Action<Collider> unreg)
		{
			manager = m;
			collidedObjects = new List<GameObject>();
			transform.parent = parentOb;
			transform.localPosition = new Vector3(0, offset, 0);
			hand = h;
			onColEnter = onColEnterCallback;
			onColStay = onColStayCallback;
			onColExit = onColExitCallback;
			registerCallback = reg;
			unregisterCallback = unreg;

			// Makes sure the collider is set to isTrigger.
			GetComponent<Collider>().isTrigger = true;
		}

		public void OnTriggerEnter(Collider other)
		{
			if(other.tag == "freezeSurface" || other.tag == "pickupableObject")
			{
				GameObject go = other.gameObject;
				collidedObjects.Add(go);
				onColEnter(GetComponent<Collider>());
				registerCallback(other);
			}
		}

		public void OnTriggerStay(Collider other)
		{
			if(other.tag == "freezeSurface" || other.tag == "pickupableObject")
			{
				GameObject go = other.gameObject;
				if(!collidedObjects.Contains(go))
					collidedObjects.Add(go);
				onColStay(GetComponent<Collider>());
				registerCallback(other);
			}
		}

		private void OnTriggerExit(Collider other)
		{
			if(other.tag == "freezeSurface" || other.tag == "pickupableObject")
			{
				GameObject go = other.gameObject;
				if(collidedObjects.Contains(go))
					collidedObjects.Remove(go);
				else
					return;
				onColExit(GetComponent<Collider>());
				unregisterCallback(other);
			}
		}
	}
}
