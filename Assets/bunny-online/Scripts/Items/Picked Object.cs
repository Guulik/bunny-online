using System;
using NPC;
using BunnyPlayer;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Items
{
    public class PickedObject : Interactable, IInteractable
    { 
        [SerializeField] private Item inventoryItem;
        private PlayerInput _playerInput;
        
        private GameObject _player;
        protected PlayerInventory _playerInventory;  // Инвентарь конкретного игрока

        private void Awake()
        {
            _playerInput = new PlayerInput();
            _playerInput.Enable();
        }

        public Item InventoryItem { get => inventoryItem; set => inventoryItem = value; }
        private void OnEnable()
        {
            _playerInput.Player.Interact.started += Interact;
            
        }
        private void OnDisable()
        {
            _playerInput.Player.Interact.started -= Interact;
           
        }
    
        private event Action<Item> PickItem;

        private void OnItemPicked(Item item)
        {
            PickItem?.Invoke(item);
        }

        private void Update()
        {
            if (IsPlayerNearby())
            {
                _canInteract = true;
                
                if (_player != null && _playerInventory == null)
                {
                    _playerInventory = _player.GetComponentInChildren<PlayerInventory>();
                }
            }
            else
            {
                _canInteract = false;
            }
        }

        public void Interact(InputAction.CallbackContext context)
        {
            Interact();
        }
        public void Interact()
        {
            if(!_canInteract) return;

            if (_playerInventory.IsHave(inventoryItem)) return;
            
            _playerInventory.ReceiveItem(inventoryItem);
            
            //OnItemPicked(inventoryItem);

            Destroy(gameObject);
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
