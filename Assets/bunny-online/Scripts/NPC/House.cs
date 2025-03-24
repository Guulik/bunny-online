using System;
using System.Linq;
using Items;
using BunnyPlayer;
using Dolls.Health;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace NPC
{
    public class House : Npc, IInteractable
    {
        //[SerializeField] private GameObject reward;
        [SerializeField] private Item richItem;
        [SerializeField] private int richReward = 5; 
        [SerializeField] private Item commonItem;
        [SerializeField] private int commonReward = 1;

        private void Awake()
        {
            _playerInput = new PlayerInput();
            _playerInput.Enable();
        }

        private void OnEnable()
        {
            _playerInput.Player.Interact.started += Interact;
        }

        private void OnDisable()
        {
            _playerInput.Player.Interact.started -= Interact;
        }

        public void Interact(InputAction.CallbackContext context)
        {
            Interact();
        }

        public void Interact()
        {
            if (!_canInteract) return;

            if (CheckMilk())
            {
                TakeRequiredItem(richItem);
                GiveScore(richReward);
            }

            if (CheckBerry())
            {
                TakeRequiredItem(commonItem);
                GiveScore(commonReward);
            }
        }

        private bool CheckMilk()
        {
            return _playerInventory.ActiveItem == richItem;
        }
        
        private bool CheckBerry()
        {
            return _playerInventory.ActiveItem == commonItem;
        }

        private void TakeRequiredItem(Item item)
        {
            _playerInventory.RemoveItemServerRpc(item.ID);
        }
        private void GiveScore(int score)
        {
            _dollScore.AddScore(score);
        }
    }
}