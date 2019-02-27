using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private string spawnpointObjectName;
    private float timeRemaining;
    private bool outOfTime;
    private int gameState; //0 = Running, 1 = Game Over
    //private GameObject camera;

    public HUD gameDisplay;
    public GameObject spawningObject;
    public GameObject player;
    

    // Start is called before the first frame update
    public void Start()
    {
        //DontDestroyOnLoad(this);
        Init();
    }


    public void Init()
    {
        timeRemaining = 300f;
        outOfTime = false;
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
                if(minutes <= 0 && seconds <= 0)
                {
                    outOfTime = true;
                    gameDisplay.SetTimeRemainingText(0, 0);
                    gameState = 1;
                }
                if (!outOfTime)
                {
                    gameDisplay.SetTimeRemainingText(minutes, seconds);
                }
                break;
            case 1:
                gameDisplay.OpenGameOverPanel();
                break;
        }
        
        
    }

    public void UpdateTime()
    {
        timeRemaining -= Time.deltaTime;
    }
}
