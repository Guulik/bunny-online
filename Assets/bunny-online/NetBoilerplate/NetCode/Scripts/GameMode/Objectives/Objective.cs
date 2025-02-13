using System;
using UnityEngine;

namespace GameMode
{
    public class Objective : MonoBehaviour
    {
        public event Action OnComplete;

        // public virtual void  Reset() {}
        protected void CompleteObjective()
        {
            OnComplete?.Invoke();
        }
    }
}