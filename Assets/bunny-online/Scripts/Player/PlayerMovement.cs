using UnityEngine;

namespace BunnyPlayer
{
    public class PlayerMovement : MonoBehaviour
    {
        [SerializeField] private float movementSpeed;
        private Rigidbody2D _rb2D;
        private Vector2 _moveDirection;
        private PlayerInput _playerInput;
        private Animator _animator;

        private void Awake()
        {
            _playerInput = new PlayerInput();
            _playerInput.Enable();
            _rb2D = GetComponent<Rigidbody2D>();
            _animator = GetComponent<Animator>();
        }

        private void Update()
        {
            ProcessInput();
        }

        private void FixedUpdate()
        {
            Move();
        }

        private void ProcessInput()
        {
            var movement = _playerInput.Player.Move.ReadValue<Vector2>();
            _moveDirection = new Vector2(movement.x, movement.y).normalized;
            
            AnimationChange(_moveDirection);
        }

        private void AnimationChange(Vector2 movement)
        {
            _animator.SetBool("isGoing", movement != Vector2.zero);
            _animator.SetFloat("X", movement.x < 0 ? -1 : movement.x > 0 ? 1 : 0);
            _animator.SetFloat("Y", movement.y < 0 ? -1 : movement.y > 0 ? 1 : 0);
        }

        private void Move()
        {
            _rb2D.linearVelocity = new Vector2(_moveDirection.x * movementSpeed, _moveDirection.y * movementSpeed);
        }
    }
}