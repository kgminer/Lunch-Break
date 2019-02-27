using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private string spawnpointObjectName;
    private float timeRemaining;
    private GameObject camera;

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
        //camera = GameObject.Find("Main Camera");
        //Instantiate(player, spawningObject.transform.position, Quaternion.identity);
        //camera.GetComponent<CameraFollow>().target = player.transform;
    }
    
    // Update is called once per frame
    void Update()
    {
        UpdateTime();
        gameDisplay.SetTimeRemainingText(timeRemaining);
    }

    public void UpdateTime()
    {
        timeRemaining -= Time.deltaTime;
    }
}
