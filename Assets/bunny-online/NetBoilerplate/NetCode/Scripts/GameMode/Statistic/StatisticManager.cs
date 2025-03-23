using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Dolls.Health;
using FishNet;
using FishNet.Connection;
using FishNet.Object;
using UnityEngine;

namespace GameMode
{
    public struct Statistic
    {
        public int Kills;
        public int Deaths;
    }

    public class StatisticManager : SingletonNetworkBehavior<StatisticManager>
    {
        private Dictionary<Player, Statistic> _statTable = new();
        private Dictionary<Player, int> _scoreTable = new();

        private void OnEnable()
        {
            DollScore.OnTakeScore += HandleDollScore;
        }

        private void OnDisable()
        {
            DollScore.OnTakeScore -= HandleDollScore;
        }

        public event Action OnSynchronized;

        public override void OnStartClient()
        {
            if (!IsServerInitialized)
            {
                RequestStatTable(InstanceFinder.ClientManager.Connection);
                Debug.Log("[CLIENT] Statistic Manager: Table requested from SERVER");
            }
        }

        public event Action<StatisticChangedEventArgs> OnStatisticsChanged;

        public static event Action<Player> OnPlayerStatisticUpdated;
        
        
        [ServerRpc(RequireOwnership = false)]
        public void AddPlayerServerRpc(Player player)
        {
            if (!_scoreTable.ContainsKey(player))
                _scoreTable.Add(player, 0);
            OnStatisticsChanged?.Invoke(new StatisticChangedEventArgs(player, 0));
            SyncPlayerObserversRpc(player);
            player.OnDisconnected += OnDisconnected;

            foreach (Player playerInStat in _scoreTable.Keys)
            foreach (var playerToAdd in _scoreTable.Keys)
            {
                playerToAdd.SendPlayerDataToClients(playerInStat.Owner, playerToAdd.SteamID, playerToAdd.PlayerName);
                Debug.Log(
                    $"[CLIENT] Statistic Manager: {player.PlayerName} data has been synced by statics manager from SERVER.");
            }
        }

        [ObserversRpc]
        private void SyncPlayerObserversRpc(Player player)
        {
            if (!_scoreTable.ContainsKey(player))
            {
                _scoreTable.Add(player, 0);
                OnStatisticsChanged?.Invoke(new StatisticChangedEventArgs(player, 0));
                Debug.Log($"[CLIENT] Statistic Manager: {player.PlayerName} has added to statics manager.");
            }
        }
        
        
        private void HandleDollScore(TakeScoreEventArgs eventArgs)
        {
            AddScore(eventArgs.Taker, eventArgs.Score);
        }

        private void OnDisconnected(Player disconnectedPlayer)
        {
            _scoreTable.Remove(disconnectedPlayer);
        }

        [ServerRpc(RequireOwnership = false)]
        private void AddScore(Player taker, int amount)
        {
            Debug.Log(taker);
            var playerScore = _scoreTable[taker];
            playerScore+= amount;
            _scoreTable[taker] = playerScore;
            SyncScore(taker, _scoreTable[taker]);
        }

        [ServerRpc(RequireOwnership = false)]
        private void RequestStatTable(NetworkConnection myConnection)
        {
            SyncScoreTable(myConnection, _scoreTable);

            string targetName = "";

            foreach (var player in _scoreTable.Keys.Where(player => player.Owner == Owner))
                targetName = player.PlayerName;

            Debug.Log($"[SERVER] Statistic Manager: Send table to {targetName}");
        }

        [TargetRpc]
        private void SyncScoreTable(NetworkConnection target, Dictionary<Player, int> currentScoreTable)
        {
            _scoreTable = currentScoreTable;
            OnSynchronized?.Invoke();
            Debug.Log("[CLIENT] Statistic Manager: Sync table with server");
        }
        
        
        [ObserversRpc]
        private void SyncScore(Player player, int playerScore)
        {
            _scoreTable[player] = playerScore;
            Debug.Log(_scoreTable[player]);
            OnPlayerStatisticUpdated?.Invoke(player);
            Debug.Log($"[CLIENT] Statistics Manager: {player.PlayerName} statistics synced.");
            //PrintStatisticsToDebug();
            OnStatisticsChanged?.Invoke(new StatisticChangedEventArgs(player, _scoreTable[player]));
        }

        public int GetPlayerKills(Player player)
        {
            return _statTable[player].Kills;
        }

        public Dictionary<Player, Statistic> GetPlayerStatistics()
        {
            return _statTable;
        }
        
        public Dictionary<Player, int> GetPlayerScores()
        {
            return _scoreTable;
        }
        
    }
}