using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private EnemyController enemyController;
    public float currentHealthPoints;

    void Awake()
    {
        enemyController = GetComponent<EnemyController>();
        currentHealthPoints = enemyController.GetHealthPoints();
    }

    public void ChangeHealth(float healthChange)
    {
        currentHealthPoints += healthChange;

        if (currentHealthPoints <= 0)
        {
            Destroy(gameObject);
        }
    }
}
