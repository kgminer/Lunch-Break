using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundaryDestroyer : MonoBehaviour
{

   public GameObject spawnBurger;
    public GameObject spawnDonut;
    public GameObject spawnDrink;
    public GameObject spawnFries;

    // When an object falls through the grass, spawn a version of it on the floor and delete the thrown one. 
    // This is a missed projectile throw
    void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<Burger>())
        {
            Vector3 hitPosition = other.gameObject.transform.position;
            Quaternion hitRotation = other.gameObject.transform.rotation;
            hitPosition.y = 0.23f;

            Instantiate(spawnBurger, hitPosition, hitRotation);
        }

        else if (other.GetComponent<Donut>())
        {
            Vector3 hitPosition = other.gameObject.transform.position;
            Quaternion hitRotation = other.gameObject.transform.rotation;
            hitPosition.y = 0.23f;

            Instantiate(spawnDonut, hitPosition, hitRotation);
        }

        else if (other.GetComponent<Drink>())
        {
            Vector3 hitPosition = other.gameObject.transform.position;
            Quaternion hitRotation = other.gameObject.transform.rotation;
            hitPosition.y = 0.23f;

            Instantiate(spawnDrink, hitPosition, hitRotation);
        }

        else if (other.GetComponent<Burger>())
        {
            Vector3 hitPosition = other.gameObject.transform.position;
            Quaternion hitRotation = other.gameObject.transform.rotation;
            hitPosition.y = 0.23f;

            Instantiate(spawnFries, hitPosition, hitRotation);
        }

        Destroy(other.gameObject);
    }
}
