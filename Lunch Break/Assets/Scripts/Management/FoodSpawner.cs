using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodSpawner : MonoBehaviour
{
    public  GameObject[] spawnableFoods;
    private int activeFoodIndex;
    [SerializeField]
    private int MIN_TIME = 1, MAX_TIME = 30, MIN_INDEX = 0, MAX_INDEX = 6;
    private bool canCheck;

    
    // Start is called before the first frame update
    void Start()
    {
        canCheck = false;
        StartCoroutine("SpawnNewFood");
        InvokeRepeating("CheckFood", 0f, 1f);
    }

    
    //private void Update()
    private void CheckFood()
    {
        if(!spawnableFoods[activeFoodIndex].activeSelf && canCheck)
        {
            canCheck = false;
            StartCoroutine("SpawnNewFood");
        }
    }
    

    IEnumerator SpawnNewFood()
    {
        float spawnTime = Random.Range(MIN_TIME, MAX_TIME);
        activeFoodIndex = Random.Range(MIN_INDEX, MAX_INDEX);
        yield return new WaitForSeconds(spawnTime);
        spawnableFoods[activeFoodIndex].SetActive(true);
        canCheck = true;
    }
    
    
}
