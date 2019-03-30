using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioClip hitSound;

    public void HitSound(Transform t)
    {
        AudioSource.PlayClipAtPoint(hitSound, t.position);
    }
}
