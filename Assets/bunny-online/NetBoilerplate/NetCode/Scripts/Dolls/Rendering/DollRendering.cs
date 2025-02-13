using UnityEngine;

namespace Dolls.Rendering
{
    public class DollRendering : MonoBehaviour
    {
        [SerializeField] private Transform targetCamera;

        [Header("Side Sprites")] [SerializeField]
        private Sprite frontSide, leftSide, rightSide, backSide;

        private SpriteRenderer _spriteRenderer;

        private void Start()
        {
            _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }

        private void Update()
        {
            UpdateSprite();
        }

        public void SetTargetCamera(Transform newTargetCamera)
        {
            targetCamera = newTargetCamera;
        }

        private void UpdateSprite()
        {
            Vector3 directionToCamera = targetCamera.position - transform.position;
            float angle = Vector3.SignedAngle(transform.forward, directionToCamera, Vector3.up);

            _spriteRenderer.sprite = angle switch
            {
                >= -45 and < 45 => frontSide,
                >= 45 and < 135 => leftSide,
                >= -135 and < -45 => rightSide,
                _ => backSide
            };
        }
    }
}