using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySound : MonoBehaviour
{
    public AudioSource audioSource;
    public List<AudioClip> onDialogueTextAppear;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayRandomDialogueSound()
    {
        audioSource.PlayOneShot(onDialogueTextAppear[Random.Range(0, onDialogueTextAppear.Count)]);
    }
}
