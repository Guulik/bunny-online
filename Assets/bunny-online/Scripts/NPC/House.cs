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
    public class House : NetworkBehaviour, IInteractable
    {
        //[SerializeField] private GameObject reward;
        [SerializeField] private Item richItem;
        [SerializeField] private int richReward = 5; 
        [SerializeField] private Item commonItem;
        [SerializeField] private int commonReward = 1;

        private Rigidbody2D _rb2d;
        
        private PlayerInventory _inventory;
        protected DollScore _dollScore;
        protected PlayerInput _playerInput;
        
        protected bool _canInteract = false;
        
        
        [SerializeField] protected InteractUI _interactionMarker;
        private void Awake()
        {
            _rb2d = GetComponent<Rigidbody2D>();
            _rb2d.gravityScale = 0f;
            
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
                _canInteract = true;
                _interactionMarker.Show();
                
                var doll = other.GetComponentInChildren<Doll>();
                Debug.Log("houdse doll detected: "  + doll);
                
                _inventory = doll.gameObject.GetComponentInChildren<PlayerInventory>();
                _dollScore = doll.gameObject.GetComponentInChildren<DollScore>();
                Debug.Log(_inventory);
                Debug.Log("houdse dollScore detected: " + _dollScore);
                Debug.Log("houdse doll Owner detected: "+ _dollScore.GetPlayerOwner().PlayerName);
            }
        }

        public void OnTriggerExit2D(Collider2D other)
        {
                _interactionMarker.Hide();
                _canInteract = false;
        }

        public void Interact(InputAction.CallbackContext context)
        {
            if (!IsOwner) return;
            Interact();
        }

        public void Interact()
        {
            if (!_canInteract) return;

            Debug.Log("interacting");
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
            Debug.Log(_inventory.ActiveItem);
            _inventory.RemoveItemServerRpc(item.ID);
        }
        private void GiveScore(int score)
        {
            _dollScore.AddScore(score);
        }
    }
}