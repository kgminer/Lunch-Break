using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodSpawner : MonoBehaviour
{
    public GameObject foodItem;
    private const int MIN_TIME = 1, MAX_TIME = 30;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    IEnumerator SpawnNewFood()
    {
        float spawnTime = Random.Range(MIN_TIME, MAX_TIME);
        yield return new WaitForSeconds(spawnTime);
        Instantiate(foodItem, gameObject.transform.position, Quaternion.identity);
    }
    
    public void OnTriggerExit(Collider other)
    {
        if(other.tag == "Food")
        {
            Debug.Log("Food picked up spawning new food");
            StartCoroutine("SpawnNewFood");
        }
    }
}
