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
    public class House : Npc, IInteractable
    {
        //[SerializeField] private GameObject reward;
        [SerializeField] private Item requiredPass;
        [SerializeField] private int scoreReward = 10; // Количество очков за передачу

        private void Awake()
        {
            _playerInput = new PlayerInput();
            _playerInput.Enable();
        }
        private void OnEnable()
        {
            _playerInput.Player.Interact.started += Interact;
           // TakeItem += _playerInventory.RemoveItem;
        }

        private void OnDisable()
        {
            _playerInput.Player.Interact.started -= Interact;
            //TakeItem -= _playerInventory.RemoveItem;
        }

        public void Interact(InputAction.CallbackContext context)
        {
            Interact();
        }

        public void Interact()
        {
            if (!_canInteract) return;
            
            if (CheckPass())
                GiveScore();
            else 
                ShowChatBubble("Надо Млеко");
        }
        
        private bool CheckPass()
        {
                return _playerInventory.ActiveItem == requiredPass 
                       && _playerInventory.ActiveItem.ToPass().isCorrect;
        }

        private void TakeMilk()
        {
            _playerInventory.RemoveItem(requiredPass);
            //OnTakeItem(requiredPass);
        }
        private void GiveScore()
        {
            if (CheckPass())
            {
                TakeMilk();
                
                // Начисляем очки
                DollScore.Instance.AddScore(scoreReward);
            }
        }
    }
}

