using FPS.Scripts;
using UnityEngine;
using System;

public class PlayerSliding
{
    public PlayerSliding(Rigidbody rb, PlayerMover playerMover, Transform playerTransform, float slideForce,
        float slideYScale, float slideTimer,Transform camera)
    {
        _rb = rb ?? throw new ArgumentNullException(nameof(rb));
        _playerMover = playerMover ?? throw new ArgumentNullException(nameof(playerMover));
        _playerTransform = playerTransform ?? throw new ArgumentNullException(nameof(playerTransform));
        _camera = camera ?? throw new ArgumentNullException(nameof(camera));
        if (slideForce <= 0f)
        {
            throw new ArgumentOutOfRangeException(nameof(slideForce));
        }

        if (slideYScale <= 0f)
        {
            throw new ArgumentOutOfRangeException(nameof(slideYScale));
        }

        if (slideTimer <= 0f)
        {
            throw new ArgumentOutOfRangeException(nameof(slideTimer));
        }

        _maxSlideTime = slideTimer;
        _slideForce = slideForce;
        _slideYScale = slideYScale;
        _startYScale = _playerTransform.localScale.y;
    }

    public bool IsSliding => _isSliding;

    public void FixedUpdate(Vector3 moveInput)
    {
        if (!_isSliding) return;
        SlideMovement(moveInput);
    }

    public void SlideMovement(Vector2 direction)
    {
        Vector3 inputDirection = _camera.forward * direction.y + _camera.right * direction.x;
         if (inputDirection.sqrMagnitude < 0.0001f)
        {
            return;
        }

        Vector3 currentVelocity = _rb.linearVelocity;
        Vector3 slideVelocity = inputDirection.normalized * _slideForce;
        _rb.linearVelocity = new Vector3(
            slideVelocity.x,
            currentVelocity.y,
            slideVelocity.z);
        _slideTimer -= Time.fixedDeltaTime;
        if (_slideTimer <= 0)
            StopSlide();
    }

    public void StartSlide()
    {
        if (_isSliding) return;
        _isSliding = true;
        _playerTransform.localScale =
            new Vector3(_playerTransform.localScale.x, _slideYScale, _playerTransform.localScale.z);
        Vector3 velocity = _playerTransform.forward * _slideForce;
        velocity.y = _rb.linearVelocity.y;
        _rb.linearVelocity = velocity;
        _slideTimer = _maxSlideTime;
    }


    public void StopSlide()
    {
        if (!_isSliding) return;
        _isSliding = false;
        _playerTransform.localScale =
            new Vector3(_playerTransform.localScale.x, _startYScale, _playerTransform.localScale.z);
    }

    [Header("Referrences")] 
    private Transform _playerTransform;

    private Rigidbody _rb;
    private PlayerMover _playerMover;
    private Transform _camera;

    [Header("Sliding")] private float _maxSlideTime;
    private bool _isSliding;


    private float _slideForce;

    private float _slideTimer;

    private float _slideYScale;

    private float _startYScale;
}