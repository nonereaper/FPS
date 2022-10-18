using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System.Net;
using System.Net.Sockets;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LobbyController : MonoBehaviour
{
    private NetworkManager networkManager;
    private Unity.Netcode.Transports.UTP.UnityTransport utpTransport;

    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject s_menu;
    [SerializeField] private GameObject m_menu;
    [SerializeField] private GameObject s_sceneSelect;
    [SerializeField] private GameObject m_addressInput;
    [SerializeField] private GameObject m_portInput;
    private int s_sceneToLoad;
    [SerializeField] private string[] s_sceneNames;
    // Start is called before the first frame update
    void Start()
    {
        networkManager = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();
        utpTransport = (Unity.Netcode.Transports.UTP.UnityTransport)networkManager.NetworkConfig.NetworkTransport;
        
        utpTransport.ConnectionData.Address = getThisComputerAddress();
        m_addressInput.GetComponent<InputField>().text = utpTransport.ConnectionData.Address;
        m_portInput.GetComponent<InputField>().text = utpTransport.ConnectionData.Port;

        s_sceneToLoad = 0;
        List<TMP_Dropdown.OptionData> s_scenesTemp = new List<TMP_Dropdown.OptionData>();
        for (int i = 0; i < s_sceneNames.Length; i++) {
            s_scenesTemp.Add(new TMP_Dropdown.OptionData(s_sceneNames[i]));
        }
        s_sceneSelect.GetComponent<TMP_Dropdown>().options = s_scenesTemp;
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
    public void s_openSingleplayer() {
        mainMenu.SetActive(false);
        s_menu.SetActive(true);
    }
    public void m_openMultiplayer() {
        mainMenu.SetActive(false);
        m_menu.SetActive(true);
    }
    public void backToMainScreen() {
        mainMenu.SetActive(true);
        s_menu.SetActive(false);
        m_menu.SetActive(false);
    }
    public void s_selectScene() {
        s_sceneToLoad = s_sceneSelect.GetComponent<TMP_Dropdown>().value;
    }
    public void s_loadScene() {
        SceneManager.LoadScene(s_sceneNames[s_sceneToLoad]);
    }
    public void m_setAddress() {
        utpTransport.ConnectionData.Address = m_addressInput.GetComponent<InputField>().text;
    }
    public void m_setPort() {
        utpTransport.ConnectionData.Address = m_addressInput.GetComponent<InputField>().text;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}