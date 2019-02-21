using UnityEngine;
using UnityEngine.AI;

public class AI1 : MonoBehaviour
{
    // import Transforms of food bars
    // import Transforms of vending machines
    // import food types
    // import match time and scores

    public float viewRad = 25f;                 // distacne at which enemies can be detected
    public float targetingRad = 10f;            // distance at which character turns to face enemy
    public float strafeRad = 5f;                // distance at which character attacks/approaches enemy
    public float health = 3f;
    public float money = 0f;

    NavMeshAgent nav;
    Transform nearestEnemy;
    Transform vendor;
    float enemyDistance;

    GameObject[] Ammo; // maybe need a list

    private void Awake()
    {
        //nearestEnemy = FindNearestEnemy();

        vendor = FindNearestVendor();

        nav = GetComponent<NavMeshAgent>();

    }

    private void Update()
    {
        nearestEnemy = FindNearestEnemy();
        /*
        if (Ammo == null)
            nav.SetDestination(vendor.position);
        else 
        */
        if (nearestEnemy != null) // attack mode
        {

            nav.SetDestination(nearestEnemy.position);

            /*
            if(enemyDistance < targetingRad)
                transform.LookAt(nearestEnemy.position);
            */

            if (enemyDistance <= strafeRad)
                //if(Ammo != null)
                nav.isStopped = true;
            else nav.isStopped = false;
        }

        

    
        
    }

    private Transform FindNearestEnemy()
    {
        GameObject[] enemies;
        enemies = GameObject.FindGameObjectsWithTag("Team2");

        Transform nearest = null;
        float curDistance = Mathf.Infinity;

        foreach(GameObject enemy in enemies)
        {
            float calculatedDist = (enemy.transform.position - transform.position).sqrMagnitude;

            //Debug.Log("sqrMag distance to enemy " + calculatedDist);
            if (calculatedDist > viewRad)
                continue;

            if (calculatedDist < curDistance)
            {
                nearest = enemy.transform;
                curDistance = calculatedDist;
                enemyDistance = calculatedDist;
            }
        }
        return nearest;
    }

    private Transform FindNearestVendor()
    {
        GameObject[] vendors;

        if (money >= 2)
            vendors = GameObject.FindGameObjectsWithTag("Purchaser");
        else vendors = GameObject.FindGameObjectsWithTag("Bar");

        Transform nearest = null;
        float curDistance = Mathf.Infinity;

        foreach (GameObject vendor in vendors)
        {
            float calculatedDist = (vendor.transform.position - transform.position).sqrMagnitude;

            if (calculatedDist < curDistance)
            {
                nearest = vendor.transform;
                curDistance = calculatedDist;
            }
        }
        return nearest;
    }
}