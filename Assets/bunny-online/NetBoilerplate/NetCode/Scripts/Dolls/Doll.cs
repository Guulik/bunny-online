using System;
using System.Collections;
using Dolls.Health;
using Dolls.Movement;
using Dolls.Rendering;
using FishNet.Object;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class Doll : NetworkBehaviour
{
    
    [SerializeField] private Camera characterCamera;

    [SerializeField] private DollMovement dollMovement;

    public Player PlayerOwner;
    
    private PlayerInput _playerInput;
    [field: SerializeField] public DollScore DollScore { get; private set; }

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
    
    public event Action<int> OnInteract; 
    
    
    private void Interact(InputAction.CallbackContext context)
    {
        var id = (int)PlayerOwner.SteamID.m_SteamID;
        OnInteract?.Invoke(id);
    }
    
    
    public override void OnStartClient()
    {
        if (!IsOwner)
        {
            characterCamera.gameObject.SetActive(false);

        }

        StartCoroutine(WaitAndAssignMainCamera());
    }

    private IEnumerator WaitAndAssignMainCamera()
    {
        while (Camera.main == null) yield return null; // Ждём следующий кадр

        var mainCamera = Camera.main;
        //dollRendering.SetTargetCamera(mainCamera.transform);
        //spriteBillboard.SetTargetCamera(mainCamera.transform);
    }
    
    public void SetPlayerOwner(Player newOwner)
    {
        Debug.Log(newOwner);
        PlayerOwner = newOwner;
    }

    [ObserversRpc]
    public void SyncDollPosition(Vector3 transformPosition)
    {
        transform.position = transformPosition;
    }
}