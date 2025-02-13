using System.Collections.Generic;
using GameMode;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WinScreenManager : SingletonMonoBehavior<WinScreenManager>
{
    [SerializeField] private GameObject winnerPanel;
    [SerializeField] private GameObject statisticsPanel;
    [SerializeField] private TextMeshProUGUI winnerNameTMP;
    [SerializeField] private TextMeshProUGUI killDeathTMP;
    [SerializeField] private EntriesSpawner entriesSpawner;
    [SerializeField] private Button exitButton;

    private void Start()
    {
        exitButton.onClick.AddListener(() =>
        {
            SceneManagerService.LoadSceneAdditive("Menu");
            SceneManagerService.UnloadScene("WinScreen");
        });
    }

    public void SetWinnerName(string winnerName)
    {
        winnerNameTMP.text = winnerName;
    }

    public void SetKda(int kills, int deaths)
    {
        killDeathTMP.text = $"K:{kills} D:{deaths}";
    }

    public void OpenWinnerPanel()
    {
        winnerPanel.SetActive(true);
    }

    public void OpenStatistics()
    {
        winnerPanel.SetActive(false);
        statisticsPanel.SetActive(true);
    }

    public void CreateStatTable(Dictionary<Player, Statistic> statisticsPairs)
    {
        foreach (var statisticPair in statisticsPairs)
            entriesSpawner.AddNewEntry(statisticPair.Key, statisticPair.Value);
    }
}