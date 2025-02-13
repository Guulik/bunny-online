using System.Collections.Generic;
using System.Linq;
using Items;
using UnityEngine;
using UnityEngine.InputSystem;

namespace BunnyPlayer
{
    public class PlayerInventory : MonoBehaviour
    {
        public static PlayerInventory Inventory;
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
            Inventory = this;
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

        public void ReceiveItem(Item item)
        {
            Debug.Log("Recieved:" + item);
            
            if (!Items.Find(i => i == item))
                Items.Add(item);
            
            Items.Find(i => i == item).count++;
            
            SetActiveItem(item);
        }

        public void RemoveItem(Item item)
        {
            var itemInstance = Items.Find(i => i == item);
            itemInstance.count--;
            if (itemInstance.count > 0) return;
                    
            Items.Remove(item);
            if (ActiveItem == item)
                ActiveItem = Items.LastOrDefault();
            SetActiveItem(ActiveItem);

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

        public void ChangeActiveItemLeft()
        {
            if (IsInventoryEmpty()) return;

            var calculatedPos = Items.IndexOf(ActiveItem) - 1;
            int newItemPos =
                calculatedPos < 0 ? 0 : calculatedPos >= Items.Count ? Items.Count - 1 : calculatedPos;

            var newActiveItem = Items.Count != 0 ? Items[newItemPos] : null;
            SetActiveItem(newActiveItem);
        }
        public void ChangeActiveItemRight()
        {
            if (IsInventoryEmpty()) return;

            var calculatedPos = Items.IndexOf(ActiveItem) + 1;
            int newItemPos =
                calculatedPos < 0 ? 0 : calculatedPos >= Items.Count ? Items.Count - 1 : calculatedPos;

            var newActiveItem = Items.Count != 0 ? Items[newItemPos] : null;
            SetActiveItem(newActiveItem);
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
        
        
        public void ShowChatBubble(string text) //not SOLID but pohui. I just want to complete this subject as soon as i can
        {
            if (ChatBubbleHandler.BubbleInstance)
            {
                ChatBubbleHandler.BubbleInstance.OnShowUp(gameObject.transform, text);
            }
            else
                Debug.Log("Bubble НЕ существует");
        }
    }
}