using SkateWorld.FinalCharacterController;
using UnityEngine;


public class PlayerGroundedState : PlayerBaseState
{
    public PlayerGroundedState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) : base (currentContext, playerStateFactory)
    { }

    public override void EnterState() {
        Debug.Log("HELLOOOO");
    }

    public override void UpdateState() { }

    public override void ExitState() { }

    public override void CheckSwitchStates() {
        //if (_ctx.IsJumpPressed) {
        //    SwitchState(_factory.Jump());
        //}
    }

    public override void InitializeSubState() { }
}
