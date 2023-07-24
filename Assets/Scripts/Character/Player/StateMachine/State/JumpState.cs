using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpState : PlayerBaseState
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
        bool fall = player._animator.GetBool(player._animIDFreeFall);
        if (player.Grounded == true && fall == true)
        {
            player.SwitchState(player.idlingState);
        }
    }
}
