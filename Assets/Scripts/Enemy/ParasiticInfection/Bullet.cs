using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Rigidbody2D rb;

    [Header("Bullet Settings"), Space] public Vector2 forceDirection; // Direction of the force
    public float forceMagnitude = 10f; // Magnitude of the force

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (enabled)
        {
            rb.AddForce(forceDirection * forceMagnitude, ForceMode2D.Impulse);
        }
        else
        {
            rb.velocity = Vector2.zero;
            transform.position = Vector3.zero;
        }
    }
}