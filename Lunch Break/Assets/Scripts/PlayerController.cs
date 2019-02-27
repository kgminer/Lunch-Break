using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 5f;

    Vector3 movement;
    Animator anim;
    Rigidbody playerRigid;
    int floorMask;
    float camRayLen = 100f;

    //public GameObject projectile;
    public Transform projSpawn;
    public float fireRate = 0.5f;
    private float nextFire;
    public float health = 3f;

    public GameObject burger;

    public Inventory inventory;
    public HUD hud;
    public GameObject Hand;

    // Use Update method for throwing projectiles
    void Update ()
    {
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

        if (Input.GetButton("Fire1") && Time.time > nextFire && !inventory.mSlots[0].IsEmpty)
        {
            // Debug.Log("inv" + inventory.mSlots[0].FirstItem);
            InventoryItemBase item = inventory.mSlots[0].FirstItem;

            // Each Projectile will have an Item script defining its sprite, use this to identify weapon used
            if (item.GetComponent<Burger>())
            {
                nextFire = Time.time + fireRate;
                Instantiate(burger, projSpawn.position, projSpawn.rotation);
                inventory.RemoveItem(item);
            }
        }
    }

    private void Awake()
    {
        floorMask = LayerMask.GetMask("Floor");
        anim = GetComponent<Animator>();
        playerRigid = GetComponent<Rigidbody>();
    }

    void Start()
    {
        inventory.ItemUsed += Inventory_ItemUsed;
    }

    private void Inventory_ItemUsed(object sender, InventoryEventArgs e)
    {
        InventoryItemBase item = e.Item;

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

        Move(h, v);
        TurnWithMouse();
        // Animate(h, v);
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

        if (Physics.Raycast(camRay, out floorHit, camRayLen, floorMask)) ;
        {
            Vector3 playerToMouse = floorHit.point - transform.position;
            playerToMouse.y = 0f;

            Quaternion newRotation = Quaternion.LookRotation(playerToMouse);

            playerRigid.MoveRotation(newRotation);
        }
    }

    private void Animate(float h, float v)
    {
        bool moving = h != 0f || v != 0f;
        anim.SetBool("IsWalking", moving);
    }

    private InventoryItemBase mItemToPickup = null;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Food"))
        {
            InventoryItemBase item = other.GetComponent<InventoryItemBase>();

            if (item != null)
            {
                mItemToPickup = item;
                hud.OpenMessagePanel("");
            }
        }

        if (other.gameObject.CompareTag("Projectile1"))
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

        if (other.gameObject.CompareTag("Projectile3"))
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

