using System;
using System.Linq;
using Items;
using BunnyPlayer;
using Dolls.Health;
using FishNet.Object;
using Unity.Mathematics;
using Unity.VisualScripting;
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

        private PlayerInventory _inventory;
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

        public void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                var doll = other.GetComponentInChildren<Doll>();
                Debug.Log(doll);
                if (doll.gameObject.GetComponentInChildren<NetworkObject>().IsOwner)
                {
                    _inventory = doll.GetComponentInChildren<PlayerInventory>();
                };
            }
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
            return _inventory.ActiveItem.ID == richItem.ID;
        }
        
        private bool CheckBerry()
        {
            return _inventory.ActiveItem.ID == commonItem.ID;
        }

        private void TakeRequiredItem(Item item)
        {
            Debug.Log(_nearbyPlayer);
            Debug.Log(_inventory.ActiveItem);
            _inventory.RemoveItemServerRpc(item.ID);
        }
        private void GiveScore(int score)
        {
            _dollScore.AddScore(score);
        }
    }
}