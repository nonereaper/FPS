using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Controller : MonoBehaviour
{
    private SceneLoader sceneLoader;
    [SerializeField] private bool isMult;
        public bool isIsMult() {
            return this.isMult;
        }
        public void setIsMult(bool isMult) {
            this.isMult = isMult;
        }
    private NetworkManager networkManager;
    [SerializeField] private GameObject playerPrefab;

    private List<GameObject> props; 
    private List<GameObject> weapons;
    private List<GameObject> projectiles;
    private List<GameObject> decay;
    private List<GameObject> zombiePathes;
    private List<GameObject> players;

    private float savedDistance;
    private bool setupPath;
    // Start is called before the first frame update
    void Start()
    {
        //sceneLoader = GameObject.Find("SceneLoader").GetComponent<SceneLoader>();
        //isMult = sceneLoader.isIsMult();
        setupPath = false;
        if (isMult) {
            networkManager = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();
            Destroy(GameObject.Find("Player (Single)"));
            //GameObject.Find("Player (Single)").GetComponent<s_Player>().selfDestroy();

            /*if (networkManager.IsServer) {
                IReadOnlyList<NetworkClient> list = networkManager.ConnectedClientsList;
                for (int i = 0; i < list.Count; i++) {
                    NetworkObject mpc = networkManager.ConnectedClients[list[i].ClientId].PlayerObject;
                    mpc.Despawn();
                    Destroy(mpc.GetComponent<GameObject>());
                    GameObject temp = Instantiate(playerPrefab);
                    temp.GetComponent<NetworkObject>().SpawnAsPlayerObject(list[i].ClientId);
                }  
            }*/
        } else {


        }
            props = new List<GameObject>();
            weapons = new List<GameObject>();
            projectiles = new List<GameObject>();
            decay = new List<GameObject>();
            zombiePathes = new List<GameObject>();
            players = new List<GameObject>();
            for (int i = 0; i < transform.childCount; i++) {
                Transform tempTf = transform.GetChild(i);
                for (int q = 0; q < tempTf.childCount; q++) {
                    if (i == 1) {
                        weapons.Add(tempTf.GetChild(q).gameObject);
                        //if (isMult)
                        //tempTf.GetChild(q).gameObject.GetComponent<NetworkObject>().Spawn();
                    } else if (i == 2) {
                        props.Add(tempTf.GetChild(q).gameObject);
                        //if (isMult)
                        //tempTf.GetChild(q).gameObject.GetComponent<NetworkObject>().Spawn();
                    } else if (i == 4) {
                        zombiePathes.Add(tempTf.GetChild(q).gameObject);
                    } else if (i == 5) {
                        players.Add(tempTf.GetChild(q).gameObject);
                    }
                }
            }
    }
    public GameObject getClosestPlayer(Vector3 p) {
        int index = 0;
        float distance = float.MaxValue;
        for (int i = 0; i < players.Count; i++) {
            float tempDistance = Vector3.Distance(players[i].transform.position,p);
            if (tempDistance < distance) {
                index = i;  
                distance = tempDistance;
            }
        }
        return players[index];
    }
    public int findClosestPath(Vector3 p) {
        int index = -1;
        float distance = float.MaxValue;
        for (int i = 0; i < zombiePathes.Count; i++) {
            float tempDistance = Vector3.Distance(zombiePathes[i].transform.position,p);
            if (tempDistance < distance) {
                index = i;  
                distance = tempDistance;
            }
        }
        return index;
    }
    public int getClosestWeapon(Vector3 p, float rad) {
        float distance = float.MaxValue;
        int index = -1;
        for (int i = 0; i < weapons.Count; i++) {
            float tempDistance = Vector3.Distance(weapons[i].GetComponent<Transform>().position,p);
            if (tempDistance <= rad && tempDistance < distance) {
                index = i;  
                distance = tempDistance;
            }
        }
        savedDistance = distance;
        return index;
    }
    public int getClosestProp(Vector3 p, float rad) {
        float distance = float.MaxValue;
        int index = -1;
        for (int i = 0; i < props.Count; i++) {
            float tempDistance = Vector3.Distance(props[i].GetComponent<Transform>().position,p);
            if (tempDistance <= rad && tempDistance < distance) {
                index = i;  
                distance = tempDistance;
            }
        }
        savedDistance = distance;
        return index;
    }
    public float getSavedDistance() {
		return this.savedDistance;
	}
    public GameObject getWeapon(int index) {
        return weapons[index];
    }
    public GameObject getProp(int index) {
        return props[index];
    }
    public GameObject getPathes(int index) {
        return zombiePathes[index];
    }
    public void removeWeapon(GameObject o) {
        weapons.Remove(o);
    }
    public void addWeapon(GameObject o) {
        weapons.Add(o);
    }
    public void addProjectile(GameObject o) {
        projectiles.Add(o);
    } 
    public void removeProjectile(GameObject o) {
        projectiles.Remove(o);
    }
    public void addDecay(GameObject o) {
        decay.Add(o);
    }
    public void removeDecay(GameObject o) {
        decay.Remove(o);
    }
    public Transform getWeaponTf() {
        return transform.GetChild(1);
    }
    public Transform getProjectileTf() {
        return transform.GetChild(0);
    }
    public Transform getDecayTf() {
        return transform.GetChild(3);
    }
    [ServerRpc]
    public void spawnPlayerServerRpc(ulong clientId) {
        if (!networkManager.IsServer) return;
        NetworkObject mpc = networkManager.ConnectedClients[clientId].PlayerObject;
                    mpc.Despawn();
        GameObject temp = Instantiate(playerPrefab);
        temp.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId);
    }
    // Update is called once per frame
    void Update()
    {
        /*if (!networkManager.IsServer) return;
        IReadOnlyList<NetworkClient> list = networkManager.ConnectedClientsList;
        for (int i = 0; i < list.Count; i++) {
            NetworkManager t = networkManager.ConnectedClients[list[i].ClientId].PlayerObject.NetworkManager;
            
        }*/
        if (!setupPath && ZombiePathes.getAllID() == zombiePathes.Count) {
            for (int i = 0; i < zombiePathes.Count; i++) {
                zombiePathes[i].GetComponent<ZombiePathes>().setup();
            }
            setupPath = true;
        }
    }
}
