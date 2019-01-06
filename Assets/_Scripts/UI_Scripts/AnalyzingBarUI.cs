using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnalyzingBarUI : MonoBehaviour {

    public Image analyzeBar;
    public bool analyzing;
    public float waitTime = 5.0f;

    // Update is called once per frame
    void Update()
    {
        if (analyzing == true)
        {
            //Reduce fill amount over 30 seconds
            analyzeBar.fillAmount -= 1.0f / waitTime * Time.deltaTime;
        }
    }
}
