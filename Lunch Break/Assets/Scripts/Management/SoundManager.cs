using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioClip hitSound;
    public AudioClip uiSelection;

    public void HitSound(Transform t)
    {
        AudioSource.PlayClipAtPoint(hitSound, t.position);
    }

    public void UISound(Transform t)
    {
        AudioSource.PlayClipAtPoint(uiSelection, t.position);
    }
}
