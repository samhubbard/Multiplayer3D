using System.Net;
using System.Threading.Tasks;
using FishNet;
using FishNet.Discovery;
using FishNet.Managing;
using FishNet.Object;
using FishNet.Transporting;
using UnityEngine;

namespace FarmGame3D
{
    public class NetworkSetup : MonoBehaviour
    {
        [Header("Start Client On Server Start")]
        [SerializeField] private bool AutoStartClientOnServer;
        
        [Header("Server Searching Fields")]
        [SerializeField] private int MaximumServerSearchAttempts = 25;
        [SerializeField] [Tooltip("In milliseconds")] private int PollRate = 1000;

        private NetworkManager networkManager; 
        private NetworkDiscovery networkDiscovery;
        private LocalConnectionStates clientState = LocalConnectionStates.Stopped;
        private LocalConnectionStates serverState = LocalConnectionStates.Stopped;
        private bool serverFound;

        private void Awake()
        {
            networkManager = GetComponent<NetworkManager>();
            if (networkManager == null)
            {
                Debug.LogError("Network Manager not found, aborting...");
                return;
            }
            
            networkDiscovery = GetComponent<NetworkDiscovery>();
            if (networkDiscovery == null)
            {
                Debug.LogError("Network Discovery not found, aborting...");
                return;
            }
            
            networkManager.ServerManager.OnServerConnectionState += ServerManagerOnOnServerConnectionState;
            networkManager.ClientManager.OnClientConnectionState += ClientManagerOnOnClientConnectionState;
            networkDiscovery.ServerFoundCallback += NetworkDiscoveryOnServerFoundCallback;
        }

        private void OnDestroy()
        {
            networkManager.ServerManager.OnServerConnectionState -= ServerManagerOnOnServerConnectionState;
            networkManager.ClientManager.OnClientConnectionState -= ClientManagerOnOnClientConnectionState;
            networkDiscovery.ServerFoundCallback -= NetworkDiscoveryOnServerFoundCallback;
        }

        public void RequestServerStart()
        {
            switch (serverState)
            {
                case LocalConnectionStates.Stopped:
                    networkManager.ServerManager.StartConnection();
                    break;
                case LocalConnectionStates.Started:
                    networkManager.ServerManager.StopConnection(true);
                    break;
            }
        }

        private void RequestServerClientStart()
        {
            if (AutoStartClientOnServer)
                networkManager.ClientManager.StartConnection();
        }

        public void RequestClientStart()
        {
            switch (clientState)
            {
                case LocalConnectionStates.Stopped:
                    if (networkManager.IsServer)
                        networkManager.ClientManager.StartConnection();
                    else
                        Task.Run(LocateServer);
                    break;
                case LocalConnectionStates.Started:
                    networkManager.ClientManager.StopConnection();
                    break;
            }
        }

        private async void LocateServer()
        {
            serverFound = false;
            int currentAttempts = 0;

            while (!serverFound && currentAttempts <= MaximumServerSearchAttempts)
            {
                networkDiscovery.StartSearchingForServers();
                await Task.Delay(PollRate);
                networkDiscovery.StopSearchingForServers();
                currentAttempts++;
            }

            if (currentAttempts > MaximumServerSearchAttempts)
            {
                Debug.LogWarning("No server found.");
            }
        }
        
        private void ServerManagerOnOnServerConnectionState(ServerConnectionStateArgs obj)
        {
            if (obj.ConnectionState == LocalConnectionStates.Started)
            {
                serverState = LocalConnectionStates.Started;
                networkDiscovery.StartAdvertisingServer();
                RequestServerClientStart();
            }

            if (obj.ConnectionState == LocalConnectionStates.Stopped)
            {
                serverState = LocalConnectionStates.Stopped;
            }
        }
        
        private void ClientManagerOnOnClientConnectionState(ClientConnectionStateArgs obj)
        {
            if (obj.ConnectionState == LocalConnectionStates.Started)
            {
                clientState = LocalConnectionStates.Started;
            }

            if (obj.ConnectionState == LocalConnectionStates.Stopped)
            {
                clientState = LocalConnectionStates.Stopped;
            }
        }
        
        private void NetworkDiscoveryOnServerFoundCallback(IPEndPoint obj)
        {
            serverFound = true;
            string serverAddress = obj.Address.ToString();
            networkManager.ClientManager.StartConnection(serverAddress);
        }
    }
}