using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;

public class MultLobby : NetworkBehaviour
{
    private string connectedClientList;
    private TMP_Text connectedClientListText;

    public override void OnNetworkSpawn() {
        connectedClientListText = GameObject.Find("LobbyClients").GetComponent<TMP_Text>();
        if (IsServer) {
            connectedClientList = "Server Address: " + ((Unity.Netcode.Transports.UTP.UnityTransport)NetworkManager.NetworkConfig.NetworkTransport).ConnectionData.Address +"\nH: " + OwnerClientId + "\n";
            connectedClientListText.text = connectedClientList;
        } else 
        addToConnectedClientListServerRpc();
    }

    [ServerRpc]
    public void addToConnectedClientListServerRpc() {
        connectedClientList += "C: " + OwnerClientId + "\n";
        addToConnectedClientListClientRpc(connectedClientList);
    }
    [ClientRpc]
    public void addToConnectedClientListClientRpc(string s) {
        connectedClientListText.text = s;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    }
}
