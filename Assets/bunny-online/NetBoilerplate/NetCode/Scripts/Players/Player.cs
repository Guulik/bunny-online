using System;
using FishNet.Connection;
using FishNet.Object;
using GameMode;
using Steamworks;
using UnityEngine;

public class Player : NetworkBehaviour
{
    private string _playerName;
    public CSteamID SteamID { get; private set; }
    public Doll Doll { get; private set; }

    public string PlayerName
    {
        get => _playerName == "" ? SteamFriends.GetFriendPersonaName(SteamID) : _playerName;
        private set => _playerName = value;
    }

    public event Action<Player> OnDataUpdated;
    public event Action<Player> OnDisconnected;

    public override void OnStartClient()
    {
        base.OnStartClient();

        if (IsOwner)
        {
            SteamID = GetSteamID();
            PlayerName = GetPlayerName();
            SendPlayerDataToServer(SteamID, PlayerName);
            SpawnManager.Instance.SpawnDollServerRpc(this);
        }
    }

    public override void OnStopClient()
    {
        OnDisconnected?.Invoke(this);
    }

    private CSteamID GetSteamID()
    {
        return !SteamAPI.Init() ? new CSteamID() : SteamUser.GetSteamID();
    }

    private string GetPlayerName()
    {
        return !SteamAPI.Init() ? "Unknown Player" : SteamFriends.GetFriendPersonaName(SteamID);
    }

    [ServerRpc(RequireOwnership = false)]
    private void SendPlayerDataToServer(CSteamID steamID, string playerName)
    {
        SteamID = steamID;
        PlayerName = playerName;

        Debug.Log($"[SERVER] Received data from CLIENT: Steam ID = {SteamID}, Player Name = {PlayerName}");
        StatisticManager.Instance.AddPlayerServerRpc(this);
        //SendPlayerDataToClients(_steamID, PlayerName);
    }

    [TargetRpc]
    public void SendPlayerDataToClients(NetworkConnection connection, CSteamID steamID, string playerName)
    {
        PlayerName = playerName;
        SteamID = steamID;
        Debug.Log($"[CLIENT] Received player name '{playerName}' &  Steam ID {steamID} from SERVER");
        OnDataUpdated?.Invoke(this);
    }

    [ServerRpc(RequireOwnership = false)]
    public void SetDollServerRpc(Doll newDollObject)
    {
        if (newDollObject != null && newDollObject.TryGetComponent(out Doll newDoll))
        {
            Doll = newDoll;
            Debug.Log($"[SERVER] Doll ({Doll.gameObject.name}) assigned to player {PlayerName}.");
        }
        else
        {
            Debug.LogWarning("[SERVER] Attempted to assign a null or invalid doll object.");
        }
    }
}