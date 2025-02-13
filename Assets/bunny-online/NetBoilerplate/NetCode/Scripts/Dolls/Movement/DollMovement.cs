using FishNet.Object;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Dolls.Movement
{
    public class DollMovement : NetworkBehaviour
    {
        [SerializeField] [Range(0, float.MaxValue)]
        private float moveSpeed = 1f;

        [SerializeField] [Range(0, float.MaxValue)]
        private float sensitivity = 1f;

        [SerializeField] [Range(0, float.MaxValue)]
        private float jumpStrength = 1f;

        [SerializeField] [Range(0f, 90f)] private float cameraRotationLimit;
        private Transform _cameraTransform;
        private float _currentVerticalAngle;

        private PlayerInput _input;
        private bool _isGrounded;

        private MovementData _lastMovementData;

        private Rigidbody _rigidbody;


        private void Awake()
        {
            _input = new PlayerInput();
            _rigidbody = GetComponentInChildren<Rigidbody>();
            _cameraTransform = GetComponentInChildren<Camera>().transform;
        }

        private void FixedUpdate()
        {
            Move(_lastMovementData.MoveDirection);
        }

        private void LateUpdate()
        {
            HorizontalRotate(_lastMovementData.RotateDirection);
            VerticalRotate(_lastMovementData.RotateDirection);
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


        public void OnRotate(InputAction.CallbackContext context)
        {
            if (!IsOwner) return;
            var rotateInput = context.ReadValue<Vector2>();
            _lastMovementData.RotateDirection = rotateInput;
        }

        private void Move(Vector2 direction)
        {
            Vector2 normalizedSpeed = direction.normalized;

            var velocity = new Vector3(normalizedSpeed.x * moveSpeed, _rigidbody.linearVelocity.y,
                normalizedSpeed.y * moveSpeed);

            velocity = transform.TransformDirection(velocity);

            _rigidbody.linearVelocity = velocity;
        }

        private void HorizontalRotate(Vector2 direction)
        {
            transform.Rotate(Vector3.up * (direction.x * sensitivity));
        }

        private void VerticalRotate(Vector2 direction)
        {
            _currentVerticalAngle -= direction.y * sensitivity;
            _currentVerticalAngle = Mathf.Clamp(_currentVerticalAngle, -cameraRotationLimit, cameraRotationLimit);

            _cameraTransform.localRotation = Quaternion.Euler(_currentVerticalAngle, 0f, 0f);
        }
        
        private void Jump()
        {
            var jumpForce = Vector3.zero;

            if (_isGrounded) jumpForce = Vector3.up * jumpStrength;
            _rigidbody.AddForce(jumpForce, ForceMode.Impulse);
        }

        private void OnJump(InputAction.CallbackContext context)
        {
            if (!IsOwner) return;
            Jump();
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