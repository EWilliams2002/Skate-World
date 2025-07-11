using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SkateWorld.FinalCharacterController;
using Unity.Cinemachine;
using System.Threading;
using System.Threading.Tasks;



public class PlayerSkateState : PlayerBaseState
{
    public PlayerSkateState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
    : base(currentContext, playerStateFactory)
    {
        IsRootState = true;
    }


    

    public override void EnterState()
    {

        

        Ctx.Animator.SetBool(Ctx.IsMountingHash, true);
        InitializeSubState();
        HandleGravity();


        
        
        Debug.Log("enter skate");
        
        
    }


    public override void UpdateState()
    {
        CheckSwitchStates();
    }

    public override void ExitState() { }

    public override void InitializeSubState() { }

    public override void CheckSwitchStates()
    {
        //if (Ctx.IsJumpPressed && !Ctx.RequireNewJumpPress)
        //{
        //    Debug.Log("ollie");
        //}

        //else if (!Ctx.CharacterController.isGrounded)
        //{
        //    Debug.Log("tricks");
        //}

        //if (!Ctx.IsMovementPressed)
        //{
        //    //Debug.Log("cruising");
        //}

        //else if (Ctx.IsMovementPressed)
        //{
        //    Debug.Log("pushing");
        //}

        if (Ctx.IsMountPressed && !Ctx.RequireNewMountPress)
        {
            Debug.Log("dismounting");
            Ctx.Animator.SetBool(Ctx.IsDismountingHash, true);

            Ctx.RequireNewMountPress = true;

            SwitchState(Factory.Grounded());

        }
    }

    public void HandleGravity()
    {
        Ctx.CurrentMovementY = Ctx.Gravity;
        Ctx.AppliedMovementY = Ctx.Gravity;
    }

}