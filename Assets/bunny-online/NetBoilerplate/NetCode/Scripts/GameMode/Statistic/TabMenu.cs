using System.Collections.Generic;
using System.Linq;
using GameMode;
using UnityEngine;
using UnityEngine.InputSystem;

public class TabMenu : SingletonNetworkBehavior<TabMenu>
{
    [SerializeField] private GameObject tabMenu;

    [SerializeField] private StatisticManager statisticManager;
    [SerializeField] private TabEntry entryPrefab;

    [SerializeField] private Color color1 = Color.HSVToRGB(220 / 255f, 22 / 255f, 32 / 255f);
    [SerializeField] private Color color2 = Color.HSVToRGB(0, 0, 32 / 255f);

    [SerializeField] private GameObject entriesParent;

    private readonly Dictionary<Player, TabEntry> _playerEntries = new();

    private PlayerInput _playerInput;

    protected override void Awake()
    {
        base.Awake();
        _playerInput = new PlayerInput();
    }

    private void OnEnable()
    {
        _playerInput.Enable();
        statisticManager.OnStatisticsChanged += SyncStatistics;
        statisticManager.OnSynchronized += Initialize;
        _playerInput.UI.TABMenu.started += OpenTabMenu;
        _playerInput.UI.TABMenu.canceled += CloseTabMenu;
    }

    private void OnDisable()
    {
        _playerInput.Disable();
        statisticManager.OnStatisticsChanged -= SyncStatistics;
        statisticManager.OnSynchronized -= Initialize;
        _playerInput.UI.TABMenu.started -= OpenTabMenu;
        _playerInput.UI.TABMenu.canceled -= CloseTabMenu;
    }

    private void OpenTabMenu(InputAction.CallbackContext context)
    {
        tabMenu.SetActive(true);
    }

    private void CloseTabMenu(InputAction.CallbackContext context)
    {
        tabMenu.SetActive(false);
    }

    private void Initialize()
    {
        if (IsServerInitialized) return;

        Debug.Log("[CLIENT] TAB-MENU: Initialization started");
        var stats = statisticManager.GetPlayerStatistics();
        foreach (var player in stats.Keys.Where(player => !_playerEntries.ContainsKey(player)))
        {
            AddPlayerToTab(player);
            Debug.Log($"[CLIENT] TAB-MENU: Add {player.PlayerName} to TAB-MENU");
        }

        Debug.Log("[CLIENT] TAB-MENU: Initialization ended");
    }

    private void AddPlayerToTab(Player player)
    {
        var tabEntry = Instantiate(entryPrefab, entriesParent.transform);

        tabEntry.SetPlayerName(player.PlayerName);
        tabEntry.SetKills("0");
        tabEntry.SetDeaths("0");

        _playerEntries.Add(player, tabEntry);
        ChangeColorBackground(tabEntry);
        player.OnDataUpdated += UpdateEntryPlayerName;
        player.OnDisconnected += OnPlayerDisconnected;
    }

    private void OnPlayerDisconnected(Player player)
    {
        RemovePlayerEntry(player);
    }

    private void ChangeColorBackground(TabEntry playerRow)
    {
        var rows = _playerEntries.Values.ToList();
        var index = rows.IndexOf(playerRow);
        var backgroundColor = index % 2 == 0 ? color1 : color2;
        playerRow.SetBackgroundColor(backgroundColor);
    }

    private void UpdateEntryPlayerName(Player player)
    {
        _playerEntries[player].SetPlayerName(player.PlayerName);
        player.OnDataUpdated -= UpdateEntryPlayerName;
    }

    private void ClearTabMenu()
    {
        foreach (var kPlayerEntry in _playerEntries.Keys) RemovePlayerEntry(kPlayerEntry);
    }

    private void RemovePlayerEntry(Player player)
    {
        Destroy(_playerEntries[player].gameObject);
        _playerEntries.Remove(player);
    }

    private void SyncStatistics(StatisticChangedEventArgs eventArgs)
    {
        if (!_playerEntries.TryGetValue(eventArgs.Player, out var entry))
        {
            AddPlayerToTab(eventArgs.Player);
            return;
        }

        entry.SetKills(eventArgs.Statistic.Kills.ToString());
        entry.SetDeaths(eventArgs.Statistic.Deaths.ToString());
    }
}