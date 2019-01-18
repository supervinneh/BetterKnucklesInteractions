using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BetterKnucklesInteractions
{
	public interface IBetterPickupable
	{
		BKI_PickupableObject GetPickupable();
	}
}