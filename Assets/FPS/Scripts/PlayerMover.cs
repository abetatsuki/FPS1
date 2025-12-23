using UnityEngine;
using System;
namespace FPS.Scripts
{
    public class PlayerMover
    {
        public PlayerMover(Rigidbody rb, Transform camera, float jumpForce, Transform tf, float playerHeight
            , float maxSlopeAngle)
        {
            _rb = rb ?? throw new ArgumentNullException(nameof(rb));
            _camera = camera ?? throw new ArgumentNullException(nameof(camera));
            _tf = tf ?? throw new ArgumentNullException(nameof(tf));

            if (jumpForce <= 0f)
            {
                throw new ArgumentOutOfRangeException(nameof(jumpForce));
            }

            if (playerHeight <= 0f)
            {
                throw new ArgumentOutOfRangeException(nameof(playerHeight));
            }

            if (maxSlopeAngle <= 0f || maxSlopeAngle > 90f)
            {
                throw new ArgumentOutOfRangeException(nameof(maxSlopeAngle));
            }
        
            _jumpForce = jumpForce;
            _playerHeight = playerHeight;
            _maxSlopeAngle = maxSlopeAngle;
        }

        public void Move(Vector2 input, float speed,bool exitingSlope)
        {
            Vector3 direction = Calculate(input, _camera);

            if (OnSlope()&&!exitingSlope)
            {
                direction = GetSlopeMoveDirection(direction);
                Vector3 SlopeVelocity = direction * speed;
                _rb.linearVelocity = SlopeVelocity;
            }
            else
            {
                Vector3 velocity = direction * speed;

                _rb.linearVelocity = new Vector3(
                    velocity.x,
                    _rb.linearVelocity.y,
                    velocity.z);
            }
        }

        public Vector3 Calculate(Vector3 input, Transform camera)
        {
            Vector3 forward = camera.forward;
            Vector3 right = camera.right;
            forward.y = 0;
            right.y = 0;
            forward.Normalize();
            right.Normalize();
            Vector3 direction = (forward * input.y) + (right * input.x).normalized;

            return direction;
        }

        public void Jump()
        {
            _rb.linearVelocity = new Vector3(_rb.linearVelocity.x, 0f, _rb.linearVelocity.z);
            _rb.AddForce(_tf.up * _jumpForce, ForceMode.Impulse);
        }

        public void ResetJump()
        {
        }

        public bool OnSlope()
        {
            if (Physics.Raycast(_tf.position, Vector3.down, out _slopeHit, _playerHeight * 0.5f + 0.3f))
            {
                float angle = Vector3.Angle(Vector3.up, _slopeHit.normal);
                return angle < _maxSlopeAngle && angle != 0;
            }

            return false;
        }

        public Vector3 GetSlopeMoveDirection(Vector3 direction)
        {
            Vector3 slopeDirection = Vector3.ProjectOnPlane(direction, _slopeHit.normal).normalized;
            return slopeDirection;
        }


        public void Sprint(float speed)
        {
        }

        private Rigidbody _rb;
        private Transform _camera;
        private readonly float _jumpForce;
        private readonly Transform _tf;
        private readonly float _playerHeight;
        private RaycastHit _slopeHit;
        private readonly float _maxSlopeAngle;
    }
}