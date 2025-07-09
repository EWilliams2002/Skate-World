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
        [SerializeField] CinemachineOrbitalFollow _orbFollow;

        PlayerInput _playerInput;
        


        private static int inputXHash = Animator.StringToHash("inputX");
        private static int inputYHash = Animator.StringToHash("inputY");
        private Vector3 _currentBlendInput = Vector3.zero;
        private Vector2 LookInput;

        Vector2 _currentMovementInput;
        Vector3 _currentMovement;
        Vector3 _appliedMovement;
        bool _isMovementPressed;


        [Header("Movement")]
        public float _runAcceleration = 0.25f;
        public float _runSpeed = 4f;
        public float _drag = 0.1f;
        public float _rotationSpeed = 480f;

        // gravity variables 
        float _gravity = -9.8f;
        //float _groundedGravity = -.05f;
        //float _rotationFactorPerFrame = 15.0f;
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
        Dictionary<int, float> _initialJumpVelocities = new Dictionary<int, float>();
        Dictionary<int, float> _jumpGravities = new Dictionary<int, float>();

        // mount
        bool _isMountedPressed = false;
        bool _requireNewMountPress = false;

        // animation hashes
        int _isWalkingHash;
        int _isFallingHash;

        // state logic
        PlayerBaseState _currentState;
        PlayerStateFactory _states;


        // getters and setters

        // Types
        public PlayerBaseState CurrentState { get { return _currentState; } set { _currentState = value; } }
        public Animator Animator { get { return _animator; } }
        public CharacterController CharacterController { get { return _characterController; } }
        public Dictionary<int, float> InitialJumpVelocities { get { return _initialJumpVelocities; } }
        public Dictionary<int, float> JumpGravities { get { return _jumpGravities; } }
        public Camera PlayerCamera { get { return _playerCamera; } }
        public CinemachineOrbitalFollow OrbFollow { get { return _orbFollow; } }



        // hashes
        public int IsWalkingHash { get { return _isWalkingHash; } }
        public int IsFallingHash { get { return _isFallingHash; } }
        public int IsJumpingHash { get { return _isJumpingHash; } }
        public int JumpCountHash { get { return _jumpCountHash; } }



        // input
        public bool IsMovementPressed { get { return _isMovementPressed; } }
        public bool IsMountPressed { get { return _isMountedPressed; } set { _isMountedPressed = value; } }
        public bool IsJumpPressed { get { return _isJumpPressed; } }
        public bool RequireNewJumpPress { get { return _requireNewJumpPress; } set { _requireNewJumpPress = value; } }
        public bool RequireNewMountPress { get { return _requireNewMountPress; } set { _requireNewMountPress = value; } }
        public Vector2 CurrentMovementInput { get { return _currentMovementInput; } }
        public Vector3 CurrentBlendInput { get { return _currentBlendInput; } set { _currentBlendInput = value; } }
        public int InputXHash { get { return inputXHash; } }
        public int InputYHash { get { return inputYHash; } }



        // movement
        public float Gravity { get { return _gravity; } }
        public bool IsJumping { set { _isJumping = value; } }
        public float CurrentMovementY { get { return _currentMovement.y; } set { _currentMovement.y = value; } }
        public float AppliedMovementY { get { return _appliedMovement.y; } set { _appliedMovement.y = value; } }
        public float AppliedMovementX { get { return _appliedMovement.x; } set { _appliedMovement.x = value; } }
        public float AppliedMovementZ { get { return _appliedMovement.z; } set { _appliedMovement.z = value; } }
        


        // other
        public float RunAcceleration { get { return _runAcceleration; } }
        public float RunSpeed { get { return _runSpeed; } }
        public float Drag { get { return _drag; } }
        public float RotationSpeed { get { return _rotationSpeed; } }
        public float LocoBlendSpeed { get { return locoBlendSpeed; } }


        void Awake()
        {
            _playerInput = new PlayerInput();
            _characterController = GetComponent<CharacterController>();
            _animator = GetComponent<Animator>();


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

            _playerInput.CharacterControls.Mount.started += OnMount;
            
            SetupJumpVariables();

        }

        //Any other functions needed to setup variables go here

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
            
            _currentState.UpdateStates();
            _characterController.Move(_appliedMovement * Time.deltaTime);

            
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

        void OnMount(InputAction.CallbackContext context)
        {
            
            _isMountedPressed = context.ReadValueAsButton();
            _requireNewMountPress = false;
            Debug.Log(_isMountedPressed);

        }

        void OnJump(InputAction.CallbackContext context)
        {

            //Debug.Log("receiving jump input");
            _isJumpPressed = context.ReadValueAsButton();
            _requireNewJumpPress = false;

        }

        void OnEnable()
        {

            _playerInput.CharacterControls.Enable();

        }

        void OnDisable()
        {

            _playerInput.CharacterControls.Disable();

        }
    }
}