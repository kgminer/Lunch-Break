using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorScript : MonoBehaviour
{
    Animator anim;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }
    private void OnTriggerEnter(Collider other)
    {
        anim.SetTrigger("OpenDoor");
    }

    private void OnTriggerExit(Collider other)
    {
        anim.enabled = true;
    }

    private void PauseAnimationEvent()
    {
        anim.enabled = false;
    }
}
