using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System;

namespace BetterKnucklesInteractions
{
	public class BKI_CombiGestureMirrorClass
	{
		public string gestureIdentifier = "Name";
		public string lhGesture, rhGesture;
		// Default ctor
		public BKI_CombiGestureMirrorClass(string lh, string rh, string id)
		{
			lhGesture = lh;
			rhGesture = rh;
			gestureIdentifier = id;
		}
	}
}
