using System.Collections;
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

            collider.gameObject.GetComponent<UnityStandardAssets.Vehicles.Car.CarController>().UpdatePositionAndRotation();

        }
        //player Passing Last Point and Getting Results
        if (collider.gameObject.tag == "Player" && this.gameObject.name == "LastPoint")
        {
            collider.gameObject.GetComponent<UnityStandardAssets.Vehicles.Car.CarController>().UpdatePositionAndRotation();

            if (GameManager.cl_GameManager.Playerlaps == 2)
            {
                Timer.cl_Timer.GetsResultsForPlayerLap3();
               // Debug.Log("Player hits last point third time");
                GameManager.cl_GameManager.Playerlaps++;
            }

            if (GameManager.cl_GameManager.Playerlaps == 1)
            {
                Timer.cl_Timer.GetsResultsForPlayerLap2();
               // Debug.Log("Player hits last point second time");
                GameManager.cl_GameManager.Playerlaps++;
            }


            if (GameManager.cl_GameManager.Playerlaps == 0)
            {
               // Debug.Log("Player hits last point first time");
                Timer.cl_Timer.GetsResultsForPlayerLap1();
                GameManager.cl_GameManager.Playerlaps++;
            }
            nextPoint.SetActive(true);
            this.gameObject.SetActive(false);

        }

    }

    void Update () {
		
	}
}
