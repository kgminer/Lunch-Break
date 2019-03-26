﻿// Recklessly aggressive AI for Team2
// Find Ammo and attack nearest enemy
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class AI3Aggressive : MonoBehaviour
{
    public float viewRad;                 // distance at which enemies can be detected
    public float pursueRad;                // distance at which character pursues enemy
    public float targetingRad;             // distance at which character actively targets/fires on enemy
    public float patrolRad;               // distance at which character starts cap zone wander
    public float runRad;
    public float wanderRad;

    public float runMagMin;
    public float runMagMax;
    float runMag;

    public float runTilMin;
    public float runTilMax;
    float runTimer;
    float runTil = 0f;

    private float health;
    public float startingHealth;
    private float money;
    public float startingMoney;
    public int maxInv;

    public float barCooldown;
    float nextBar = 0f;

    public float fireRate;
    public GameObject burger;
    public float burgerCost;
    public GameObject projectile;
    public Transform projSpawn;
    public AudioClip hitSound;

    NavMeshAgent nav;
    Transform nearestEnemy;
    Transform nearestCap;
    Transform vendor;
    float enemyDistance;
    float capDistance;
    float nextFire;
    List<GameObject> Ammo;

    public bool alive;
    public GameObject JocksSpawnObject;
    public GameObject RespawnObject;

    private void Awake()
    {
        vendor = FindNearestVendor();

        nav = GetComponent<NavMeshAgent>();

        Ammo = new List<GameObject>();

        health = startingHealth;
        money = startingMoney;
    }

    private void Update()
    {
        if (!alive)
        {
            return;
        }

        nearestEnemy = FindNearestEnemy();

        if (!Ammo.Any())
        {
            vendor = FindNearestVendor();
            nav.SetDestination(vendor.position);
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
        else
        {
            nearestCap = FindNearestCap();
            nav.SetDestination(nearestCap.position);

            if (capDistance < patrolRad) // wander in cap
            {
                nav.SetDestination(Wander(transform.position, wanderRad));
            }
        }
    }

    IEnumerator Respawn()
    {
        alive = false;
        gameObject.transform.position = RespawnObject.transform.position;
        health = startingHealth;
        money = startingMoney;
        yield return new WaitForSeconds(10);
        gameObject.transform.position = JocksSpawnObject.transform.position;
        alive = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "bookWormThrown" || other.gameObject.tag == "scienceGeekThrown")
        {
            health--;
            Destroy(other.gameObject);
            AudioSource.PlayClipAtPoint(hitSound, transform.position);

            if (health <= 0)
            {
                // increase score for projectile team
                if (other.gameObject.tag == "bookWormThrown")
                {
                    GameManager.bookWormsScore++;
                }
                else if (other.gameObject.tag == "scienceGeekThrown")
                {
                    GameManager.scienceGeeksScore++;
                }
                // start respawn timer
                // die; wait for animation
                // spawn money equal to amount before death

                //Destroy(gameObject);
                //Respawn();
                StartCoroutine("Respawn");
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Vending")
        {
            if (money >= 2 && Ammo.Count < maxInv)
            {
                burger.tag = "jocksThrown";
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
                    burger.tag = "jocksThrown";
                    Ammo.Add(burger);

                    nextBar = Time.time + barCooldown;
                }
            }
        }
    }

    private Transform FindNearestEnemy()
    {
        GameObject[] enemies1 = GameObject.FindGameObjectsWithTag("bookWorm");
        GameObject[] enemies2 = GameObject.FindGameObjectsWithTag("scienceGeek");
        // GameObject[] enemies4 = GameObject.FindGameObjectsWithTag("Team4");

        GameObject[] allEnemies = new GameObject[enemies1.Length + enemies2.Length /* + enemies4.Length */];
        enemies1.CopyTo(allEnemies, 0);
        enemies2.CopyTo(allEnemies, enemies1.Length);
        // enemies4.CopyTo(allEnemies, enemies3.Length);

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

    private Vector3 Wander(Vector3 position, float magnitude)
    {
        Vector3 randomVector = Random.insideUnitSphere * magnitude;

        randomVector += position;

        NavMesh.SamplePosition(randomVector, out NavMeshHit navPos, magnitude, -1);

        return navPos.position;
    }
}