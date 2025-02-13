using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FishNet.Object;
using GameMode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FFAManager : NetworkBehaviour
{
    [SerializeField] private Objective[] objectives;

    private bool _isGameEnded;

    private void Awake()
    {
        if (!IsServerInitialized) enabled = false;
        foreach (var objective in objectives) objective.OnComplete += HandleObjectiveComplete;
    }

    public static event Action OnGameEnded;


    private void HandleObjectiveComplete()
    {
        if (_isGameEnded) return;
        _isGameEnded = true;
        StartEndGame(StatisticManager.Instance.GetTopKiller(), StatisticManager.Instance.GetPlayerStatistics());
    }

    [ObserversRpc]
    private void StartEndGame(Player winner, Dictionary<Player, Statistic> statistics)
    {
        EndGame(winner, statistics);
    }

    private async void EndGame(Player winner, Dictionary<Player, Statistic> statistics)
    {
        OnGameEnded?.Invoke();

        AsyncOperation asyncLoad =
            UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("WinScreen", LoadSceneMode.Additive);

        // Ждём, пока сцена загрузится
        while (asyncLoad is { isDone: false }) await Task.Yield();

        WinScreenManager.Instance.SetWinnerName(winner.PlayerName);
        WinScreenManager.Instance.SetKda(statistics[winner].Kills, statistics[winner].Deaths);
        WinScreenManager.Instance.CreateStatTable(statistics);

        LobbyManager.LeaveLobby();
        ConnectionService.Instance.StopConnection();

        // TODO: add current play scene name to smthManager
        SceneManagerService.UnloadScene("TestScene");

        WinScreenManager.Instance.OpenWinnerPanel();
    }
}