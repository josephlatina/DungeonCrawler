using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class PlayerRangedState : IState
{
    private PlayerController player;
    private PlayerStats stats;

    private float rangedCounter, rangedCoolCounter;

    public PlayerRangedState(PlayerController player)
    {
        this.player = player;
    }

    public void Enter()
    {
        player.GetComponent<SpriteRenderer>().color = Color.yellow;
        stats = player.GetComponent<PlayerStats>();

        Attack();
    }

    public void Update()
    {
        RangedAttackCooldown();

        // Prevent player from rolling when attacking
        if (player.rolling)
        {
            player.rolling = false;
        }

        if (player.isRangedAttacking == false)
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

        rangedCounter = 0;
        rangedCoolCounter = 0;
    }

    void RangedAttackCooldown()
    {
        if (rangedCoolCounter <= 0 && rangedCounter <= 0)
        {
            rangedCounter = 0.25f; //TODO this will be replaced by the length of the attack animation
        }

        if (rangedCounter > 0)
        {
            rangedCounter -= Time.deltaTime;
            if (rangedCounter <= 0)
            {
                rangedCoolCounter = stats.currentAttackSpeed; //Time between attacks
                player.isRangedAttacking = false;
            }
        }
        if (rangedCoolCounter > 0)
        {
            rangedCoolCounter -= Time.deltaTime;
        }
    }

    void Attack()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        Vector3 rotation = mousePos - player.transform.position;
        float rotZ = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg;

        Debug.DrawRay(player.transform.position, rotation, Color.yellow, 0.25f);
    }


}
