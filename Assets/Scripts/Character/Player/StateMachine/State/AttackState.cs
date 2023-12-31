using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState : PlayerBaseState
{
    bool canCombo;
    bool attack;
    int comboCount;

    public override void EnterState(PlayerStateManager player)
    {
        comboCount = 0;

        player.animator.SetInteger("Combo", comboCount);
        player.animator.SetTrigger("Attack");
        player.animator.SetBool("AttackDone", false);

        canCombo = false;
        attack = false;
    }
    public override void ExitState(PlayerStateManager player)
    {
    }

    public override void HandleInput(PlayerStateManager player)
    {
        if (player.playerInput.actions["Attack"].triggered == true && canCombo == true)
        {
            attack = true;
        }
    }

    public override void UpdateState(PlayerStateManager player)
    {
        bool curAnim = player.animator.GetCurrentAnimatorStateInfo(0).IsName("Attack" + comboCount);
        if (player.animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.7f && curAnim)
        {
            canCombo = true;

            if (attack == true)
            {
                comboCount++;
                if (comboCount >= 3)
                {
                    comboCount = 0;
                }

                player.animator.SetInteger("Combo", comboCount);
                attack = false;
                canCombo = false;
            }
        }
        if (player.animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.98f && curAnim)
        {
            player.animator.SetBool("AttackDone", true);
            player.SwitchState(player.idlingState);
        }
    }
}
