using UnityEngine;
using System;
namespace FPS.Scripts
{
    public class PlayerLook 
    {
        public  PlayerLook(Transform me, Transform camera)
        {
            _me = me ?? throw new ArgumentNullException(nameof(me));
            _camera = camera ?? throw new ArgumentNullException(nameof(camera));
        }
        public void ChangeLook(Vector2 LookInput)
        {
            (_yaw,_pitch) = CalculatorLook(LookInput, _speed, Time.deltaTime, _maxDt,_yaw,_pitch);
            _me.rotation = Quaternion.Euler(0f, _yaw, 0f);
            _camera.localRotation = Quaternion.Euler(_pitch, 0f, 0f);
        }
        public (float yaw, float pitch) CalculatorLook(Vector2 direction, float speed, float deltaTime,float maxDt,float yaw , float pitch)
        {
       
            yaw += direction.x * speed * deltaTime;
            pitch -= direction.y * speed * deltaTime;
            pitch  = Mathf.Clamp(pitch, -8, maxDt);
            return (yaw, pitch);

        }
        private float _speed = 5f;
        private float _maxDt = 45f; 
        private float _yaw = 0f;
        private float _pitch = 0f;
        private Transform _me;
        private Transform _camera;
    } 
}

