using UnityEngine;

namespace NPC 
{
    public class Interactable : MonoBehaviour
    {
        protected GameObject _player;
        [Space(20)] 
        [SerializeField] protected float interactRange = 1f;
        protected bool _canInteract = false;
        private void OnEnable()
        {
            _player = GameObject.FindGameObjectWithTag("Player");
        }
        
        
    }
}