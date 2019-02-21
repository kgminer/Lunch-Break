using UnityEngine;

public class AITriggers : MonoBehaviour
{
    // buy item
    // take damage
    //      potentially despawn


    private void OnTriggerEnter(Rigidbody other)
    {
        if(other.gameObject.tag == "Purchaser")
        {

        }
    }

}
