using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class ScienceGeekAI : MonoBehaviour
{
    // Limits
    public float viewRad;
    public float nextFire;
    public float health;
    public float startingHealth;
    public int maxInv;
    public float fireRate;
    public float barCooldown;
    public float nextBar;
    public float respawnTimer;


    // Random variables and their ranges
    public float shuffleTime, shuffleMin, shuffleMax;
    public float pursueRad, pursueMin, pursueMax;
    public float targetingRange, rangeMin, rangeMax;
    public float runRad, runMin, runMax;
    public float fleeShort, fleeMag, fleeTil, fleeTilMin, fleeTilMax;
    public float idleDist, idleMin, idleMax;
    public float personalityTimer, personTimeMin, personTimeMax;
    public int fleePersonality, capPersonality;
    public bool idling;

    // Projectile object references
    public GameObject burger;
    public GameObject donut;
    public GameObject drink;
    public GameObject cake;
    public GameObject mainFries;
    public GameObject sideFries;

    // Held object references
    public GameObject heldBurger;
    public GameObject heldDonut;
    public GameObject heldDrink;
    public GameObject heldCake;
    public GameObject heldTray;
    public GameObject heldFries;
    private GameObject activeItem;

    // Calculated variables
    float respawnTime;
    float enemyDistance;
    float capDistance;
    Transform nearestEnemy;
    Transform nearestCap;
    Transform nearestVendor;

    //Component Objects
    List<GameObject> Ammo;
    NavMeshAgent nav;
    public Transform projSpawn;
    public Transform leftSpawn;
    public Transform rightSpawn;
    Animator NPCAnimator;
    public AudioClip hitSound;
    public AudioClip hitSound2;
    public AudioClip hitSound3;

    public bool alive;

    private void Awake()
    {
        nearestVendor = FindNearestVendor(null);
        nearestCap = GameManager.centerCap;
        capDistance = (GameManager.centerCap.position - transform.position).sqrMagnitude;
        NPCAnimator = GetComponent<Animator>();
        nav = GetComponent<NavMeshAgent>();

        Ammo = new List<GameObject>();

        health = startingHealth;

        VariableShuffle();
    }


    private void Update()
    {
        if (Time.time < respawnTime)
            return;
        else if (!alive) // respawn
        {
            alive = true;
            health = startingHealth;
            nav.Warp(GameManager.ScienceGeeksSpawnObject.transform.position);
            idling = false;
        }

        if (!alive)
            return;

        if (!NPCAnimator.GetCurrentAnimatorStateInfo(0).IsName("Unarmed-Attack-R3"))
            if (!NPCAnimator.GetCurrentAnimatorStateInfo(0).IsName("Unarmed-Death1"))
                if (!NPCAnimator.GetCurrentAnimatorStateInfo(0).IsName("Unarmed-GetHit-F1"))
                    if (!idling)
                    {
                        nav.isStopped = false;
                        NPCAnimator.SetBool("Moving", true);
                    }

        if (Time.time > shuffleTime)
            VariableShuffle();

        if (Time.time < fleeTil)
            return;

        nearestEnemy = FindNearestEnemy();

        if (!Ammo.Any())
        {
            idling = false;
            nearestVendor = FindNearestVendor(null);
            nav.SetDestination(nearestVendor.position);

            if (nearestEnemy != null && enemyDistance < runRad)
            {
                switch (fleePersonality)
                {
                    case 0: // run away from enemy
                        nav.SetDestination((transform.position - nearestEnemy.position) * fleeMag);
                        //fleeTil = Time.time + Random.Range(fleeTilMin, fleeTilMax);
                        fleeTil = Time.time + fleeShort;
                        break;

                    case 1: // find new vendor
                        nav.SetDestination(FindNearestVendor(nearestVendor).position);
                        fleeTil = Time.time + Random.Range(fleeTilMin, fleeTilMax);
                        break;

                    case 2:
                        nav.SetDestination(GameManager.ScienceGeeksSpawnObject.transform.position);
                        fleeTil = Time.time + Random.Range(fleeTilMin, fleeTilMax);
                        break;
                }
            }
        }
        else if (nearestEnemy != null) // attack mode
        {
            idling = false;
            nav.SetDestination(nearestEnemy.position);

            if (enemyDistance <= targetingRange) // enemy within range
            {
                if (Time.time > nextFire) // shooting cooldown expired
                {
                    if (Ammo.Any()) // has ammo
                    {
                        nav.isStopped = true;
                        NPCAnimator.SetTrigger("Attack");
                        nextFire = Time.time + fireRate;
                    }
                }
            }
            else
                GoToCap();
        }
        else
            GoToCap();
    }

    void GoToCap()
    {
        idling = false;
        switch (capPersonality)
        {
            case 0:
                nav.SetDestination(GameManager.centerCap.position); // go to cafeteria cap
                break;

            case 1:
                Transform dest = FindNearestCap(0);
                if (dest == null) // if all caps are taken...
                {
                    nav.SetDestination(GameManager.centerCap.position); // go to cafeteria cap
                }
                else
                    nav.SetDestination(FindNearestCap(0).position); // find a cap not yet taken
                break;

            case 2:
                nav.SetDestination(FindNearestCap(1).position); // go to any vending cap
                break;
        }


        if (capDistance < idleDist || (GameManager.centerCap.position - transform.position).sqrMagnitude < idleDist)
        {
            idling = true;
            nav.isStopped = true;
            NPCAnimator.SetBool("Moving", false);
        }
        else
            idling = false;

    }

    void FaceEnemy()
    {
        transform.LookAt(nearestEnemy.position); // face enemy
    }

    void Fire()
    {
        GameObject item = Ammo.First();

        // Fries fire in a special pattern
        if (item.GetComponent<MainFries>())
        {
            GameObject thrown1 = Instantiate(sideFries, leftSpawn.position, leftSpawn.rotation);
            thrown1.tag = this.tag + "Thrown";
            GameObject thrown2 = Instantiate(mainFries, projSpawn.position, projSpawn.rotation);
            thrown2.tag = this.tag + "Thrown";
            GameObject thrown3 = Instantiate(sideFries, rightSpawn.position, rightSpawn.rotation);
            thrown3.tag = this.tag + "Thrown";
        }
        else
        {
            GameObject thrown = Instantiate(item, projSpawn.position, projSpawn.rotation);
            thrown.tag = this.tag + "Thrown";
        }
        Ammo.Remove(item);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!alive)
            return;

        if (other.gameObject.tag == "jocksThrown" || other.gameObject.tag == "bookWormThrown")
        {
            health--;
            Destroy(other.gameObject);

            switch (Random.Range(0, 3)) // hit sound shuffle
            {
                case 0:
                    AudioSource.PlayClipAtPoint(hitSound, transform.position);
                    break;

                case 1:
                    AudioSource.PlayClipAtPoint(hitSound2, transform.position);
                    break;

                case 2:
                    AudioSource.PlayClipAtPoint(hitSound3, transform.position);
                    break;
            }

            if (health <= 0)
            {
                nav.isStopped = true;
                alive = false;
                NPCAnimator.SetTrigger("Die");
                respawnTime = Time.time + respawnTimer;

                // increase score for projectile team
                if (other.gameObject.tag == "jocksThrown")
                {
                    GameManager.jocksScore++;
                }
                else if (other.gameObject.tag == "bookWormThrown")
                {
                    GameManager.bookWormsScore++;
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
        Debug.Log("perished");
        nav.Warp(GameManager.RespawnObject.transform.position);
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Vending")
            if (Ammo.Count() < maxInv)
                if (Time.time > nextBar)
                {
                    int ammoType = Random.Range(0, 5); // set to 6 for cake

                    switch (ammoType)
                    {
                        case 0: // burger
                            Ammo.Add(burger);
                            nextBar = Time.time + barCooldown;
                            break;

                        case 1: // drink
                            Ammo.Add(drink);
                            nextBar = Time.time + barCooldown;
                            break;

                        case 2:
                            Ammo.Add(donut);
                            nextBar = Time.time + barCooldown;
                            break;

                        case 3:
                            Ammo.Add(mainFries);
                            nextBar = Time.time + barCooldown;
                            break;
                    }
                }
    }

    private void VariableShuffle()
    {
        pursueRad = Random.Range(pursueMin, pursueMax);
        targetingRange = Random.Range(rangeMin, rangeMax);
        fleeTil = Random.Range(fleeTilMin, fleeTilMax);
        runRad = Random.Range(runMin, runMax);
        idleDist = Random.Range(idleMin, idleMax);

        if (Time.time > personalityTimer)
        {
            ChangeCapPersonality(3);
            ChangeFleePersonality(4);
            personalityTimer = Time.time + Random.Range(personTimeMin, personTimeMax);
        }

        shuffleTime = Time.time + Random.Range(shuffleMin, shuffleMax);
    }

    private void ChangeFleePersonality(int mode)
    {
        if (mode == 4)
            fleePersonality = Random.Range(0, 3);

        if (mode == 0)
            fleePersonality = 0;
        if (mode == 1)
            fleePersonality = 1;
        if (mode == 2)
            fleePersonality = 2;
        //if (mode == 3)
        //fleePersonality = 3;
    }

    private void ChangeCapPersonality(int mode)
    {
        if (mode == 3)
            capPersonality = Random.Range(0, 3);

        if (mode == 0)
            capPersonality = 0;
        if (mode == 1)
            capPersonality = 1;
        if (mode == 2)
            capPersonality = 2;
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
            if (mode == 0 && cap.GetComponent<CapZone>().GetOwner() == this.tag) // don't consider team's caps
                continue;

            if (mode == 1 && cap.transform == nearestCap) // find any new cap
                continue;

            if (mode == 2 && cap.GetComponent<CapZone>().GetOwner() != this.tag) // only consider team's cap
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