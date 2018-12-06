
//#define ES_USE_TMPRO

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;



public class EventScript : MonoBehaviour {
    [System.Serializable] public class mEvent : UnityEvent { }

    [System.Serializable]
    public class eventText
    {
        public string textContent;
#if ES_USE_TMPRO
		public TMPro.TextMeshProUGUI TMProField;
#endif
        public Text textField;
    }


    [System.Serializable]
    public class eventTexts {
        public eventText titleText;
        public eventText questionText;
        public eventText answerLeft;
        public eventText answerRight;
        public eventText answerUp;
        public eventText answerDown;
    }

#if ES_USE_TMPRO
	public static bool useTextMeshPro = true;
#else
    public static bool useTextMeshPro = false;
#endif

    [Tooltip("Define your card texts and text fields here. The strings can be terms for the 'TranslationManager'.")]
    public eventTexts textFields;

    [Tooltip("If a card is high priority, it will be draw before all other normal cards, but after follow up cards.")]
    public bool isHighPriorityCard = false;
    [Tooltip("Only drawable cards can be randomly drawn because of their condition. Non drawable cards are follow up cards which are defined by previous cards or cards like the gameover statistics.")]
    public bool isDrawable = true;

    [Tooltip("The propability applies to all cards, which met the conditions. Cards with a higher propability are more likely to be drawn.")]
    [Range(0f, 1f)] public float cardPropability = 1f;
    [Tooltip("To limit the maximum draws of a card per game, define the 'maxDraws'.")]
    public int maxDraws = 100;
    [Tooltip("After a card is drawn, define for how many cycles it is blocked to be redrawn. This doesn't apply to follow up cards.")]
    public int redrawBlockCnt = 0;


    [System.Serializable]
    public enum E_ConditionType {
        standard,
        compareValues
    }

    [System.Serializable]
    public enum E_ConditionCompareType
    {
        greaterThan,
        greaterThanOrEqual,
        equals,
        equalsAsInt,
        lessThan,
        lessThanOrEqual
    }

        [System.Serializable]
	public class condition{
        public E_ConditionType type;

		public valueDefinitions.values value;
		public float valueMin = 0f;
		public float valueMax = 100f;

        public E_ConditionCompareType compareType;
        public valueDefinitions.values rValue;
	}

	[Tooltip("Define under wich conditions this card can be drawn. E.g. a marriage card should only be possible if a value type 'age' is in the range of 18 to 100 or the value type 'marriage' is zero (not married yet)")]
	public condition[] conditions;

	[System.Serializable]
	public class resultModifier{
		public valueDefinitions.values modifier;
		public float valueAdd = 0f;
	}

	[System.Serializable]
	public class resultModifierPreview{
		public resultModifier resultModification;
		public bool modificationIsRandomIndependant = true;
	}

	[System.Serializable]
	public enum resultTypes{
		simple,
		conditional,
		randomConditions,
		random
	}

	[System.Serializable]
	public class modifierGroup{

		public resultModifier[] valueChanges;
		[Tooltip("If this path was taken, will there be a 'follow up' card which takes the story further? Can be left empty.")]
		public GameObject followUpCard;
	}

	[System.Serializable]
	public class result{
		public resultTypes resultType;
		[Tooltip("Which values are modified, if this result is selected?")]
		public modifierGroup modifiers;
		[Tooltip("Depending on further conditions the result can split into two different outcomes. If all conditions are true, the 'Modifiers True' are executed. If one of the conditions fails, the 'Modifiers False'. E.g. the user selected he wants to take a race but his 'agility' value is to low, as outcome he will lose.")]
		public condition[] conditions;
		[Tooltip("Group of value changes, if all conditions are met.")]
		public modifierGroup	modifiersTrue;
		[Tooltip("Group of value changes, if at least one of the conditions fails.")]
		public modifierGroup	modifiersFalse;
		[Tooltip("A result can be split in multible outcomes. The 'Random Mofifiers' can be predefined, the selection of the outcome is randomly one of these.")]
		public modifierGroup[]  randomModifiers;
	}

    [System.Serializable]
    public class resultGroup
    {
        [Tooltip("Define the result (the changes in values and perhaps a follow up card) if the user swipes the card left.")]
        public result resultLeft;

        [Tooltip("Define the result (the changes in values and perhaps a follow up card) if the user swipes the card right.")]
        public result resultRight;

        [Tooltip("Define the result (the changes in values and perhaps a follow up card) if the user swipes the card up.")]
        public result resultUp;

        [Tooltip("Define the result (the changes in values and perhaps a follow up card) if the user swipes the card down.")]
        public result resultDown;


        [Tooltip("Define the result (the changes in values and perhaps a follow up card) if the user selects addtional choices.")]
        public result additional_choice_0;

        [Tooltip("Define the result (the changes in values and perhaps a follow up card) if the user selects addtional choices.")]
        public result additional_choice_1;

    }

	public resultGroup Results;

    //Configuration: use 2 way swipe style or use 4 way swipe
    [System.Serializable]
    public enum E_SwipeType {
        LeftRight,
        FourDirection
    }
    [Tooltip("Define the type of the swipe for this card. \n'LeftRight' enables 2 choices. \n'FourDirection' allows 4 choices.\nFor more choices please use the multi-choice template.")]
    public E_SwipeType swipeType;

    [Tooltip("Add two additional choices for e. g. multichoice card.")]
    public bool additionalChoices = false;

    //Try to translate and write the configurated texts to their text-fields. 
#if ES_USE_TMPRO
	void writeTextFields(){
		if (textFields.titleText.TMProField != null) {
			textFields.titleText.TMProField.text  =  TranslationManager.translateIfAvail( textFields.titleText.textContent  );
		}
		if (textFields.questionText.TMProField != null) {
			textFields.questionText.TMProField.text = TranslationManager.translateIfAvail (textFields.questionText.textContent);
		}
		if (textFields.answerLeft.TMProField != null) {
			textFields.answerLeft.TMProField.text = TranslationManager.translateIfAvail( textFields.answerLeft.textContent);
		}		
		if (textFields.answerRight.TMProField != null) {
			textFields.answerRight.TMProField.text = TranslationManager.translateIfAvail( textFields.answerRight.textContent);
		}
	}
#else
    void writeTextFields()
    {
        if (textFields.titleText.textField != null)
        {
            textFields.titleText.textField.text = TranslationManager.translateIfAvail(textFields.titleText.textContent);
        }
        if (textFields.questionText.textField != null)
        {
            textFields.questionText.textField.text = TranslationManager.translateIfAvail(textFields.questionText.textContent);
        }
        if (textFields.answerLeft.textField != null)
        {
            textFields.answerLeft.textField.text = TranslationManager.translateIfAvail(textFields.answerLeft.textContent);
        }
        if (textFields.answerRight.textField != null)
        {
            textFields.answerRight.textField.text = TranslationManager.translateIfAvail(textFields.answerRight.textContent);
        }
        if (textFields.answerUp.textField != null)
        {
            textFields.answerUp.textField.text = TranslationManager.translateIfAvail(textFields.answerUp.textContent);
        }
        if (textFields.answerDown.textField != null)
        {
            textFields.answerDown.textField.text = TranslationManager.translateIfAvail(textFields.answerDown.textContent);
        }
    }
	#endif

	/*
	 * Called by an event from the swipe script. 
	 * This triggers the computation of the results for swiping LEFT and afterward the spawning of a new card.
	 */
	public void onLeftSwipe(){
		result res = Results.resultLeft;
		computeResult (res);
		OnSwipeLeft.Invoke ();
	}
	/*
	 * Called by an event from the swipe script. 
	 * This triggers the computation of the results for swiping RIGHT and afterward the spawning of a new card.
	 */
	public void onRightSwipe(){
		result res = Results.resultRight;
		computeResult (res);
		OnSwipeRight.Invoke ();
	}
    /*
     * Called by an event from the swipe script. 
     * This triggers the computation of the results for swiping UP and afterward the spawning of a new card.
     * For compatibility: The execution is discarded if the swipe type is not configured for four directions.
     */
    public void onUpSwipe()
    {
        if (swipeType == E_SwipeType.FourDirection)
        {
            result res = Results.resultUp;
            computeResult(res);
            OnSwipeRight.Invoke();
        }
    }
    /*
     * Called by an event from the swipe script. 
     * This triggers the computation of the results for swiping DOWN and afterward the spawning of a new card.
     * For compatibility: The execution is discarded if the swipe type is not configured for four directions.
     */
    public void onDownSwipe()
    {
        if (swipeType == E_SwipeType.FourDirection)
        {
            result res = Results.resultDown;
            computeResult(res);
            OnSwipeRight.Invoke();
        }
    }

    public bool ExecuteAddtionalChoices(int choiceNr){
		bool executed = false;

		switch (choiceNr) {
		case 0:

			AdditionalChoice_0_Selection();
			executed = true;

			break;
		case 1:

			AdditionalChoice_1_Selection();
			executed = true;

			break;
		default:
			Debug.LogError("ADDITIONAL_CHOICE_"+choiceNr.ToString()+" is not configured in 'EventScript'.");
			break;	
		}

		return executed;
	}


	/* 
	 * This triggers the computation of the results for additonal options and afterward the spawning of a new card.
	 */
	public void AdditionalChoice_0_Selection(){
		result res = Results.additional_choice_0;
		computeResult (res);
		OnAdditionalChoice_0.Invoke ();
	}
	// Preview method for addition choice 0
	public void AdditionalChoice_0_Preview(){
		result res = Results.additional_choice_0;
		resetResultPreviews ();
		computeResultPreview (res);
	}

	/* 
	 * This triggers the computation of the results for additonal options and afterward the spawning of a new card.
	 */
	public void AdditionalChoice_1_Selection(){
		result res = Results.additional_choice_1;
		computeResult (res);
		OnAdditionalChoice_1.Invoke ();
	}
	// Preview method for addition choice 1
	public void AdditionalChoice_1_Preview(){
		result res = Results.additional_choice_1;
		resetResultPreviews ();
		computeResultPreview (res);
	}


	// Methods for previews of changes in values 

	/*
	 * Called by an event from the swipe script.
	 * This triggers the generation of a preview on the values for an LEFT swipe.
	 */
	public void onLeftSpwipePreview(){
		result res = Results.resultLeft;
		resetResultPreviews ();
		computeResultPreview (res);
		computeEventResultsPreview (OnSwipeLeft);
		computeEventResultsPreview (OnCardDespawn);
	}
	/*
	 * Called by an event from the swipe script.
	 * This triggers the generation of a preview on the values for an RIGHT swipe.
	 */
	public void onRightSpwipePreview(){
		result res = Results.resultRight;
		resetResultPreviews ();
		computeResultPreview (res);
		computeEventResultsPreview (OnSwipeRight);
		computeEventResultsPreview (OnCardDespawn);
	}
    /*
     * Called by an event from the swipe script.
     * This triggers the generation of a preview on the values for an UP swipe.
     */
    public void onUpSpwipePreview()
    {
        result res = Results.resultUp;
        resetResultPreviews();
        computeResultPreview(res);
        computeEventResultsPreview(OnSwipeUp);
        computeEventResultsPreview(OnCardDespawn);
    }
    /*
     * Called by an event from the swipe script.
     * This triggers the generation of a preview on the values for an DOWN swipe.
     */
    public void onDownSpwipePreview()
    {
        result res = Results.resultDown;
        resetResultPreviews();
        computeResultPreview(res);
        computeEventResultsPreview(OnSwipeDown);
        computeEventResultsPreview(OnCardDespawn);
    }

    //get possible results from events calling addValueToValue and add them to the preview
    public void computeEventResultsPreview(mEvent _mEvent){
		int eventCnt = _mEvent.GetPersistentEventCount();
		addValueToValue avtv;
		Object o;

		//check for each persistent call, if it is a 'addValueToValue' script
		for (int i = 0; i < eventCnt; i++) {

			//get the type of the persistent object, if it is correct cast it to 'addValueToValue'
			o = _mEvent.GetPersistentTarget (i);
			if (o.GetType () == typeof(addValueToValue)) {
				avtv = (addValueToValue)o;
			} else {
				avtv = null;
			}

			if (avtv != null) {

				//the object was a 'addValueToValue' script, therefore generate the previews

				previewModifiers.Clear();

				if (avtv.valuesToChange.Length > 0) {
					float rValue = 0f;
					float diff = 0f;
					foreach (addValueToValue.resultModifierForAddingValueToValue valueAddValue in avtv.valuesToChange) {

						//get the change in the value
						rValue =  valueManager.instance.getFirstFittingValue(valueAddValue.rArgument).value;
						diff = rValue * valueAddValue.multiplier;

						//generate a preview instance the change
						resultModifierPreview preview = new resultModifierPreview ();
						preview.resultModification = new resultModifier ();

						preview.modificationIsRandomIndependant = true;
						preview.resultModification.modifier = valueAddValue.lArgument;	// get the affected value
						preview.resultModification.valueAdd = diff;

						previewModifiers.Add (preview);

						//Debug.Log ("added event preview for " + preview.resultModification.modifier.ToString () + ", change is " + diff.ToString ());
					}
				}

				//Tell the valueManager to tell the ValueScripts to show these previews.
				valueManager.instance.setPreviews(ref previewModifiers);
			}
		}
	}

	/*
	 * Called by an event from the swipe script.
	 * This resets the preview on the values.
	 */
	public void onSwipePreviewReset(){
		previewModifiers.Clear ();
		resetResultPreviews ();
	}

	public void PreviewAddtionalChoices(int choiceNr){
		switch (choiceNr) {
		case 0:

			AdditionalChoice_0_Preview();

			break;
		case 1:

			AdditionalChoice_1_Preview();

			break;
		default:
			Debug.LogError("ADDITIONAL_CHOICE_"+choiceNr.ToString()+" is not configured in 'EventScript'.");
			break;	
		}
	}



	//Computation logic for executing a result.
	//Depending on the configuration of the card the corresponding results are selected.
	void computeResult(result res){

		if (res.resultType == resultTypes.simple) {
			
			//If the result is configured as 'simple' just execute the value modifiers.
			executeValueChanges (res.modifiers);

		} else if (res.resultType == resultTypes.conditional) {
		
			//If the result is configured as 'conditional' validate the conditions and
			//execute the depending modifiers.
			if (AreConditinsForResultMet (res.conditions)) {
				executeValueChanges (res.modifiersTrue);
			} else {
				executeValueChanges (res.modifiersFalse);
			}

		} else if (res.resultType == resultTypes.randomConditions) {

			//If the result is configured as 'randomConditions':
			//1. Randomize the borders of predefined value-typ dependencies.
			//2. Validate the new conditions.
			//3. Execute outcome dependent value changes.

			float rndResult = 1f;
			ValueScript v = null;
			foreach (condition c in res.conditions) {
				rndResult = Random.Range (0f, 1f);
				v = valueManager.instance.getFirstFittingValue (c.value);

				if (v != null) {
					//set the minimum border for the conditon between min and max, 
					//if the real value is over min, the path 'true' is executed
					c.valueMin = v.limits.min + rndResult*(v.limits.max - v.limits.min);	
					c.valueMax = v.limits.max;
				} else {
					Debug.LogWarning ("Missing value type: " + c.value);
				}

			}

			if (AreConditinsForResultMet (res.conditions)) {
				executeValueChanges (res.modifiersTrue);
			} else {
				executeValueChanges (res.modifiersFalse);
			}

		}else if (res.resultType == resultTypes.random) {

			//If the result is configured as 'random':
			//Select randomly a modifier-group out of the defined pool and execute the value changes.
			if (res.randomModifiers.Length != 0) {
				int rndResult = Random.Range (0, res.randomModifiers.Length);
				executeValueChanges (res.randomModifiers[rndResult]);
			} else {
				Debug.LogWarning ("Missing random results-list");
			}

		} else {
			Debug.LogError ("Path not reachable?");
		}

		foreach (resultModifier rm in  changeValueOnCardDespawn) {
			valueManager.instance.changeValue (rm.modifier, rm.valueAdd);
		}
			
		OnCardDespawn.Invoke ();
	}

	//Computation logic for generating a previews of a result.
	//Depending on the configuration of the card the corresponding results are selected.

	List <resultModifierPreview> previewModifiers = new List<resultModifierPreview>(); // List of values which will be modified with additional informations

	void computeResultPreview(result res){

		previewModifiers.Clear ();

		if (res.resultType == resultTypes.simple) {

			//If the result is configured as 'simple' just add the value modifiers to list.
			addPreviewValueChanges (res.modifiers);

		} else if (res.resultType == resultTypes.conditional) {

			//If the result is configured as 'conditional' validate the conditions and
			//execute the depending modifiers.
			if (AreConditinsForResultMet (res.conditions)) {
				addPreviewValueChanges (res.modifiersTrue);
			} else {
				addPreviewValueChanges (res.modifiersFalse);
			}

		} else if (res.resultType == resultTypes.randomConditions) {

			//If the result is configured as 'randomConditions':
			//Value changes are unknown for the preview. Mark them as unknown.
			//Is it possible to preview this correctly? In some circumstances, but I think this is not worth the effort.

			//Add both possibilities, mark them as 'randomIndependent' = false
			addPreviewValueChanges (res.modifiersTrue, false);
			addPreviewValueChanges (res.modifiersFalse, false);

		} else if (res.resultType == resultTypes.random) {

			//If the result is configured as 'random':
			//Add all possible value changes for the preview to the list, marked as 'randomIndependent' = false
			if (res.randomModifiers.Length != 0) {
				for(int i = 0; i<res.randomModifiers.Length; i++){
					addPreviewValueChanges (res.randomModifiers[i]);
				}
			} else {
				Debug.LogWarning ("Missing random results-list");
			}

		} else {
			Debug.LogError ("Path not reachable?");
		}

		//Now all the possible value changes are known.

		//Attention! There can be still duplicates in the list because of randomization.
		//If this happens, the value script itself will detect the setting of duplicates and then setting the outcome to 'randomIndependant' = true

		//Tell the valueManager to tell the ValueScripts to show these previews.
		valueManager.instance.setPreviews(ref previewModifiers);
	}

	//Reset for all value previews.
	void resetResultPreviews(){
		valueManager.instance.clearAllPreviews ();
	}

	//execution of a group of value modifications
	void executeValueChanges(modifierGroup modsGroup){

		//reset the user info
		//InfoDisplay.instance.clearDisplay ();

		foreach (resultModifier rm in  modsGroup.valueChanges) {
			valueManager.instance.changeValue (rm.modifier, rm.valueAdd);
		}

		//Tell the cardstack the follow up card.
		//Follow up card can be NULL, the cardstack itself checks the cards before spawning.
		CardStack.instance.followUpCard = modsGroup.followUpCard;

		//show the value changes over the animation (if available)
		//InfoDisplay.instance.startAnimationIfNotEmpty();
	}

	//add element preview of a group of value modifications
	void addPreviewValueChanges(modifierGroup modsGroup, bool randomIndependent = true){
		if (modsGroup.valueChanges == null) {
			Debug.LogError ("Can not show preview, modifier group is null.");
			return;
		}

		foreach (resultModifier rm in  modsGroup.valueChanges) {

			resultModifierPreview rmp = new resultModifierPreview ();
			rmp.resultModification = new resultModifier ();
			rmp.modificationIsRandomIndependant = randomIndependent;
			rmp.resultModification.modifier = rm.modifier;
			rmp.resultModification.valueAdd = rm.valueAdd;
			previewModifiers.Add (rmp);
		}
	}

	//check for a set of conditions if everything is met
	bool AreConditinsForResultMet(condition[] cond){
		
		bool conditionOk = true;

		foreach (EventScript.condition c in cond) {
			if (valueManager.instance.getConditionMet (c) == true) {
				//condition is ok.
			} else {
				conditionOk = false;
				break;
			}
		}

		return conditionOk;
	}


	void Awake(){
		writeTextFields ();
	}
		
	void Start () {
		OnCardSpawn.Invoke ();
	}

	[Tooltip("Changes of values after the computation of the conditional results. Useful if a value is changed independent of the result, like 'Age +1'.")]
	public resultModifier[] changeValueOnCardDespawn;

	public mEvent OnCardSpawn;
	public mEvent OnCardDespawn;

	public mEvent OnSwipeLeft;
	public mEvent OnSwipeRight;
    public mEvent OnSwipeUp;
    public mEvent OnSwipeDown;


    public mEvent OnAdditionalChoice_0;
	public mEvent OnAdditionalChoice_1;

}
