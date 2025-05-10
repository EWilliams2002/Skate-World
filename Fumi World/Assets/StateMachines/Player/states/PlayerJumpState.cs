using SkateWorld.FinalCharacterController;
using UnityEngine;

public class PlayerJumpState : PlayerBaseState
{
    public PlayerJumpState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) : base(currentContext, playerStateFactory)
    { }
    public override void EnterState() {
        HandleJump();
    }

    public override void UpdateState() { }

    public override void ExitState() { }

    public override void CheckSwitchStates() { }

    public override void InitializeSubState() { }

    void HandleJump() {
        // jump code
    }
}
