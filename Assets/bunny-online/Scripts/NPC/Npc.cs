using System;
using System.Collections;
using System.Linq;
using BunnyPlayer;
using Dolls.Health;
using FishNet.Object;
using Items;
using Steamworks;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;


namespace NPC
{
    public class Npc : NetworkBehaviour
    { 
        [SerializeField]protected bool Movable = false;
        protected bool isAllowedToMove = false;
        [SerializeField] private float moveSpeed;
        [SerializeField] private float movingTime = 1f;
        [SerializeField] private float stopTime = 1f;
        [Space(20)] 

        private Vector2 circleRadius = Vector2.zero;
        private Coroutine _coroutine;
        private Rigidbody2D _rb2d;
        
        private GameObject _player;
        protected Player _nearbyPlayer;
        protected PlayerInventory _playerInventory; 
        protected DollScore _dollScore;  // Инвентарь конкретного игрока
        
        [SerializeField] private InteractUI _interactionMarker;
        private Animator _animator;
        private SpriteRenderer _sprite;
        protected PlayerInput _playerInput;
        
            
        [SerializeField] protected float interactRange = 1f;
        protected bool _canInteract = false;
        

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
        

        private Player GetNearbyPlayer()
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
                        return doll.PlayerOwner;
                    }
                }
            }
            return null;
        }

        private void Update()
        {
            _nearbyPlayer = GetNearbyPlayer();
           UpdatePlayerComponents();
            _canInteract = _nearbyPlayer != null;

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
                StartCoroutine(Move());
            }
        }
        private void UpdatePlayerComponents()
        {
            if (_nearbyPlayer != null && _nearbyPlayer.Doll != null)
            {
                _playerInventory = _nearbyPlayer.Doll.GetComponentInChildren<PlayerInventory>();
                _dollScore = _nearbyPlayer.Doll.GetComponentInChildren<DollScore>();
            
                if (_playerInventory == null)
                {
                    Debug.LogWarning($"PlayerInventory not found on Doll for player {_nearbyPlayer.PlayerName}");
                }
            }
        }
        
        
        #region Moving

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
        
        #endregion

        private bool IsPlayerNearby()
        {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, interactRange);
            foreach (var collider in colliders)
            {
                if (collider.CompareTag("Player"))
                {
                    _player = collider.gameObject;
                    return true;
                }
            }
            return false;
        }
        protected void ShowChatBubble(string text)
        {
            if (ChatBubbleHandler.BubbleInstance)
            {
                ChatBubbleHandler.BubbleInstance.OnShowUp(gameObject.transform, text);
            }
            else
                Debug.Log("Bubble НЕ существует");
        }

        protected event Action<Item> GiveItem;

        protected void OnGiveItem(Item item)
        {
            GiveItem?.Invoke(item);
        }

        protected event Action<Item> TakeItem;

        protected void OnTakeItem(Item item)
        {
            TakeItem?.Invoke(item);
        }
    }
}