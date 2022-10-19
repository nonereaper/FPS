using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;

public class m_LobbyPlayer : NetworkBehaviour
{
    [SerializeField] private TMP_Text playerList;
    private string serverAddress;
    private string playerName;

    public override void OnNetworkSpawn() {
        if (IsServer) {
            playerList.text = "";
            serverAddress = ((Unity.Netcode.Transports.UTP.UnityTransport)NetworkManager.NetworkConfig.NetworkTransport).ConnectionData.Address+"";
            playerName = GameObject.Find("LobbyController").GetComponent<LobbyController>().m_getPlayerName();
        }
    }
    [ClientRpc]
    private void updatePlayerListClientRpc(string s) {
        playerList.text = s;
    }
    private string getPlayerName() {
        return playerName;
    }
    // Update is called once per frame
    void Update()
    {
        if (IsServer) {
            IReadOnlyList<NetworkClient> list = NetworkManager.ConnectedClientsList;
            string temp = "Server Address: "+ serverAddress + ".\n";
            for (int i = 0; i < list.Count; i++) {
                m_LobbyPlayer mpc = NetworkManager.Singleton.ConnectedClients[list[i].ClientId].PlayerObject.GetComponent<m_LobbyPlayer>();
                string temp2 = "Client: ";
                if (i == 0)
                    temp2 = "Host: ";
                temp+= temp2 + mpc.getPlayerName() + " " + list[i].ClientId + ".\n";
            }
            
            temp += "Server is listening: " +NetworkManager.IsListening + "\n";
            Dictionary<ulong, PendingClient> list2 = NetworkManager.PendingClients;
            foreach (var thing in list2) {
                temp+= "Incoming: " + thing.Key + ", and is " + thing.Value.ConnectionState + ".\n";
            }
            updatePlayerListClientRpc(temp);
        }
    }
}
