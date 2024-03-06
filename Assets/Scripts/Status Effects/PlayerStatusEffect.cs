using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatusEffect : BaseStatusEffect
{
    public NormalStatus normal;
    public ImmobileStatus immobile;
    public PoisonStatus poison;
    public StunStatus stun;

    public PlayerStatusEffect(PlayerController player)
    {
        this.normal = new NormalStatus(player);
        this.immobile = new ImmobileStatus(player);
        this.poison = new PoisonStatus(player);
        this.stun = new StunStatus(player);
    }
}