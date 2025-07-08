using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SkateWorld.FinalCharacterController;
using Unity.Cinemachine;



public class PlayerWalkState : PlayerBaseState
{
    public PlayerWalkState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
    : base(currentContext, playerStateFactory){}

    public override void EnterState(){
        Debug.Log("Walking state");
        Ctx.Animator.SetBool(Ctx.IsWalkingHash, true);
        //Ctx.Animator.SetBool(Ctx.IsRunningHash, false);

    }

    public override void UpdateState(){

        


        Vector2 inputTarget = Ctx.CurrentMovementInput;
        Ctx.CurrentBlendInput = Vector3.Lerp(Ctx.CurrentBlendInput, inputTarget, Ctx.LocoBlendSpeed * Time.deltaTime);

        //Ctx.Animator.SetFloat(Ctx.InputXHash, Ctx.CurrentBlendInput.x);
        //Ctx.Animator.SetFloat(Ctx.InputYHash, Ctx.CurrentBlendInput.y);

        Vector3 cameraForwardXZ = new Vector3(Ctx.PlayerCamera.transform.forward.x, 0f, Ctx.PlayerCamera.transform.forward.z).normalized;
        Vector3 cameraRightXZ = new Vector3(Ctx.PlayerCamera.transform.right.x, 0f, Ctx.PlayerCamera.transform.right.z).normalized;
        Vector3 movementDirection = cameraRightXZ * Ctx.CurrentMovementInput.x + cameraForwardXZ * Ctx.CurrentMovementInput.y;

        
        Vector3 movementDelta = movementDirection * Ctx.RunAcceleration * Time.deltaTime;
        Vector3 newVelocity = Ctx.CharacterController.velocity + movementDelta;

        Vector3 currentDrag = newVelocity.normalized * Ctx.Drag * Time.deltaTime;
        newVelocity = (newVelocity.magnitude > Ctx.Drag * Time.deltaTime) ? newVelocity - currentDrag : newVelocity * (1 - Ctx.Drag * Time.deltaTime);
        //newVelocity = (newVelocity.magnitude > Ctx.Drag * Time.deltaTime) ? newVelocity - currentDrag : Vector3.zero;
        newVelocity = Vector3.ClampMagnitude(newVelocity, Ctx.RunSpeed);


        Ctx.AppliedMovementX = newVelocity.x;
        Ctx.AppliedMovementZ = newVelocity.z;

        //Ctx.AppliedMovementX = Ctx.CurrentMovementInput.x;
        //Ctx.AppliedMovementZ = Ctx.CurrentMovementInput.y;

        // Rotation of character to point towards move direction

        Quaternion toRotation = Quaternion.LookRotation(movementDirection, Vector3.up);

        

        if (inputTarget.y < 0 && inputTarget.x == 0)
        {
           
            if (Ctx.OrbFollow != null)  
            {
                Debug.Log("going bacck");
                Ctx.OrbFollow.HorizontalAxis.CancelRecentering();
            }
       
        }

        if (movementDirection != Vector3.zero)
        {
            Ctx.CharacterController.transform.rotation = Quaternion.RotateTowards(Ctx.CharacterController.transform.rotation, toRotation, Ctx.RotationSpeed * Time.deltaTime);
        }

        //if (Ctx.CurrentMovementInput == Vector2.right || Ctx.CurrentMovementInput == Vector2.left)
        //{
            
        //    Ctx.OrbFollow.transform.rotation = Quaternion.RotateTowards(Ctx.OrbFollow.transform.rotation, toRotation, 360f * Time.deltaTime);
        //}

        CheckSwitchStates();
    }

    public override void ExitState(){}

    public override void InitializeSubState(){}

    public override void CheckSwitchStates() {
        if (!Ctx.IsMovementPressed) {
            SwitchState(Factory.Idle());
        }
    }


}

