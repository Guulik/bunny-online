using System;
using System.Threading.Tasks;
using Items;
using BunnyPlayer;
using UnityEngine;
using UnityEngine.InputSystem;

namespace NPC
{
    public class Harvest : Interactable, IInteractable
    {
        private InteractUI _interactionMarker;
        [SerializeField] private Item _playerAxe;
        [SerializeField] private GameObject _reward;
        private PlayerInput _playerInput;

        private GameObject _player;
        protected PlayerInventory _playerInventory;  // Инвентарь конкретного игрока
        
        
        private void Awake()
        {
            _interactionMarker = GetComponentInChildren<InteractUI>();
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

        private void Update()
        {
            if (IsPlayerNearby())
            {
                _interactionMarker.Show();
                _canInteract = true;
                
                if (_player != null && _playerInventory == null)
                {
                    _playerInventory = _player.GetComponentInChildren<PlayerInventory>();
                }
            }
            else
            {
                _interactionMarker.Hide();
                _canInteract = false;
            }
        }

        public void Interact(InputAction.CallbackContext context)
        {
            Interact();
        }

        public void Interact()
        {
            if (!_canInteract) return;

            if (IsPlayerHaveAxe())
            {
                Destroy(gameObject, 0.5f);

                Instantiate(_reward, transform.position, Quaternion.identity);
                _reward.transform.localPosition = new Vector3(2f, 0.5f, 0f);
            }
            else
            {
                _playerInventory.ShowChatBubble("Нужен топорик"); // не лучшее решение...
            }
        }

        private bool IsPlayerHaveAxe()
        {
            return _playerInventory.ActiveItem == _playerAxe;
        }

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
    }
}