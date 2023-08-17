using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RollState : PlayerBaseState
{
    public override void EnterState(PlayerStateManager player)
    {
        player.animator.SetTrigger("Roll");
    }
    public override void ExitState(PlayerStateManager player)
    {
        player.animator.ResetTrigger("Roll");
    }

    public override void HandleInput(PlayerStateManager player)
    {
    }

    public override void UpdateState(PlayerStateManager player)
    {
       
    }
}