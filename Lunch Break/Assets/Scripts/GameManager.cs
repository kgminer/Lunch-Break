using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private float timeRemaining;
    private bool outOfTime, scoreReached;
    private int gameState; //0 = Running, 1 = Game Over
    private int scienceGeeksScore, bookWormsScore, jocksScore;
    //private GameObject camera;

    public HUD gameDisplay;
    public GameObject ScienceGeeksSpawnObject;
    public GameObject JocksSpawnObject;
    public GameObject BookWormsSpawnObject;
    public GameObject Player;
    public GameObject NPC1;
    public GameObject NPC2;
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
        PositionPlayers();
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
                    gameDisplay.SetScoreText(scienceGeeksScore, bookWormsScore, jocksScore);
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
                gameDisplay.SetFinalScoreText(scienceGeeksScore, bookWormsScore, jocksScore);
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
        switch (TeamSelection.GetTeam())
        {
            case 0:
                Player.transform.position = ScienceGeeksSpawnObject.transform.position;
                NPC1.transform.position = JocksSpawnObject.transform.position;
                NPC2.transform.position = BookWormsSpawnObject.transform.position;
                break;
            case 1:
                Player.transform.position = JocksSpawnObject.transform.position;
                NPC1.transform.position = ScienceGeeksSpawnObject.transform.position;
                NPC2.transform.position = BookWormsSpawnObject.transform.position;
                break;
            case 2:
                Player.transform.position = BookWormsSpawnObject.transform.position;
                NPC1.transform.position = JocksSpawnObject.transform.position;
                NPC2.transform.position = ScienceGeeksSpawnObject.transform.position;
                break;
        }
    }

    private void SpawnFood()
    {
        Instantiate(foodItem, foodSpawnPosition1.transform.position, Quaternion.identity);
        Instantiate(foodItem, foodSpawnPosition2.transform.position, Quaternion.identity);
        Instantiate(foodItem, foodSpawnPosition3.transform.position, Quaternion.identity);
        Instantiate(foodItem, foodSpawnPosition4.transform.position, Quaternion.identity);
    }
}
