using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SkateWorld.FinalCharacterController;

public class PlayerIdleState : PlayerBaseState
{
    public PlayerIdleState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
    : base(currentContext, playerStateFactory){}

    public override void EnterState(){

        Debug.Log("idle state");
        Ctx.IsMountPressed = false;
        Ctx.Animator.SetBool(Ctx.IsWalkingHash, false);
        //Ctx.Animator.SetBool(Ctx.IsRunningHash, false);
        Ctx.AppliedMovementX = 0;
        Ctx.AppliedMovementZ = 0;

    }

    public override void UpdateState(){

        CheckSwitchStates();

    }

    public override void ExitState(){}

    public override void InitializeSubState(){}

    public override void CheckSwitchStates(){

        if (Ctx.IsMovementPressed) {

            SwitchState(Factory.Walk());

        }

        if (Ctx.IsJumpPressed && !Ctx.RequireNewJumpPress)
        {
            //Debug.Log("hye there");
            SwitchState(Factory.Jump());
        }

        //if (Ctx.IsMountPressed)
        //{
        //    SwitchState(Factory.Skate());
        //}

    }
}