using System.Collections;
using Items;
using BunnyPlayer;
using Dolls.Health;
using FishNet.Object;
using UnityEngine;
using UnityEngine.InputSystem;

namespace NPC
{
    public class Cow : NetworkBehaviour, IInteractable
    {
        [SerializeField] private Item filledMilk;
        [SerializeField] private Item food;
        private bool isFed = false;
        
        [SerializeField]protected bool Movable = false;
        private bool isAllowedToMove = false;
        [SerializeField] private float moveSpeed;
        [SerializeField] private float movingTime = 1f;
        [SerializeField] private float stopTime = 1f;
        [Space(20)] 
        
        private Doll _nearbyDoll;
        private PlayerInventory _playerInventory; 
        private PlayerInput _playerInput;
       [SerializeField] protected float interactRange = 1f;
       private bool _canInteract = false;

       private Vector2 circleRadius = Vector2.zero;
       private Coroutine _coroutine;
       private Rigidbody2D _rb2d;
       
       [SerializeField] protected InteractUI _interactionMarker;
       private Animator _animator;
       private SpriteRenderer _sprite;
       
       private void Awake()
       {
           _rb2d = GetComponent<Rigidbody2D>();
           _rb2d.gravityScale = 0f;
           //_interactionMarker = GetComponentInChildren<InteractUI>();
           _animator = GetComponent<Animator>();
           _sprite = GetComponent<SpriteRenderer>();
           _playerInput = new PlayerInput();
           _playerInput.Enable();
       }
        private void OnEnable()
        {
            _playerInput.Player.Interact.started += Interact;
        }

        private void OnDisable()
        {
            _playerInput.Player.Interact.started -= Interact;
        }

        public void Interact(InputAction.CallbackContext context)
        {
            UpdatePlayerComponents();
            
            if (!_canInteract) return;

            Debug.Log("trying to interact");
            if (CheckFood())
            {
                ChangeMilk();
                //ShowChatBubble("Вот твое млеко");
            }
        }
        
        private void Update()
        {
            _nearbyDoll = GetNearbyPlayer();
            _canInteract = _nearbyDoll != null;

            if (_canInteract)
            {
                _interactionMarker.Show();
                isAllowedToMove = false;
            }
            else
            {
                _interactionMarker.Hide();
                isAllowedToMove = Movable;
            }

            if (isAllowedToMove)
            {
                _coroutine ??= StartCoroutine(Move());
            }
        }
        
        private Doll GetNearbyPlayer()
        {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, interactRange);
            foreach (var collider in colliders)
            {
                if (collider.CompareTag("Player"))
                {
                    // Ищем Doll в родителях (если коллайдер на дочернем объекте)
                    var doll = collider.GetComponentInParent<Doll>();
                    Debug.Log("doll");
                    if (doll != null && doll.PlayerOwner != null)
                    {
                        return doll;
                    }
                }
            }
            return null;
        }

        
        private void UpdatePlayerComponents()
        {
            if (_nearbyDoll != null && _nearbyDoll != null)
            {
                _playerInventory = _nearbyDoll.GetComponentInChildren<PlayerInventory>();
                //_dollScore = _nearbyDoll.GetComponentInChildren<DollScore>();
            
                if (_playerInventory == null)
                {
                    Debug.LogWarning($"PlayerInventory not found on Doll for player {_nearbyDoll}");
                }
            }
        }

        private void ChangeMilk()
        {
            _playerInventory.RemoveItemServerRpc(food.ID);;
            _playerInventory.ReceiveItemServerRpc(filledMilk.ID);
        }

        private bool CheckFood()
        {
            Debug.Log(_playerInventory);
            return _playerInventory.ActiveItem.ID == food.ID;
        }
        
        
        private void SetVelocity(Vector2 circleRadius)
        {
            _rb2d.linearVelocity = new Vector2(circleRadius.x, circleRadius.y);
        }

        private void moveRadiusCalculation()
        {
            circleRadius = Random.insideUnitCircle * moveSpeed;
            _sprite.flipX = circleRadius.x < 0;
        }

        private IEnumerator Move()
        {
            _animator.SetBool("isWalking",true);
            moveRadiusCalculation();
            SetVelocity(circleRadius);
            yield return new WaitForSeconds(movingTime);
            _animator.SetBool("isWalking",false);
            SetVelocity(Vector2.zero);
            yield return new WaitForSeconds(stopTime);
            _coroutine = null;
        }
    }
}