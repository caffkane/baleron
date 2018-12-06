using UnityEngine;
using System.Collections;
using UnityEngine.Events;



public class Swipe : MonoBehaviour {

	private Vector3 startPosition;
	private Vector3 stopPosition;
	private Vector3 swipe;
	private Vector3 lastPosition;


	[System.Serializable] public class mEvent : UnityEvent {}

	[System.Serializable]
	public class normSwipe{
		[Tooltip("After which swipe up/down distance is an event executed?")]
		[Range(0f,1f)] public float swipeDetectionLimit_UD = 0.3f;

		[Tooltip("After which swipe left/right distance is an event executed?")]
		[Range(0f,1f)]  public float swipeDetectionLimit_LR = 0.3f;
		public mEvent swipeUp;							//execute an Event on Swipe up
		public mEvent swipeDown;						//...down
		public mEvent swipeLeft;						//...left
		public mEvent swipeRight;						//...right
        public mEvent swipeUpPreview;
        public mEvent swipeDownPreview;
		public mEvent swipeLeftPreview;
		public mEvent swipeRightPreview;
		public mEvent swipePreviewReset;
	}

	public normSwipe usualSwipes;

	public enum SwipeDirections
	{
		none,
		up,
		down,
		left,
		right
	}
	[ReadOnlyInspector] public SwipeDirections direction;
	SwipeDirections old_direction; //memorize last swipe direction to detect a change

	Vector3 swipeVector = Vector3.zero;

	public Vector2 getSwipeVector(){
		return swipeVector;
	}

	[Tooltip("Scale the swipe for getting it with 'getScaledSwipeVector()'.\nThis is used for linking it with the blend tree of the card movement.")]
	public Vector2 swipeScale = Vector3.zero;

	[ReadOnlyInspector] public bool pressed = false;
	private bool oldPressed = false;
	[ReadOnlyInspector] public float actualSwipeDistance = 0f;

	public Vector2 getScaledSwipeVector(){
		Vector2 retVal = Vector2.zero;
		retVal.x = swipeVector.x * swipeScale.x;
		retVal.y = swipeVector.y * swipeScale.y;
		return retVal;
	}

	void Update(){

		//get the actual mouse or finger press

			if (Input.GetMouseButton (0)) {
				pressed = true;
			} else {
				pressed = false;
			}

		//on an change of the press, compute the distance

			if (oldPressed == false && pressed == true) {
				onKlick ();	
			}
			if (oldPressed == true && pressed == false) {
				onRelease ();
			}

		//track the swipe distance and scale it to to the screen size

		if (pressed == true) {
			swipeVector = Input.mousePosition - startPosition;
			swipeVector.x = swipeVector.x / Screen.width;
			swipeVector.y = swipeVector.y / Screen.height;
			actualSwipeDistance = swipeVector.magnitude;
		} else {
			actualSwipeDistance = 0f;
			swipeVector = Vector3.zero;
		}

		//generate events for preview of value changes
		processSwipePreview();

		oldPressed = pressed;
	}


	void onKlick(){

		startPosition = Input.mousePosition;
		lastPosition = startPosition;
	}

	void onRelease(){

		stopPosition = Input.mousePosition;
		processSwipe ();
	}

	//Swipe preview is tested each frame.
	//If it exceeds the limits, it generates the preview events.
	void processSwipePreview(){

		direction = SwipeDirections.none;

		//process the usual swipes

		if (Mathf.Abs (swipeVector.x) > Mathf.Abs (swipeVector.y)) {

			//decide: left/right swipe or up/down
			if (swipeVector.magnitude > usualSwipes.swipeDetectionLimit_LR) {
				// left or right
				if (swipeVector.x > 0f) {
					direction = SwipeDirections.right;
				} else {
					direction = SwipeDirections.left;
				}
			}
		} else {
			if (swipeVector.magnitude > usualSwipes.swipeDetectionLimit_UD) {
				// up or down
				if (swipeVector.y > 0f) {
					direction = SwipeDirections.up;
				} else {
					direction = SwipeDirections.down;
				}
			}
		}

		if (old_direction != direction) {
            switch (direction)
            {
                case SwipeDirections.left:
                    usualSwipes.swipeLeftPreview.Invoke();
                    break;
                case SwipeDirections.right:
                    usualSwipes.swipeRightPreview.Invoke();
                    break;
                case SwipeDirections.up:
                    usualSwipes.swipeUpPreview.Invoke();
                    break;
                case SwipeDirections.down:
                    usualSwipes.swipeDownPreview.Invoke();
                    break;
                default:
                    //on (up, down) or none: reset the preview
                    usualSwipes.swipePreviewReset.Invoke();
                    break;
            }

			old_direction = direction;
		}
	}

	void processSwipe(){

		//process the usual swipes

		if (Mathf.Abs (swipeVector.x) > Mathf.Abs (swipeVector.y)) {

			//decide: left/right swipe or up/down
			if (swipeVector.magnitude > usualSwipes.swipeDetectionLimit_LR) {
				// left or right
				if (swipeVector.x > 0f) {
					usualSwipes.swipeRight.Invoke ();	
				} else {
					usualSwipes.swipeLeft.Invoke ();	
				}
			}
		} else {
			if (swipeVector.magnitude > usualSwipes.swipeDetectionLimit_UD) {
				// up or down
				if (swipeVector.y > 0f) {
					usualSwipes.swipeUp.Invoke ();	
				} else {
					usualSwipes.swipeDown.Invoke ();	
				}
			}
		}
	
		//after swipe: reset the positions to zeros
		startPosition = stopPosition = Vector3.zero;
	}

}
