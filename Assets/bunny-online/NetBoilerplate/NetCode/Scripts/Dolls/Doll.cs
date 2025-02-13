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
    //[SerializeField] private AudioListener audioListener;
    [SerializeField] private DollMovement dollMovement;

    //[field: SerializeField] public DollAttack DollAttack { get; private set; }
    //[field: SerializeField] public DollHealth DollHealth { get; private set; }

    //[FormerlySerializedAs("healthBar")] [SerializeField] private GameObject HUD;

    //[SerializeField] private SpriteRenderer characterSpriteRenderer;

    public override void OnStartClient()
    {
        if (!IsOwner)
        {
            characterCamera.gameObject.SetActive(false);
            //spriteBillboard.enabled = true;
            //HUD.SetActive(false);
            //characterSpriteRenderer.enabled = true;
        }
        
        //audioListener.enabled = true;

        StartCoroutine(WaitAndAssignMainCamera());
    }

    private IEnumerator WaitAndAssignMainCamera()
    {
        while (Camera.main == null) yield return null; // Ждём следующий кадр

        var mainCamera = Camera.main;
        //dollRendering.SetTargetCamera(mainCamera.transform);
        //spriteBillboard.SetTargetCamera(mainCamera.transform);
    }

    [ObserversRpc]
    public void SyncDollPosition(Vector3 transformPosition)
    {
        transform.position = transformPosition;
    }
}