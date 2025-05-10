using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SkateWorld.FinalCharacterController
{
    [DefaultExecutionOrder(-1)]
    public class PlayerController : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private CharacterController _characterController;
        [SerializeField] private Camera _playerCamera;

        [Header("Base Movement")]
        public float runAcceleration = 0.25f;
        public float runSpeed = 4f;
        public float drag = 0.1f;
        public float rotationSpeed = 720f;

        //[Header("Camera Settings")]
        //public float lookSenseH = 0.1f;
        //public float lookSenseV = 0.1f;
        //public float lookLimitV = 89f;

        private PlayerLocoInput _playerLocoInput;
        //private Vector2 _cameraRotation = Vector2.zero;
        //private Vector2 _playerTargetRotation = Vector2.zero;

        private void Awake()
        {
            _playerLocoInput = GetComponent<PlayerLocoInput>();
        }

        private void Update()
        {
            Vector3 cameraForwardXZ = new Vector3(_playerCamera.transform.forward.x, 0f, _playerCamera.transform.forward.z).normalized;
            Vector3 cameraRightXZ = new Vector3(_playerCamera.transform.right.x, 0f, _playerCamera.transform.right.z).normalized;
            Vector3 movementDirection = -cameraRightXZ * _playerLocoInput.MovementInput.x + -cameraForwardXZ * _playerLocoInput.MovementInput.y;
            

            Vector3 movementDelta = movementDirection * runAcceleration * Time.deltaTime;
            Vector3 newVelocity = _characterController.velocity + -movementDelta;

            Vector3 currentDrag = newVelocity.normalized * drag * Time.deltaTime;
            newVelocity = (newVelocity.magnitude > drag * Time.deltaTime) ? newVelocity - currentDrag : Vector3.zero;
            newVelocity = Vector3.ClampMagnitude(newVelocity, runSpeed);

            _characterController.Move(newVelocity * Time.deltaTime);

            if (movementDirection != Vector3.zero)
            {
                Quaternion toRotation = Quaternion.LookRotation(movementDirection, Vector3.up);
                
                transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
            }
        }

        // OLD CAMERA CODE - could use some of this for first person

        //private void LateUpdate()
        //{

        //    _cameraRotation.x += lookSenseH * _playerLocoInput.LookInput.x;
        //    _cameraRotation.y = Mathf.Clamp(_cameraRotation.y - lookSenseV * _playerLocoInput.LookInput.y, -lookLimitV, lookLimitV);

        //    _playerTargetRotation.x += transform.eulerAngles.x + lookSenseH * _playerLocoInput.LookInput.x;
        //    transform.rotation = Quaternion.Euler(0f, _playerTargetRotation.x, 0f);

        //    _playerCamera.transform.rotation = Quaternion.Euler(_cameraRotation.y, _cameraRotation.x, 0f);

        //}


    }
    

}