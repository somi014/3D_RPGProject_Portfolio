using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealState : PlayerBaseState
{
    public override void EnterState(PlayerStateManager player)
    {
        player._animator.SetTrigger(player._animIDHeal);
    }
    public override void ExitState(PlayerStateManager player)
    {
    }

    public override void HandleInput(PlayerStateManager player)
    {
    }

    public override void UpdateState(PlayerStateManager player)
    {
        if (player._animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f &&
                    player._animator.GetCurrentAnimatorStateInfo(0).IsName("Heal"))
        {
            player._animator.ResetTrigger(player._animIDHeal);
            player.SwitchState(player.idlingState);
        }
    }
}