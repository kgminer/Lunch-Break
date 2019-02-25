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

    public GameObject projectile;
    public Transform projSpawn;
    public float fireRate;
    private float nextFire;

    public Inventory inventory;
    public HUD hud;

    // Use Update method for throwing projectiles
    void Update ()
    {
        // Check if there is a keypress for an item to pickup
        if (mItemToPickup != null && Input.GetKeyDown(KeyCode.E))
        {
            inventory.AddItem(mItemToPickup);
            mItemToPickup.OnPickup();
            hud.CloseMessagePanel();
        }

        if (Input.GetButton("Fire1") && Time.time > nextFire)
        {
            nextFire = Time.time + fireRate;
            Instantiate(projectile, projSpawn.position, projSpawn.rotation);
        }
    }

    private void Awake()
    {
        floorMask = LayerMask.GetMask("Floor");
        anim = GetComponent<Animator>();
        playerRigid = GetComponent<Rigidbody>();
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

