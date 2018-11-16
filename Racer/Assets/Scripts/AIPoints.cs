using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIPoints : MonoBehaviour {

    public static AIPoints cl_AIPoints;

    public GameObject go_CamaroAI1;
    public GameObject go_CamaroAI2;
    public GameObject go_CamaroAI3;


    public float f_countTurnsForAI1 = 0;
    public float f_countTurnsForAI2 = 0;
    public float f_countTurnsForAI3 = 0;

    public GameObject[] AIpoints;
    

    public int temp1 = 0;
    public int temp2 = 0;
    public int temp3 = 0;

    public bool[] b_pointActiveAI1;
    public bool[] b_pointActiveAI2;
    public bool[] b_pointActiveAI3;


    void Start () {



        // Turn off Mesh Renderer for AI points
        for (int i = 0; i < AIpoints.Length; i++)
        {
            AIpoints[i].GetComponent<MeshRenderer>().enabled = false;

            AIpoints[i].GetComponent<Transform>();


            //Turn off Last point for AI on Start
            if (AIpoints[i].gameObject.name == "LastPointAI")
            {
                AIpoints[i].SetActive(false);
            }

        }
        
    }


    private void OnTriggerEnter(Collider collider)
    {
        
        for (int i = 0; i< AIpoints.Length; i++)
        {

            //// CHECKING COLLISIONS FOR CAMARO1 STARTS HERE ------------------------------------------>

            //Turn on Last Point For AI1
            if (collider.gameObject.name == "CamaroAI01" && AIpoints[i].gameObject.name == "LastPointAI") 
            {
                AIpoints[i].SetActive(true);
                AIpoints[i].GetComponent<AIPoints>().b_pointActiveAI1[i] = false;

            }

            //Check collision with Last Point Only for AI1
            if (collider.gameObject.name == "CamaroAI01" && this.gameObject == AIpoints[i] && AIpoints[i].gameObject.name == "LastPointAI")
            {
                GameManager.cl_GameManager.AI1laps++;
                Debug.Log(GameManager.cl_GameManager.AI1laps);
                if (i - 1 == temp1)
                {

                    for (int t = 0; t < AIpoints.Length; t++)
                    {

                        AIpoints[temp1].GetComponent<AIPoints>().b_pointActiveAI1[temp1] = false;
                        AIpoints[t].GetComponent<AIPoints>().temp1 = 0;
                        
                    }
                   

                }

                Debug.Log("CAMARO1 WITH " + AIpoints[i] + " POINT FROM ARRAY");

                for (int j = 0; j < b_pointActiveAI1.Length; j++)
                {
                    b_pointActiveAI1[j] = false;

                    if (i == j)
                    {
                        b_pointActiveAI1[j] = true;

                    }
                }
            }

            //Check Collisions with all others check points for AI1
            if (collider.gameObject.name == "CamaroAI01" && this.gameObject == AIpoints[i] && AIpoints[i].gameObject.name != "LastPointAI")
               {


                    if (i - 1 == temp1)
                    {

                        for (int t = 0; t < AIpoints.Length; t++)
                        {
                       
                        AIpoints[temp1].GetComponent<AIPoints>().b_pointActiveAI1[temp1] = false;
                        AIpoints[t].GetComponent<AIPoints>().temp1++;



                    }

                    Debug.Log(temp1);

                }

                Debug.Log("CAMARO1 WITH " + AIpoints[i] + " POINT FROM ARRAY");

                for (int j = 0; j < b_pointActiveAI1.Length; j++)
                {
                    b_pointActiveAI1[j] = false;
                     
                    if (i == j)
                    {
                        b_pointActiveAI1[j] = true;
                      
                    }
                }



            }
            //// CHECKING COLLISIONS FOR CAMARO1 ENDS HERE ------------------------------------------>







            //// CHECKING COLLISIONS FOR CAMARO2 STARTS HERE ------------------------------------------>

            //Turn on Last Point For AI2
            if (collider.gameObject.name == "CamaroAI02" && AIpoints[i].gameObject.name == "LastPointAI")
            {
                AIpoints[i].SetActive(true);
                AIpoints[i].GetComponent<AIPoints>().b_pointActiveAI2[i] = false;

            }


            //Check collision with Last Point Only for AI2
            if (collider.gameObject.name == "CamaroAI02" && this.gameObject == AIpoints[i] && AIpoints[i].gameObject.name == "LastPointAI")
            {
                GameManager.cl_GameManager.AI2laps++;
                Debug.Log(GameManager.cl_GameManager.AI2laps);
                if (i - 1 == temp2)
                {

                    for (int t = 0; t < AIpoints.Length; t++)
                    {

                        AIpoints[temp2].GetComponent<AIPoints>().b_pointActiveAI2[temp2] = false;
                        AIpoints[t].GetComponent<AIPoints>().temp2 = 0;

                    }


                }

                Debug.Log("CAMARO2 WITH " + AIpoints[i] + " POINT FROM ARRAY");

                for (int j = 0; j < b_pointActiveAI2.Length; j++)
                {
                    b_pointActiveAI2[j] = false;

                    if (i == j)
                    {
                        b_pointActiveAI2[j] = true;

                    }
                }
            }

            //Check Collisions with all others check points for AI2
            if (collider.gameObject.name == "CamaroAI02" && this.gameObject == AIpoints[i] && AIpoints[i].gameObject.name != "LastPointAI")
            {


                if (i - 1 == temp2)
                {

                    for (int t = 0; t < AIpoints.Length; t++)
                    {

                        AIpoints[temp2].GetComponent<AIPoints>().b_pointActiveAI2[temp2] = false;
                        AIpoints[t].GetComponent<AIPoints>().temp2++;



                    }

                    Debug.Log(temp2);

                }

                Debug.Log("CAMARO2 WITH " + AIpoints[i] + " POINT FROM ARRAY");

                for (int j = 0; j < b_pointActiveAI2.Length; j++)
                {
                    b_pointActiveAI2[j] = false;

                    if (i == j)
                    {
                        b_pointActiveAI2[j] = true;

                    }
                }



            }
            //// CHECKING COLLISIONS FOR CAMARO2 ENDS HERE ------------------------------------------>









            //// CHECKING COLLISIONS FOR CAMARO3 STARTS HERE ------------------------------------------>

            //Turn on Last Point For AI3
            if (collider.gameObject.name == "CamaroAI03" && AIpoints[i].gameObject.name == "LastPointAI")
            {
                AIpoints[i].SetActive(true);
                AIpoints[i].GetComponent<AIPoints>().b_pointActiveAI3[i] = false;

            }


            //Check collision with Last Point Only for AI3
            if (collider.gameObject.name == "CamaroAI03" && this.gameObject == AIpoints[i] && AIpoints[i].gameObject.name == "LastPointAI")
            {
                GameManager.cl_GameManager.AI3laps++;
                Debug.Log(GameManager.cl_GameManager.AI3laps);
                if (i - 1 == temp3)
                {

                    for (int t = 0; t < AIpoints.Length; t++)
                    {

                        AIpoints[temp3].GetComponent<AIPoints>().b_pointActiveAI3[temp3] = false;
                        AIpoints[t].GetComponent<AIPoints>().temp3 = 0;

                    }


                }

                Debug.Log("CAMARO3 WITH " + AIpoints[i] + " POINT FROM ARRAY");

                for (int j = 0; j < b_pointActiveAI3.Length; j++)
                {
                    b_pointActiveAI3[j] = false;

                    if (i == j)
                    {
                        b_pointActiveAI3[j] = true;

                    }
                }
            }

            //Check Collisions with all others check points for AI3
            if (collider.gameObject.name == "CamaroAI03" && this.gameObject == AIpoints[i] && AIpoints[i].gameObject.name != "LastPointAI")
            {


                if (i - 1 == temp3)
                {

                    for (int t = 0; t < AIpoints.Length; t++)
                    {

                        AIpoints[temp3].GetComponent<AIPoints>().b_pointActiveAI3[temp3] = false;
                        AIpoints[t].GetComponent<AIPoints>().temp3++;



                    }

                    Debug.Log(temp3);

                }

                Debug.Log("CAMARO3 WITH " + AIpoints[i] + " POINT FROM ARRAY");

                for (int j = 0; j < b_pointActiveAI3.Length; j++)
                {
                    b_pointActiveAI3[j] = false;

                    if (i == j)
                    {
                        b_pointActiveAI3[j] = true;

                    }
                }



            }
            //// CHECKING COLLISIONS FOR CAMARO2 ENDS HERE ------------------------------------------>


        }

    }


    void Update () {






        for (int i = 0; i < AIpoints.Length; i++)
        {
            AIpoints[i].GetComponent<Transform>();
        }


    }













}
