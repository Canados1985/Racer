using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityStandardAssets.Vehicles.Car;

public class GameManager : MonoBehaviour {

    public static GameManager cl_GameManager;

    public GameObject go_Player;


    public GameObject[] gameStates;
    public enum GameStates { MENU, GAME, PAUSE, GAMEOVER, GAMEWIN }
    private GameStates e_gameStates;
    public GameObject go_MenuCamera;


    public GameObject[] winPoints; // List Of WinPoints
    public GameObject go_winPoints; //
    public GameObject[] carsAI; // List Of Cars in Race under control AI

    public GameObject go_canvasGameMode;
    public GameObject go_Player1stPlace;
    public GameObject go_Player2stPlace;
    public GameObject go_Player3stPlace;

    public GameObject go_map; // UI minimap
    public GameObject go_PlayerMapMark; // UI player mark on minimap
    public GameObject go_AI1Mark; // UI AI1 mark on minimap
    public GameObject go_AI2Mark; // UI AI1 mark on minimap
    public GameObject go_AI3Mark; // UI AI1 mark on minimap

    public Transform playerTransform; // Player position
    public Transform AI1Transform; // AI1 position
    public Transform AI2Transform; // AI2 position
    public Transform AI3Transform; // AI3 position

    public GameObject go_leftMirror; // UI left mirror
    public GameObject go_rightMirror; // UI right mirror

    public bool b_Start = false;
    public bool b_GameModeIsActive = false;
    public bool b_GameIsPaused = false;
    private bool b_GameModeThemePlaying = false;
    private bool b_count_3 = false;
    private bool b_count_2 = false;
    private bool b_count_1 = false;
    public float f_countDown = 3f;

    public GameObject go_startCount3; // UI count 3
    public GameObject go_startCount2; // UI count 2
    public GameObject go_startCount1; // UI count 1
    public GameObject go_startCountGO;  // UI race starts

    public GameObject go_2LapsLeft; 
    public GameObject go_1LapLeft;
    public GameObject go_youWin;
    public GameObject go_youLose;

    
    public int AI1laps = 0;
    public int AI2laps = 0;
    public int AI3laps = 0;
    public bool b_AI1Won = false;
    public bool b_AI2Won = false;
    public bool b_AI3Won = false;
    public bool b_PlayerWon = false;


    float f_count = 5;
    public int Playerlaps = 0;
    bool b_FirstLapDone = false;
    bool b_SecondLapDone = false;
    bool b_ThirdLapDone = false;

    bool b_1stRacaTrackPlaing = false;
    bool b_2ndRaceTrackPlaying = false;


    void Start () {

        cl_GameManager = this;

        go_Player1stPlace.SetActive(false);
        go_Player2stPlace.SetActive(false);
        go_Player3stPlace.SetActive(false);
        go_startCount3.SetActive(true);
        go_startCount2.SetActive(false);
        go_startCount1.SetActive(false);
        go_startCountGO.SetActive(false);
        go_2LapsLeft.SetActive(false);
        go_1LapLeft.SetActive(false);
        go_youWin.SetActive(false);
        go_youLose.SetActive(false);
        e_gameStates = GameStates.MENU;
        ChangeGameStates();
        AI1laps = 0;
        AI2laps = 0;
        AI3laps = 0;
        Playerlaps = 0;
    }


    //Start Race Mode Here --->
    public void StartRaceMode()
    {
        e_gameStates = GameStates.GAME;
        ChangeGameStates();
        FindObjectOfType<AudioManager>().Pause("menuTheme");
        b_GameModeIsActive = true;
        go_MenuCamera.SetActive(false);

    }
    //Start Playing Race Theme
    void GameModeAudioThemeStart()
    {
        //Let game pick the soundtrack for game mode --->
        float i = Random.Range(0f, 5f);
        //Debug.Log("Start race track" + i);
        if (i <= 3f)
        { FindObjectOfType<AudioManager>().Play("raceTheme"); b_1stRacaTrackPlaing = true; }
        if (i > 3f)
        { FindObjectOfType<AudioManager>().Play("raceTheme2"); b_2ndRaceTrackPlaying = true; }
           
    }
    // Set Game to Pause Mode
    void PauseGame()
    {
        e_gameStates = GameStates.PAUSE;
        ChangeGameStates();
        go_canvasGameMode.SetActive(false);
        b_GameIsPaused = true;
        Time.timeScale = 0f;

        //Set soundtrack on pause based on which one is playing 
        if (b_1stRacaTrackPlaing == true)
        { FindObjectOfType<AudioManager>().Pause("raceTheme"); }
        if (b_2ndRaceTrackPlaying == true)
        { FindObjectOfType<AudioManager>().Pause("raceTheme2"); }
        


    }
    // Unpause Game
    void UnpauseGame()
    {
        e_gameStates = GameStates.GAME;
        ChangeGameStates();
        go_canvasGameMode.SetActive(true);
        b_GameIsPaused = false;
        Time.timeScale = 1f;

        //Unpause soundtrack based on which one is playing 
        if (b_1stRacaTrackPlaing == true)
        { FindObjectOfType<AudioManager>().Unpause("raceTheme"); }
        if (b_2ndRaceTrackPlaying == true)
        { FindObjectOfType<AudioManager>().Unpause("raceTheme2"); }

    }

    // Game Over State
    internal void GameOver()
    {

        e_gameStates = GameStates.GAMEOVER;
        ChangeGameStates();

        if (b_1stRacaTrackPlaing == true)
        { FindObjectOfType<AudioManager>().Stop("raceTheme"); }
        if (b_2ndRaceTrackPlaying == true)
        { FindObjectOfType<AudioManager>().Stop("raceTheme2"); }

        FindObjectOfType<AudioManager>().Unpause("menuTheme");
        b_GameModeIsActive = true;
        go_Player.SetActive(false);
        go_MenuCamera.SetActive(true);


    }

    // Game Win State
    internal void GameWin()
    {

        e_gameStates = GameStates.GAMEWIN;
        ChangeGameStates();

        if (b_1stRacaTrackPlaing == true)
        { FindObjectOfType<AudioManager>().Stop("raceTheme"); }
        if (b_2ndRaceTrackPlaying == true)
        { FindObjectOfType<AudioManager>().Stop("raceTheme2"); }

        FindObjectOfType<AudioManager>().Unpause("menuTheme");
        b_GameModeIsActive = true;
        go_Player.SetActive(false);
        go_MenuCamera.SetActive(true);

        //Calculating 1st place
        if (b_AI1Won == false && b_AI2Won == false && b_AI3Won == false && b_PlayerWon == true)
        {
            go_Player1stPlace.SetActive(true);


        }
        //Calculating 2st place
        if (b_AI1Won == true && b_AI2Won == false && b_AI3Won == false && b_PlayerWon == true || b_AI1Won == false && b_AI2Won == true && b_AI3Won == false && b_PlayerWon == true || b_AI1Won == false && b_AI2Won == false && b_AI3Won == true && b_PlayerWon == true)
        {
            go_Player2stPlace.SetActive(true);


        }
        //Calculating 3st place
        if (b_AI1Won == true && b_AI2Won == true && b_AI3Won == false && b_PlayerWon == true || b_AI1Won == true && b_AI2Won == false && b_AI3Won == true && b_PlayerWon == true || b_AI1Won == false && b_AI2Won == true && b_AI3Won == true && b_PlayerWon == true)
        {
            go_Player3stPlace.SetActive(true);


        }
    }


    // Reload Race
    public void ReloadRace()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
        
    }


    // Game Exit
    public void GameExit()
    {
        Application.Quit();
    }

    // Switching between scenes based on ENUM
    private void ChangeGameStates()
    {

        switch (e_gameStates)
        {
            case GameStates.MENU:
                go_Player.SetActive(false);
                gameStates[0].SetActive(true); // MENU
                gameStates[1].SetActive(false); // GAME
                gameStates[2].SetActive(false); // PAUSE
                gameStates[3].SetActive(false); // GAMEOVER
                gameStates[4].SetActive(false); // GAMEWIN
                FindObjectOfType<AudioManager>().Play("menuTheme");

                break;

            case GameStates.GAME:
                go_Player.SetActive(true);
                gameStates[0].SetActive(false); // MENU
                gameStates[1].SetActive(true); // GAME
                gameStates[2].SetActive(false); // PAUSE
                gameStates[3].SetActive(false); // GAMEOVER
                gameStates[4].SetActive(false); // GAMEWIN

                break;

            case GameStates.PAUSE:
                gameStates[0].SetActive(false); // MENU
                gameStates[1].SetActive(true); // GAME
                gameStates[2].SetActive(true); // PAUSE
                gameStates[3].SetActive(false); // GAMEOVER
                gameStates[4].SetActive(false); // GAMEWIN
                break;

            case GameStates.GAMEOVER:
                gameStates[0].SetActive(false); // MENU
                gameStates[1].SetActive(false); // GAME
                gameStates[2].SetActive(false); // PAUSE
                gameStates[3].SetActive(true); // GAMEOVER
                gameStates[4].SetActive(false); // GAMEWIN

                break;

            case GameStates.GAMEWIN:
                gameStates[0].SetActive(false); // MENU
                gameStates[1].SetActive(false); // GAME
                gameStates[2].SetActive(false); // PAUSE
                gameStates[3].SetActive(false); // GAMEOVER
                gameStates[4].SetActive(true); // GAMEWIN
                

                break;

        }


    }


    private void FixedUpdate()
    {

        //Start CountDown for Race
        if (f_countDown <= 2 && gameStates[1].activeSelf == true)
        {
            go_startCount3.SetActive(false);
            go_startCount2.SetActive(true);
           

            if (b_count_2 == false) { FindObjectOfType<AudioManager>().Play("countDown"); b_count_2 = true; }

        }
        if (f_countDown <= 1 && gameStates[1].activeSelf == true)
        {
            go_startCount2.SetActive(false);
            go_startCount1.SetActive(true);

            if (b_count_1 == false) { FindObjectOfType<AudioManager>().Play("countDown"); b_count_1 = true; }
        }
        if (f_countDown <= 0 && gameStates[1].activeSelf == true)
        {
            go_startCount1.SetActive(false);
            go_startCountGO.SetActive(true);
            b_Start = true;
            
            if (b_GameModeThemePlaying == false)
            { FindObjectOfType<AudioManager>().Play("startRace"); GameModeAudioThemeStart(); b_GameModeThemePlaying = true; }
            
        }
        if (f_countDown <= -1 && gameStates[1].activeSelf == true)
        {
            go_startCountGO.SetActive(false);
            
        }
        if (f_countDown > -1 && gameStates[1].activeSelf == true)
        {
            f_countDown -= Time.deltaTime / 1.5f;
            if (b_count_3 == false) { FindObjectOfType<AudioManager>().Play("countDown"); b_count_3 = true; }
        }

        //Checking lap 1
        if (Playerlaps == 1 && b_FirstLapDone == false && gameStates[1].activeSelf == true)
        {
            go_2LapsLeft.SetActive(true); // Activation UI text
            
            f_count -= Time.deltaTime;
            //Debug.Log(f_count);
            if (f_count <= 0)
            {
                go_2LapsLeft.SetActive(false);
                f_count = 5f;
                b_FirstLapDone = true;
            }
        }
        //Checking lap 2
        if (Playerlaps == 2 && b_SecondLapDone == false && gameStates[1].activeSelf == true)
        {
            go_1LapLeft.SetActive(true); // Activation UI text

            f_count -= Time.deltaTime;
            //Debug.Log(f_count);
            if (f_count <= 0)
            {
                go_1LapLeft.SetActive(false);
                f_count = 5f;
                b_SecondLapDone = true;
            }
        }
        //Checking lap3
        if (Playerlaps == 3 && b_ThirdLapDone == false && gameStates[1].activeSelf == true)
        {
            //Here also need to add win or lose condotions
            b_PlayerWon = true;
            go_youWin.SetActive(true);
            
            f_count -= Time.deltaTime;
            //Debug.Log(f_count);
            if (f_count <= 0)
            {
                go_youWin.SetActive(false);
                f_count = 5f;
                b_ThirdLapDone = true;
            }
        }


    }

    void Update () {


        //MiniMap Activation/Deactivation
        if (Input.GetKeyDown(KeyCode.M))
        {
            
            go_map.SetActive(!go_map.activeInHierarchy);
        }
        //Mirrors  Activation/Deactivation
        if (Input.GetKeyDown(KeyCode.R))
        {
            
            go_leftMirror.SetActive(!go_leftMirror.activeInHierarchy);
            go_rightMirror.SetActive(!go_rightMirror.activeInHierarchy);
        }

        //Update position for marks on miniMap
        go_PlayerMapMark.transform.position = new Vector3(playerTransform.transform.position.x, 175, playerTransform.transform.position.z);
        go_PlayerMapMark.transform.rotation = new Quaternion(playerTransform.transform.rotation.x, playerTransform.transform.rotation.y, playerTransform.transform.rotation.z, playerTransform.transform.rotation.w);
        go_AI1Mark.transform.position = new Vector3(AI1Transform.transform.position.x, 175, AI1Transform.transform.position.z);
        go_AI1Mark.transform.rotation = new Quaternion(AI1Transform.transform.rotation.x, AI1Transform.transform.rotation.y, AI1Transform.transform.rotation.z, AI1Transform.transform.rotation.w);
        go_AI2Mark.transform.position = new Vector3(AI2Transform.transform.position.x, 175, AI2Transform.transform.position.z);
        go_AI2Mark.transform.rotation = new Quaternion(AI2Transform.transform.rotation.x, AI2Transform.transform.rotation.y, AI2Transform.transform.rotation.z, AI2Transform.transform.rotation.w);
        go_AI3Mark.transform.position = new Vector3(AI3Transform.transform.position.x, 175, AI3Transform.transform.position.z);
        go_AI3Mark.transform.rotation = new Quaternion(AI3Transform.transform.rotation.x, AI3Transform.transform.rotation.y, AI3Transform.transform.rotation.z, AI3Transform.transform.rotation.w);

        //Pause and Unpause Game
        if (Input.GetKeyDown(KeyCode.Escape) && b_GameModeIsActive == true)
        {

            if (b_GameIsPaused)
            {

                UnpauseGame();
            }
            else
            {
               PauseGame();
            }

        }

        //Checking if player wins
        if (b_PlayerWon == true)
        {
             GameWin();
              
        }

        //Checking win condition for AI1/AI2/AI3
        if (AI1laps >= 3)
        {
            Debug.Log("AI1 WON!!!");
            b_AI1Won = true;
            
            
        }
        if (AI2laps >= 3)
        {
            Debug.Log("AI2 WON!!!");
            b_AI2Won = true;
           
        }
        if (AI3laps >= 3)
        {
            Debug.Log("AI3 WON!!!");
            b_AI3Won = true;
            
        }
        //Game Over Condition
        if (b_AI1Won == true && b_AI2Won == true && b_AI3Won == true)
        {
            GameOver();
        }

        
    }
}
