using System;
using System.Linq;
using Items;
using BunnyPlayer;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

namespace NPC
{
    public class Chicken : Npc, IInteractable
    {
        [SerializeField] private GameObject reward;
        [SerializeField] private Item requiredPass;

        private void OnEnable()
        {
            _playerInput.Player.Interact.started += Interact;
            TakeItem += PlayerInventory.Inventory.RemoveItem;
        }

        private void OnDisable()
        {
            _playerInput.Player.Interact.started -= Interact;
            TakeItem -= PlayerInventory.Inventory.RemoveItem;
        }

        public void Interact(InputAction.CallbackContext context)
        {
            Interact();
        }

        public void Interact()
        {
            if (!_canInteract) return;
            
            if (CheckPass())
                GiveEgg();
            else 
                ShowChatBubble("Принеси пожалуйста Млеко");
        }
        
        private bool CheckPass()
        {
                return PlayerInventory.Inventory.ActiveItem == requiredPass 
                       && PlayerInventory.Inventory.ActiveItem.ToPass().isCorrect;
        }

        private void TakeMilk()
        {
            OnTakeItem(requiredPass);
        }
        private void GiveEgg()
        {
            if (CheckPass())
            {
                TakeMilk();
                
                ShowChatBubble("Держи яйко");
                
                GameObject rewardObject = Instantiate(reward, transform.position, quaternion.identity);
                rewardObject.transform.localPosition = new Vector3(2f, 0.5f, 0f);
            }
        }
    }
}

