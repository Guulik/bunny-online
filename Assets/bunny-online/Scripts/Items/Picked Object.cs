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

        private void Awake()
        {
            _playerInput = new PlayerInput();
            _playerInput.Enable();
            _player = GameObject.FindGameObjectWithTag("Player");
        }

        public Item InventoryItem { get => inventoryItem; set => inventoryItem = value; }
        private void OnEnable()
        {
            _playerInput.Player.Interact.started += Interact;
            PickItem += PlayerInventory.Inventory.ReceiveItem;
        }
        private void OnDisable()
        {
            _playerInput.Player.Interact.started -= Interact;
            PickItem -= PlayerInventory.Inventory.ReceiveItem;
        }
    
        private event Action<Item> PickItem;

        private void OnItemPicked(Item item)
        {
            PickItem?.Invoke(item);
        }

        private void Update()
        {
            _canInteract = IsPlayerNearby();
        }

        public void Interact(InputAction.CallbackContext context)
        {
            Interact();
        }
        public void Interact()
        {
            if(!_canInteract) return;
            
            OnItemPicked(inventoryItem);

            Destroy(gameObject);
        }
        
        private bool IsPlayerNearby()
        {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, interactRange);
            foreach (var collider in colliders)
            {
                if (collider.gameObject == _player)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
