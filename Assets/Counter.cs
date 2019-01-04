using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Counter : MonoBehaviour {

    Text counterText;
    public float timeRemaining;
    public GameObject counterParent;


	// Use this for initialization
	void Start () {
        counterText = this.GetComponent<Text>();
        counterText.text = timeRemaining.ToString();

	}
	
	// Update is called once per frame
	void Update () {
        timeRemaining -= Time.deltaTime;
        if (timeRemaining > 0){
            counterText.text = ((int)timeRemaining).ToString(); 
        }else{
            counterParent.SetActive(false);
        }


	}
}
