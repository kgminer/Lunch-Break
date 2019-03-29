using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private float timeRemaining;
    private bool outOfTime, scoreReached;
    private int gameState; //0 = Running, 1 = Game Over
    public static int scienceGeeksScore, bookWormsScore, jocksScore;
    private const int VENDING_MACHINE_CAP_VALUE = 1, CENTRAL_CAP_VALUE = 3;
    //private GameObject camera;

    public HUD gameDisplay;
    public GameObject ScienceGeeksSpawnObject;
    public GameObject JocksSpawnObject;
    public GameObject BookWormsSpawnObject;
    public GameObject Player;
    public GameObject NPC1Team1;
    public GameObject NPC2Team1;
    public GameObject NPC1Team2;
    public GameObject NPC1Team3;
    public GameObject NPC2Team3;
    public GameObject foodSpawnPosition1;
    public GameObject foodSpawnPosition2;
    public GameObject foodSpawnPosition3;
    public GameObject foodSpawnPosition4;
    public GameObject foodItem;
    

    // Start is called before the first frame update
    public void Start()
    {
        Init();
    }


    public void Init()
    {
        timeRemaining = 300f;
        outOfTime = false;
        scoreReached = false;
        scienceGeeksScore = 0;
        bookWormsScore = 0;
        jocksScore = 0;
        InvokeRepeating("SpawnFood", 0.0f, 10f);
        //InvokeRepeating("ScoreCapZones", 0.0f, 3f);
        PositionPlayers();
        Time.timeScale = 1f;
        //camera = GameObject.Find("Main Camera");
        //Instantiate(player, spawningObject.transform.position, Quaternion.identity);
        //camera.GetComponent<CameraFollow>().target = player.transform;
    }
    
    // Update is called once per frame
    void Update()
    {
        switch (gameState)
        {
            case 0:
                UpdateTime();
                int minutes = Mathf.FloorToInt(timeRemaining / 60F);
                int seconds = Mathf.FloorToInt(timeRemaining - minutes * 60);
                outOfTime = minutes <= 0 && seconds <= 0;
                if (!outOfTime)
                {
                    gameDisplay.SetTimeRemainingText(minutes, seconds);
                    gameDisplay.SetScoreText();
                    scoreReached = scienceGeeksScore >= 100 || bookWormsScore >= 100 || jocksScore >= 100;
                    if(scoreReached)
                    {
                        gameState = 1;
                    }
                }
                else
                {
                    outOfTime = true;
                    gameDisplay.SetTimeRemainingText(0, 0);
                    gameState = 1;
                }
                break;
            case 1:
                gameDisplay.SetFinalScoreText();
                gameDisplay.OpenGameOverPanel();
                break;
        }
        
        
    }

    public void UpdateTime()
    {
        timeRemaining -= Time.deltaTime;
    }

    public void PositionPlayers()
    {
        /*
        switch (TeamSelection.GetTeam())
        {
            case 0:
                Player.transform.position = ScienceGeeksSpawnObject.transform.position;
                Player.tag = scienceGeek;
                SpawnScienceGeeksNPCs(NUM_NPCS - 1);
                SpawnJocksNPCs(NUM_NPCS);
                SpawnBookWormsNPCs(NUM_NPCS);
                break;

            case 1:
                SpawnScienceGeeksNPCs(NUM_NPCS);
                Player.transform.position = JocksSpawnObject.transform.position;
                Player.tag = jock;
                SpawnJocksNPCs(NUM_NPCS - 1);
                SpawnBookWormsNPCs(NUM_NPCS);
                break;

            case 2:
                SpawnScienceGeeksNPCs(NUM_NPCS);
                SpawnJocksNPCs(NUM_NPCS);
                Player.transform.position = BookWormsSpawnObject.transform.position;
                Player.tag = bookWorm;
                SpawnBookWormsNPCs(NUM_NPCS - 1);
                break;
        }
        */
        Player.transform.position = ScienceGeeksSpawnObject.transform.position;
        NPC1Team2.transform.position = ScienceGeeksSpawnObject.transform.position;

        NPC1Team1.transform.position = BookWormsSpawnObject.transform.position;
        NPC2Team1.transform.position = BookWormsSpawnObject.transform.position;

        NPC1Team3.transform.position = JocksSpawnObject.transform.position;
        NPC2Team3.transform.position = JocksSpawnObject.transform.position;
    }

    private void SpawnFood()
    {
        Instantiate(foodItem, foodSpawnPosition1.transform.position, Quaternion.identity);
        Instantiate(foodItem, foodSpawnPosition2.transform.position, Quaternion.identity);
        Instantiate(foodItem, foodSpawnPosition3.transform.position, Quaternion.identity);
        Instantiate(foodItem, foodSpawnPosition4.transform.position, Quaternion.identity);
    }

    private void ScoreCapZones()
    {
        GameObject[] capZones = GameObject.FindGameObjectsWithTag("Cap");
        GameObject centralCapZone = GameObject.FindGameObjectWithTag("Center Cap");
        //string centralCapOwner = centralCapZone.getTeam();
        //ScoreCap(centralCapZone, CENTRAL_CAP_VALUE);
        foreach (GameObject cap in capZones)
        {
            //string capOwner = cap.getTeam();
            //ScoreCap(capOwner, VENDING_MACHINE_CAP_VALUE);
        }
    }

    private void ScoreCap(string owner, int pointValue)
    {
        if(owner == "scienceGeek")
        {
            GameManager.scienceGeeksScore += pointValue;
        }
        else if(owner == "bookWorm")
        {
            GameManager.bookWormsScore += pointValue;
        }
        else if(owner == "jocks")
        {
            GameManager.jocksScore += pointValue;
        }
    }
}
