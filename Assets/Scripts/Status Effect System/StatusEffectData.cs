using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Status Effect")]
public class StatusEffectData : ScriptableObject
{
    public string effectName;
    public float damageOverTimeAmount;
    public float movementPenalty;
    public float tickSpeed;
    public float lifetime;

    public int immobilizedHealth;

    public GameObject poisonParticles;
    public Color immobilizedEffect;
    public Color stunEffect;
}
