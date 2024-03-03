using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySound : MonoBehaviour
{
    public AudioSource audioSource;
    public List<AudioClip> onDialogueTextAppear;

    public void PlayRandomDialogueSound()
    {
        audioSource.PlayOneShot(onDialogueTextAppear[Random.Range(0, onDialogueTextAppear.Count)]);
    }
}
