using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SkateWorld.FinalCharacterController
{
    [DefaultExecutionOrder(-2)]
    public class PlayerLocoInput : MonoBehaviour, PlayerControls.IPlayerLocomapActions
    {
        public PlayerControls PlayerControls { get; private set; }

        public Vector2 MovementInput { get; private set; }
        public Vector2 LookInput { get; private set; }

        public Vector2 CameraZoom { get; private set; }


        private void OnEnable()
        {
            PlayerControls = new PlayerControls();
            PlayerControls.Enable();

            PlayerControls.PlayerLocomap.Enable();
            PlayerControls.PlayerLocomap.SetCallbacks(this);
        }

        private void OnDisable()
        {
            PlayerControls.PlayerLocomap.Disable();
            PlayerControls.PlayerLocomap.RemoveCallbacks(this);
        }

        public void OnMovement(InputAction.CallbackContext context)
        {
            MovementInput = context.ReadValue<Vector2>();
            print(MovementInput);
        }

        public void OnLook(InputAction.CallbackContext context)
        {
            LookInput = context.ReadValue<Vector2>();
        }

        //public void OnJump(InputAction.CallbackContext context)
        //{
        //    _isJumpPressed = context.ReadValue<Vector2>();
        //}

        //public void OnZoom(InputAction.CallbackContext context)
        //{
        //    CameraZoom = context.ReadValue<Vector2>();
        //    print(CameraZoom);
        //}
    }

}