using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public float speed = 5f;

    Vector3 movement;
    Rigidbody playerRigid;
    int floorMask;
    float camRayLen = 100f;

    public Transform projSpawn;
    public float fireRate = 0.5f;
    private float nextFire;
    public float health = 3f;

    // Projectile object references
    public GameObject burger;
    public GameObject donut;
    public GameObject drink;
    public AudioClip hitSound;

    public Inventory inventory;
    public HUD hud;
    public GameObject Hand;

    // Variable to be set upon projectile selection within Update for either item 0, 1, or 2. Initialized to 0 for first slot
    private int projectileSelect = 0;

    // Tracking each inventory slot for easy highlighting
    Button slot1;
    Button slot2;
    Button slot3;

    // Slot color presets for reassigning 
    ColorBlock usedSlot;
    ColorBlock unusedSlot;

    // Use Update method for throwing projectiles
    void Update ()
    {
        // Projectile selection will change slot of use and update ui component to reflect selection
        if (Input.GetButton("Item0"))
        {
            projectileSelect = 0;
            slot1.colors = usedSlot;
            slot2.colors = unusedSlot;
            slot3.colors = unusedSlot;
        }

        // Projectile selection will change slot of use and update ui component to reflect selection
        if (Input.GetButton("Item1"))
        {
            projectileSelect = 1;
            slot1.colors = unusedSlot;
            slot2.colors = usedSlot;
            slot3.colors = unusedSlot;
        }

        // Projectile selection will change slot of use and update ui component to reflect selection
        if (Input.GetButton("Item2"))
        {
            projectileSelect = 2;
            slot1.colors = unusedSlot;
            slot2.colors = unusedSlot;
            slot3.colors = usedSlot;
        }

        // Check if there is a keypress for an item to pickup
        //if (mItemToPickup != null && Input.GetKeyDown(KeyCode.E))
        if (mItemToPickup != null && Input.GetButton("Submit"))
        {
            // TODO: Add If logic to say if the object is moving you can't pick it up
            inventory.AddItem(mItemToPickup);
            mItemToPickup.OnPickup();
            hud.CloseMessagePanel();
            mItemToPickup = null;
        }

        // If a projectile has been selected allow for firing
        if (projectileSelect >= 0)
        {
            if (Input.GetButton("Fire1") && Time.time > nextFire && !inventory.mSlots[projectileSelect].IsEmpty)
            {
                InventoryItemBase item = inventory.mSlots[projectileSelect].FirstItem;

                // Each Projectile will have an Item script defining its sprite, use this to identify weapon used
                if (item.GetComponent<Burger>())
                {
                    nextFire = Time.time + fireRate;
                    Instantiate(burger, projSpawn.position, projSpawn.rotation);
                    inventory.RemoveItem(item, projectileSelect);
                }

                if (item.GetComponent<Donut>())
                {
                    nextFire = Time.time + fireRate;
                    Instantiate(donut, projSpawn.position, projSpawn.rotation);
                    inventory.RemoveItem(item, projectileSelect);
                }

                if (item.GetComponent<Drink>())
                {
                    nextFire = Time.time + fireRate;
                    Instantiate(drink, projSpawn.position, projSpawn.rotation);
                    inventory.RemoveItem(item, projectileSelect);
                }
            }
        }
    }

    private void Awake()
    {
        floorMask = LayerMask.GetMask("Floor");
        playerRigid = GetComponent<Rigidbody>();

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
        
    }

    private void Move(float h, float v)
    {
        movement.Set(h, 0f, v);
        movement = movement.normalized * speed * Time.deltaTime;

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

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Food"))
        {
            // Get the item reference
            InventoryItemBase item = other.GetComponent<InventoryItemBase>();

            // If the item exists and there is room for it in the inventory, display pick up message
            if (!inventory.IsFull(item))
            {
                mItemToPickup = item;
                hud.OpenMessagePanel("");
            }
        }

        if (other.gameObject.CompareTag("Projectile1"))
        {
            health--;
            Destroy(other.gameObject);
            AudioSource.PlayClipAtPoint(hitSound, transform.position);

            if (health <= 0)
            {
                // increase score for projectile team
                // start respawn timer
                // die; wait for animation
                // spawn money equal to amount before death
                Destroy(gameObject);
            }
        }

        if (other.gameObject.CompareTag("Projectile3"))
        {
            health--;
            Destroy(other.gameObject);
            AudioSource.PlayClipAtPoint(hitSound, transform.position);

            if (health <= 0)
            {
                // increase score for projectile team
                // start respawn timer
                // die; wait for animation
                // spawn money equal to amount before death
                Destroy(gameObject);
            }
        }

        /*
        if (other.gameObject.CompareTag("Projectile4"))
        {
            health--;
            Destroy(other.gameObject);
            if (health <= 0)
            {
                // increase score for projectile team
                // start respawn timer
                // die; wait for animation
                // spawn money equal to amount before death
                Destroy(gameObject);
            }
        }
       */
    }

    private void OnTriggerExit(Collider other)
    {
        IInventoryItem item = other.GetComponent<IInventoryItem>();
        if (item != null)
        {
            hud.CloseMessagePanel();
            mItemToPickup = null;
        }
    }
}

