using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SkateWorld.FinalCharacterController;

public class PlayerGroundedState : PlayerBaseState, IRootState
{
    public PlayerGroundedState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
    : base (currentContext, playerStateFactory) {

        IsRootState = true;

    }

    public override void EnterState(){

        Debug.Log("grounded state");
        Ctx.IsMountPressed = false;
        InitializeSubState();
        HandleGravity();

    }


    public override void UpdateState(){
        CheckSwitchStates();
    }

    public override void ExitState(){}

    public override void CheckSwitchStates(){

        // if player is grounded and jump is pressed, switch to jump state
        //if (Ctx.IsJumpPressed && !Ctx.RequireNewJumpPress)
        //{
        //    Debug.Log("hye there");
        //    SwitchState(Factory.Jump());
        //}

        // if player is not grounded and jump is not pressed, switch to fall state
        if (!Ctx.CharacterController.isGrounded) {

            SwitchState(Factory.Fall());

        }

        if (Ctx.IsMountPressed && !Ctx.RequireNewMountPress)
        {
            SwitchState(Factory.Skate());
        }

    }

    public override void InitializeSubState(){

        if (!Ctx.IsMovementPressed) {

            SetSubState(Factory.Idle());

        } else if (Ctx.IsMovementPressed) {

            SetSubState(Factory.Walk());

        }

        //if (Ctx.IsMountPressed)
        //{
        //        SetSubState(Factory.Skate());
        //}

    }

    public void HandleGravity()
    {

        Ctx.CurrentMovementY = Ctx.Gravity;
        Ctx.AppliedMovementY = Ctx.Gravity;

    }

}