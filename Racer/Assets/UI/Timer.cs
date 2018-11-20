using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour {

    public static Timer cl_Timer;
    float f_theTime;
    public float f_speed = 1;

    Text text;

    public GameObject go_resultLap1;
    public GameObject go_resultLap2;
    public GameObject go_resultLap3;

    public Text resultLap1;
    public Text resultLap2;
    public Text resultLap3;

    public Text totalTimeRace1st;
    public Text totalTimeRace2nd;
    public Text totalTimeRace3rd;

    public bool b_lap1Player = false;
    public bool b_lap2Player = false;
    public bool b_lap3Player = false;

    string minutes;
    string seconds;
    string milliseconds;


    void Start() {

        cl_Timer = this;
        text = GetComponent<Text>();
        go_resultLap1.SetActive(false);
        go_resultLap2.SetActive(false);
        go_resultLap3.SetActive(false);

    }

    //Add time results for 1st lap for player on screen
    public void GetsResultsForPlayerLap1()
    {
        go_resultLap1.SetActive(true);
        resultLap1.text = minutes + ":" + seconds + ":" + milliseconds;
    }
    //Add time results for 2nd lap for player on screen
    public void GetsResultsForPlayerLap2()
    {
        go_resultLap2.SetActive(true);
        resultLap2.text = minutes + ":" + seconds + ":" + milliseconds;
    }
    //Add time results for 3rd lap for player on screen
    public void GetsResultsForPlayerLap3()
    {
        go_resultLap3.SetActive(true);
        resultLap3.text = minutes + ":" + seconds + ":" + milliseconds;
        totalTimeRace1st.text = minutes + ":" + seconds + ":" + milliseconds;
        totalTimeRace2nd.text = minutes + ":" + seconds + ":" + milliseconds;
        totalTimeRace3rd.text = minutes + ":" + seconds + ":" + milliseconds;
    }

    void Update () {

        //Timer --->
        if (GameManager.cl_GameManager.b_Start == true)
        { f_theTime += Time.deltaTime * f_speed;
        minutes = Mathf.Floor((f_theTime % 216000) / 3600).ToString("00");
        seconds = Mathf.Floor((f_theTime % 3600) / 60).ToString("00");
        milliseconds = (f_theTime % 60).ToString("00");

        text.text = minutes + ":" + seconds + ":" + milliseconds;

        }

        
        // Just for testing 
        /*   if (Input.GetKeyDown(KeyCode.Alpha1) && GameManager.cl_GameManager.b_Start == true)
        {
           
            GetsResultsForPlayerLap1();
        }
        if (Input.GetKeyDown(KeyCode.Alpha2) && GameManager.cl_GameManager.b_Start == true)
        {
            
            GetsResultsForPlayerLap2();
        }
        if (Input.GetKeyDown(KeyCode.Alpha3) && GameManager.cl_GameManager.b_Start == true)
        {
            
            GetsResultsForPlayerLap3();
        }
        */
    }
}
