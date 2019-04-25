using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private float timeRemaining;
    [SerializeField]
    private int scoreLimit;
    private bool outOfTime, scoreReached;
    private int gameState; //0 = Running, 1 = Game Over
    public static int scienceGeeksScore, bookWormsScore, jocksScore, winningScore;
    private const int VENDING_MACHINE_CAP_VALUE = 1, CENTRAL_CAP_VALUE = 3, NUM_NPCS = 6;

    public HUD gameDisplay;
    public static GameObject ScienceGeeksSpawnObject;
    public static GameObject JocksSpawnObject;
    public static GameObject BookWormsSpawnObject;
    public static GameObject RespawnObject;
    public  GameObject ScienceGeeksBase;
    public  GameObject JocksBase;
    public  GameObject BookWormsBase;
    public  GameObject RespawnPoint;
    public GameObject Player;
    public GameObject BookWormNPC;
    public GameObject SciGeekNPC;
    public GameObject JockNPC;
    private GameObject[] scienceGeeksNPCs;
    private GameObject[] bookWormsNPCs;
    private GameObject[] jocksNPCs;

    public static Transform centerCap;
    public static GameObject[] allCharacters;
    public static List<Transform> vendors;
    public static List<GameObject> caps;

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
        scienceGeeksNPCs = new GameObject[NUM_NPCS];
        bookWormsNPCs = new GameObject[NUM_NPCS];
        jocksNPCs = new GameObject[NUM_NPCS];
        CollectPositions();
        GameManager.ScienceGeeksSpawnObject = ScienceGeeksBase;
        GameManager.JocksSpawnObject = JocksBase;
        GameManager.BookWormsSpawnObject = BookWormsBase;
        GameManager.RespawnObject = RespawnPoint;
        InvokeRepeating("ScoreCapZones", 0.0f, 5f);

        PositionPlayers();
        Time.timeScale = 1f;
        Cursor.visible = false;
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
                    gameDisplay.SetScoreText(scoreLimit);
                    scoreReached = scienceGeeksScore >= scoreLimit || bookWormsScore >= scoreLimit || jocksScore >= scoreLimit;
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
                winningScore = Mathf.Max(scienceGeeksScore, bookWormsScore, jocksScore);
                gameDisplay.SetFinalScoreText();
                gameDisplay.OpenGameOverPanel();
                Cursor.visible = true;
                gameState = 2;
                break;
            case 2:
                Time.timeScale = 0f;
                break;
        }

        GameObject[] sciencegeeks = GameObject.FindGameObjectsWithTag("scienceGeek");
        GameObject[] jocks = GameObject.FindGameObjectsWithTag("jocks");
        GameObject[] bookworms = GameObject.FindGameObjectsWithTag("bookWorm");

        allCharacters = new GameObject[sciencegeeks.Length + jocks.Length + bookworms.Length];
        sciencegeeks.CopyTo(allCharacters, 0);
        jocks.CopyTo(allCharacters, sciencegeeks.Length);
        bookworms.CopyTo(allCharacters, sciencegeeks.Length + jocks.Length);
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
                Player.tag = "scienceGeek";
                SpawnScienceGeeksNPCs(NUM_NPCS - 1);
                SpawnJocksNPCs(NUM_NPCS);
                SpawnBookWormsNPCs(NUM_NPCS);
                break;

            case 1:
                SpawnScienceGeeksNPCs(NUM_NPCS);
                Player.transform.position = JocksSpawnObject.transform.position;
                Player.tag = "jocks";
                SpawnJocksNPCs(NUM_NPCS - 1);
                SpawnBookWormsNPCs(NUM_NPCS);
                break;

            case 2:
                SpawnScienceGeeksNPCs(NUM_NPCS);
                SpawnJocksNPCs(NUM_NPCS);
                Player.transform.position = BookWormsSpawnObject.transform.position;
                Player.tag = "bookWorm";
                SpawnBookWormsNPCs(NUM_NPCS - 1);
                break;
        }
        
    }

    private void CollectPositions()
    {
        centerCap = GameObject.FindGameObjectWithTag("Center Cap").transform;

        caps = new List<GameObject>();
        vendors = new List<Transform>();

        GameObject[] capcircles = GameObject.FindGameObjectsWithTag("Cap");
        foreach (GameObject circle in capcircles)
            caps.Add(circle);

        GameObject[] vending = GameObject.FindGameObjectsWithTag("Vending");
        foreach (GameObject machine in vending)
            vendors.Add(machine.transform);
    }

    private void SpawnScienceGeeksNPCs(int spawnNumber)
    {
        for (int i = 0; i < spawnNumber; i++)
        {
            scienceGeeksNPCs[i] = Instantiate(SciGeekNPC, ScienceGeeksSpawnObject.transform.position, Quaternion.identity);
        }
    }

    private void SpawnJocksNPCs(int spawnNumber)
    {
        for (int i = 0; i < spawnNumber; i++)
        {
            jocksNPCs[i] = Instantiate(JockNPC, JocksSpawnObject.transform.position, Quaternion.identity);
        }
    }

    private void SpawnBookWormsNPCs(int spawnNumber)
    {
        for (int i = 0; i < spawnNumber; i++)
        {
            bookWormsNPCs[i] = Instantiate(BookWormNPC, BookWormsSpawnObject.transform.position, Quaternion.identity);
        }
    }

    private void ScoreCapZones()
    {
        GameObject[] capZones = GameObject.FindGameObjectsWithTag("Cap");
        GameObject centralCapZone = GameObject.FindGameObjectWithTag("Center Cap");
        string centralCapOwner = centralCapZone.GetComponent<CapZone>().GetTeam();
        ScoreCap(centralCapOwner, CENTRAL_CAP_VALUE);
        foreach (GameObject cap in capZones)
        {
            string capOwner = cap.GetComponent<CapZone>().GetTeam();
            ScoreCap(capOwner, VENDING_MACHINE_CAP_VALUE);
        }
    }

    public static void setObjectLocation(GameObject gameObject, string location)
    {
        if(location == "scienceGeek")
        {
            gameObject.transform.position = ScienceGeeksSpawnObject.transform.position;
        }
        else if(location == "bookWorm")
        {
            gameObject.transform.position = BookWormsSpawnObject.transform.position;
        }
        else if(location == "jocks")
        {
            gameObject.transform.position = JocksSpawnObject.transform.position;
        }
        else if(location == "respawn")
        {
            gameObject.transform.position = RespawnObject.transform.position;
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
