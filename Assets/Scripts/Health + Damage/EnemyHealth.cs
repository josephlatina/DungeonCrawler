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
        currentHealthPoints = enemyController.GetMaxHealthPoints();
    }

    public void ChangeHealth(float healthChange)
    {
        currentHealthPoints += healthChange;

        if (currentHealthPoints <= 0)
        {
            StartCoroutine(DieTimer());
        }
        else if (healthChange < 0 && currentHealthPoints > 0)
        {
            enemyController.Hurt();
        }


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
