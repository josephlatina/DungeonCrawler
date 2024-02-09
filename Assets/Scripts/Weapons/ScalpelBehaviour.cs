using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScalpelBehaviour : MonoBehaviour
{
    private Vector3 direction;
    public float currentSpeed;

    public void SetDirection(Vector3 dir)
    {
        direction = dir;
    }

    void Update()
    {
        if (currentSpeed > 0)
        {
            transform.position += currentSpeed * Time.deltaTime * direction;
        }
    }
}
