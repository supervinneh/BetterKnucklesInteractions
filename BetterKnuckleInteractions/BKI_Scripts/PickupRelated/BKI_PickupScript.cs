using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

namespace BetterKnucklesInteractions
{
	[RequireComponent(typeof(Collider))]
	public class BKI_PickupScript : MonoBehaviour
	{
		[SerializeField]
		private BKI_Hand hand;
		[SerializeField]
		private Transform pickedUpParent;
		[SerializeField]
		private bool isHandGripping, isHoldingObject;
		[SerializeField]
		private int handIndex = 0;
		private BKI_PickupableObject pickedUpObject;

		private BKI_PickupableObject collidedObject;

		private List<IBetterPickupable> registeredColliders;

		private void Start()
		{
			// Makes sure the attached collider is a trigger.
			registeredColliders = new List<IBetterPickupable>();
			GetComponent<Collider>().isTrigger = true;
			isHandGripping = BKI_FingerCurler.BKI_Curler_Instance.IsHandGripping(hand);
			isHoldingObject = false;
		}

		private void Update()
		{
			isHandGripping = BKI_FingerCurler.BKI_Curler_Instance.IsHandGripping(hand);
		}

		private void OnTriggerEnter(Collider other)
		{
			BKI_PickupableObject bpo = GetValidPickupable(other.gameObject);
			if(bpo == null)
				return;
			if(bpo != null && collidedObject == null)
			{
				if(!registeredColliders.Contains(bpo))
					registeredColliders.Add(bpo);
			}
			
			if(registeredColliders.Count > 0)
				BKI_FingerCurler.BKI_Curler_Instance.SetHandUpdateBlend(hand, false);
			else
				BKI_FingerCurler.BKI_Curler_Instance.SetHandUpdateBlend(hand, true);
		}

		private void OnTriggerStay(Collider other)
		{
			BKI_PickupableObject bpo = GetValidPickupable(other.gameObject);
			if(!isHandGripping && pickedUpObject != null)
			{
				ReleaseObject();
			}

			if(bpo != null)
			{
				bpo = bpo.GetPickupable();
				if(!registeredColliders.Contains(bpo))
					registeredColliders.Add(bpo);
			}

			// Functions as another "TriggerEnter" moment.
			if(pickedUpObject == null && registeredColliders.Count > 0)
			{
				foreach(IBetterPickupable b in registeredColliders)
				{
					if(BKI_FingerCurler.BKI_Curler_Instance.IsHandGrippingDown(hand))
						return;

					BKI_PickupableObject bp = null;

					if(b != null)
						bp = b.GetPickupable();
					else
						continue;

					if(bp == null)
						continue;

					if(BKI_FingerCurler.BKI_Curler_Instance.GetGrippedObject(hand) != null && BKI_FingerCurler.BKI_Curler_Instance.GetGrippedObject(hand).GetPickupable() == bp.GetPickupable())
					{
						pickedUpObject = BKI_FingerCurler.BKI_Curler_Instance.GetGrippedObject(hand);
						pickedUpObject.PickupObject(pickedUpParent);
						isHoldingObject = true;
						break;
					}

				}
			}

			if(registeredColliders.Count > 0)
				BKI_FingerCurler.BKI_Curler_Instance.SetHandUpdateBlend(hand, false);
			else
				BKI_FingerCurler.BKI_Curler_Instance.SetHandUpdateBlend(hand, true);

		}

		private void OnTriggerExit(Collider other)
		{
			if(other.tag == "pickupableObject")
			{
				BKI_PickupableObject bpo = GetValidPickupable(other.gameObject);
				if(bpo == null)
					return;

				if(registeredColliders.Contains(bpo))
				{
					registeredColliders.Remove(bpo);
					if(bpo.GetComponent<Valve.VR.InteractionSystem.Interactable>() != null)
						bpo.GetComponent<Valve.VR.InteractionSystem.Interactable>().RemoveHighlight();
				}

				if(bpo.GetPickupable() == pickedUpObject)
					ReleaseObject();

				if(registeredColliders.Count > 0)
					BKI_FingerCurler.BKI_Curler_Instance.SetHandUpdateBlend(hand, false);
				else
					BKI_FingerCurler.BKI_Curler_Instance.SetHandUpdateBlend(hand, true);
			}
		}

		private void ReleaseObject()
		{
			
			if(pickedUpObject != null)
			{
				if(pickedUpObject.CanRelease(pickedUpParent))
					pickedUpObject.ReleaseObject();
				pickedUpObject = null;
				isHoldingObject = false;
			}

			if(collidedObject != null)
			{
				collidedObject = null;
			}
		}

		// TO DO: port this so it is set in a script at startup
		private BKI_PickupableObject GetValidPickupable(GameObject c, int tries = 0)
		{
			if(tries > 20 || c == null)
				return null;
			return (c.GetComponent<IBetterPickupable>() != null) ? c.GetComponent<IBetterPickupable>().GetPickupable() :
				(c.transform.parent == null) ? null : GetValidPickupable(c.transform.parent.gameObject, tries++);
		}
	}
}