using System.Collections;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public float smoothing = 5f;

    Vector3 offset = new Vector3(0, 5, 0);

    private void Start()
    {
        //offset = transform.position - target.position;

        transform.position = target.position + offset;
    }

    private void FixedUpdate()
    {
        Vector3 targetCamPosition = target.position + offset;
        transform.position = Vector3.Lerp(transform.position, targetCamPosition, smoothing * Time.deltaTime);
    }
}
