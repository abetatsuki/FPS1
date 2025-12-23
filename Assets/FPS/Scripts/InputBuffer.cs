using UnityEngine;
using UnityEngine.InputSystem;

namespace FPS.Scripts
{
    public class InputBuffer 
    {
        public InputBuffer(PlayerInput playerInput)
        {
            _playerInput = playerInput;

        }
        public InputActionEntity<Vector2> LookAction => _lookActionEntity;
        public InputActionEntity<Vector2> MoveAction => _moveActionEntity;
        public InputActionEntity<float> JumpAction => _jumpActionEntity;
        public InputActionEntity<float> SprintAction => _sprintActionEntity;
        public InputActionEntity<float> CrouchAction => _crouchActionEntity;
        

        private const string _lookActionName = "Look";
        private const string _moveActionName = "Move";
        private const string _jumpActionName = "Jump";
        private const string _sprintActionName = "Sprint";
        private const string _crouchActionName = "Crouch";

        private PlayerInput _playerInput;

        private InputActionEntity<Vector2> _lookActionEntity;
        private InputActionEntity<Vector2> _moveActionEntity;
        private InputActionEntity<float> _jumpActionEntity;
        private InputActionEntity<float> _sprintActionEntity;
        private InputActionEntity<float> _crouchActionEntity;
        
        public void Init()
        {
            if (_playerInput != null)
            {
                _playerInput.notificationBehavior = PlayerNotifications.InvokeCSharpEvents;
                _lookActionEntity = new InputActionEntity<Vector2>(_playerInput.actions[_lookActionName]);
                _moveActionEntity = new InputActionEntity<Vector2>(_playerInput.actions[_moveActionName]);
                _jumpActionEntity = new InputActionEntity<float>(_playerInput.actions[_jumpActionName]);
                _sprintActionEntity = new InputActionEntity<float>(_playerInput.actions[_sprintActionName]);
                _crouchActionEntity = new InputActionEntity<float>(_playerInput.actions[_crouchActionName]);
            }
        }
    }
}

