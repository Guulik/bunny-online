using System;
using System.Collections;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;

namespace Dolls.Health
{
    [DisallowMultipleComponent]
    public class DollScore : NetworkBehaviour
    {
        
        public static DollScore Instance;
        private int score;
        public static event Action<int> OnItemPassed; // Новый ивент
        
        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }
        
        public void AddScore(int amount)
        {
            score += amount;
            OnItemPassed?.Invoke(score); // Вызываем ивент, передавая обновленный счет
        }
    }
}