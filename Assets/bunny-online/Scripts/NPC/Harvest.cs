using System;
using System.Threading.Tasks;
using Items;
using BunnyPlayer;
using FishNet.Object;
using UnityEngine;
using UnityEngine.InputSystem;

namespace NPC
{
    public class Harvest : NetworkBehaviour, IInteractable
    {
        private InteractUI _interactionMarker;
        [SerializeField] private Item _playerAxe;
        [SerializeField] private GameObject _reward;
        private PlayerInput _playerInput;

        private GameObject _player;
        protected PlayerInventory _playerInventory; // Инвентарь конкретного игрока
        
        [SerializeField] protected float interactRange = 1f;
        protected bool _canInteract = false;

        private void Awake()
        {
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

                if (_player != null && _playerInventory == null)
                {
                    _playerInventory = _player.GetComponentInChildren<PlayerInventory>();
                }
            }
            else
            {
                _interactionMarker.Hide();
                _canInteract = false;
            }
        }

        public void Interact(InputAction.CallbackContext context)
        {
            //if (!IsOwner) return; // Только локальный игрок может инициировать
            //Interact();
            InteractServerRpc();
        }
        
        
        [ServerRpc(RequireOwnership = false)]
        private void InteractServerRpc()
        {
            if (!_canInteract || _playerInventory == null) return;
            //if (!IsPlayerHaveAxe()) return;

            Destroy(gameObject);
            SpawnRewardServerRpc();
        }
        
        [ServerRpc(RequireOwnership = false)]
        private void SpawnRewardServerRpc()
        { 
            Instantiate(_reward, transform.position, Quaternion.identity);
            _reward.transform.localPosition = new Vector3(2f, 0.5f, 0f);
            
            // Спавним объект в сети, чтобы он появился на всех клиентах
            ServerManager.Spawn(_reward);
            SpawnRewardObserversRpc();
        }

        [ObserversRpc]
        private void SpawnRewardObserversRpc()
        {
            Debug.Log("[CLIENT] Ресурс собран и предмет создан.");
        }
        
        /*public void Interact()
        {
            if (!_canInteract) return;


            Destroy(gameObject, 0.5f);

            Instantiate(_reward, transform.position, Quaternion.identity);
            _reward.transform.localPosition = new Vector3(2f, 0.5f, 0f);
            
        }*/

        private bool IsPlayerHaveAxe()
        {
            return _playerInventory.ActiveItem == _playerAxe;
        }

        private bool IsPlayerNearby()
        {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, interactRange);
            foreach (var collider in colliders)
            {
                if (collider.CompareTag("Player"))
                {
                    _player = collider.gameObject;
                    return true;
                }
            }

            return false;
        }
    }
}