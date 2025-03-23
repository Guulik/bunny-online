using UnityEngine;

namespace NPC 
{
    public class Interactable : MonoBehaviour
    {
      
        [Space(20)] 
        [SerializeField] protected float interactRange = 1f;
        protected bool _canInteract = false;
        
    }
}