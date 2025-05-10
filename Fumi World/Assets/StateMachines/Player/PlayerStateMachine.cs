using UnityEngine;




namespace SkateWorld.FinalCharacterController
{
    [DefaultExecutionOrder(-1)]
    public class PlayerStateMachine : MonoBehaviour
    {

        [Header("Components")]
        [SerializeField] private CharacterController _characterController;
        [SerializeField] private Camera _playerCamera;

        [Header("Base Movement")]
        public float runAcceleration = 0.25f;
        public float runSpeed = 4f;
        public float drag = 0.1f;
        public float rotationSpeed = 720f;

        private PlayerLocoInput _playerLocoInput;
        

        // state logic
        PlayerBaseState _currentState;
        PlayerStateFactory _states;

        // getters and setters
        public PlayerBaseState CurrentState { get { return _currentState; } set { _currentState = value; } }
        //public bool IsJumpPressed {  get { return _isJumpPressed;  } }

        private void Awake()
        {   
            _playerLocoInput = GetComponent<PlayerLocoInput>();

         
            _states = new PlayerStateFactory(this);
            _currentState = _states.Grounded();
            _currentState.EnterState();
        }

        //Any other functions needed to setup variables

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
    }

}
