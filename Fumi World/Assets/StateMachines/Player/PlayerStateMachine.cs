using System.Collections.Generic;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
namespace SkateWorld.FinalCharacterController
{
    [DefaultExecutionOrder(-1)]
    public class PlayerStateMachine : MonoBehaviour
    {

        [Header("Components")]
        [SerializeField] private CharacterController _characterController;
        [SerializeField] private Camera _playerCamera;
        [SerializeField] private Animator _animator;
        [SerializeField] private float locoBlendSpeed = 0.02f;

        PlayerInput _playerInput;
        [SerializeField] CinemachineOrbitalFollow _orbFollow;

        //public CinemachineCamera vcam;
        //public CinemachineOrbitalFollow oFolow;
        //public InputAxis _iAxis;




        private static int inputXHash = Animator.StringToHash("inputX");
        private static int inputYHash = Animator.StringToHash("inputY");
        private Vector3 _currentBlendInput = Vector3.zero;
        private Vector2 LookInput;

        Vector2 _currentMovementInput;
        Vector3 _currentMovement;
        Vector3 _appliedMovement;
        bool _isMovementPressed;


        [Header("Base Movement")]
        public float _runAcceleration = 0.25f;
        public float _runSpeed = 4f;
        public float _drag = 0.1f;
        public float _rotationSpeed = 480f;

        // gravity variables 
        float _gravity = -9.8f;
        //float _groundedGravity = -.05f;
        float _rotationFactorPerFrame = 15.0f;
        int _zero = 0;

        // jumping variables
        bool _isJumpPressed = false;
        float _initialJumpVelocity;
        [SerializeField] float _maxJumpHeight = 4.0f;
        [SerializeField] float _maxJumpTime = .75f;
        bool _isJumping = false;
        int _isJumpingHash;
        int _jumpCountHash;
        bool _requireNewJumpPress = false;
        int _jumpCount = 0;
        Dictionary<int, float> _initialJumpVelocities = new Dictionary<int, float>();
        Dictionary<int, float> _jumpGravities = new Dictionary<int, float>();
        Coroutine _currentJumpResetRoutine = null;


        int _isWalkingHash;
       
        int _isFallingHash;


        // state logic
        PlayerBaseState _currentState;
        PlayerStateFactory _states;

        // getters and setters
        public PlayerBaseState CurrentState { get { return _currentState; } set { _currentState = value; } }
        public Animator Animator { get { return _animator; } }
        public CharacterController CharacterController { get { return _characterController; } }
        public Coroutine CurrentJumpResetRoutine { get { return _currentJumpResetRoutine; } set { _currentJumpResetRoutine = value; } }
        public Dictionary<int, float> InitialJumpVelocities { get { return _initialJumpVelocities; } }
        public Dictionary<int, float> JumpGravities { get { return _jumpGravities; } }
        public int JumpCount { get { return _jumpCount; } set { _jumpCount = value; } }
        public int IsWalkingHash { get { return _isWalkingHash; } }
       
        public int IsFallingHash { get { return _isFallingHash; } }
        public int IsJumpingHash { get { return _isJumpingHash; } }
        public int JumpCountHash { get { return _jumpCountHash; } }
        public bool IsMovementPressed { get { return _isMovementPressed; } }
       
        public bool RequireNewJumpPress { get { return _requireNewJumpPress; } set { _requireNewJumpPress = value; } }
        public bool IsJumping { set { _isJumping = value; } }
        public bool IsJumpPressed { get { return _isJumpPressed; } }
        public float Gravity { get { return _gravity; } }
        public float CurrentMovementY { get { return _currentMovement.y; } set { _currentMovement.y = value; } }
        public float AppliedMovementY { get { return _appliedMovement.y; } set { _appliedMovement.y = value; } }
        public float AppliedMovementX { get { return _appliedMovement.x; } set { _appliedMovement.x = value; } }
        public float AppliedMovementZ { get { return _appliedMovement.z; } set { _appliedMovement.z = value; } }
        
        public Vector2 CurrentMovementInput { get { return _currentMovementInput; } }






        public Vector3 CurrentBlendInput {  get { return _currentBlendInput; } set { _currentBlendInput = value; } }

        public float LocoBlendSpeed { get { return locoBlendSpeed; } }
        public Camera PlayerCamera { get { return _playerCamera; } }

        public CinemachineOrbitalFollow OrbFollow { get { return _orbFollow; } }

        //public CinemachineCamera VirCam { get { return vcam; } }
        //public CinemachineOrbitalFollow OrbFollow { get { return oFolow; } }


        public float RunAcceleration { get { return _runAcceleration; } }
        public float RunSpeed { get { return _runSpeed; } }
        public float Drag { get { return _drag; } }
        public float RotationSpeed { get { return _rotationSpeed; } }

        public int InputXHash { get { return inputXHash; } }
        public int InputYHash { get { return inputYHash; } }


        void Awake()
        {
            _playerInput = new PlayerInput();
            _characterController = GetComponent<CharacterController>();
            _animator = GetComponent<Animator>();
            
            //Debug.Log(_orbFollow);

            //CinemachineOrbitalFollow orbFollow = vcam.GetCinemachineComponent<CinemachineOrbitalFollow>();
            //Cinemachivcam.GetCinemachineComponent
            //oFolow = GetComponent<CinemachineOrbitalFollow>();


            _states = new PlayerStateFactory(this);
            _currentState = _states.Grounded();
            _currentState.EnterState();

            // set the parameter hash references
            _isWalkingHash = Animator.StringToHash("isWalking");
            //_isRunningHash = Animator.StringToHash("isRunning");
            _isFallingHash = Animator.StringToHash("isFalling");
            _isJumpingHash = Animator.StringToHash("isJumping");
            _jumpCountHash = Animator.StringToHash("jumpCount");

            _playerInput.CharacterControls.Movement.started += OnMovementInput;
            _playerInput.CharacterControls.Movement.canceled += OnMovementInput;
            _playerInput.CharacterControls.Movement.performed += OnMovementInput;
            
            _playerInput.CharacterControls.Jump.started += OnJump;
            _playerInput.CharacterControls.Jump.canceled += OnJump;



            SetupJumpVariables();
        }

        //Any other functions needed to setup variables

        void SetupJumpVariables()
        {
            float timeToApex = _maxJumpTime / 2;
            float initialGravity = (-2 * _maxJumpHeight) / Mathf.Pow(timeToApex, 2);
            _initialJumpVelocity = (2 * (_maxJumpHeight)) / timeToApex;
            float secondJumpGravity = (-2 * (_maxJumpHeight + 2)) / Mathf.Pow((timeToApex * 1.25f), 2);
            float secondJumpInitialVelocity = (2 * (_maxJumpHeight + 2)) / (timeToApex * 1.25f);
            float thirdJumpGravity = (-2 * (_maxJumpHeight + 4)) / Mathf.Pow((timeToApex * 1.5f), 2);
            float thirdJumpInitialVelocity = (2 * (_maxJumpHeight + 4)) / (timeToApex * 1.5f);

            _initialJumpVelocities.Add(1, _initialJumpVelocity);
            _initialJumpVelocities.Add(2, secondJumpInitialVelocity);
            _initialJumpVelocities.Add(3, thirdJumpInitialVelocity);

            _jumpGravities.Add(0, initialGravity);
            _jumpGravities.Add(1, initialGravity);
            _jumpGravities.Add(2, secondJumpGravity);
            _jumpGravities.Add(3, thirdJumpGravity);
        }


        
        void Start()
        {
            _characterController.Move(_appliedMovement * Time.deltaTime);
        }
        
        void Update()
        {



            //if ( _currentState.ToString() != "PlayerGroundedState" && !printedGrounded)
            //{
            //    printedGrounded = true;
            //    Debug.Log(_currentState);
            //} else
            //{
            //    Debug.Log(_currentState);
            //}

            //HandleRotation();
            
            _currentState.UpdateStates();
            _characterController.Move(_appliedMovement * Time.deltaTime);


        }

        //Vector3 movement()
        //{

        //    Vector2 inputTarget = _currentMovementInput;
        //    _currentBlendInput = Vector3.Lerp(_currentBlendInput, inputTarget, locoBlendSpeed * Time.deltaTime);

        //    _animator.SetFloat(inputXHash, _currentBlendInput.x);
        //    _animator.SetFloat(inputYHash, _currentBlendInput.y);

        //    Vector3 cameraForwardXZ = new Vector3(_playerCamera.transform.forward.x, 0f, _playerCamera.transform.forward.z).normalized;
        //    Vector3 cameraRightXZ = new Vector3(_playerCamera.transform.right.x, 0f, _playerCamera.transform.right.z).normalized;
        //    Vector3 movementDirection = cameraRightXZ * _currentMovementInput.x + cameraForwardXZ * _currentMovementInput.y;


        //    Vector3 movementDelta = movementDirection * _runAcceleration * Time.deltaTime;
        //    Vector3 newVelocity = _characterController.velocity + movementDelta;

        //    Vector3 currentDrag = newVelocity.normalized * _drag * Time.deltaTime;
        //    newVelocity = (newVelocity.magnitude > _drag * Time.deltaTime) ? newVelocity - currentDrag : Vector3.zero;
        //    newVelocity = Vector3.ClampMagnitude(newVelocity, _runSpeed);

            
        //    //_characterController.Move(newVelocity * Time.deltaTime);

            

        //    return newVelocity;
        //}

        void HandleRotation()
        {
            Vector3 positionToLookAt;
            // the change in position our character should point to
            positionToLookAt.x = _currentMovementInput.x;
            positionToLookAt.y = _zero;
            positionToLookAt.z = _currentMovementInput.y;
            // the current rotation of our character
            Quaternion currentRotation = transform.rotation;

            if (_isMovementPressed)
            {
                // creates a new rotation based on where the player is currently pressing
                Quaternion targetRotation = Quaternion.LookRotation(positionToLookAt);
                // rotate the character to face the positionToLookAt            
                transform.rotation = Quaternion.Slerp(currentRotation, targetRotation, _rotationFactorPerFrame * Time.deltaTime);
            }
        }


        void OnEnable()
        {
            
            
            _playerInput.CharacterControls.Enable();
            

            //PlayerControls = new PlayerInput();
            //PlayerControls.Enable();

            //PlayerControls.PlayerLocomap.Enable();
            //PlayerControls.PlayerLocomap.SetCallbacks(this);

            //_playerLocoInput.PlayerLocomap.Enable();
        }

        void OnDisable()
        {

            _playerInput.CharacterControls.Disable();
            //PlayerControls.PlayerLocomap.Disable();
            //PlayerControls.PlayerLocomap.RemoveCallbacks(this);
        }


        void OnMovementInput(InputAction.CallbackContext context)
        {
            //Debug.Log("receiving walk input");
            _currentMovementInput = context.ReadValue<Vector2>();
            _isMovementPressed = _currentMovementInput.x != _zero || _currentMovementInput.y != _zero;
            //print(_currentMovementInput);
        }

        void OnLook(InputAction.CallbackContext context)
        {
            LookInput = context.ReadValue<Vector2>();
        }

        void OnJump(InputAction.CallbackContext context)
        {
            //Debug.Log("receiving jump input");
            _isJumpPressed = context.ReadValueAsButton();
            _requireNewJumpPress = false;
        }
    }

}
