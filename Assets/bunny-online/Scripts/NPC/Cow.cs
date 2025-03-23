using Items;
using BunnyPlayer;
using UnityEngine;
using UnityEngine.InputSystem;

namespace NPC
{
    public class Cow : Npc, IInteractable
    {
        [SerializeField] private Item emptyMilk;
        [SerializeField] private Item filledMilk;
        [SerializeField] private Item food;
        private bool isFed = false;

        private void OnEnable()
        {
            _playerInput.Player.Interact.started += Interact;
           //TakeItem += _playerInventory.RemoveItem;
           //GiveItem += _playerInventory.ReceiveItem;
        }

        private void OnDisable()
        {
            _playerInput.Player.Interact.started -= Interact;
            //TakeItem -= _playerInventory.RemoveItem;
            //GiveItem -= _playerInventory.ReceiveItem;
        }

        public void Interact(InputAction.CallbackContext context)
        {
            Interact();
        }

        public void Interact()
        {
            if (!_canInteract) return;

            if (isFed)
            {
                if (CheckBottle())
                {
                    ChangeMilk();
                    ShowChatBubble("Вот твое млеко");
                }
                else
                {
                    ShowChatBubble("Ты пришел без бутылочки((");
                }
            }
            else
            {
                if (CheckFood())
                {
                    ShowChatBubble("Вот спасибо!");
                    isFed = true;
                    _playerInventory.RemoveItem(food);
                   // OnTakeItem(food);
                }
                else
                {
                    ShowChatBubble("Я голодна, дай пожалуйста пшенички");
                }
            }
        }
        private void ChangeMilk()
        {
            _playerInventory.RemoveItem(emptyMilk);
            _playerInventory.ReceiveItem(filledMilk);
            //OnTakeItem(emptyMilk);
            //OnGiveItem(filledMilk);
        }

        private bool CheckFood()
        {
            return _playerInventory.ActiveItem == food;
        }

        private bool CheckBottle()
        {
            return _playerInventory.ActiveItem == emptyMilk;
        }
    }
}