using System;
using System.Collections.Generic;
using System.Linq;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using FishNet.Serializing;
using Items;
using UnityEngine;
using UnityEngine.InputSystem;

namespace BunnyPlayer
{
    public class PlayerInventory : NetworkBehaviour
    {
        private PlayerInput _playerInput;

        [SerializeField] private GameObject handledItem;

        private SpriteRenderer _handledItemSprite;

        [SerializeField] private List<Item> items = new List<Item>();

        public readonly SyncVar<int> ActiveItemIndex = new();
        
        private List<Item> Items
        {
            get => items;
            set => items = value;
        }
        
        public Item _activeItem;
        public Item ActiveItem 
        {
            get => _activeItem;
            set
            {
                _activeItem = value;
                _handledItemSprite.sprite = _activeItem?.sprite;
            }
        }
        
        public override void OnStartClient()
        {
            base.OnStartClient();
            if (!IsOwner)
            {
                // Отключаем управление инвентарем для других игроков
                enabled = false;
            }
        }

        private void Awake()
        {
            foreach (Item item in Resources.LoadAll<Item>("Items"))
            {
                ItemManager.RegisterItem(item);
            }

            ActiveItemIndex.Value = 0;
            Items = new List<Item>();
            _handledItemSprite = handledItem.GetComponent<SpriteRenderer>();
            _playerInput = new PlayerInput();
            _playerInput.Enable();
        }

        private void OnEnable()
        {
            _playerInput.Player.ChangeItem.started += ChangeActiveItem;
           // _playerInput.Player.DropItem.started += DropItem;
        }

        private void OnDisable()
        {
            _playerInput.Player.ChangeItem.started -= ChangeActiveItem;
            //_playerInput.Player.DropItem.started -= DropItem;
        }
        
        public bool HasItem(Item item)
        {
            return Items.Exists(i => i == item);
        }
        [ServerRpc(RequireOwnership = false)]
        public void ReceiveItemServerRpc(int itemID)
        {
            // На сервере получаем предмет по ID
            Item item = ItemManager.GetItemByID(itemID);
            if (item == null) return;
            Debug.Log(item);

            // Добавляем предмет на сервере
            var existingItem = Items.FirstOrDefault(i => i.ID == itemID);
            if (existingItem != null)
            {
                existingItem.count++;
            }
            else
            {
                Item newItem = Instantiate(item); // Создаем копию ScriptableObject
                newItem.count = 1;
                Items.Add(newItem);
            }

            // Синхронизируем изменения
            SyncInventoryClientRpc(itemID);
        }

        [ObserversRpc]
        private void SyncInventoryClientRpc(int itemID)
        {
            // На клиенте получаем предмет по ID
            Item item = ItemManager.GetItemByID(itemID);
            if (item == null) return;

            var existingItem = Items.FirstOrDefault(i => i.ID == itemID);
            if (existingItem != null)
            {
                existingItem.count++;
            }
            else
            {
                Item newItem = Instantiate(item);
                newItem.count = 1;
                Items.Add(newItem);
            }

            if (ActiveItem == null)
            {
                SetActiveItem(Items[0]);
            }
        }
        
        [ServerRpc(RequireOwnership = false)]
        public void RemoveItemServerRpc(int itemID)
        {
            var item = Items.FirstOrDefault(i => i.ID == itemID);
            if (item != null)
            {
                Items.Remove(item);
                if (ActiveItem != null && ActiveItem.ID == itemID)
                {
                    SetActiveItem(Items.Count > 0 ? Items[0] : null);
                }
            }
    
            SyncRemoveItemClientRpc(itemID);
        }

        [ObserversRpc]
        private void SyncRemoveItemClientRpc(int itemID)
        {
            var item = Items.FirstOrDefault(i => i.ID == itemID);
            if (item != null)
            {
                Items.Remove(item);
                if (ActiveItem != null && ActiveItem.ID == itemID)
                {
                    SetActiveItem(Items.Count > 0 ? Items[0] : null);
                }
            }
        }
       
        private void SetActiveItem(Item newActiveItem)
        {
            if (!IsOwner) return;
            Debug.Log("new active item  = " + newActiveItem);
            ActiveItem = newActiveItem;
            _handledItemSprite.sprite = ActiveItem?.sprite;

            SetActiveItemServerRpc(newActiveItem?.ID ?? -1);
            
        }

        [ServerRpc]
        private void SetActiveItemServerRpc(int itemId)
        {
            Item item = itemId == -1 ? null : ItemManager.GetItemByID(itemId);
            _activeItem = item;
            
            SyncActiveItemClientRpc(itemId);
        }
        [ObserversRpc]
        private void SyncActiveItemClientRpc(int itemId)
        {
            if (IsOwner) return;
    
            _activeItem = itemId == -1 ? null : ItemManager.GetItemByID(itemId);
            _handledItemSprite.sprite = _activeItem?.sprite;
        }
        

        private void ChangeActiveItem(InputAction.CallbackContext context)
        {
            if (IsInventoryEmpty()) return;

            var calculatedPos = ActiveItemIndex.Value + (int)_playerInput.Player.ChangeItem.ReadValue<float>();
            int newItemPos =
                calculatedPos < 0 ? 0 : calculatedPos >= Items.Count ? Items.Count - 1 : calculatedPos;

            ActiveItemIndex.Value = newItemPos;

            Debug.Log("new active item index = " + newItemPos);
            var newActiveItem = Items.Count != 0 ? Items[newItemPos] : null;
            Debug.Log("new active item  = " + newActiveItem);

            SetActiveItem(newActiveItem);
        }

        /*private void DropItem(InputAction.CallbackContext context)
        {
            if (IsInventoryEmpty()) return;

            Instantiate(ActiveItem.prefab, transform.position + new Vector3(0.5f, -0.5f, 0f), Quaternion.identity);
            RemoveItem(ActiveItem);
        }*/

        private bool IsInventoryEmpty()
        {
            return Items.Count == 0;
        }
    }
}