using UnityEngine;

namespace FPS.Scripts
{
    public class PlayerMover 
    {
        public PlayerMover(Rigidbody rb,Transform camera)
        {
            _rb = rb;
            _camera = camera;
        }

        public void Move(Vector2 direction,float speed)
        {
            Vector3 delta = Calculate(direction,speed,Time.deltaTime,_camera);
            _rb.linearVelocity = delta;
        }

        public Vector3 Calculate(Vector3 direction, float speed, float deltaTime,Transform camera)
        {
            Vector3 forward = camera.forward;
            Vector3 right = camera.right;
            forward.y = 0;
            right.y = 0;
            forward.Normalize();
            right.Normalize();
            Vector3 moveDir = (forward * direction.y) + (right * direction.x);
            Vector3 _velocity = moveDir * speed;
            return _velocity;
        }

        public void Sprint(float speed)
        {
            
        }
        
        private Rigidbody _rb;
        private Transform _camera;
   
    } 
}


