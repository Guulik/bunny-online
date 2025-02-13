using FishNet.Transporting;
using UnityEngine;

public class ConnectionService : SingletonMonoBehavior<ConnectionService>
{
    [SerializeField] private Transport transport;

    public void StartHost()
    {
        transport.StartConnection(true);
        transport.StartConnection(false);
    }

    public void StartConnection(string address)
    {
        transport.SetClientAddress(address);
        transport.StartConnection(false);
    }

    public void StopConnection()
    {
        transport.StopConnection(false);
        transport.StopConnection(true);
    }
}