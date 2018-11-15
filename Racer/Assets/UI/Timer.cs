using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour {

    public static Timer cl_Timer;
    float f_theTime;
    public float f_speed = 1;

    Text text;

	void Start () {
        text = GetComponent<Text>();
	}



	void Update () {

        if (GameManager.cl_GameManager.b_Start == true)
        { f_theTime += Time.deltaTime * f_speed;
        string minutes = Mathf.Floor((f_theTime % 216000) / 3600).ToString("00");
        string seconds = Mathf.Floor((f_theTime % 3600) / 60).ToString("00");
        string miliseconds = (f_theTime % 60).ToString("00");

        text.text = minutes + ":" + seconds + ":" + miliseconds; }

        }
}
