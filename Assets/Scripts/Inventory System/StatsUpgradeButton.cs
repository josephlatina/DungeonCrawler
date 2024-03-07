using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class StatsUpgradeButton : MonoBehaviour
{
    [Header("Object References"), Space] [SerializeField]
    private Button button;

    [SerializeField] PlayerController player;

    [Header("Stat Upgrades"), Space] [SerializeField]
    float attackSpeed;

    [SerializeField] float strength;
    [SerializeField] int defence;

    [Header("Sound"), Space] public AudioClip powerupSound;
    public AudioClip onHoverSound;
    
    void OnEnable()
    {
        // Register Button Events
        button.onClick.AddListener(() => buttonCallBack());

        // pause game when pill is picked up
        player.GetComponent<PlayerInput>().enabled = false;
    }

    private void buttonCallBack()
    {
        player.StatUpgrade(attackSpeed, strength, defence);
        player.GetComponent<AudioSource>().PlayOneShot(powerupSound);
    }

    void OnDisable()
    {
        // Unregister Button Events
        button.onClick.RemoveAllListeners();
        player.GetComponent<PlayerInput>().enabled = true;
    }

    public void PlayOnHoverSound()
    {
        player.GetComponent<AudioSource>().PlayOneShot(onHoverSound, .5f);
    }
}