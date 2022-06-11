using FishNet.Managing;
using FishNet.Transporting;
using UnityEngine;
using UnityEngine.UIElements;

public class MultiplayerConnectionUi : MonoBehaviour
{
    //UI Document Object.
    public UIDocument doc;
    //The very first visual element object, also known as the 'root'.
    private VisualElement root;
    //The blue UI button.
    private Button serverButton;
    private Button clientButton;
    private NetworkManager networkManager;
    private LocalConnectionStates serverState = LocalConnectionStates.Stopped;
    private LocalConnectionStates clientState = LocalConnectionStates.Stopped;
    
    void Start()
    {
        root = doc.rootVisualElement;
        networkManager = FindObjectOfType<NetworkManager>();
        
        serverButton = root.Q<Button>("server-button");
        serverButton.style.backgroundColor = Color.gray;
        serverButton.clicked += ServerButtonEngaged;

        clientButton = root.Q<Button>("client-button");
        clientButton.style.backgroundColor = Color.gray;
        clientButton.clicked += ClientButtonEngaged;

        networkManager.ServerManager.OnServerConnectionState += OnServerConnectionState;
        networkManager.ClientManager.OnClientConnectionState += OnClientConnectionState;
    }

    private void OnDestroy()
    {
        serverButton.clicked -= ServerButtonEngaged;
        clientButton.clicked -= ClientButtonEngaged;
        
        networkManager.ServerManager.OnServerConnectionState -= OnServerConnectionState;
        networkManager.ClientManager.OnClientConnectionState -= OnClientConnectionState;
    }

    void ServerButtonEngaged()
    {
        if (networkManager == null)
            return;

        if (serverState != LocalConnectionStates.Stopped)
            networkManager.ServerManager.StopConnection(true);
        else
            networkManager.ServerManager.StartConnection();
    }

    void ClientButtonEngaged()
    {
        if (networkManager == null)
            return;

        if (clientState != LocalConnectionStates.Stopped)
            networkManager.ClientManager.StopConnection();
        else
            networkManager.ClientManager.StartConnection();
    }

    void OnServerConnectionState(ServerConnectionStateArgs obj)
    {
        serverState = obj.ConnectionState;
        UpdateColor(serverState, ref serverButton);
    }

    void OnClientConnectionState(ClientConnectionStateArgs obj)
    {
        clientState = obj.ConnectionState;
        UpdateColor(clientState, ref clientButton);
    }

    void UpdateColor(LocalConnectionStates state, ref Button button)
    {
        button.style.backgroundColor = state switch
        {
            LocalConnectionStates.Started => Color.green,
            LocalConnectionStates.Starting => Color.yellow,
            LocalConnectionStates.Stopping => Color.yellow,
            LocalConnectionStates.Stopped => Color.gray,
            _ => Color.gray
        };
    }
}