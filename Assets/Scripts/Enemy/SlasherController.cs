using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlasherController : EnemyController
{
    private void FixedUpdate()
    {
        Move();
    }

    private void Move()
    {
        rb.AddForce(new Vector2(movementSpeed * Time.deltaTime, 0), ForceMode2D.Impulse);
    }
}
