using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDamage : MonoBehaviour
{
    private SlasherController slasherController;

    void Awake()
    {
        slasherController = GetComponentInParent<SlasherController>();
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            col.gameObject.GetComponent<PlayerHealth>().ChangeHealth(-slasherController.strength);
        }
    }
}
