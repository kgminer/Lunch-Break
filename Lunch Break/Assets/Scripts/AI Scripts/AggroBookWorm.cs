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

    public float patrolRad;               // distance at which character starts cap zone wander
    public float wanderRad;

    // Random variables and their ranges
    public float shuffleTime, shuffleMin, shuffleMax;
    public float pursueRad, pursueMin, pursueMax;
    public float targetingRange, rangeMin, rangeMax;
    public float runRad, runMin, runMax;
    public float fleeTil, fleeTilMin, fleeTilMax;
    public float personalityTimer, personTimeMin, personTimeMax;

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

        personality = Random.Range(0, 3);
    }

    private void Update()
    {
        if (!NPCAnimator.GetCurrentAnimatorStateInfo(0).IsName("Unarmed-Attack-R3"))
            if (!NPCAnimator.GetCurrentAnimatorStateInfo(0).IsName("Unarmed-Death1"))
                if (!NPCAnimator.GetCurrentAnimatorStateInfo(0).IsName("Unarmed-GetHit-F1"))
                    nav.isStopped = false;

        if (!alive)
        {
            nav.isStopped = true;
            return;
        }

        if (Time.time > shuffleTime)
            VariableShuffle();

        if (Time.time < fleeTil)
            return;

        nearestEnemy = FindNearestEnemy();

        if (!Ammo.Any())
        {
            nearestVendor = FindNearestVendor(null);
            nav.SetDestination(nearestVendor.position);

            if (nearestEnemy != null && enemyDistance < runRad)
            {
                switch (personality)
                {
                    case 0: // run away from enemy
                        nav.SetDestination(-nearestEnemy.position);
                        fleeTil = Time.time + Random.Range(fleeTilMin, fleeTilMax);
                        break;

                    case 1: // run parallel to enemy
                        nav.SetDestination(nearestEnemy.position + nearestVendor.position);
                        fleeTil = Time.time + Random.Range(fleeTilMin, fleeTilMax);
                        break;

                    case 2: // find new vendor
                        nav.SetDestination(FindNearestVendor(nearestVendor).position);
                        fleeTil = Time.time + Random.Range(fleeTilMin, fleeTilMax);
                        break;
                }
            }
        }
        else if (nearestEnemy != null) // attack mode
        {
            nav.SetDestination(nearestEnemy.position);

            if (enemyDistance <= targetingRange) // enemy within range
            {
                if (Time.time > nextFire) // shooting cooldown expired
                {
                    if (Ammo.Any()) // has ammo
                    {
                        transform.LookAt(nearestEnemy.position); // face enemy

                        nav.isStopped = true;
                        NPCAnimator.SetTrigger("Attack");
                        nextFire = Time.time + fireRate;
                    }
                }
            }
        }
        else
        {
            switch(personality)
            {
                case 0:
                    nav.SetDestination(GameManager.centerCap.position);
                    break;

                case 1:
                    nav.SetDestination(FindNearestCap(0).position);
                    break;

                case 2:
                    nav.SetDestination(FindNearestCap(1).position);
                    break;
            }
        }
    }

    void Fire()
    {
        GameObject item = Ammo.First();
        GameObject thrown = Instantiate(item, projSpawn.position, projSpawn.rotation);
        thrown.tag = this.tag + "Thrown";
        Ammo.Remove(item);
    }

    IEnumerator Respawn()
    {
        alive = false;
        GameManager.setObjectLocation(gameObject, "respawn");
        health = startingHealth;
        yield return new WaitForSeconds(10);
        GameManager.setObjectLocation(gameObject, "bookWorm");
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
                nav.isStopped = true;
                NPCAnimator.SetTrigger("Die");

                // increase score for projectile team
                if (other.gameObject.tag == "jocksThrown")
                {
                    GameManager.jocksScore++;
                }
                else if (other.gameObject.tag == "scienceGeekThrown")
                {
                    GameManager.scienceGeeksScore++;
                }
            }
            else
            {
                nav.isStopped = true;
                NPCAnimator.SetTrigger("TakeDamage");
            }
        }
    }

    void Perish()
    {
        StartCoroutine("Respawn");
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
        pursueRad = Random.Range(pursueMin, pursueMax);
        targetingRange = Random.Range(rangeMin, rangeMax);
        fleeTil = Random.Range(fleeTilMin, fleeTilMax);
        runRad = Random.Range(runMin, runMax);

        if (Time.time > personalityTimer)
        {
            ChangePersonality(3);
            personalityTimer = Time.time + Random.Range(personTimeMin, personTimeMax);
        }

        shuffleTime = Time.time + Random.Range(shuffleMin, shuffleMax);
    }

    private void ChangePersonality(int mode)
    {
        if (mode == 3)
            personality = Random.Range(0, 3);

        if (mode == 0)
            personality = 0;
        if (mode == 1)
            personality = 1;
        if (mode == 2)
            personality = 2;
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