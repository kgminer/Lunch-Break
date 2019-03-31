// Recklessly aggressive AI for BookWorms
// Find Ammo and attack nearest enemy
// Move to Cafeteria cap point when no enemies present
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class AggroBookWorm : MonoBehaviour
{
    public float viewRad;                 // distance at which enemies can be detected
    public float pursueRad;                // distance at which character pursues enemy

    public float patrolRad;               // distance at which character starts cap zone wander
    public float wanderRad;

    public float shuffleTime, shuffleMin, shuffleMax;
    public float targetingRad, rangeMin, rangeMax;
    public float runRad, runMin, runMax;
    public float fleeTil, fleeTilMin, fleeTilMax;

    public int personality;

    private float health;
    public float startingHealth;
    public int maxInv;

    public float barCooldown;
    float nextBar = 0f;

    public float fireRate;

    // Projectile object references
    public GameObject burger;
    public GameObject donut;
    public GameObject drink;
    public GameObject cake;
    public GameObject fries;

    // Held object references
    public GameObject heldBurger;
    public GameObject heldDonut;
    public GameObject heldDrink;
    public GameObject heldCake;
    public GameObject heldTray;
    public GameObject heldFries;
    private GameObject activeItem;

    public Transform projSpawn;
    Animator NPCAnimator;

    public AudioClip hitSound;

    NavMeshAgent nav;

    Transform nearestEnemy;
    Transform nearestCap;
    Transform nearestVendor;

    float enemyDistance;
    float capDistance;
    float nextFire;
    List<GameObject> Ammo;

    public bool alive;
    public GameObject JocksSpawnObject;
    public GameObject RespawnObject;

    private void Awake()
    {
        nearestVendor = FindNearestVendor(null);
        NPCAnimator = GetComponent<Animator>();
        nav = GetComponent<NavMeshAgent>();

        Ammo = new List<GameObject>();

        health = startingHealth;

        VariableShuffle();

        personality = (int)Random.Range(0, 2);
    }

    private void Update()
    {
        if (!alive)
        {
            return;
        }

        if (Time.time > shuffleTime)
            VariableShuffle();

        nearestEnemy = FindNearestEnemy();

        if (!Ammo.Any())
        {
            nearestVendor = FindNearestVendor(null);
            nav.SetDestination(nearestVendor.position);

            if (enemyDistance < runRad)
            {
                switch (personality)
                {
                    case 0: // run away from enemy
                        nav.SetDestination(-nearestEnemy.position);
                        break;

                    case 1: // run parallel to enemy
                        nav.SetDestination(nearestEnemy.position + nearestVendor.position);
                        break;

                    case 2: // find new vendor
                        nav.SetDestination(FindNearestVendor(nearestVendor).position);
                        break;
                }
            }
                
                    
                
                
        }
        else if (nearestEnemy != null) // attack mode
        {
            nav.SetDestination(nearestEnemy.position);

            if (enemyDistance <= targetingRad) // enemy within range
            {
                if (Time.time > nextFire) // shooting cooldown expired
                {
                    if (Ammo.Any()) // has ammo
                    {
                        transform.LookAt(nearestEnemy.position); // face enemy

                        NPCAnimator.SetTrigger("Attack");
                        nextFire = Time.time + fireRate;
                    }
                }
            }
        }
        else
        {
            nav.SetDestination(GameManager.centerCap.position);
        }
    }

    IEnumerator Respawn()
    {
        alive = false;
        GameManager.setObjectLocation(gameObject, "respawn");
        health = startingHealth;
        yield return new WaitForSeconds(10);
        GameManager.setObjectLocation(gameObject, "jocks");
        alive = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "jocksThrown" || other.gameObject.tag == "scienceGeekThrown")
        {
            health--;
            Destroy(other.gameObject);
            AudioSource.PlayClipAtPoint(hitSound, transform.position);

            if (health <= 0)
            {
                // increase score for projectile team
                if (other.gameObject.tag == "jocksThrown")
                {
                    GameManager.jocksScore++;
                }
                else if (other.gameObject.tag == "scienceGeekThrown")
                {
                    GameManager.scienceGeeksScore++;
                }
                // start respawn timer
                // die; wait for animation
                // spawn money equal to amount before death

                StartCoroutine("Respawn");
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.tag == "Vending")
            if(Ammo.Count() < maxInv)
                if(Time.time > nextBar)
                {
                    Ammo.Add(burger);
                    nextBar = Time.time + barCooldown;
                }
    }

    private void VariableShuffle()
    {
        targetingRad = Random.Range(rangeMin, rangeMax);
        fleeTil = Random.Range(fleeTilMin, fleeTilMax);
        runRad = Random.Range(runMin, runMax);

        shuffleTime = Time.time + Random.Range(shuffleMin, shuffleMax);
    }


    private Transform FindNearestEnemy()
    {
        Transform nearest = null;
        float curDistance = Mathf.Infinity;

        foreach (GameObject enemy in GameManager.allCharacters)
        {
            if (enemy.tag == this.tag)
                continue;

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

    private Transform FindNearestVendor(Transform previous)
    {
        Transform nearestTF = null;
        float curDistance = Mathf.Infinity;

        foreach (Transform vendor in GameManager.vendors)
        {
            if (vendor == previous)
                continue;

            float calculatedDist = (vendor.position - transform.position).sqrMagnitude;

            if (calculatedDist < curDistance)
            {
                nearestTF = vendor;
                curDistance = calculatedDist;
            }
        }
        return nearestTF;
    }

    private Transform FindNearestCap(int mode) 
    {
        Transform nearest = null;
        float curDistance = Mathf.Infinity;

        foreach (GameObject cap in GameManager.caps)
        {
            if (mode == 0 && cap.GetComponent<CapZone>().GetOwner() == "bookWorm") // mode 0: don't consider team's caps
                continue;

            if (mode == 1 && cap.transform == nearestCap) // find any new cap
                continue;

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
}