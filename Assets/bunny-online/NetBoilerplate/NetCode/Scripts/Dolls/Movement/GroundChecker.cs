using UnityEngine;
using UnityEngine.Serialization;

namespace Dolls.Movement
{
    public class GroundChecker : MonoBehaviour
    {
        [FormerlySerializedAs("characterMovement")] [SerializeField]
        private DollMovement dollMovement;

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject == dollMovement.gameObject)
                return;

            dollMovement.SetGrounded(true);
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject == dollMovement.gameObject)
                return;

            dollMovement.SetGrounded(false);
        }

        // useless part???
        /*private void OnTriggerStay(Collider other)
        {
            if (other.gameObject == characterMovement.gameObject)
                return;

            characterMovement.SetGrounded(true);
        }*/
    }
}