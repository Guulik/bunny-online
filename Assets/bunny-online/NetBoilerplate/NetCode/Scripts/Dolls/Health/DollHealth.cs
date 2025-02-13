using System;
using System.Collections;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;

namespace Dolls.Health
{
    [DisallowMultipleComponent]
    public class DollHealth : NetworkBehaviour
    {
        [Header("Health Points setup")] [SerializeField] [Range(1, 100)]
        private int maxHealth = 100;

        [Header("Regeneration setup")] [SerializeField] [Tooltip("Задержка до начала регенерации")]
        private float regenDelay = 5f;

        [SerializeField] [Tooltip("Процент восстанавливаемого здоровья")]
        private int regenAmount = 5;

        [SerializeField] [Tooltip("Время между циклами регенерации")]
        private float regenCycle = 1f;

        /// <summary>
        ///     Текущее ОЗ персонажа, синхронизированное между клиентами
        /// </summary>
        public readonly SyncVar<int> CurrentHealth = new();

        private Coroutine _checkRegenPossibilityCoroutine;

        private Player _lastAttacker;

        private Player _playerOwner;

        private Coroutine _regenCoroutine;

        public static event Action<DollDeathEventArgs> OnDeath;

        public override void OnStartServer()
        {
            Rejuvenate();
        }

        public void SetPlayerOwner(Player newOwner)
        {
            _playerOwner = newOwner;
        }

        public void Rejuvenate()
        {
            CurrentHealth.Value = maxHealth;
        }

        [Server]
        public void TakeDamage(int damage, Player attacker)
        {
            _lastAttacker = attacker;
            CurrentHealth.Value -= damage;
            if (CurrentHealth.Value <= 0)
            {
                Die();
                CancelRegeneration();
                return;
            }

            ProcessDamage();
        }

        [Server]
        private void ProcessDamage()
        {
            if (_checkRegenPossibilityCoroutine != null)
            {
                StopCoroutine(_checkRegenPossibilityCoroutine);
                _checkRegenPossibilityCoroutine = StartCoroutine(CheckRegenPossibility());
            }
            else
            {
                _checkRegenPossibilityCoroutine = StartCoroutine(CheckRegenPossibility());
            }

            if (_regenCoroutine != null)
            {
                StopCoroutine(_regenCoroutine);
                _regenCoroutine = null;
            }
        }

        private IEnumerator CheckRegenPossibility()
        {
            yield return new WaitForSeconds(regenDelay);
            _regenCoroutine = StartCoroutine(Regen());
            _checkRegenPossibilityCoroutine = null;
        }

        private IEnumerator Regen()
        {
            while (CurrentHealth.Value < maxHealth)
            {
                CurrentHealth.Value = Mathf.Min(CurrentHealth.Value + regenAmount, maxHealth);
                yield return new WaitForSeconds(regenCycle);
            }

            _regenCoroutine = null;
        }

        [Server]
        private void Die()
        {
            OnDeath?.Invoke(new DollDeathEventArgs(_lastAttacker, _playerOwner));
            Debug.Log($"[Server] {_playerOwner.PlayerName} died. Killer is {_lastAttacker.PlayerName}");
            CancelRegeneration();
        }

        private void CancelRegeneration()
        {
            if (_regenCoroutine != null)
            {
                StopCoroutine(_regenCoroutine);
                _regenCoroutine = null;
            }

            if (_checkRegenPossibilityCoroutine != null)
            {
                StopCoroutine(_checkRegenPossibilityCoroutine);
                _checkRegenPossibilityCoroutine = null;
            }
        }

        public int GetMaxHealth()
        {
            return maxHealth;
        }
    }
}