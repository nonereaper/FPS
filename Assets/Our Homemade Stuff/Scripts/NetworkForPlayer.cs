using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;
using Unity.Networking.Transport;

public class NetworkForPlayer : NetworkBehaviour
{
    public TMP_Text multInfo, serverInfo;
    // Start is called before the first frame update
    void Start()
    {
        if (NetworkManager.Singleton.IsHost) {
            multInfo.text = "Host";
        } else {
            multInfo.text = "Client";
        }
        serverInfo.text = "";
    }

    // Update is called once per frame
    void Update()
    {
        serverInfo.text = "";
        if (NetworkManager.Singleton.IsHost) {
            IReadOnlyList<NetworkClient> t = NetworkManager.ConnectedClientsList;
            for (int i = 0; i < t.Count; i++) {
                serverInfo.text += t[i].ClientId + ", ";
            }
            //serverInfo 
        } else {
            serverInfo.text = NetworkManager.Singleton.ConnectedHostname;
        }
        var utpTransport = (Unity.Netcode.Transports.UTP.UnityTransport)NetworkManager.Singleton.NetworkConfig.NetworkTransport;

        serverInfo.text += "   " + utpTransport.ConnectionData.Address;
    }
}
