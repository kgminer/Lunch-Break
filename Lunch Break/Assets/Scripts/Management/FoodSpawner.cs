using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodSpawner : MonoBehaviour
{
    public  GameObject[] spawnableFoods;
    private int activeFoodIndex;
    private const int MIN_TIME = 1, MAX_TIME = 30, MIN_INDEX = 0, MAX_INDEX = 5;

    /*
    // Start is called before the first frame update
    void Start()
    {
        spawnableFoods = new GameObject[6];
        Debug.Log("Starting Coroutine");
        StartCoroutine("SpawnNewFood");
    }

    
    private void Update()
    {
        if(!spawnableFoods[activeFoodIndex].activeSelf)
        {
            StartCoroutine("SpawnNewFood");
        }
    }
    */

    IEnumerator SpawnNewFood()
    {
        float spawnTime = Random.Range(MIN_TIME, MAX_TIME);
        Debug.Log("Time to spawn is: " + spawnTime);
        activeFoodIndex = Random.Range(MIN_INDEX, MAX_INDEX);
        Debug.Log("Spawn index is: " + activeFoodIndex);
        yield return new WaitForSeconds(spawnTime);
        Debug.Log("Wait complete");
        spawnableFoods[activeFoodIndex].SetActive(true);
        Debug.Log("Food is spawned");
    }
    
    
}
