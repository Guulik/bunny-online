using Steamworks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUIManager : MonoBehaviour
{
    [SerializeField] private GameObject hostButton;
    [SerializeField] private GameObject startGameButton;
    [SerializeField] private TextMeshProUGUI lobbyNameText;
    [SerializeField] private GameObject leaveButton;

    [SerializeField] private GameObject lobbyMemberList;
    [SerializeField] private GameObject lobbyMemberPrefab;

    private void Awake()
    {
        Callback<LobbyEnter_t>.Create(SetupUI);

        hostButton.GetComponent<Button>().onClick.AddListener(LobbyManager.HostLobby);
        startGameButton.GetComponent<Button>().onClick.AddListener(LobbyManager.StartGame);
        leaveButton.GetComponent<Button>().onClick.AddListener(LeaveLobby);
    }

    private void OnEnable()
    {
        LobbyManager.OnLobbyListChanged += UpdateLobbyMembersList;
    }

    private void OnDisable()
    {
        LobbyManager.OnLobbyListChanged -= UpdateLobbyMembersList;
    }

    private void UpdateLobbyMembersList(ulong lobbyID)
    {
        ClearLobbyMemberList();

        AddMembersToLobbyList(lobbyID);
    }

    private void AddMembersToLobbyList(ulong lobbyID)
    {
        int memberCount = SteamMatchmaking.GetNumLobbyMembers(new CSteamID(lobbyID));
        for (int i = 0; i < memberCount; i++)
        {
            CSteamID memberSteamID = SteamMatchmaking.GetLobbyMemberByIndex(new CSteamID(lobbyID), i);
            CreateLobbyMemberObject(memberSteamID);
        }
    }

    private void CreateLobbyMemberObject(CSteamID steamID)
    {
        GameObject newMemberObject = Instantiate(lobbyMemberPrefab, lobbyMemberList.transform);
        LobbyMember newMember = newMemberObject.GetComponent<LobbyMember>();

        newMember.Initialize(steamID);
    }

    private void ClearLobbyMemberList()
    {
        int lobbyMemberObjectsCount = lobbyMemberList.transform.childCount;

        for (int currentLobbyMember = 0; currentLobbyMember < lobbyMemberObjectsCount; currentLobbyMember++)
            Destroy(lobbyMemberList.transform.GetChild(currentLobbyMember).gameObject);
    }

    private void LeaveLobby()
    {
        LobbyManager.LeaveLobby();
        hostButton.SetActive(true);
        startGameButton.SetActive(false);
        lobbyNameText.gameObject.SetActive(false);
        leaveButton.SetActive(false);
        ClearLobbyMemberList();
    }

    private void SetupUI(LobbyEnter_t callback)
    {
        hostButton.SetActive(false);
        leaveButton.SetActive(true);
        if (LobbyManager.IsHost()) startGameButton.SetActive(true);

        lobbyNameText.gameObject.SetActive(true);
        lobbyNameText.text = SteamMatchmaking.GetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), "name");

        Debug.Log("Lobby Manager: Lobby name got successfully");
    }
}