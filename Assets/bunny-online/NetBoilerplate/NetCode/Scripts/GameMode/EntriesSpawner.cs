using GameMode;
using UnityEngine;

public class EntriesSpawner : MonoBehaviour
{
    [SerializeField] private TabEntry entryPrefab;
    [SerializeField] private GameObject entryContainer;

    /*public void AddNewEntry(Player player, Statistic statistic)
    {
        var tabEntry = Instantiate(entryPrefab, entryContainer.transform);

        tabEntry.SetPlayerName(player.PlayerName);
        //tabEntry.SetKills(statistic.Kills.ToString());
        //tabEntry.SetDeaths(statistic.Deaths.ToString());
        tabEntry.SetDeaths(statistic.Deaths.ToString());
    }*/
    public void AddNewEntry(Player player, int score)
    {
        var tabEntry = Instantiate(entryPrefab, entryContainer.transform);

        tabEntry.SetPlayerName(player.PlayerName);
        //tabEntry.SetKills(statistic.Kills.ToString());
        //tabEntry.SetDeaths(statistic.Deaths.ToString());
        tabEntry.SetScore(score.ToString());
    }
}