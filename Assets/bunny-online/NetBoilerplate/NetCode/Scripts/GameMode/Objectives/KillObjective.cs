using GameMode;
using UnityEngine;

/*public class KillObjective : Objective
{
    [SerializeField] private int killTarget;

    private void OnEnable()
    {
        StatisticManager.OnPlayerStatisticUpdated += HandlePlayerStatisticChanged;
    }

    private void OnDisable()
    {
        StatisticManager.OnPlayerStatisticUpdated -= HandlePlayerStatisticChanged;
    }

    private void HandlePlayerStatisticChanged(Player player)
    {
        if (StatisticManager.Instance.GetPlayerKills(player) >= killTarget)
            CompleteObjective();
    }
}*/