using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Rigidbody2D rb;

    [Header("Bullet Settings"), Space] public Vector2 forceDirection; // Direction of the force
    public float forceMagnitude = 10f; // Magnitude of the force
    public AudioClip shootSound;
    public AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (rb.velocity == Vector2.zero)
        {
            Reset();
        }
    }

    public void Shoot()
    {
        if (rb.velocity == Vector2.zero)
        {
            rb.AddForce(forceDirection * forceMagnitude, ForceMode2D.Impulse);
            audioSource.PlayOneShot(shootSound);
        }
    }

    public void Reset()
    {
        rb.velocity = Vector2.zero;
        // transform.position = Vector3.zero;
        transform.localPosition = Vector3.zero;
    }
}