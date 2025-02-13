using System;
using System.Threading.Tasks;
using Items;
using BunnyPlayer;
using UnityEngine;
using UnityEngine.InputSystem;

namespace NPC
{
    public class Harvest : Interactable, IInteractable
    {
        private InteractUI _interactionMarker;
        [SerializeField] private Item _playerAxe;
        [SerializeField] private GameObject _reward;
        private PlayerInput _playerInput;

        private void Awake()
        {
            _player = GameObject.FindGameObjectWithTag("Player");
            _interactionMarker = GetComponentInChildren<InteractUI>();
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

        private void Update()
        {
            if (IsPlayerNearby())
            {
                _interactionMarker.Show();
                _canInteract = true;
            }
            else
            {
                _interactionMarker.Hide();
                _canInteract = false;
            }
        }

        public void Interact(InputAction.CallbackContext context)
        {
            Interact();
        }

        public void Interact()
        {
            if (!_canInteract) return;

            if (IsPlayerHaveAxe())
            {
                Destroy(gameObject, 0.5f);

                Instantiate(_reward, transform.position, Quaternion.identity);
                _reward.transform.localPosition = new Vector3(2f, 0.5f, 0f);
            }
            else
            {
                _player.GetComponent<PlayerInventory>().ShowChatBubble("Нужен топорик"); // not the best approach
            }
        }

        private bool IsPlayerHaveAxe()
        {
            return PlayerInventory.Inventory.ActiveItem == _playerAxe;
        }

        private bool IsPlayerNearby()
        {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, interactRange);
            foreach (var collider in colliders)
            {
                if (collider.gameObject == _player)
                {
                    return true;
                }
            }

            return false;
        }
    }
}