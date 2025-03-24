using System;
using NPC;
using BunnyPlayer;
using FishNet.Component.Transforming;
using FishNet.Connection;
using FishNet.Object;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Items
{
    public class PickedObject : NetworkBehaviour, IInteractable
    { 
        [SerializeField] private Item inventoryItem;
        private PlayerInput _playerInput;
        
        private GameObject _player;
        protected PlayerInventory _playerInventory;  // Инвентарь конкретного игрок

        [SerializeField] protected float interactRange = 1f;
        protected bool _canInteract = false;
        
        // Сетевое состояние объекта (будет подбирать его только один игрок)
        private bool _isPicked = false;
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

            if (_playerInventory.HasItem(inventoryItem)) return;
            
            
            
            // Вызываем ServerRpc для подбора предмета
            PickupItemServerRpc(_player);
            
            
            //_playerInventory.ReceiveItem(inventoryItem);
            
            //OnItemPicked(inventoryItem);

            //Destroy(gameObject);
        }


        [ServerRpc(RequireOwnership = false)]
        private void PickupItemServerRpc(GameObject doll)
        {
            Debug.Log("picking");
            Debug.Log(doll);
            // Получаем инвентарь игрока, который подбирает предмет
            var playerInventory = doll.GetComponentInChildren<PlayerInventory>();
            Debug.Log(playerInventory);
            if (playerInventory == null) return;

            // Добавляем предмет в инвентарь игрока
            playerInventory.ReceiveItemServerRpc(inventoryItem.ID);

            // Уничтожаем объект на всех клиентах
            DespawnObjectObserversRpc();
        }
        [ObserversRpc]
        private void DespawnObjectObserversRpc()
        {
            _isPicked = true;
            ServerManager.Despawn(gameObject);
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
