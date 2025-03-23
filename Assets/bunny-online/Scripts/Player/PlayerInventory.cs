using System.Collections.Generic;
using System.Linq;
using Items;
using UnityEngine;
using UnityEngine.InputSystem;

namespace BunnyPlayer
{
    public class PlayerInventory : MonoBehaviour
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

        public Item ActiveItem { get; set; }

        private void Awake()
        {
            Items = new List<Item>();
            _handledItemSprite = handledItem.GetComponent<SpriteRenderer>();
            _playerInput = new PlayerInput();
            _playerInput.Enable();
        }

        private void OnEnable()
        {
            _playerInput.Player.ChangeItem.started += ChangeActiveItem;
            _playerInput.Player.DropItem.started += DropItem;
        }

        private void OnDisable()
        {
            _playerInput.Player.ChangeItem.started -= ChangeActiveItem;
            _playerInput.Player.DropItem.started -= DropItem;
        }

        public bool HasItem(Item item)
        {
            return Items.Exists(i => i == item);
        }
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

        private void ChangeActiveItem(InputAction.CallbackContext context)
        {
            if (IsInventoryEmpty()) return;

            var calculatedPos = Items.IndexOf(ActiveItem) + (int)_playerInput.Player.ChangeItem.ReadValue<float>();
            int newItemPos =
                calculatedPos < 0 ? 0 : calculatedPos >= Items.Count ? Items.Count - 1 : calculatedPos;

            var newActiveItem = Items.Count != 0 ? Items[newItemPos] : null;
            SetActiveItem(newActiveItem);
        }

        private void DropItem(InputAction.CallbackContext context)
        {
            if (IsInventoryEmpty()) return;

            Instantiate(ActiveItem.prefab, transform.position + new Vector3(0.5f, -0.5f, 0f), Quaternion.identity);
            RemoveItem(ActiveItem);
        }

        private bool IsInventoryEmpty()
        {
            return Items.Count == 0;
        }
    }
}