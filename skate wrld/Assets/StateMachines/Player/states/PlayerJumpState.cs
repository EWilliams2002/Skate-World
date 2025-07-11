using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SkateWorld.FinalCharacterController;

public class PlayerJumpState : PlayerBaseState, IRootState
{
    

    public PlayerJumpState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
    : base(currentContext, playerStateFactory){
        //IsRootState = true;
    }

    public override void EnterState(){

        Debug.Log("jumping");
        //InitializeSubState();
        HandleJump();

    }

    public override void UpdateState(){
        HandleGravity();
        CheckSwitchStates();
    }


    public override void CheckSwitchStates() {
       
        if (Ctx.CharacterController.isGrounded)
        {
            SwitchState(Factory.Grounded());
        }

    }

    public override void ExitState(){

        Ctx.Animator.SetBool(Ctx.IsJumpingHash, false);

        if (Ctx.IsJumpPressed) {

            Ctx.RequireNewJumpPress = true;

        }

        //Ctx.CurrentJumpResetRoutine = Ctx.StartCoroutine(IJumpResetRoutine());
        //if (Ctx.JumpCount == 3)
        //{
        //    Ctx.JumpCount = 0;
        //    Ctx.Animator.SetInteger(Ctx.JumpCountHash, Ctx.JumpCount);
        //}

    }

    public override void InitializeSubState(){

        if (!Ctx.IsMovementPressed) {

            SetSubState(Factory.Idle());

        } else if (Ctx.IsMovementPressed) {

            SetSubState(Factory.Walk());

        } 
    }
    // jump height 4 time 0.75
    void HandleJump() {
        
        Ctx.Animator.SetBool(Ctx.IsJumpingHash, true);
        Ctx.IsJumping = true;
        
        Ctx.CurrentMovementY = Ctx.InitialJumpVelocities[1];
        Ctx.AppliedMovementY = Ctx.InitialJumpVelocities[1];
    
    }

    public void HandleGravity() {

        bool isFalling = Ctx.CurrentMovementY <= 0.0f || !Ctx.IsJumpPressed;
        float fallMultiplier = 1.0f;

        if (isFalling)
        {

            float previousYVelocity = Ctx.CurrentMovementY;
            Ctx.CurrentMovementY = Ctx.CurrentMovementY + (Ctx.JumpGravities[1] * fallMultiplier * Time.deltaTime);
            Ctx.AppliedMovementY = Mathf.Max((previousYVelocity + Ctx.CurrentMovementY) * .5f, -20.0f);

        }
        else
        {

            float previousYVelocity = Ctx.CurrentMovementY;
            Ctx.CurrentMovementY = Ctx.CurrentMovementY + (Ctx.JumpGravities[1] * Time.deltaTime);
            Ctx.AppliedMovementY = (previousYVelocity + Ctx.CurrentMovementY) * .5f;

        }
    }
}