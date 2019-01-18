using UnityEngine;
using Valve.VR.InteractionSystem;

namespace BetterKnucklesInteractions
{
	public class BKI_PickupableObjectChild : MonoBehaviour, IBetterPickupable
	{
		private BKI_PickupableObject parentObj;

		public void Start()
		{
			parentObj = GetValidPickupable(gameObject);

			if(parentObj == null)
				Destroy(this);
		}

		public BKI_PickupableObject GetPickupable()
		{
			return parentObj.GetPickupable();
		}

		private BKI_PickupableObject GetValidPickupable(GameObject c, int tries = 0)
		{
			if(tries > 20 || c == null)
				return null;
			return (c.GetComponent<BKI_PickupableObject>() != null && c != this.gameObject) ? c.GetComponent<BKI_PickupableObject>() :
				(c.transform.parent == null) ? null : GetValidPickupable(c.transform.parent.gameObject, tries++);
		}
	}
}