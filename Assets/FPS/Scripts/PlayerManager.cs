using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace FPS.Scripts
{
    [RequireComponent(typeof(PlayerInput))]
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerManager : MonoBehaviour
    {
        public MoveStatement State;

        public enum MoveStatement
        {
            walking,
            sprinting,
            crouching,
            air
        }

        [Header("Commponents")] private InputBuffer _inputBuffer;
        private PlayerInput _playerInput;
        private Rigidbody _rb;
        private Transform _transform;

        [Header("PureClass")] private PlayerMover _playerMover;
        private PlayerLook _playerLook;
        private Vector3 _moveInput;
        private PlayerSliding _playerSliding;

        [Header("MoveSpeed")] [SerializeField, Tooltip("CurrentSpeed")]
        private float _speed = 3f;

        [SerializeField, Tooltip("WalkSpeed")] private float _walkSpeed = 5f;

        [SerializeField, Tooltip("SprintSpeed")]
        private float _sprintSpeed = 7f;

        [Header("Jump")] [SerializeField, Tooltip("JumpForce")]
        private float _jumpForce = 5f;

        [Header("Crouch")] [SerializeField, Tooltip("CrouchSpeed")]
        private float _crouchSpeed = 1f;

        [SerializeField, Tooltip("CrouchYScale")]
        private float _crouchYScale = 0.5f;

        [Header("Slide")] [SerializeField, Tooltip("SlideForce")]
        private float _slideForce;

        [SerializeField, Tooltip("SlideYScale")]
        private float _slideYScale;
        [SerializeField,Tooltip("SlideTimer")]
        private float _slideTimer;


        [Header("Slope Handing")] [SerializeField, Tooltip("Slope max Angle")]
        private float _maxSlopeAngle;

        private bool _exitingSlope;


        [Header("GroundCheck")] [SerializeField, Tooltip("PlayerHeight")]
        private float _playerHeight;

        private bool _isGrounded;
        private RaycastHit _groundCheck;

        [SerializeField, Tooltip("GroundLayer")]
        private LayerMask _groundLayer;


        private float _startYScale;

        [SerializeField, Tooltip("CameraTransform")]
        private Transform _camera;

        private void Awake()
        {
            Init();
        }

        private void Start()
        {
            _startYScale = transform.localScale.y;
        }

        private void Update()
        {
           
        }

        private void FixedUpdate()
        {
            _playerMover.Move(_moveInput, _speed, _exitingSlope);
            _playerSliding.FixedUpdate(_moveInput);
            _isGrounded = GroundCheck();
        }

       

        private void Init()
        {
            InitComponents();
            InitPlayerInput();
            InitPureClass();
            _inputBuffer.Init();
            Event();
        }

        private void InitComponents()
        {
            _playerInput = GetRequiredComponent<PlayerInput>();
            _rb = GetRequiredComponent<Rigidbody>();
            _transform = GetRequiredComponent<Transform>();
        }

        private void InitPlayerInput()
        {
            _playerInput.defaultActionMap = "Player";
            _playerInput.notificationBehavior = PlayerNotifications.InvokeCSharpEvents;
        }

        private void InitPureClass()
        {
            _inputBuffer = new InputBuffer(_playerInput);
            _playerMover = new PlayerMover(_rb, _camera, _jumpForce, _transform, _playerHeight,
                _maxSlopeAngle);
            _playerSliding = new PlayerSliding(_rb, _playerMover, _transform, _slideForce, _slideYScale,_slideTimer,_camera);
            _playerLook = new PlayerLook(_transform, _camera);
        }

        private T GetRequiredComponent<T>() where T : Component
        {
            T component = GetComponent<T>();

            if (component == null)
            {
                Debug.LogError($"{gameObject.name} に {typeof(T).Name} が存在しません");
            }

            return component;
        }

        private void Event()
        {
            if (_inputBuffer == null)
            {
                Debug.Log("Input buffer is null");
                return;
            }

            _inputBuffer.MoveAction.Performed += InputVector2;
            _inputBuffer.MoveAction.Canceled += InputVector2;
            _inputBuffer.LookAction.Performed += Look;
            _inputBuffer.LookAction.Canceled += Look;
            _inputBuffer.SprintAction.Performed += Sprint;
            _inputBuffer.SprintAction.Canceled += Sprint;
            _inputBuffer.CrouchAction.Performed += Crouch;
            _inputBuffer.SlideAction.Performed += Slide;
            _inputBuffer.SlideAction.Canceled += Slide;
            _inputBuffer.CrouchAction.Canceled += CrouchCansel;
            _inputBuffer.JumpAction.Performed += Jump;
        }

        private bool GroundCheck()
        {
            Vector3 origin = transform.position + Vector3.up * 0.1f;
            float rayLength = _playerHeight * 0.5f + 0.3f;
            if (Physics.Raycast(origin, Vector3.down, out _groundCheck, rayLength, _groundLayer))
            {
                return true;
            }

            return false;
        }

        private void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.CompareTag("Ground"))
            {
                _exitingSlope = false;
                Debug.Log("OnGround");
            }

            Debug.Log("Air");
        }

        private void Jump(float obj)
        {
            if (_isGrounded)
            {
                _exitingSlope = true;
                _playerMover.Jump();
            }
        }

        private void Slide(float input)
        {
            if (input > 0f)
            {
                _playerSliding.StartSlide();
            }
            else
            {
                _playerSliding.StopSlide();
            }
        }

        private void Crouch(float obj)
        {
            Debug.Log("Crouch");
            State = MoveStatement.crouching;
            _speed = _crouchSpeed;
            transform.localScale = new Vector3(transform.localScale.x, _crouchYScale, transform.localScale.z);
            // _rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
        }

        private void CrouchCansel(float obj)
        {
            transform.localScale = new Vector3(transform.localScale.x, _startYScale, transform.localScale.z);
        }

        private void Look(Vector2 lookInput)
        {
            _playerLook.ChangeLook(lookInput);
        }

        private void Sprint(float speed)
        {
            if (_isGrounded && speed > 0)
            {
                State = MoveStatement.sprinting;
                _speed = _sprintSpeed;
            }
            else if (_isGrounded && speed == 0)
            {
                State = MoveStatement.walking;
                _speed = _walkSpeed;
            }
        }

        private void InputVector2(Vector2 input)
        {
            _moveInput = input;
            if (_isGrounded)
            {
                State = MoveStatement.walking;
                _speed = _walkSpeed;
            }
        }

        private void IsGrounded()
        {
            State = MoveStatement.air;
        }
    }
}