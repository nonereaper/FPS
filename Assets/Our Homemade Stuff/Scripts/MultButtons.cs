using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using Unity.Netcode;
using TMPro;
using UnityEngine.UI;
using UnityEngine;

public class MultButtons : MonoBehaviour
{
    private NetworkManager networkManager;
    private Unity.Netcode.Transports.UTP.UnityTransport utpTransport;
    [SerializeField] TMP_Text infoText, runText, lobbyText;
    [SerializeField] GameObject inputFieldGameObject, startButton;

    // https://learn.unity.com/tutorial/working-with-textmesh-pro#
    [SerializeField] GameObject findLobby, inLobby;
    // Start is called before the first frame update
    private int stateOfMult;
    private bool foundClient;
    
    void Start()
    {
        networkManager = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();
        utpTransport = (Unity.Netcode.Transports.UTP.UnityTransport)networkManager.NetworkConfig.NetworkTransport;
        foundClient = false;
        stateOfMult = 0;
        startButton.SetActive(false);
        inputFieldGameObject.SetActive(false);
        // https://learn.microsoft.com/en-us/dotnet/api/system.net.ipaddress?view=net-6.0
        // 
        utpTransport.ConnectionData.Address = getThisComputerAddress();
        runText.text = "";
        infoText.text = "";
    }
    // https://www.codegrepper.com/code-examples/csharp/how+to+get+your+ipv4+address+in+C%23
    private string getThisComputerAddress() {
        IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
        IPAddress[] allAddress = host.AddressList;
            for (int i = 0; i < allAddress.Length; i++) {
                if (allAddress[i].AddressFamily == AddressFamily.InterNetwork) {
                    return allAddress[i].ToString();
                }
            }
        return "";
    }
    // Update is called once per frame
    void Update()
    {       
        if (inLobby.activeInHierarchy && networkManager.IsHost) {
            lobbyText.text = "Host: " + networkManager.ConnectedHostname + "\n";
            IReadOnlyList<NetworkClient> l = networkManager.ConnectedClientsList;
            for (int i = 1; i < l.Count; i++) {
                lobbyText.text += "Client: " + l[i].ClientId;
                if (i+1 < l.Count) {
                    lobbyText.text += "\n";
                }
            }
        }
    }
    public void runHost() {
        if (stateOfMult == 1) {
        stateOfMult = 0;
        infoText.text = "";
        inputFieldGameObject.SetActive(false);
        startButton.SetActive(false);
        } else  {
        stateOfMult = 1;
        inputFieldGameObject.SetActive(true);
        startButton.SetActive(true);
        infoText.text = "Running as host";
        runText.text = "Host game";
        inputFieldGameObject.GetComponent<InputField>().text = utpTransport.ConnectionData.Address;
        }
    }
    public void runClient() {
        if (stateOfMult == 2) {
        stateOfMult = 0;
        infoText.text = "";
        inputFieldGameObject.SetActive(false);
        startButton.SetActive(false);
        } else  {
        stateOfMult = 2;
        inputFieldGameObject.SetActive(true);
        startButton.SetActive(true);
        infoText.text = "Running as client";
        runText.text = "Connect to game";
        inputFieldGameObject.GetComponent<InputField>().text = utpTransport.ConnectionData.ServerListenAddress;
        }
    }
    public void setAddress() {
        InputField inputField = inputFieldGameObject.GetComponent<InputField>();
        if (stateOfMult == 1) {
            utpTransport.ConnectionData.Address = inputField.text;
        } else {
           utpTransport.ConnectionData.ServerListenAddress = inputField.text; 
        }
    }
    public void runMult() {
        if (stateOfMult == 1) {
            networkManager.StartHost();
            findLobby.SetActive(false);
        } else {
            networkManager.StartClient();
            if (!networkManager.IsConnectedClient) {
                infoText.text = "Failed to connect";
            } else {
                findLobby.SetActive(false);
                inLobby.SetActive(true);
            }
        }

    }
    public void escapeLobby() {
        networkManager.Shutdown();
        stateOfMult = 0;
        startButton.SetActive(false);
        inputFieldGameObject.SetActive(false);
        infoText.text = "";
        runText.text = "";
        findLobby.SetActive(true);
        inLobby.SetActive(false);
    }
}
