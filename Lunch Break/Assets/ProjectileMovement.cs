using UnityEngine;

public class ProjectileMovement : MonoBehaviour
{
    // Start is called before the first frame update
    public float speed;

    void Start()
    {
        GetComponent<Rigidbody>().velocity = transform.forward * speed;
    }
}