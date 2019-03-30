using System.Collections;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public float smoothing = 5f;
    public float height = 5f;

    Vector3 offset;

    private void Start()
    {
        offset = new Vector3(0, height, 0);

        transform.position = target.position + offset;
    }

    private void FixedUpdate()
    {
        if(target != null)
        {
            Vector3 targetCamPosition = target.position + offset;
            transform.position = Vector3.Lerp(transform.position, targetCamPosition, smoothing * Time.deltaTime);
        }
    }
}
