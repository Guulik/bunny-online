using System;
using System.Collections.Generic;
using System.Linq;
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

        private void OnEnable()
        {
            DollHealth.OnDeath += HandleDollDeath;
        }

        private void OnDisable()
        {
            DollHealth.OnDeath -= HandleDollDeath;
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
            if (!_statTable.ContainsKey(player))
                _statTable.Add(player, new Statistic());
            OnStatisticsChanged?.Invoke(new StatisticChangedEventArgs(player, new Statistic()));
            SyncPlayerObserversRpc(player);
            player.OnDisconnected += OnDisconnected;

            foreach (Player playerInStat in _statTable.Keys)
            foreach (var playerToAdd in _statTable.Keys)
            {
                playerToAdd.SendPlayerDataToClients(playerInStat.Owner, playerToAdd.SteamID, playerToAdd.PlayerName);
                Debug.Log(
                    $"[CLIENT] Statistic Manager: {player.PlayerName} data has been synced by statics manager from SERVER.");
            }
        }

        [ObserversRpc]
        private void SyncPlayerObserversRpc(Player player)
        {
            if (!_statTable.ContainsKey(player))
            {
                _statTable.Add(player, new Statistic());
                OnStatisticsChanged?.Invoke(new StatisticChangedEventArgs(player, new Statistic()));
                Debug.Log($"[CLIENT] Statistic Manager: {player.PlayerName} has added to statics manager.");
            }
        }

        private void HandleDollDeath(DollDeathEventArgs eventArgs)
        {
            AddKills(eventArgs.Killer);
            AddDeath(eventArgs.Victim);
        }

        private void OnDisconnected(Player disconnectedPlayer)
        {
            _statTable.Remove(disconnectedPlayer);
        }

        [ServerRpc(RequireOwnership = false)]
        private void AddKills(Player killer)
        {
            var playerStat = _statTable[killer];
            playerStat.Kills++;
            _statTable[killer] = playerStat;
            SyncStatistics(killer, playerStat);
        }

        [ServerRpc(RequireOwnership = false)]
        private void AddDeath(Player victim)
        {
            var playerStat = _statTable[victim];
            playerStat.Deaths++;
            _statTable[victim] = playerStat;
            SyncStatistics(victim, playerStat);
        }

        [ServerRpc(RequireOwnership = false)]
        private void RequestStatTable(NetworkConnection myConnection)
        {
            SyncStatTable(myConnection, _statTable);

            string targetName = "";

            foreach (var player in _statTable.Keys.Where(player => player.Owner == Owner))
                targetName = player.PlayerName;

            Debug.Log($"[SERVER] Statistic Manager: Send table to {targetName}");
        }

        [TargetRpc]
        private void SyncStatTable(NetworkConnection target, Dictionary<Player, Statistic> currentStatTable)
        {
            _statTable = currentStatTable;
            OnSynchronized?.Invoke();
            Debug.Log("[CLIENT] Statistic Manager: Sync table with server");
        }


        [ObserversRpc]
        private void SyncStatistics(Player player, Statistic playerStatistic)
        {
            _statTable[player] = new Statistic { Kills = playerStatistic.Kills, Deaths = playerStatistic.Deaths };
            OnPlayerStatisticUpdated?.Invoke(player);
            Debug.Log($"[CLIENT] Statistics Manager: {player.PlayerName} statistics synced.");
            PrintStatisticsToDebug();
            OnStatisticsChanged?.Invoke(new StatisticChangedEventArgs(player, _statTable[player]));
        }

        private void PrintStatisticsToDebug()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Player\t\tKills\tDeaths");

            foreach (var entry in _statTable)
            {
                string playerName = entry.Key.PlayerName; // или другой способ получить имя игрока
                int kills = entry.Value.Kills;
                int deaths = entry.Value.Deaths;

                sb.AppendLine($"{playerName}\t\t{kills}\t{deaths}");
            }

            Debug.Log(sb.ToString());
        }

        public Player GetTopKiller()
        {
            return _statTable.Aggregate((x, y) => x.Value.Kills > y.Value.Kills ? x : y).Key;
        }

        public int GetPlayerKills(Player player)
        {
            return _statTable[player].Kills;
        }

        public Dictionary<Player, Statistic> GetPlayerStatistics()
        {
            return _statTable;
        }
    }
}