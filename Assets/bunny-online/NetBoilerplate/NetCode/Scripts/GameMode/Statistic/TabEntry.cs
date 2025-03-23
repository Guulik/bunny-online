using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TabEntry : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI playerNameTMP;
    //[SerializeField] private TextMeshProUGUI killsTMP;
    //[SerializeField] private TextMeshProUGUI deathsTMP;
    [SerializeField] private TextMeshProUGUI scoreTMP;
    [SerializeField] private Image background;

    public void SetPlayerName(string playerName)
    {
        playerNameTMP.text = playerName == "" ? $"Player {Random.Range(0, 1000)}" : playerName;
    }

    /*public void SetKills(string kills)
    {
        killsTMP.text = kills;
    }

    public void SetDeaths(string deaths)
    {
        deathsTMP.text = deaths;
    }*/
    public void SetScore(string score)
    {
        scoreTMP.text = score;
    }
    
    public void SetBackgroundColor(Color color)
    {
        background.color = color;
    }
}