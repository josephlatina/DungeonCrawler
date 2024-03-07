using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Status Effect")]
public class StatusEffectData : ScriptableObject
{
    public string EffectName;
    public float DamageOverTimeAmount;
    public float MovementPenalty;
    public float TickSpeed;
    public float Lifetime;

    public GameObject EffectParticles;
}
