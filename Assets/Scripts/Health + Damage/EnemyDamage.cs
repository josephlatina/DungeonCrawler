using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDamage : MonoBehaviour
{
    private EnemyController enemyController;
    public StatusEffectData data;

    void Awake()
    {
        enemyController = GetComponentInParent<EnemyController>();
        data = enemyController.effectEnemyApplies;
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            IEffectable effectable = col.gameObject.GetComponent<IEffectable>();
            if (effectable != null)
            {
                effectable.ApplyEffect(data);
            }
            col.gameObject.GetComponent<PlayerHealth>().ChangeHealth(-enemyController.strength);
        }
    }
}
