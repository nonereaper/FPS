using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using TMPro;
using UnityEngine.UI;
using UnityEngine;

public class MultButtons : MonoBehaviour
{
    private NetworkManager networkManager;
    [SerializeField] GameObject inputFieldGameObject;

    // https://learn.unity.com/tutorial/working-with-textmesh-pro#
    [SerializeField] GameObject camera, canvas;
    // Start is called before the first frame update

    
    void Start()
    {
        networkManager = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();
    }

    // Update is called once per frame
    void Update()
    {
    }
    public void runHost() {
        networkManager.StartHost();
        Debug.Log("Ran Host");
        camera.SetActive(false);
        canvas.SetActive(false);
    }
    public void runClient() {
        networkManager.StartClient();
        Debug.Log("Ran Client");
        camera.SetActive(false);
        canvas.SetActive(false);
    }
    public void setAddress() {
        InputField inputField = inputFieldGameObject.GetComponent<InputField>();
        
    }
}
