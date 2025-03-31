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
        
        [SerializeField]private int score;
        private Player _playerOwner;
        public static event Action<TakeScoreEventArgs> OnTakeScore;
        
        public void AddScore(int amount)
        {
            score += amount;
            Debug.Log(_playerOwner);
            OnTakeScore?.Invoke(new TakeScoreEventArgs(_playerOwner, amount));
        }
        public void SetPlayerOwner(Player newOwner)
        {
            Debug.Log(newOwner);
            _playerOwner = newOwner;
        }

        public Player GetPlayerOwner()
        {
            return _playerOwner;
        }
    }
}