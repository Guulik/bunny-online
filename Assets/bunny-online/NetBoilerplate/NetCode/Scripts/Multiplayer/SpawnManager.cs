using Dolls.Health;
using FishNet;
using FishNet.Object;
using UnityEngine;

public class SpawnManager : SingletonNetworkBehavior<SpawnManager>
{
    [SerializeField] private GameObject characterPrefab;
    [SerializeField] private Transform[] spawnPoints;

    private void OnEnable()
    {
        DollHealth.OnDeath += HandleDollDeath;
    }

    private void OnDisable()
    {
        DollHealth.OnDeath -= HandleDollDeath;
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
        if (!IsServerInitialized) Destroy(gameObject);
    }

    [ServerRpc(RequireOwnership = false)]
    public void SpawnDollServerRpc(Player player)
    {
        Transform randomSpawnPoint = GetRandomSpawnPoint();

        GameObject dollObject = Instantiate(characterPrefab, randomSpawnPoint.position, randomSpawnPoint.rotation);
        InstanceFinder.ServerManager.Spawn(dollObject, player.Owner);
        dollObject.name = $"{player.PlayerName}'s Doll";

        // Устанавливаем куклу игроку
        if (dollObject.TryGetComponent(out Doll doll))
        {
            player.SetDollServerRpc(doll);
            //doll.DollAttack.SetPlayerOwner(player);
            //doll.DollHealth.SetPlayerOwner(player);
            doll.DollScore.SetPlayerOwner(player);
        }

        // Обновляем имя куклы у всех клиентов
        SetDollName(dollObject.GetComponent<NetworkObject>(), player.PlayerName);

        Debug.Log($"[SERVER] Doll '{dollObject.name}' has been spawned. Owner — {player.PlayerName}");
    }

    private Transform GetRandomSpawnPoint()
    {
        Transform randomSpawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        return randomSpawnPoint;
    }

    [ObserversRpc]
    private void SetDollName(NetworkObject dollObject, string playerName)
    {
        if (dollObject != null) dollObject.gameObject.name = $"{playerName}'s Doll";
    }

    private void HandleDollDeath(DollDeathEventArgs dollDeathEventArgs)
    {
        Transform randomSpawnPoint = GetRandomSpawnPoint();
        RespawnCharacter(dollDeathEventArgs.Victim, randomSpawnPoint.position);
    }

    [ServerRpc(RequireOwnership = false)]
    private void RespawnCharacter(Player player, Vector3 positionToRespawn)
    {
        if (player.Doll != null)
        {
            player.Doll.transform.position = positionToRespawn;
            //player.Doll.DollHealth.Rejuvenate();
            player.Doll.SyncDollPosition(positionToRespawn);
        }
    }
}