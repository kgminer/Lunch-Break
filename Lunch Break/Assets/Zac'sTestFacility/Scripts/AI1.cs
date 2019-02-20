using UnityEngine;
using UnityEngine.AI;

public class AI1 : MonoBehaviour
{
    // import Transforms of food bars
    // import Transforms of vending machines
    // import food types

    public float attackRad = 5f;
    public float health = 3f;
    public float money = 0f;

    NavMeshAgent nav;
    Transform nearestEnemy;
    Transform vendor;
    int viewRad = 25;

    GameObject[] Ammo; // maybe need a list

    private void Awake()
    {
        nearestEnemy = FindNearestEnemy();

        vendor = FindNearestVendor();

        nav = GetComponent<NavMeshAgent>();

    }

    private void Update()
    {
        if (Ammo == null)

            nav.SetDestination(vendor.position);
        else
        if(nav.enabled == true)
            nav.SetDestination(nearestEnemy.position);

        if ((nearestEnemy.position - transform.position).sqrMagnitude <= attackRad)
            if(Ammo != null)
                nav.Stop();

        else nav.Resume();
        
    }

    private Transform FindNearestEnemy()
    {
        GameObject[] enemies;
        enemies = GameObject.FindGameObjectsWithTag("Team2");

        Transform nearest = null;
        float distance = Mathf.Infinity;

        foreach(GameObject enemy in enemies)
        {
            Vector3 dist = enemy.transform.position - transform.position;
            float calculatedDist = dist.sqrMagnitude;

            if (calculatedDist < distance)
            {
                nearest = enemy.transform;
                distance = calculatedDist;
            }
        }
        return nearest;
    }

    private Transform FindNearestVendor()
    {
        GameObject[] vendors;

        if (money >= 2)
            vendors = GameObject.FindGameObjectsWithTag("Vending");
        else vendors = GameObject.FindGameObjectsWithTag("Bar");

        Transform nearest = null;
        float distance = Mathf.Infinity;

        foreach (GameObject vendor in vendors)
        {
            Vector3 dist = vendor.transform.position - transform.position;
            float calculatedDist = dist.sqrMagnitude;

            if (calculatedDist < distance)
            {
                nearest = vendor.transform;
                distance = calculatedDist;
            }
        }
        return nearest;
    }
}