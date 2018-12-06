using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MathExtension : MonoBehaviour  {

	public static float linearInterpolate(float actual, float target, float step)
	{
		float retVal = 0f;
		if (actual < (target - step)) {
			retVal = actual + step;

			if (retVal >= target) {
				retVal = target;
			}
		} else if (actual > (target + step)) {
			retVal = actual - step;

			if (retVal <= target) {
				retVal = target;
			}
		} else {
			retVal = target;
		}

		return retVal;
	
	}
}
