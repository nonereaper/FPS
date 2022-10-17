using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using Unity.Netcode;
using TMPro;
using UnityEngine.UI;

public class LobbyController : MonoBehaviour
{
    private NetworkManager networkManager;
    private Unity.Netcode.Transports.UTP.UnityTransport utpTransport;

    private GameObject mainScreen = GameObject.Find("Main Screen");
    private GameObject singleplayer = GameObject.Find("S_Scene");
    private GameObject multiplayer = GameObject.Find("M_Scene");
    private int stateOfMenu = 0;
    // Start is called before the first frame update
    void Start()
    {

    }
    private void goToSinglePlayer() {
        mainScreen.SetActive(false);
        singleplayer.SetActive(true);
        stateOfMenu = 1;
    }
    private void goToMultiplayer() {
        mainScreen.SetActive(false);
        multiplayer.SetActive(true);
        stateOfMenu = 2;
    }
    private void goToMainScreen() {
        mainScreen.SetActive(false);
        singleplayer.SetActive(true);
        multiplayer.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
