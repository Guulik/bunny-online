using System.Collections;
using Dolls.Health;
using Dolls.Movement;
using Dolls.Rendering;
using FishNet.Object;
using UnityEngine;
using UnityEngine.Serialization;

public class Doll : NetworkBehaviour
{
    
    [SerializeField] private Camera characterCamera;

    [SerializeField] private DollMovement dollMovement;

    public Player PlayerOwner;
    [field: SerializeField] public DollScore DollScore { get; private set; }

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