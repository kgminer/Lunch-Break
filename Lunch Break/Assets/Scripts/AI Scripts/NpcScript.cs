using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class NpcScript : MonoBehaviour
{
    // Limits
    public float viewRadius;
    public float attackCooldown;
    private float nextAttackTime = 0;
    private float health;
    public float startingHealth;
    public int maxInventory;
    public float vendorCooldown;
    private float nextVendorTime;
    public float respawnTimer;


    // Random variables and their ranges
    private float nextShuffleTime;
    public float shuffleDelayMin, shuffleDelayMax;

    private float pursuitRadius;
    public float pursuitRadiusMin, pursuitRadiusMax;

    private float attackRadius;
    public float attackRadiusMin, attackRadiusMax;

    private float fleeRadius;
    public float fleeRadiusMin, fleeRadiusMax;

    public float shortFleeTimer;
    public float fleeMagnitude;

    private float fleeUntilTime;
    public float fleeTimerMin, fleeTimerMax;

    private float idleAnimationRadius;
    public float idleRadiusMin, idleRadiusMax;

    private float personalityShuffleTimer;
    public float personalityShuffleDelayMin, personalityShuffleDelayMax;

    private CapPersonality capPersonality;
    private FleePersonality fleePersonality;
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
    private Team team;

    enum CapPersonality
    {
        Cafeteria, Attacker, Defender, AnyCap
    };

    enum FleePersonality
    {
        Direct, ToVendor, ToBase
    };

    enum Food
    {
        Burger, Drink, Donut, Fries, Cake
    };
    enum Team
    {
        scienceGeek, jocks, bookWorm
    };

    private void Awake()
    {
        if (this.tag == "scienceGeek")
            team = Team.scienceGeek;
        if (this.tag == "jocks")
            team = Team.jocks;
        if (this.tag == "bookWorm")
            team = Team.bookWorm;

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

            if (team == Team.bookWorm)
                nav.Warp(GameManager.BookWormsSpawnObject.transform.position);
            if (team == Team.jocks)
                nav.Warp(GameManager.JocksSpawnObject.transform.position);
            if (team == Team.scienceGeek)
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

        if (Time.time > nextShuffleTime)
            VariableShuffle();

        if (Time.time < fleeUntilTime)
            return;

        nearestEnemy = FindNearestEnemy();

        if (!Ammo.Any())
        {
            idling = false;
            nearestVendor = FindNearestVendor(null);
            nav.SetDestination(nearestVendor.position);

            if (nearestEnemy != null && enemyDistance < fleeRadius)
            {
                switch (fleePersonality)
                {
                    case FleePersonality.Direct: // run away from enemy
                        nav.SetDestination((transform.position - nearestEnemy.position) * fleeMagnitude);
                        //fleeUntilTime = Time.time + Random.Range(fleeTimerMin, fleeTimerMax);
                        fleeUntilTime = Time.time + shortFleeTimer;
                        break;

                    case FleePersonality.ToVendor: // find nearest vendor
                        nav.SetDestination(FindNearestVendor(nearestVendor).position);
                        fleeUntilTime = Time.time + Random.Range(fleeTimerMin, fleeTimerMax);
                        break;

                    case FleePersonality.ToBase: // flee to base
                        nav.SetDestination(GameManager.JocksSpawnObject.transform.position);
                        fleeUntilTime = Time.time + Random.Range(fleeTimerMin, fleeTimerMax);
                        break;
                }
            }
        }
        else if (nearestEnemy != null) // attack mode
        {
            idling = false;
            nav.SetDestination(nearestEnemy.position);

            if (enemyDistance <= attackRadius) // enemy within range
            {
                if (Time.time > nextAttackTime) // shooting cooldown expired
                {
                    if (Ammo.Any()) // has ammo
                    {
                        nav.isStopped = true;
                        NPCAnimator.SetTrigger("Attack");
                        nextAttackTime = Time.time + attackCooldown;
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
            case CapPersonality.Cafeteria:
                nav.SetDestination(GameManager.centerCap.position); // go to cafeteria cap
                break;

            case CapPersonality.Attacker:
                nav.SetDestination(FindNearestCap(CapPersonality.Attacker).position); // find a cap not yet taken
                break;

            case CapPersonality.Defender:
                nav.SetDestination(FindNearestCap(CapPersonality.Defender).position); // go to team's nearest cap
                break;

            case CapPersonality.AnyCap:
                nav.SetDestination(FindNearestCap(CapPersonality.AnyCap).position); // go to any nearest cap
                break;
        }


        if (capDistance < idleAnimationRadius ||
            (GameManager.centerCap.position - transform.position).sqrMagnitude < idleAnimationRadius ||
            (nearestVendor.position - transform.position).sqrMagnitude < idleAnimationRadius)
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

        if (other.gameObject.tag == "bookWormThrown" || other.gameObject.tag == "scienceGeekThrown" || other.gameObject.tag == "jocksThrown")
        {
            if (team == Team.bookWorm && other.gameObject.tag == "bookWormThrown")
                return;
            if (team == Team.scienceGeek && other.gameObject.tag == "scienceGeekThrown")
                return;
            if (team == Team.jocks && other.gameObject.tag == "jocksThrown")
                return;

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
                if (other.gameObject.tag == "bookWormThrown")
                {
                    GameManager.bookWormsScore++;
                }
                else if (other.gameObject.tag == "scienceGeekThrown")
                {
                    GameManager.scienceGeeksScore++;
                }
                else if (other.gameObject.tag == "jocksThrown")
                {
                    GameManager.jocksScore++;
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
            if (Ammo.Count() < maxInventory)
                if (Time.time > nextVendorTime)
                {
                    Food ammoType = (Food)Random.Range(0, System.Enum.GetNames(typeof(Food)).Length);

                    switch (ammoType)
                    {
                        case Food.Burger:
                            Ammo.Add(burger);
                            nextVendorTime = Time.time + vendorCooldown;
                            break;

                        case Food.Drink:
                            Ammo.Add(drink);
                            nextVendorTime = Time.time + vendorCooldown;
                            break;

                        case Food.Donut:
                            Ammo.Add(donut);
                            nextVendorTime = Time.time + vendorCooldown;
                            break;

                        case Food.Fries:
                            Ammo.Add(mainFries);
                            nextVendorTime = Time.time + vendorCooldown;
                            break;
                    }
                }
    }

    private void VariableShuffle()
    {
        pursuitRadius = Random.Range(pursuitRadiusMin, pursuitRadiusMax);
        attackRadius = Random.Range(attackRadiusMin, attackRadiusMax);
        fleeUntilTime = Random.Range(fleeTimerMin, fleeTimerMax);
        fleeRadius = Random.Range(fleeRadiusMin, fleeRadiusMax);
        idleAnimationRadius = Random.Range(idleRadiusMin, idleRadiusMax);

        if (Time.time > personalityShuffleTimer)
        {
            fleePersonality = (FleePersonality)Random.Range(0, System.Enum.GetNames(typeof(FleePersonality)).Length);
            capPersonality = (CapPersonality)Random.Range(0, System.Enum.GetNames(typeof(CapPersonality)).Length);

            personalityShuffleTimer = Time.time + Random.Range(personalityShuffleDelayMin, personalityShuffleDelayMax);
        }

        nextShuffleTime = Time.time + Random.Range(shuffleDelayMin, shuffleDelayMax);
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

            if (calculatedDist > viewRadius)
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

    private Transform FindNearestCap(CapPersonality mode)
    {
        Transform nearest = null;
        float curDistance = Mathf.Infinity;

        foreach (GameObject cap in GameManager.caps)
        {
            // If attacker, ignore own team's zones
            if (mode == CapPersonality.Attacker && cap.GetComponent<CapZone>().GetOwner() == this.tag)
                continue;

            // If any cap, ignore current nearest zone
            if (mode == CapPersonality.AnyCap && cap.transform == nearestCap)
                continue;

            // If defender, ignore other teams' zones
            if (mode == CapPersonality.Defender && cap.GetComponent<CapZone>().GetOwner() != this.tag)
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