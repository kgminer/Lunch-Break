using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class NpcScript : MonoBehaviour
{
    // Limits
    public float viewRadius;
    public float attackCooldown;
    private float nextAttackTime;
    private float hitAndRunTimer;
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

    public float hitAndRunDelay;

    private float fleeUntilTime;
    public float fleeTimerMin, fleeTimerMax;

    private float idleAnimationRadius;
    public float idleRadiusMin, idleRadiusMax;

    private float personalityShuffleTimer;
    public float personalityShuffleDelayMin, personalityShuffleDelayMax;

    private CapPersonality capPersonality;
    private FleePersonality fleePersonality;
    private AttackingPersonality attackingPersonality;

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

    public string fleePerson;
    public string capPerson;

    enum CapPersonality
    {
        Cafeteria, Attacker, Defender, AnyCap, CapTaker
    };

    enum FleePersonality
    {
        ToVendor, ToBase
    };

    enum AttackingPersonality
    {
        Strafe, BackAway, HitnRun
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
        PersonalityShuffle();
    }


    private void Update()
    {
        if (NPCAnimator.GetCurrentAnimatorStateInfo(0).IsName("Unarmed-Death1"))
            return;

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

        if (Time.time > personalityShuffleTimer)
            PersonalityShuffle();

        if (Time.time < fleeUntilTime && !Ammo.Any())
            return;

        if (Time.time < hitAndRunTimer)
            return;

        nearestEnemy = FindNearestEnemy();

        if (!Ammo.Any()) // no ammo, seek vendor location
        {
            idling = false;
            nearestVendor = FindNearestVendor(null);
            nav.SetDestination(nearestVendor.position);

            if (nearestEnemy != null && enemyDistance < fleeRadius)
            {
                switch (fleePersonality)
                {
                    case FleePersonality.ToVendor: // find nearest vendor
                        nav.SetDestination(FindNearestVendor(nearestVendor).position);
                        fleeUntilTime = Time.time + Random.Range(fleeTimerMin, fleeTimerMax);
                        break;

                    case FleePersonality.ToBase: // flee to base
                        if (team == Team.jocks)
                            nav.SetDestination(GameManager.JocksSpawnObject.transform.position);
                        if (team == Team.bookWorm)
                            nav.SetDestination(GameManager.BookWormsSpawnObject.transform.position);
                        if (team == Team.scienceGeek)
                            nav.SetDestination(GameManager.ScienceGeeksSpawnObject.transform.position);

                        fleeUntilTime = Time.time + Random.Range(fleeTimerMin, fleeTimerMax);
                        break;
                }
            }
        }
        else if (nearestEnemy != null) // attack mode
        {
            if(Time.time < nextAttackTime) // if on cooldoown, reference AttackPersonality to avoid hugging enemy
            {
                switch(attackingPersonality)
                {
                    case AttackingPersonality.BackAway:
                        FaceEnemy(nearestEnemy);
                        nav.SetDestination(-nearestEnemy.position * 2);
                        break;

                    case AttackingPersonality.HitnRun:
                        if (team == Team.jocks)
                            nav.SetDestination(GameManager.JocksSpawnObject.transform.position);
                        if (team == Team.bookWorm)
                            nav.SetDestination(GameManager.BookWormsSpawnObject.transform.position);
                        if (team == Team.scienceGeek)
                            nav.SetDestination(GameManager.ScienceGeeksSpawnObject.transform.position);

                        hitAndRunTimer = Time.time + hitAndRunDelay;
                        break;

                    case AttackingPersonality.Strafe:
                        FaceEnemy(nearestEnemy);
                        nav.SetDestination(Vector3.Cross(transform.position, nearestEnemy.position));
                        break;
                }
            }

            idling = false;
            nav.SetDestination(nearestEnemy.position);

            if (enemyDistance <= attackRadius) // enemy within range
            {
                if (Time.time > nextAttackTime) // shooting cooldown expired
                {
                    nav.isStopped = true;
                    NPCAnimator.SetTrigger("Attack");
                    nextAttackTime = Time.time + attackCooldown;
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
                Transform enemycap = FindNearestCap(CapPersonality.Attacker); // find a cap not yet taken by team
                if (enemycap == null)
                    nav.SetDestination(GameManager.centerCap.position); // all caps taken, default to cafeteria
                else
                    nav.SetDestination(enemycap.position); // go to ene
                break;

            case CapPersonality.Defender:
                Transform teamcap = FindNearestCap(CapPersonality.Defender); // find a cap taken by team
                if (teamcap == null)
                    capPersonality = CapPersonality.CapTaker; // no caps taken, search for unclaimed caps
                else
                    nav.SetDestination(teamcap.position); // go to team's nearest cap
                break;

            case CapPersonality.CapTaker:
                Transform uncapped = FindNearestCap(CapPersonality.CapTaker); // find a cap with no team
                if (uncapped == null)
                    capPersonality = CapPersonality.AnyCap; // all caps claimed, search for any cap
                else
                    nav.SetDestination(uncapped.position); // go to unclaimed cap
                break;

            case CapPersonality.AnyCap:
                nav.SetDestination(FindNearestCap(CapPersonality.AnyCap).position); // go to any nearest cap
                break;                    
        }


        if ( (capDistance < idleAnimationRadius) ||
            ((GameManager.centerCap.position - transform.position).sqrMagnitude < idleAnimationRadius) ||
            (((nearestVendor.position - transform.position).sqrMagnitude < idleAnimationRadius) && !Ammo.Any()) )
        {
            idling = true;
            nav.isStopped = true;
            NPCAnimator.SetBool("Moving", false);
        }
        else
            idling = false;

    }

    void FaceEnemy(Transform enemy)
    {
        if(enemy != null)
            transform.LookAt(enemy.position); // face enemy
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
        else if(item.GetComponent<Cake>()) // cake is placed at feet
        {
            Vector3 location = projSpawn.position;
            location.y = 0;
            GameObject thrown = Instantiate(cake, location, projSpawn.rotation);
            thrown.tag = this.tag + "Thrown";
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
        nav.Warp(GameManager.RespawnObject.transform.position);
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Vending" || other.tag == "Base")
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

        nextShuffleTime = Time.time + Random.Range(shuffleDelayMin, shuffleDelayMax);
    }

    private void PersonalityShuffle()
    {
        fleePersonality = (FleePersonality)Random.Range(0, System.Enum.GetNames(typeof(FleePersonality)).Length);
        capPersonality = (CapPersonality)Random.Range(0, System.Enum.GetNames(typeof(CapPersonality)).Length);
        attackingPersonality = (AttackingPersonality)Random.Range(0, System.Enum.GetNames(typeof(AttackingPersonality)).Length);

        personalityShuffleTimer = Time.time + Random.Range(personalityShuffleDelayMin, personalityShuffleDelayMax);
    }

    private Transform FindNearestEnemy()
    {
        Transform nearest = null;
        float curDistance = Mathf.Infinity;

        foreach (GameObject enemy in GameManager.allCharacters)
        {
            if (enemy.tag == this.tag)
                continue;

            if(enemy.GetComponent<NpcScript>())
                if (!enemy.GetComponent<NpcScript>().alive)
                    continue;

            if (enemy.GetComponent<PlayerController>())
                if (!enemy.GetComponent<PlayerController>().alive)
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

            // If capTaker, only search for unclaimed zones
            if (mode == CapPersonality.CapTaker && cap.GetComponent<CapZone>().GetOwner() != "uncapped")
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