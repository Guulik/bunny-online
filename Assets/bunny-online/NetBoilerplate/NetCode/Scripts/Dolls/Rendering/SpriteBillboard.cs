using UnityEngine;

public class SpriteBillboard : MonoBehaviour
{
    [SerializeField] private Transform targetCamera;

    private void LateUpdate()
    {
        Vector3 directionToCamera = targetCamera.position - transform.position;
        directionToCamera.y = 0; // Обнуляем компонент по оси Y, чтобы избежать наклонов
        transform.rotation = Quaternion.LookRotation(-directionToCamera);
    }

    public void SetTargetCamera(Transform newTargetCamera)
    {
        targetCamera = newTargetCamera;
    }
}