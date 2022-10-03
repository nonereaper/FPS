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
    [SerializeField] TMP_Text infoText, runText;
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
        if (stateOfMult == 3) {
            if (!networkManager.IsConnectedClient) {
                    infoText.text = "Trying to Connecting";
                } else {
                    findLobby.SetActive(false);
                    inLobby.SetActive(true);
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
        if (stateOfMult == 2 || stateOfMult == 3) {
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
        inLobby.SetActive(true);
        if (stateOfMult == 1) {
            findLobby.SetActive(false);
            networkManager.StartHost();
        } else {
            networkManager.StartClient();
            stateOfMult = 3;
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
