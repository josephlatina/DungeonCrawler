/*
 * EnemyController.cs
 * Author: Jehdi Aizon, 
 * Created: January 7, 2024
 * Description: Handles enemy cntroller movements.
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// movement speed of character
[SerializeField] private float movementSpeed;
// measured in damage per second
[SerializeField] private float attackSpeed;
// amount of hearts character inflicts to other (full heart or half heart)
[SerializeField] private float strength;
// amount of heart a character has
[SerializeField] private float healthPoints;
[SerializeField] private enum enemyStatus;


public class EnemyController : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
