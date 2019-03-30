﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    // Create a sound manager reference
    private SoundManager SoundManager;

    public float speed = 5f;

    Vector3 movement;
    Rigidbody playerRigid;
    Animator playerAnimator;
    int floorMask;
    float camRayLen = 100f;

    public Transform projSpawn;
    public Transform leftSpawn;
    public Transform rightSpawn;
    public float fireRate = 0.5f;
    private float nextFire;
    private float health;
    public float startingHealth;
    public bool alive;

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

    public Inventory inventory;
    public HUD hud;
    public GameObject Hand;

    // Variable to be set upon projectile selection within Update for either item 0, 1, or 2. Initialized to 0 for first slot
    private int projectileSelect = 0;

    // Tracking each inventory slot for easy highlighting
    Button slot1;
    Button slot2;
    Button slot3;

    // List for holding each food item being collided with
    List<GameObject> touching = new List<GameObject>();

    // Slot color presets for reassigning 
    ColorBlock usedSlot;
    ColorBlock unusedSlot;

    // Use Update method for throwing projectiles
    void Update ()
    {
        if (Input.GetButton("Pause"))
        {
            hud.OpenPausePanel();
        }

        if (!alive)
        {
            return;
        }

        // Projectile selection will change slot of use and update ui component to reflect selection
        if (Input.GetButtonDown("Item0"))
        {
            projectileSelect = 0;
            slot1.colors = usedSlot;
            slot2.colors = unusedSlot;
            slot3.colors = unusedSlot;

            // If the slot is not empty, hold the item
            if (inventory.mSlots[projectileSelect].Count > 0)
            {
                InventoryItemBase item = inventory.mSlots[projectileSelect].FirstItem;

                if (item.GetComponent<Burger>())
                {
                    if (activeItem != null && activeItem != heldBurger)
                    {
                        activeItem.SetActive(false);
                    }

                    heldBurger.SetActive(true);
                    activeItem = heldBurger;
                }

                if (item.GetComponent<Donut>())
                {
                    if (activeItem != null && activeItem != heldDonut)
                    {
                        activeItem.SetActive(false);
                    }

                    heldDonut.SetActive(true);
                    activeItem = heldDonut;
                }

                if (item.GetComponent<Drink>())
                {
                    if (activeItem != null && activeItem != heldDrink)
                    {
                        activeItem.SetActive(false);
                    }

                    heldDrink.SetActive(true);
                    activeItem = heldDrink;
                }

                if (item.GetComponent<Cake>())
                {
                    if (activeItem != null && activeItem != heldCake)
                    {
                        activeItem.SetActive(false);
                    }

                    heldCake.SetActive(true);
                    activeItem = heldCake;
                }

                if (item.GetComponent<Fries>())
                {
                    if (activeItem != null && activeItem != heldFries)
                    {
                        activeItem.SetActive(false);
                    }

                    heldFries.SetActive(true);
                    activeItem = heldFries;
                }

                if (item.GetComponent<Tray>())
                {
                    if (activeItem != null && activeItem != heldTray)
                    {
                        activeItem.SetActive(false);
                    }

                    heldTray.SetActive(true);
                    activeItem = heldTray;
                }
            }
        }

        // Projectile selection will change slot of use and update ui component to reflect selection
        if (Input.GetButton("Item1"))
        {
            projectileSelect = 1;
            slot1.colors = unusedSlot;
            slot2.colors = usedSlot;
            slot3.colors = unusedSlot;

            // If the slot is not empty, hold the item
            if (inventory.mSlots[projectileSelect].Count > 0)
            {
                InventoryItemBase item = inventory.mSlots[projectileSelect].FirstItem;

                if (item.GetComponent<Burger>())
                {
                    if (activeItem != null && activeItem != heldBurger)
                    {
                        activeItem.SetActive(false);
                    }

                    heldBurger.SetActive(true);
                    activeItem = heldBurger;
                }

                if (item.GetComponent<Donut>())
                {
                    if (activeItem != null && activeItem != heldDonut)
                    {
                        activeItem.SetActive(false);
                    }

                    heldDonut.SetActive(true);
                    activeItem = heldDonut;
                }

                if (item.GetComponent<Drink>())
                {
                    if (activeItem != null && activeItem != heldDrink)
                    {
                        activeItem.SetActive(false);
                    }

                    heldDrink.SetActive(true);
                    activeItem = heldDrink;
                }

                if (item.GetComponent<Cake>())
                {
                    if (activeItem != null && activeItem != heldCake)
                    {
                        activeItem.SetActive(false);
                    }

                    heldCake.SetActive(true);
                    activeItem = heldCake;
                }

                if (item.GetComponent<Fries>())
                {
                    if (activeItem != null && activeItem != heldFries)
                    {
                        activeItem.SetActive(false);
                    }

                    heldFries.SetActive(true);
                    activeItem = heldFries;
                }

                if (item.GetComponent<Tray>())
                {
                    if (activeItem != null && activeItem != heldTray)
                    {
                        activeItem.SetActive(false);
                    }

                    heldTray.SetActive(true);
                    activeItem = heldTray;
                }
            }
        }

        // Projectile selection will change slot of use and update ui component to reflect selection
        if (Input.GetButton("Item2"))
        {
            projectileSelect = 2;
            slot1.colors = unusedSlot;
            slot2.colors = unusedSlot;
            slot3.colors = usedSlot;

            // If the slot is not empty, hold the item
            if (inventory.mSlots[projectileSelect].Count > 0)
            {
                InventoryItemBase item = inventory.mSlots[projectileSelect].FirstItem;

                if (item.GetComponent<Burger>())
                {
                    if (activeItem != null && activeItem != heldBurger)
                    {
                        activeItem.SetActive(false);
                    }

                    heldBurger.SetActive(true);
                    activeItem = heldBurger;
                }

                if (item.GetComponent<Donut>())
                {
                    if (activeItem != null && activeItem != heldDonut)
                    {
                        activeItem.SetActive(false);
                    }

                    heldDonut.SetActive(true);
                    activeItem = heldDonut;
                }

                if (item.GetComponent<Drink>())
                {
                    if (activeItem != null && activeItem != heldDrink)
                    {
                        activeItem.SetActive(false);
                    }

                    heldDrink.SetActive(true);
                    activeItem = heldDrink;
                }

                if (item.GetComponent<Cake>())
                {
                    if (activeItem != null && activeItem != heldCake)
                    {
                        activeItem.SetActive(false);
                    }

                    heldCake.SetActive(true);
                    activeItem = heldCake;
                }

                if (item.GetComponent<Fries>())
                {
                    if (activeItem != null && activeItem != heldFries)
                    {
                        activeItem.SetActive(false);
                    }

                    heldFries.SetActive(true);
                    activeItem = heldFries;
                }

                if (item.GetComponent<Tray>())
                {
                    if (activeItem != null && activeItem != heldTray)
                    {
                        activeItem.SetActive(false);
                    }

                    heldTray.SetActive(true);
                    activeItem = heldTray;
                }
            }
        }


        // Check if there is a keypress for an item to pickup
        if (mItemToPickup != null && Input.GetButton("Submit"))
        {
            // TODO: Add If logic to say if the object is moving you can't pick it up
            inventory.AddItem(mItemToPickup);
            mItemToPickup.OnPickup();
            hud.CloseMessagePanel();
            mItemToPickup = null;
            mItemGlow.enabled = false;
        }

        // If a projectile has been selected allow for firing
        if (projectileSelect >= 0)
        {
            if (Input.GetButton("Fire1") && Time.time > nextFire && !inventory.mSlots[projectileSelect].IsEmpty)
            {
                playerAnimator.SetTrigger("Attack");
                nextFire = Time.time + fireRate;
            }
        }
    }

    void Fire()
    {
        InventoryItemBase item = inventory.mSlots[projectileSelect].FirstItem;

        // Each Projectile will have an Item script defining its sprite, use this to identify weapon used
        if (item.GetComponent<Burger>())
        {
            GameObject thrown = Instantiate(burger, projSpawn.position, projSpawn.rotation);
            thrown.tag = this.tag + "Thrown";
            inventory.RemoveItem(item, projectileSelect);
        }

        if (item.GetComponent<Donut>())
        {
            GameObject thrown = Instantiate(donut, projSpawn.position, projSpawn.rotation);
            thrown.tag = this.tag + "Thrown";
            inventory.RemoveItem(item, projectileSelect);
        }

        if (item.GetComponent<Drink>())
        {
            GameObject thrown = Instantiate(drink, projSpawn.position, projSpawn.rotation);
            thrown.tag = this.tag + "Thrown";
            inventory.RemoveItem(item, projectileSelect);
        }

        if (item.GetComponent<Cake>())
        {
            Vector3 location = projSpawn.position;
            location.y = 0;
            GameObject thrown = Instantiate(cake, location, projSpawn.rotation);
            thrown.tag = this.tag + "Thrown";
            inventory.RemoveItem(item, projectileSelect);
        }

        if (item.GetComponent<Fries>())
        {
            GameObject thrown1 = Instantiate(fries, leftSpawn.position, leftSpawn.rotation);
            thrown1.tag = this.tag + "Thrown";
            GameObject thrown2 = Instantiate(fries, projSpawn.position, projSpawn.rotation);
            thrown2.tag = this.tag + "Thrown";
            GameObject thrown3 = Instantiate(fries, rightSpawn.position, rightSpawn.rotation);
            thrown3.tag = this.tag + "Thrown";
            inventory.RemoveItem(item, projectileSelect);
        }

        // Check and see if the slot is now empty, if it is remove the object from the hand
        if (inventory.mSlots[projectileSelect].Count < 1)
            activeItem.SetActive(false);
    }

    private void Awake()
    {
        floorMask = LayerMask.GetMask("Floor");
        playerRigid = GetComponent<Rigidbody>();
        playerAnimator = GetComponent<Animator>();

        // Reaching the border components of the slots for highlighting
        GameObject hudInterface = GameObject.Find("HUD");
        
        // Get the InventoryPanel object
        Transform slots = hudInterface.transform.GetChild(0);

        // Collect and assign the button of each slot to reference variables
        slot1 = slots.GetChild(0).GetChild(0).GetComponent<Button>();
        slot2 = slots.GetChild(1).GetChild(0).GetComponent<Button>();
        slot3 = slots.GetChild(2).GetChild(0).GetComponent<Button>();

        // Initialize color presets for slot in use and slot not in use
        ColorBlock cbUse = slot1.colors;
        ColorBlock cbUnuse = slot2.colors;

        usedSlot = cbUse;
        unusedSlot = cbUnuse;

        // Collect the held item references

        health = startingHealth;
    }

    void Start()
    {
        inventory.ItemUsed += Inventory_ItemUsed;
    }

    private void Inventory_ItemUsed(object sender, InventoryEventArgs e)
    {
        InventoryItemBase item = e.Item;
        Debug.Log("used");
        // Do something with the item
        GameObject goItem = (item as MonoBehaviour).gameObject;

        goItem.SetActive(true);
        goItem.GetComponent<Collider>().enabled = false;

        //goItem.transform.parent = Hand.transform;
        //goItem.transform.position = Hand.transform.position;
        //projectile = goItem;

    }

    private void FixedUpdate()
    {
        if (!alive)
        {
            return;
        }

        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        /*
        string[] gamePads = Input.GetJoystickNames();
        if(gamePads[0].Length > 1)
        {
            TurnWithStick();
        }
        else
        {
            TurnWithMouse();
        }
        */

        TurnWithMouse();
        Move(h, v);
        Animate(h, v);
        
    }

    private void Move(float h, float v)
    {
        movement.Set(h, 0f, v);
        movement = movement.normalized * speed * Time.deltaTime;

        if (!playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("Unarmed-Attack-R3"))
            if(!playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("Unarmed-Death1"))
                if(!playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("Unarmed-GetHit-F1"))
                    playerRigid.MovePosition(transform.position + movement);

    }

    private void TurnWithMouse()
    {
        Ray camRay = Camera.main.ScreenPointToRay(Input.mousePosition);

        RaycastHit floorHit;

        if (Physics.Raycast(camRay, out floorHit, camRayLen, floorMask))
        {
            Vector3 playerToMouse = floorHit.point - transform.position;
            playerToMouse.y = 0f;

            Quaternion newRotation = Quaternion.LookRotation(playerToMouse);

            playerRigid.MoveRotation(newRotation);
        }
    }

    /*
    private void TurnWithStick()
    {
        Vector3 lookVector = new Vector3(Input.GetAxis("Mouse X"), 0, Input.GetAxis("Mouse Y"));

        if(lookVector.x != 0 || lookVector.y != 0)
            transform.rotation = Quaternion.LookRotation(lookVector);
    }
    */



    private InventoryItemBase mItemToPickup = null;
    private InventoryItemBase prevItem = null;
    private Behaviour mItemGlow = null;
    private Behaviour prevGlow = null;

    IEnumerator Respawn()
    {
        alive = false;
        GameManager.setObjectLocation(gameObject, "respawn");
        health = startingHealth;
        //money = startingMoney;
        yield return new WaitForSeconds(10);
        if(gameObject.tag == "scienceGeek")
        {
            GameManager.setObjectLocation(gameObject, "scienceGeek");
        }
        else if(gameObject.tag == "bookWorm")
        {
            GameManager.setObjectLocation(gameObject, "bookWorm");
        }
        else if(gameObject.tag == "jocks")
        {
            GameManager.setObjectLocation(gameObject, "jocks");
        }
        alive = true;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Food"))
        {
            // Get the item reference
            InventoryItemBase item = other.GetComponent<InventoryItemBase>();
            //Halo glow = other.GetComponent<Halo>();
            mItemGlow = (Behaviour)other.GetComponent("Halo");

            // Ensure that the glow on the previously hovered item is off if the colliders overlap
            if (prevItem != null && item != prevItem)
            {
                prevGlow.enabled = false;
            }

            // If the item exists and there is room for it in the inventory, display pick up message
            if (!inventory.IsFull(item))
            {
                mItemGlow.enabled = true;
                prevItem = item;
                prevGlow = mItemGlow;
                mItemToPickup = item;
                hud.OpenMessagePanel("");
            }
        }
        
        if (GetHit(other))
        {
            health--;
            Destroy(other.gameObject);
            playerAnimator.SetTrigger("Hit");
            //SoundManager.HitSound(transform);

            if (health <= 0)
            {
                // increase score for projectile team
                if(other.tag == "scienceGeekThrown")
                {
                    GameManager.scienceGeeksScore++;
                }
                else if(other.tag == "jocksThrown")
                {
                    GameManager.jocksScore++;
                }
                else if(other.tag == "bookWormThrown")
                {
                    GameManager.bookWormsScore++;
                }
                // start respawn timer
                // die; wait for animation
                // spawn money equal to amount before death
                playerAnimator.SetTrigger("Die");
                StartCoroutine("Respawn");
            }
        }
    }

    // Boolean return of enemy projectile entering collider
    private Boolean GetHit(Collider other)
    {
        String myTeam = this.tag;
        String enemy1 = "";
        String enemy2 = "";

        switch (myTeam)
        {
            case "scienceGeek":
                enemy1 = "jocks";
                enemy2 = "bookWorm";
                break;

            case "jocks":
                enemy1 = "scienceGeek";
                enemy2 = "bookWorm";
                break;

            case "bookWorm":
                enemy1 = "scienceGeek";
                enemy2 = "jocks";
                break;
        }
        
        if (other.tag == (enemy1 + "Thrown") || other.tag == (enemy2 + "Thrown"))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        IInventoryItem item = other.GetComponent<IInventoryItem>();
        if (item != null)
        {
            hud.CloseMessagePanel();
            mItemToPickup = null;
            mItemGlow.enabled = false;
            mItemGlow = null;
        }
    }

    private void Animate(float h, float v)
    {
        if (h == 0 && v == 0) //not moving
            playerAnimator.SetInteger("WalkState", 4); // return to idle

        float facingAngle = playerRigid.rotation.eulerAngles.y;


        if (h == 0 && v == 1) // moving north
        {
            if (facingAngle > 315 || facingAngle <= 45) // facing north
                playerAnimator.SetInteger("WalkState", 0); // walk forward

            if (facingAngle > 45 && facingAngle <= 135) // facing east
                playerAnimator.SetInteger("WalkState", 3); // strafe left

            if (facingAngle > 135 && facingAngle <= 225) // facing south
                playerAnimator.SetInteger("WalkState", 2); // walk backwards

            if (facingAngle > 225 && facingAngle <= 315) // facing west
                playerAnimator.SetInteger("WalkState", 1); // strafe right
        }

        if (h == 1 && v == 1) // moving northeast
        {
            if (facingAngle > 0 && facingAngle <= 90) // facing northeast
                playerAnimator.SetInteger("WalkState", 0); // walk forward

            if (facingAngle > 90 && facingAngle <= 180) // facing southeast
                playerAnimator.SetInteger("WalkState", 3); // strafe left

            if (facingAngle > 180 && facingAngle <= 270) // facing southwest
                playerAnimator.SetInteger("WalkState", 2); // walk backwards

            if (facingAngle > 270 && facingAngle <= 360) // facing northwest
                playerAnimator.SetInteger("WalkState", 1); // strafe right
        }

        if (h == 1 && v == 0) // moving east
        {
            if (facingAngle > 45 && facingAngle <= 135) // facing east
                playerAnimator.SetInteger("WalkState", 0); // walk forward

            if (facingAngle > 135 && facingAngle <= 225) // facing south
                playerAnimator.SetInteger("WalkState", 3); // strafe left

            if (facingAngle > 225 && facingAngle <= 315) // facing west
                playerAnimator.SetInteger("WalkState", 2); // walk backwards

            if (facingAngle > 315 || facingAngle <= 45) // facing north
                playerAnimator.SetInteger("WalkState", 1); // strafe right
        }

        if (h == 1 && v == -1) // moving southeast
        {
            if (facingAngle > 90 && facingAngle <= 180) // facing southeast
                playerAnimator.SetInteger("WalkState", 0); // walk forward

            if (facingAngle > 180 && facingAngle <= 270) // facing southwest
                playerAnimator.SetInteger("WalkState", 3); // strafe left

            if (facingAngle > 270 && facingAngle <= 360) // facing northwest
                playerAnimator.SetInteger("WalkState", 2); // walk backwards

            if (facingAngle > 0 && facingAngle <= 90) // facing northeast
                playerAnimator.SetInteger("WalkState", 1); // strafe right
        }

        if (h == 0 && v == -1) // moving south
        {
            if (facingAngle > 135 && facingAngle <= 225) // facing south
                playerAnimator.SetInteger("WalkState", 0); // walk forward

            if (facingAngle > 225 && facingAngle <= 315) // facing west
                playerAnimator.SetInteger("WalkState", 3); // strafe left

            if (facingAngle > 315 || facingAngle <= 45) // facing north
                playerAnimator.SetInteger("WalkState", 2); // walk backwards

            if (facingAngle > 45 && facingAngle <= 135) // facing east
                playerAnimator.SetInteger("WalkState", 1); // strafe right
        }

        if (h == -1 && v == -1) // moving southwest
        {
            if (facingAngle > 90 && facingAngle <= 180) // facing southeast
                playerAnimator.SetInteger("WalkState", 1); // strafe right

            if (facingAngle > 180 && facingAngle <= 270) // facing southwest
                playerAnimator.SetInteger("WalkState", 0); // walk forward

            if (facingAngle > 270 && facingAngle <= 360) // facing northwest
                playerAnimator.SetInteger("WalkState", 3); // strafe left

            if (facingAngle > 0 && facingAngle <= 90) // facing northeast
                playerAnimator.SetInteger("WalkState", 2); // walk backwards
        }

        if (h == -1 && v == 0) // moving west
        {
            if (facingAngle > 45 && facingAngle <= 135) // facing east
                playerAnimator.SetInteger("WalkState", 2); // walk backwards

            if (facingAngle > 135 && facingAngle <= 225) // facing south
                playerAnimator.SetInteger("WalkState", 1); // strafe right

            if (facingAngle > 225 && facingAngle <= 315) // facing west
                playerAnimator.SetInteger("WalkState", 0); // walk forwards

            if (facingAngle > 315 || facingAngle <= 45) // facing north
                playerAnimator.SetInteger("WalkState", 3); // strafe left
        }

        if (h == -1 && v == 1) // moving northwest
        {
            if (facingAngle > 90 && facingAngle <= 180) // facing southeast
                playerAnimator.SetInteger("WalkState", 2); // walk backwards

            if (facingAngle > 180 && facingAngle <= 270) // facing southwest
                playerAnimator.SetInteger("WalkState", 1); // strafe right

            if (facingAngle > 270 && facingAngle <= 360) // facing northwest
                playerAnimator.SetInteger("WalkState", 0); // walk forwards

            if (facingAngle > 0 && facingAngle <= 90) // facing northeast
                playerAnimator.SetInteger("WalkState", 3); // strafe left
        }
    }
}
