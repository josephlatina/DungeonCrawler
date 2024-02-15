using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#nullable enable

public class BubbleCharacterSound : MonoBehaviour
{
    [SerializeField] float soundsPerSecond = 2;
    [SerializeField] AudioSource? audioSource;

    [SerializeField]
    [Range(0f, 1f)]
    float volumeVariance;

    private float TimeBetweenSounds => 1f / this.soundsPerSecond;

    private float timeOfLastSound = -1;


    public void OnCharacterAppeared() {
        
        // A character appeared. If more than TimeBetweenSounds seconds have
        // elapsed since the last time the sound was played, play it now.
        if ((Time.time - timeOfLastSound) >= TimeBetweenSounds) {
            PlaySound();
        }
    }

    private void PlaySound() {
        timeOfLastSound = Time.time;

        float scale = Random.value * volumeVariance;

        if (audioSource != null) {
            audioSource.PlayOneShot(audioSource.clip, 1f - scale);
        }
    }
}
