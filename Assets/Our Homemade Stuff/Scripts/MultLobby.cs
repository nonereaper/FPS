using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;

public class MultLobby : NetworkBehaviour
{
    private string address;
    private TMP_Text connectedClientListText;

    public override void OnNetworkSpawn() {
        connectedClientListText = GameObject.Find("LobbyClients").GetComponent<TMP_Text>();
        Debug.Log(connectedClientListText != null);
        if (IsServer) {
            address = ((Unity.Netcode.Transports.UTP.UnityTransport)NetworkManager.NetworkConfig.NetworkTransport).ConnectionData.Address+"";
        }
    }
    [ClientRpc]
    public void setListOfClientsClientRpc(string s) {
        connectedClientListText.text = s;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (IsServer) {
            IReadOnlyList<NetworkClient> list = NetworkManager.ConnectedClientsList;
            string temp = "Server Address: "+ address + ".\n";
            for (int i = 0; i < list.Count; i++) {
                string temp2 = "Client: ";
                if (i == 0)
                    temp2 = "Host: ";
                temp+= temp2 + list[i].ClientId + ".\n";
            }
            setListOfClientsClientRpc(temp);
        }
        
    }
}
