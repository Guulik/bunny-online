using System;
using FishNet;
using HeathenEngineering.SteamworksIntegration;
using Steamworks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyManager : SingletonMonoBehavior<LobbyManager>
{
    private const string HOST_ADDRESS_KEY = "HostAddress";

    private const int LOBBY_MAX_PLAYERS = 8;
    private static ulong _currentLobbyID;

    private void Start()
    {
        Callback<LobbyCreated_t>.Create(OnLobbyCreated);
        Callback<LobbyEnter_t>.Create(OnLobbyEntered);
        Callback<LobbyDataUpdate_t>.Create(OnDataUpdated);
        Callback<LobbyChatUpdate_t>.Create(OnLobbyChatUpdated);
        Callback<GameLobbyJoinRequested_t>.Create(OnJoinRequest);
    }

    public static event Action<ulong> OnLobbyListChanged;

    public static void HostLobby()
    {
        SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypeFriendsOnly, LOBBY_MAX_PLAYERS);
    }

    private void OnLobbyCreated(LobbyCreated_t callback)
    {
        if (callback.m_eResult != EResult.k_EResultOK) return;

        Debug.Log("[CLIENT] Lobby Manager: Lobby created successfully");

        _currentLobbyID = callback.m_ulSteamIDLobby;
        SteamMatchmaking.SetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), "name",
            UserData.Get().Nickname + "'s lobby");
        Debug.Log($"[CLIENT] Lobby Manager: Lobby name ({UserData.Get().Nickname}) settled successfully");

        OnLobbyListChanged?.Invoke(_currentLobbyID);
    }

    private void OnLobbyEntered(LobbyEnter_t callback)
    {
        if (_currentLobbyID == callback.m_ulSteamIDLobby) return;

        SteamMatchmaking.JoinLobby(new CSteamID(callback.m_ulSteamIDLobby));

        Debug.Log("Lobby Manager: Lobby entered successfully");

        _currentLobbyID = callback.m_ulSteamIDLobby;

        OnLobbyListChanged?.Invoke(_currentLobbyID);
    }


    private void OnDataUpdated(LobbyDataUpdate_t callback)
    {
        if (InstanceFinder.IsClientStarted) return;

        string gameStarted = SteamMatchmaking.GetLobbyData((CSteamID)callback.m_ulSteamIDLobby, "GameStarted");

        if (gameStarted == "true")
        {
            Debug.Log("Lobby Manager: client connecting to host");
            ConnectionService.Instance.StartConnection(SteamMatchmaking.GetLobbyData(
                new CSteamID(callback.m_ulSteamIDLobby),
                HOST_ADDRESS_KEY));
            SceneManager.UnloadSceneAsync("Menu");
        }
    }

    private void OnLobbyChatUpdated(LobbyChatUpdate_t callback)
    {
        Debug.Log("Lobby Manager: smb connected/disconnected");

        OnLobbyListChanged?.Invoke(_currentLobbyID);
    }

    private void OnJoinRequest(GameLobbyJoinRequested_t callback)
    {
        Debug.Log("Lobby Manager: requested to join lobby");
        SteamMatchmaking.JoinLobby(callback.m_steamIDLobby);
    }

    public static bool IsHost()
    {
        return SteamMatchmaking.GetLobbyOwner(new CSteamID(_currentLobbyID)) ==
               SteamUser.GetSteamID();
    }

    public static void LeaveLobby()
    {
        SteamMatchmaking.LeaveLobby(new CSteamID(_currentLobbyID));
    }

    public static void StartGame()
    {
        ConnectionService.Instance.StartHost();
        SteamMatchmaking.SetLobbyData(new CSteamID(_currentLobbyID), HOST_ADDRESS_KEY, UserData.Get().id.ToString());
        Debug.Log($"Lobby Manager: Lobby HostAddressKey ({UserData.Get().id.ToString()}) settled successfully");
        SteamMatchmaking.SetLobbyData(new CSteamID(_currentLobbyID), "GameStarted", "true");
        NetworkSceneManager.Instance.LoadScene("TestScene");
        SceneManager.UnloadSceneAsync("Menu");
    }
}