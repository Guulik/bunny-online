using GameMode;
using UnityEngine;

public class EntriesSpawner : MonoBehaviour
{
    [SerializeField] private TabEntry entryPrefab;
    [SerializeField] private GameObject entryContainer;


    public void AddNewEntry(Player player, int score)
    {
        var tabEntry = Instantiate(entryPrefab, entryContainer.transform);

        tabEntry.SetPlayerName(player.PlayerName);
        tabEntry.SetScore(score.ToString());
    }
}