using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorScript : MonoBehaviour
{
    Animator anim;
    private bool exited;

    private void Start()
    {
        anim = GetComponent<Animator>();
        anim.enabled = true;
        exited = false;
    }
    private void OnTriggerEnter(Collider other)
    {
        
        if(other.tag == "Physical Collider")
        {
            anim.SetTrigger("OpenDoor");
            exited = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Physical Collider")
        {
            anim.enabled = true;
            exited = true;
        }
    }

    private void PauseAnimationEvent()
    {
        if(!exited)
        {
            anim.enabled = false;
        }
    }
}
