using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageState : PlayerBaseState
{
    public override void EnterState(PlayerStateManager player)
    {
    }
    public override void ExitState(PlayerStateManager player)
    {
       
    }

    public override void HandleInput(PlayerStateManager player)
    {
    }

    public override void UpdateState(PlayerStateManager player)
    {       
        if (player._animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.9f &&
                    player._animator.GetCurrentAnimatorStateInfo(0).IsName("Damage"))
        {
            player.SwitchState(player.idlingState);
        }
    }
}