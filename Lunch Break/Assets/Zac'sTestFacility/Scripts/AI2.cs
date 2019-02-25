using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;



public class AI2 : MonoBehaviour
{
    // import Transforms of food bars
    // import Transforms of vending machines
    // import food types
    // import match time and scores

    public float viewRad = 20f;                 // distance at which enemies can be detected
    public float targetingRad = 5f;             // distance at which character attacks/approaches enemy
    public float patrolRad = 3f;                // distance at which character starts cap zone wander
    public float runRad = 8f;
    public float wanderRad = 8f;

    public float health = 3f;
    public float money = 0f;
    public int maxInv = 3;

    public float fireRate = 0.5f;
    public GameObject burger;
    public float burgerCost = 2f;
    public GameObject projectile;
    public Transform projSpawn;
    public float barCooldown = 5f;

    NavMeshAgent nav;
    Transform nearestEnemy;
    Transform nearestCap;
    Transform vendor;
    float enemyDistance;
    float capDistance;
    float nextFire;
    float nextBar = 0;

    List<GameObject> Ammo;

    private void Awake()
    {
        vendor = FindNearestVendor();

        nav = GetComponent<NavMeshAgent>();

        Ammo = new List<GameObject>();
    }

    private void Update()
    {
        nearestEnemy = FindNearestEnemy();

        if (!Ammo.Any()) // find ammo
        {
            vendor = FindNearestVendor();
            nav.SetDestination(vendor.position);

            if (enemyDistance < runRad && nearestEnemy != null) // flee enemies if no Ammo
            {
                Vector3 toEnemy = transform.position - nearestEnemy.position;
                Vector3 fleePos = transform.position + toEnemy + vendor.position;

                nav.SetDestination(fleePos);
            }
        }
        else if (nearestEnemy != null) // attack mode
        {
            nav.SetDestination(nearestEnemy.position);

            if (enemyDistance < targetingRad)
                transform.LookAt(nearestEnemy.position);

            if (enemyDistance <= targetingRad) // enemy within range
            {
                if (Time.time > nextFire) // shooting cooldown expired
                {
                    if (Ammo.Any()) // has ammo
                    {
                        transform.LookAt(nearestEnemy.position); // face enemy

                        projectile = Ammo[0];
                        Ammo.RemoveAt(0);
                        nextFire = Time.time + fireRate;
                        Instantiate(projectile, projSpawn.position, projSpawn.rotation); // fire projectile
                    }
                }
            }
        }
        else // cap mode; navigate to either vending or cafeteria
        {
            nearestCap = FindNearestCap();
            nav.SetDestination(nearestCap.position);

            if (capDistance < patrolRad) // wander in cap
            {
                nav.SetDestination(Wander(transform.position));
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Projectile1")
        {
            health--;
            if (health <= 0)
            {
                // increase score for projectile team
                // start respawn timer
                // die; wait for animation
                // spawn money equal to amount before death

                Destroy(gameObject);

            }
        }

        if (other.gameObject.tag == "Projectile3")
        {
            health--;
            if (health <= 0)
            {
                // increase score for projectile team
                // start respawn timer
                // die; wait for animation
                // spawn money equal to amount before death

                Destroy(gameObject);

            }
        }

        if (other.gameObject.tag == "Projectile4")
        {
            health--;
            if (health <= 0)
            {
                // increase score for projectile team
                // start respawn timer
                // die; wait for animation
                // spawn money equal to amount before death

                Destroy(gameObject);

            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Vending")
        {
            if (money >= 2 && Ammo.Count < maxInv)
            {
                burger.tag = "Projectile2";
                Ammo.Add(burger);
                money -= burgerCost;
            }
        }

        if (other.gameObject.tag == "Bar")
        {
            if (Ammo.Count < maxInv)
            {
                if (Time.time > nextBar)
                {
                    burger.tag = "Projectile2";
                    Ammo.Add(burger);

                    nextBar = Time.time + barCooldown;
                }
            }
        }
    }

    private Transform FindNearestEnemy()
    {
        GameObject[] enemies1 = GameObject.FindGameObjectsWithTag("Team1");
        GameObject[] enemies3 = GameObject.FindGameObjectsWithTag("Team3");
        GameObject[] enemies4 = GameObject.FindGameObjectsWithTag("Team4");

        GameObject[] allEnemies = new GameObject[enemies1.Length + enemies3.Length + enemies4.Length];
        enemies1.CopyTo(allEnemies, 0);
        enemies3.CopyTo(allEnemies, enemies1.Length);
        enemies4.CopyTo(allEnemies, enemies3.Length);

        Transform nearest = null;
        float curDistance = Mathf.Infinity;

        foreach (GameObject enemy in allEnemies)
        {
            float calculatedDist = (enemy.transform.position - transform.position).sqrMagnitude;

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
        GameObject[] bars;

        Transform nearestTF = null;
        float curDistance = Mathf.Infinity;

        if (money >= 2)
        {
            vendors = GameObject.FindGameObjectsWithTag("Vending");

            foreach (GameObject vendor in vendors)
            {
                float calculatedDist = (vendor.transform.position - transform.position).sqrMagnitude;

                if (calculatedDist < curDistance)
                {
                    nearestTF = vendor.transform;
                    curDistance = calculatedDist;
                }
            }
        }

        bars = GameObject.FindGameObjectsWithTag("Bar");

        foreach (GameObject bar in bars)
        {
            float calculatedDist = (bar.transform.position - transform.position).sqrMagnitude;

            if (calculatedDist < curDistance)
            {
                nearestTF = bar.transform;
                curDistance = calculatedDist;
            }
        }
        return nearestTF;
    }

    private Transform FindNearestCap()
    {
        GameObject[] caps = GameObject.FindGameObjectsWithTag("Cap");
        Transform nearest = null;
        float curDistance = Mathf.Infinity;

        foreach (GameObject cap in caps)
        {
            float calculatedDist = (cap.transform.position - transform.position).sqrMagnitude;

            if (calculatedDist < curDistance)
            {
                nearest = cap.transform;
                curDistance = calculatedDist;
                capDistance = calculatedDist;
            }
        }
        return nearest;
    }

    private Vector3 Wander(Vector3 position)
    {
        Vector3 randomVector = Random.insideUnitSphere * wanderRad;

        randomVector += position;

        NavMesh.SamplePosition(randomVector, out NavMeshHit navPos, wanderRad, -1);

        return navPos.position;
    }
}