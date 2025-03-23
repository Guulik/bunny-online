using System;
using System.Linq;
using Items;
using BunnyPlayer;
using Dolls.Health;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

namespace NPC
{
    public class Chicken : Npc, IInteractable
    {
        [SerializeField] private GameObject reward;
        [SerializeField] private Item requiredPass;
        [SerializeField] private int scoreReward = 10; // Количество очков за передачу

        private void OnEnable()
        {
            _playerInput.Player.Interact.started += Interact;
            TakeItem += _playerInventory.RemoveItem;
        }

        private void OnDisable()
        {
            _playerInput.Player.Interact.started -= Interact;
            TakeItem -= _playerInventory.RemoveItem;
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
                return _playerInventory.ActiveItem == requiredPass 
                       && _playerInventory.ActiveItem.ToPass().isCorrect;
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
                
                // Начисляем очки
                //DollScore.AddScore(scoreReward);
            }
        }
    }
}

