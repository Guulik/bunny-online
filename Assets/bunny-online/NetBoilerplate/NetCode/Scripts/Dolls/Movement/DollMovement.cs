using FishNet.Object;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Dolls.Movement
{
    public class DollMovement : NetworkBehaviour
    {
        [SerializeField] [Range(0, float.MaxValue)]
        private float moveSpeed = 1f;
        
        private Transform _cameraTransform;
        private float _currentVerticalAngle;

        private PlayerInput _input;
        private bool _isGrounded;

        private MovementData _lastMovementData;

        private Rigidbody _rigidbody;
        
        private Animator _animator;


        private void Awake()
        {
            _input = new PlayerInput();
            _rigidbody = GetComponentInChildren<Rigidbody>();
            _cameraTransform = GetComponentInChildren<Camera>().transform;
            _animator = GetComponentInChildren<Animator>();
        }

        private void FixedUpdate()
        {
            Move(_lastMovementData.MoveDirection);
        }
        

        public override void OnStartClient()
        {
            _input.Enable();
            //_input.Player.Jump.performed += OnJump;
        }

        public override void OnStopClient()
        {
            _input.Disable();
            //_input.Player.Jump.performed -= OnJump;
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            if (!IsOwner) return;
            var moveInput = context.ReadValue<Vector2>();
            _lastMovementData.MoveDirection = moveInput;
        }
        
        private void Move(Vector2 direction)
        {
            Vector2 normalizedSpeed = direction.normalized;
            _rigidbody.linearVelocity = new Vector2(normalizedSpeed.x * moveSpeed, normalizedSpeed.y * moveSpeed);

            AnimationChange(direction);
            /*
            var velocity = new Vector3(normalizedSpeed.x * moveSpeed, _rigidbody.linearVelocity.y,
                normalizedSpeed.y * moveSpeed);

            velocity = transform.TransformDirection(velocity);

            _rigidbody.linearVelocity = velocity;*/
        }

        private void AnimationChange(Vector2 movement)
        {
            _animator.SetBool("isGoing", movement != Vector2.zero);
            _animator.SetFloat("X", movement.x < 0 ? -1 : movement.x > 0 ? 1 : 0);
            _animator.SetFloat("Y", movement.y < 0 ? -1 : movement.y > 0 ? 1 : 0);
        }

        public void SetGrounded(bool state)
        {
            _isGrounded = state;
        }

        private struct MovementData
        {
            public Vector2 MoveDirection, RotateDirection;
        }
    }
}