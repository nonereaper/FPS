using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using Unity.Netcode;
using TMPro;
using UnityEngine.UI;
using UnityEngine;

public class MultLobbyController : MonoBehaviour
{
    private NetworkManager networkManager;
    private Unity.Netcode.Transports.UTP.UnityTransport utpTransport;
    [SerializeField] TMP_Text infoText, runText, lobbyClients;
    [SerializeField] GameObject IPAddressInput, startButton, selectorDropdown, backButton;

    // https://learn.unity.com/tutorial/working-with-textmesh-pro#
    // Start is called before the first frame update
    private int stateOfMult;
    
    void Start()
    {
        networkManager = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();
        utpTransport = (Unity.Netcode.Transports.UTP.UnityTransport)networkManager.NetworkConfig.NetworkTransport;
        stateOfMult = 0;
        startButton.SetActive(false);
        IPAddressInput.SetActive(false);
        backButton.SetActive(false);
        // https://learn.microsoft.com/en-us/dotnet/api/system.net.ipaddress?view=net-6.0
        // 
        utpTransport.ConnectionData.Address = getThisComputerAddress();
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
    public void setStateHost() {
        IPAddressInput.SetActive(true);
        startButton.SetActive(true);
        runText.text = "Host game";
        IPAddressInput.GetComponent<InputField>().text = utpTransport.ConnectionData.Address;
    }
    public void setStateClient() {
        IPAddressInput.SetActive(true);
        startButton.SetActive(true);
        runText.text = "Join game";
        IPAddressInput.GetComponent<InputField>().text = utpTransport.ConnectionData.ServerListenAddress;
    }
    public void setStateNone() {
        IPAddressInput.SetActive(false);
        startButton.SetActive(false);
    }
    public void doDropdown() {
        int value = selectorDropdown.GetComponent<TMP_Dropdown>().value;
        if (value == 0)
        setStateNone();
        else if (value == 1)
        setStateHost();
        else
        setStateClient();
    }
    public void run() {
        stateOfMult = 1;
        IPAddressInput.SetActive(false);
        startButton.SetActive(false);
        backButton.SetActive(true);
        selectorDropdown.SetActive(false);
        infoText.text = "Connecting";
        int value = selectorDropdown.GetComponent<TMP_Dropdown>().value;
        if (value == 1) {
            networkManager.StartHost();
            infoText.text = "";
        } else if (value == 2) {
            networkManager.StartClient();
            Debug.Log();
        }
    }
    // Update is called once per frame
    void Update()
    {   
        if (stateOfMult == 1) {
            if (networkManager.IsConnectedClient) {
                stateOfMult = 2;
                infoText.text = "";
            }
        }
        else if (stateOfMult == 2) {

        }
    }
    public void setAddress() {
        InputField inputField = IPAddressInput.GetComponent<InputField>();
        int value = selectorDropdown.GetComponent<TMP_Dropdown>().value;
        if (value == 1) {
            utpTransport.ConnectionData.Address = inputField.text;
        } else if (value == 2) {
           utpTransport.ConnectionData.ServerListenAddress = inputField.text; 
        }
    }
    public void escapeLobby() {
        if (networkManager.IsServer) {
            IReadOnlyList<NetworkClient> list = NetworkManager.Singleton.ConnectedClientsList;
            for (int i = 0; i < list.Count; i++) {
                networkManager.DisconnectClient(list[i].ClientId);
            }
        }
        networkManager.Shutdown();
        backButton.SetActive(false);
        selectorDropdown.SetActive(true);
        stateOfMult = 0;
        infoText.text = "";
        lobbyClients.text = "";
        doDropdown();
    }
}
