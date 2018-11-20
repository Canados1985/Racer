using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameModeCanvas : MonoBehaviour {

    Image bgImg;

    float i = 0;

	void Start () {
       //bgImg.GetComponent<Image>();
       
      // i = bgImg.color.a;
    }
	
	
	void Update () {



        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
           i += Time.deltaTime;

        }


	}
}
