using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;



public class SceneLoader : MonoBehaviour
{
    private NetworkManager networkManager;
    private bool isMult;
    // Start is called before the first frame update
    void Start()
    {
        networkManager = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();
        DontDestroyOnLoad(transform.gameObject);
    }
    public void m_loadScene(string[] names, int i) {
        isMult = true;
        networkManager.SceneManager.LoadScene(names[i],LoadSceneMode.Single);
    }
    public void s_loadScene(string[] names, int i) {
        isMult = false;
        SceneManager.LoadScene(names[i]);
    }
    public void s_reloadScene() {
         SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public bool isIsMult() {
        return isMult;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
