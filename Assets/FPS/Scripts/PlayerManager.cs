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

        [Header("Commponents")]
        private InputBuffer _inputBuffer;
        private PlayerInput _playerInput;
        private Rigidbody _rb;
        private Transform _transform;
        
        [Header("PureClass")]
        private PlayerMover _playerMover;
        private PlayerLook _playerLook;
        private Vector3 _moveInput;

        [Header("MoveSpeed")]
        [SerializeField, Tooltip("CurrentSpeed")]
        private float _speed = 3f;
        [SerializeField, Tooltip("WalkSpeed")]
        private float _walkSpeed = 5f;
        [SerializeField, Tooltip("SprintSpeed")]
        private float _sprintSpeed = 7f;
        
        [Header("GroundCheck")]
        private bool _isGrounded = true;

        [Header("Slope Handing")] [SerializeField, Tooltip("Slope max Angle")]
        private float _maxslopeAngle;
        private RaycastHit _slopeHit;
        
        [Header("Crouch")] 
        [SerializeField, Tooltip("CrouchSpeed")]
        private float _crouchSpeed = 1f;
        [SerializeField, Tooltip("CrouchYScale")]
        private float _crouchYScale = 0.5f;
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
            _playerMover.Move(_moveInput, _speed);
            Debug.Log(_speed);
        }

        private void Init()
        {
            _playerInput = GetComponent<PlayerInput>();
            _rb = GetComponent<Rigidbody>();
            _transform = GetComponent<Transform>();
            _playerInput.defaultActionMap = "Player";
            _playerInput.notificationBehavior = PlayerNotifications.InvokeCSharpEvents;
            _inputBuffer = new InputBuffer(_playerInput);
            _playerMover = new PlayerMover(_rb, _camera);
            _playerLook = new PlayerLook(_transform, _camera);
            _inputBuffer.Init();
            Event();
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
            _inputBuffer.CrouchAction.Canceled += CrouchCansel;
        }

        private void Crouch(float obj)
        {
            State = MoveStatement.crouching;
            _speed  = _crouchSpeed;
            transform.localScale = new Vector3(transform.localScale.x,_crouchYScale, transform.localScale.z);
           // _rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
        }

        private void CrouchCansel(float obj)
        {
            transform.localScale = new Vector3(transform.localScale.x,_startYScale, transform.localScale.z);
        }

        private void Look(Vector2 lookInput)
        {
            _playerLook.ChangeLook(lookInput);
        }

        private void Sprint(float speed)
        {
            if (_isGrounded)
            {
                State = MoveStatement.sprinting;
                _speed = _sprintSpeed;
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