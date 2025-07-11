using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SkateWorld.FinalCharacterController;

public class PlayerFallState : PlayerBaseState, IRootState
{
    public PlayerFallState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
    : base (currentContext, playerStateFactory) {
        IsRootState = true;
    }

    public override void EnterState() {
        Debug.Log("falling state");
        InitializeSubState();
        Ctx.Animator.SetBool(Ctx.IsFallingHash, true);

    }

    public override void UpdateState() {

        HandleGravity();
        CheckSwitchStates();
    }

    public override void ExitState() {

        Ctx.Animator.SetBool(Ctx.IsFallingHash, false);
    }

    public void HandleGravity() {

        float previousYVelocity = Ctx.CurrentMovementY;
        Ctx.CurrentMovementY = Ctx.CurrentMovementY + Ctx.Gravity * Time.deltaTime;
        Ctx.AppliedMovementY = Mathf.Max((previousYVelocity + Ctx.CurrentMovementY) * .5f, -20.0f);


    }

    public override void CheckSwitchStates() {

        // if player is grounded, switch to the grounded state
        if (Ctx.CharacterController.isGrounded) {

            SwitchState(Factory.Grounded());

        }

    }

    public override void InitializeSubState() {

        if (!Ctx.IsMovementPressed) {

            SetSubState(Factory.Idle());

        } else if (Ctx.IsMovementPressed) {

            SetSubState(Factory.Walk());
        } 
    }
}