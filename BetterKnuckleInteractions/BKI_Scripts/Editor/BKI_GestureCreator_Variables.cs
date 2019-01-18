using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BetterKnucklesInteractions
{
	public partial class BKI_GestureCreator
	{
		private const int WINDOW_MIN_SIZE_X = 560;
		private const int WINDOW_MIN_SIZE_Y = 630;
		private const int WINDOW_MAX_SIZE_X = 560;
		private const int WINDOW_MAX_SIZE_Y = 630;
		private const int HAND_POS_X = 0;
		private const int HAND_POS_Y = 165;
		private const int HAND_SIZE_W = 256;
		private const int HAND_SIZE_H = 256;
		private const int TITLE_FIELD_SIZE = 48;
		private const int FOOTER_FIELD_SIZE = (int)(TITLE_FIELD_SIZE * 1.75f);
		private const int DEFAULT_AREA_PADDING = 8;
		private const string RESOURCES_ROOT_PATH = "Assets/Resources/CombinationGestures";
		private const string RESOURCES_LEFTHAND_PATH = "Assets/Resources/LeftHandGestures";
		private const string RESOURCES_RIGHTHAND_PATH = "Assets/Resources/RightHandGestures";
		private const string RESOURCES_COMBI_PATH = "Assets/Resources/CombinationGestures";
		private const string RESOURCES_GESTURESTORAGE_PATH = "GestureStorageObjects/GeneralStorage";
		public const string DEFAULT_GESTURE_NAME = "Name";

		private BKI_GestureStorageClass gestureStorage;

		// Sprite textures for hand
		private Texture2D baseHandText2D;
		private Texture2D thumbHandText2D;
		private Texture2D indexHandText2D;
		private Texture2D middleHandText2D;
		private Texture2D ringHandText2D;
		private Texture2D pinkyHandText2D;
		private Texture2D thumbHandCrossText2D;
		private Texture2D indexHandCrossText2D;
		private Texture2D middleHandCrossText2D;
		private Texture2D ringHandCrossText2D;
		private Texture2D pinkyHandCrossText2D;

		// Hand Sprite Rect
		private Rect baseHandRect;

		private BKI_GestureMirrorClass leftHandValues, rightHandValues;

		private GUIStyle titleStyle;
		private GUIStyle subtitleStyle;
		private GUIStyle subsubtitleStyle;
		private Rect backgroundRect;
		private Rect handGUIContainer;
		private Texture2D backgroundTex2D;
		private Texture2D containerHandTex2D;
		private Texture2D containerLhTex2D;
		private Rect titleContainer;
		private Texture2D titleBackgroundTex2D;
		private Rect footerContainer;
		private Rect footerContainerSmall;
		private Rect buttonStackRect;
		private Texture2D footerBackgroundTex2D;
		private Rect handRect;

		private BKI_SingleGestureClass lhGesture, rhGesture;
		private BKI_SingleGestureClass lhGestureResReference, rhGestureResReference;

		private BKI_GestureMirrorClass lhMirror, rhMirror;
		private BKI_CombiGestureClass combiGesture;

		private bool isOpenedFromResourcesLh = false;
		private bool isOpenedFromResourcesRh = false;
		private bool isOpenedFromResourcesCombi = false;
		private int pickerWindow;
		private string pickerWindowGNameLh;
		private string pickerWindowGNameRh;
		private string pickerWindowGNameCombi;

		private bool currentGestureSavedCombi;
		private bool currentGestureSavedLh;
		private bool currentGestureSavedRh;

		private int buttonHeight = 40;
	}
}