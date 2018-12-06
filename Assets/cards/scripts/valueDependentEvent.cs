using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class valueDependentEvent: MonoBehaviour {


    [Tooltip("'valueDependentEvent' supports two modes.\n" +
    "automatic: conditions are evaluatet all the time. If the condition results switch from true to false or otherwise an event is invoked once.\n" +
    "manual:    conditions are only evalutated if the function 'ExecuteConditionCheck()' is called. The events are invoked accordingly.")]
    public E_EventExecutionType triggerType = E_EventExecutionType.automatic;


    public EventScript.condition[] conditionsToTest;
	[System.Serializable] public class mEvent : UnityEvent {}

	public mEvent OnConditionsTrue;
	public mEvent OnConditionsFalse;

    [System.Serializable]
    public enum E_EventExecutionType {
        automatic,
        manual
    }

	void Start(){

        if (triggerType == E_EventExecutionType.automatic)
        {
            StartCoroutine(oneFrame());
        }
    }

    IEnumerator oneFrame() {
        yield return null;
        testAndInvoke(true);
        while (true) {
            testAndInvoke();
            yield return null;
        }

    }

    public void ExecuteConditionCheck() {
        testAndInvoke(true);
    }

    bool lastResult = false;
    private void testAndInvoke(bool initialize = false)
    {

        bool result = valueManager.instance.AreConditinsForResultMet(conditionsToTest);

        if (lastResult != result || initialize == true)
        {

            if (result == true)
            {
                OnConditionsTrue.Invoke();
            }
            else
            {
                OnConditionsFalse.Invoke();
            }
        }


        lastResult = result;
    }
}
