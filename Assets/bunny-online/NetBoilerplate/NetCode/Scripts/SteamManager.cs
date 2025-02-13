using Steamworks;
using UnityEngine;

public class SteamManager : MonoBehaviour
{
    private void OnApplicationQuit()
    {
        SteamAPI.Shutdown();
    }
}
