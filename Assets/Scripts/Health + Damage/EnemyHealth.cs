using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private EnemyController enemyController;
    public float currentHealthPoints;

    public bool customDeath = false;

    void Awake()
    {
        enemyController = GetComponent<EnemyController>();
        currentHealthPoints = enemyController.GetMaxHealthPoints();
    }

    public void ChangeHealth(float healthChange)
    {
        currentHealthPoints += healthChange;

        if (currentHealthPoints <= 0)
        {
            if (!customDeath)
            {
                StartCoroutine(DieTimer());
            }
        }
        else if (healthChange < 0 && currentHealthPoints > 0)
        {
            enemyController.Hurt();
        }
    }

    /// <summary>
    /// Animation event to control death behaviour of enemy in a specific animation frame
    /// </summary>
    public void Death()
    {
        Debug.Log("DIE");
        Destroy(gameObject);
    }
    
    private IEnumerator DieTimer()
    {
        enemyController.Dead();
        Transform childTransform = transform.Find("Body");
        Vector3 enemyPosition = childTransform.position;
        yield return new WaitForSeconds(2f);
        Destroy(gameObject);
        GameManager.Instance.SpawnItem(enemyPosition);
    }
}