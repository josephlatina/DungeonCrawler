using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class PlaySound : MonoBehaviour
{
    public AudioSource audioSource;
    public List<AudioClip> audioClips;
    [Range(0, 1f)] public float volume = 1f;

    public void PlayRandomDialogueSound()
    {
        audioSource.PlayOneShot(audioClips[Random.Range(0, audioClips.Count)], volume);
    }
}