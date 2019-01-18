using UnityEngine;
using Valve.VR.InteractionSystem;

namespace BetterKnucklesInteractions
{
	[RequireComponent(typeof(Rigidbody))]
	[RequireComponent(typeof(VelocityEstimator))]
	public class BKI_PickupableObject : MonoBehaviour, IBetterPickupable
	{
		private Rigidbody rb;
		private bool originalUseGravity, originalIsKinematic;
		[SerializeField]
		private string tagToSet = "pickupableObject";
		[SerializeField]
		private bool preserveParent = false;
		private Transform originalParent;
		private LayerMask originalMask;
		public bool pickedUp = false;

		[SerializeField]
		private VelocityEstimator vEstimator;

		// Use this for initialization
		public virtual void Awake()
		{
			transform.tag = tagToSet;

			vEstimator = gameObject.GetComponent<VelocityEstimator>();

			rb = gameObject.GetComponent<Rigidbody>();

			originalIsKinematic = rb.isKinematic;
			originalUseGravity = rb.useGravity;
			originalMask = gameObject.layer;


			if(transform.parent != null)
				originalParent = transform.parent;

			foreach(Collider col in gameObject.GetComponentsInChildren<Collider>())
			{
				if(col != GetComponent<Collider>())
				{
					col.gameObject.AddComponent<BKI_PickupableObjectChild>();
					col.tag = tag;
				}
				
			}
		}

		public virtual void PickupObject(Transform pickupParent)
		{
			rb.isKinematic = true;
			rb.useGravity = false;
			pickedUp = true;
			vEstimator.BeginEstimatingVelocity();
			transform.parent = pickupParent;
		}

		public virtual bool CanRelease(Transform checkTransform)
		{
			return (transform.parent == checkTransform);
		}

		public virtual void ReleaseObject()
		{
			rb.isKinematic = originalIsKinematic;
			rb.useGravity = originalUseGravity;

			if(preserveParent)
				transform.parent = originalParent;
			else
				transform.parent = null;

			// Remove stored parent after being dropped in worldspace once as it isn't needed anymore.
			if(transform.parent == null)
				originalParent = null;

			pickedUp = false;

			// Stop estimating the velocity and apply the velocity.
			rb.AddForce(vEstimator.GetVelocityEstimate(), ForceMode.Impulse);
			vEstimator.FinishEstimatingVelocity();
		}

		public virtual BKI_PickupableObject GetPickupable()
		{
			return this;
		}
	}
}