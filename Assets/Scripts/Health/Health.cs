using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    public float healthPoints;
    public int defence;
    public float incomingDamage;


    public void ChangeHealth(float healthChange)
    {
        healthPoints += healthChange;
    }
}
