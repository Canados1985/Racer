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

        }
        if (this.gameObject.name == "LastPoint" && b_AIwon == false)
        {
            Debug.Log("Player hits last point");
            GameManager.cl_GameManager.Playerlaps++;
        }






    }

    void Update () {
		
	}
}
