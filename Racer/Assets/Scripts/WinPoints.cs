﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinPoints : MonoBehaviour {

    public GameObject nextPoint;

    public bool b_LastPoint;
    public bool b_AIwon = false;

	void Start () {
		
	}


    private void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.tag == "Player")
        {
            FindObjectOfType<AudioManager>().Play("countDown");

            nextPoint.SetActive(true);
            this.gameObject.SetActive(false);

            // Counting laps for AI
            if (this.gameObject.name == "LastPoint" && collider.gameObject.tag == "AI")
            {

                if (GameManager.cl_GameManager.b_FirstLapAI == false)
                {
                    GameManager.cl_GameManager.AIlaps++;
                    Debug.Log(GameManager.cl_GameManager.AIlaps);
                    GameManager.cl_GameManager.b_FirstLapAI = true;
                }
                if (GameManager.cl_GameManager.b_SecondLapAI == false)
                {
                    GameManager.cl_GameManager.AIlaps++;
                    Debug.Log(GameManager.cl_GameManager.AIlaps);
                    GameManager.cl_GameManager.b_SecondLapAI = true;
                }
                if (GameManager.cl_GameManager.b_ThirdLapAI == false)
                {
                    GameManager.cl_GameManager.AIlaps++;
                    Debug.Log(GameManager.cl_GameManager.AIlaps);
                    GameManager.cl_GameManager.b_ThirdLapAI = true;
                    b_AIwon = true;
                    Debug.Log("AI WON!!!");
                }


            }
            if (this.gameObject.name == "LastPoint" && b_AIwon == false)
            {
                Debug.Log("Player hits last point");
                GameManager.cl_GameManager.Playerlaps++;
            }
        }
    }

    void Update () {
		
	}
}
