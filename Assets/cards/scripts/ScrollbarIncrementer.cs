using System;
using UnityEngine;
using UnityEngine.UI;
 
[RequireComponent(typeof(Button))]
public class ScrollbarIncrementer : MonoBehaviour
{
	    public Scrollbar Target;
	    public Button TheOtherButton;
	    public float Step = 0.1f;
	 
	private Button myButton;
	void Start(){
		myButton = GetComponent<Button> ();
	}

	    public void Increment()
	    {
		        if (Target == null || TheOtherButton == null) throw new Exception("Setup ScrollbarIncrementer first!");
		        Target.value = Mathf.Clamp(Target.value + Step, 0, 1);
		//myButton.interactable = Target.value != 1;
		       // TheOtherButton.interactable = true;
		    }
	 
	    public void Decrement()
	    {
		        if (Target == null || TheOtherButton == null) throw new Exception("Setup ScrollbarIncrementer first!");
		        Target.value = Mathf.Clamp(Target.value - Step, 0, 1);
		//myButton.interactable = Target.value != 0;;
		       // TheOtherButton.interactable = true;
		    }

	void Update(){
		if (Target.size > 0.99f) {
			//the screen is big enough for the messages, deactivate buttons
			if (TheOtherButton.interactable == true) {
				TheOtherButton.interactable = false;
			}
			if (myButton.interactable == true) {
				myButton.interactable = false;
			}
		} else {
			//the screen is big enough for the messages, deactivate buttons
			if (TheOtherButton.interactable == false) {
				TheOtherButton.interactable = true;
			}
			if (myButton.interactable == false) {
				myButton.interactable = true;
			}
		}
	}


}
