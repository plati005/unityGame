using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Stat : MonoBehaviour {
	
	//The image of the stat
	private Image content;

	//The text of the stat
	[SerializeField]
	private Text statValue;
	
	//Private value reflecting the amount left of the stat, a seperate public method may need to access it if a health potion gives you health
	private float currentFill;
	private float currentValue;
	
	//Public method to access to current value of the stat
	public float MyCurrentValue{
		get{
			return currentValue;
		}
		set{
			//Check if value out of bounds
			if (value > MyMaxValue){
				currentValue = MyMaxValue;
			}
			else if ( value < 0 ){
				//TODO: Need overkill number
				currentValue = 0;
			}
			else {
				currentValue = value;
			}
			//Set fill value to match health value
			currentFill = currentValue/MyMaxValue;
			
			statValue.text = currentValue + " / " + MyMaxValue;
		}
	}
	
	//Public method to access the max of the stat in case an upgrade needs to change it
	public float MyMaxValue {
		get;
		set;
	}
	
	//Metric for gradually changing the stat instead of immediately
	[SerializeField]
	private float lerpSpeed;
	
	//Initialization
	private void Start () {
		
		MyMaxValue = 100;
		content = GetComponent<Image>();
		
	}
	
	//Update is called once per frame
	private void Update () {
		//TODO:This can be secondary form of moving the stat as aftereffect
		if (currentFill != content.fillAmount){
			content.fillAmount = Mathf.Lerp(content.fillAmount, currentFill, Time.deltaTime * lerpSpeed);
		}
		//TODO:This should be the primary form of moving the stat
		//content.fillAmount = currentFill;
	}
	
	public void Initialize(float currentValue, float maxValue)
	{
		MyMaxValue = maxValue;
		MyCurrentValue = currentValue;
	}
	
}
