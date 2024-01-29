using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMeleeState : IState
{
    private PlayerController player;
    private PlayerStats stats;

    private float meleeCounter, meleeCoolCounter;

    public PlayerMeleeState(PlayerController player)
    {
        this.player = player;

    }

    public void Enter()
    {
        player.GetComponent<SpriteRenderer>().color = Color.blue;
        stats = player.GetComponent<PlayerStats>();

        Attack();
    }

    public void Update()
    {
        MeleeAttackCooldown();

        // prevent player from rolling when attacking
        if (player.rolling)
        {
            player.rolling = false;
        }

        if (player.isMeleeAttacking == false)
        {
            if (player.moveVal.x > 0.1f || player.moveVal.y > 0.1f)
            {
                player.PlayerStateMachine.TransitionTo(player.PlayerStateMachine.moveState);
            }
            else
            {
                player.PlayerStateMachine.TransitionTo(player.PlayerStateMachine.idleState);
            }
        }
    }

    public void FixedUpdate()
    {
        player.Move();
    }

    public void Exit()
    {
        player.GetComponent<SpriteRenderer>().color = Color.white;
        player.meleeTrigger.enabled = false;

        meleeCounter = 0;
        meleeCoolCounter = 0;
    }

    void MeleeAttackCooldown()
    {
        if (meleeCoolCounter <= 0 && meleeCounter <= 0)
        {
            meleeCounter = 0.25f; //TODO this will be replaced by the length of the attack animation
        }

        if (meleeCounter > 0)
        {
            meleeCounter -= Time.deltaTime;
            if (meleeCounter <= 0)
            {
                meleeCoolCounter = stats.currentAttackSpeed; //Time between attacks
                player.isMeleeAttacking = false;
            }
        }
        if (meleeCoolCounter > 0)
        {
            meleeCoolCounter -= Time.deltaTime;
        }
    }

    void Attack()
    {
        player.meleeTrigger.enabled = true;
        
    }
}
