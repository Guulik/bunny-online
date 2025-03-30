using System.Collections.Generic;
using System.Linq;
using FishNet.Object;
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

        private List<Item> Items
        {
            get => items;
            set => items = value;
        }
        
        private Item _activeItem;
        public Item ActiveItem 
        {
            get => _activeItem;
            set
            {
                _activeItem = value;
                _handledItemSprite.sprite = _activeItem?.sprite;
        
                if (IsOwner)
                {
                    // Отправляем ID активного предмета на сервер
                    SetActiveItemServerRpc(_activeItem?.ID ?? -1);
                }
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
            // Где-то при старте игры (например, в Awake менеджера предметов)
            foreach (Item item in Resources.LoadAll<Item>("Items"))
            {
                ItemManager.RegisterItem(item);
            }

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

            ActiveItem = newActiveItem;
            _handledItemSprite.sprite = ActiveItem?.sprite;
    
            if (IsServer)
            {
                SetActiveItemServerRpc(newActiveItem?.ID ?? -1);
            }
        }

        [ServerRpc]
        private void SetActiveItemServerRpc(int itemId)
        {
            Item item = itemId == -1 ? null : ItemManager.GetItemByID(itemId);
            _activeItem = item;
    
            // Рассылаем всем клиентам
            SyncActiveItemClientRpc(itemId);
        }
        [ObserversRpc]
        private void SyncActiveItemClientRpc(int itemId)
        {
            if (IsOwner) return;
    
            _activeItem = itemId == -1 ? null : ItemManager.GetItemByID(itemId);
            _handledItemSprite.sprite = _activeItem?.sprite;
        }
        
        /*
        public void ReceiveItem(Item item)
        {
            Debug.Log("Recieved:" + item);
            
            // Если предмет уже есть в инвентаре, увеличиваем его количество
            if (Items.Exists(i => i == item))
            {
                var existingItem = Items.Find(i => i == item);
                existingItem.count = 1;  // Устанавливаем count в 1, чтобы только один предмет был в руке
                return; // Предмет не добавляем повторно
            }

            // Добавляем новый предмет в инвентарь
            Items.Add(item);
            item.count = 1; // Устанавливаем count в 1 для нового предмета
            
            SetActiveItem(item);
        }

        public void RemoveItem(Item item)
        {
            var itemInstance = Items.Find(i => i == item);
    
            // Если предмет в инвентаре, и его count равен 1, удаляем его
            
            if (itemInstance != null)
            {
                Items.Remove(item);
                if (ActiveItem == item) ActiveItem = Items.LastOrDefault();
                SetActiveItem(ActiveItem);
            }
            

            //Debug.Log(ActiveItem);
        }

        private void SetActiveItem(Item newActiveItem)
        {
            if (!newActiveItem)
            {
                _handledItemSprite.sprite = null;
                ActiveItem = null;
                return;
            }

            ActiveItem = newActiveItem;
            _handledItemSprite.sprite = ActiveItem.sprite;
        }
        */

        private void ChangeActiveItem(InputAction.CallbackContext context)
        {
            if (IsInventoryEmpty()) return;

            var calculatedPos = Items.IndexOf(ActiveItem) + (int)_playerInput.Player.ChangeItem.ReadValue<float>();
            int newItemPos =
                calculatedPos < 0 ? 0 : calculatedPos >= Items.Count ? Items.Count - 1 : calculatedPos;

            var newActiveItem = Items.Count != 0 ? Items[newItemPos] : null;
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