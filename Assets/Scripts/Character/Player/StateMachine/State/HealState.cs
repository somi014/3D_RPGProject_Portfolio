using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealState : PlayerBaseState
{
    public override void EnterState(PlayerStateManager player)
    {
        player.animator.SetTrigger(player.animIDHeal);
    }
    public override void ExitState(PlayerStateManager player)
    {
    }

    public override void HandleInput(PlayerStateManager player)
    {
    }

    public override void UpdateState(PlayerStateManager player)
    {
        if (player.animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f &&
                    player.animator.GetCurrentAnimatorStateInfo(0).IsName("Heal"))
        {
            player.animator.ResetTrigger(player.animIDHeal);
            player.SwitchState(player.idlingState);
        }
    }
}