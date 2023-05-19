using System.Collections;
using System;
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
    private List<GameObject> zombies;
    private List<GameObject> zombieSpawners;
    private List<GameObject> stores;

    private float savedDistance;
    private bool setupPath;

    private int zombiesToSpawnPerRound;
    private int zombieToSpawnLeft;
    private float timeForEachSpawnerToSpawn;
    private int round;

    [SerializeField] private GameObject zombiePrefab;
    // Start is called before the first frame update
    void Start()
    {
        //sceneLoader = GameObject.Find("SceneLoader").GetComponent<SceneLoader>();
        //isMult = sceneLoader.isIsMult();'
        
        setupPath = false;
        round = 1;
        zombiesToSpawnPerRound = 5;
        zombieToSpawnLeft = 5; // number of zombies to spawn
        timeForEachSpawnerToSpawn = 1f; // time for each spawner to spawn zombie
        sceneLoader = GameObject.Find("SceneLoader").GetComponent<SceneLoader>();
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
            zombies = new List<GameObject>();
            zombieSpawners = new List<GameObject>();
            stores = new List<GameObject>();
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
                    }else if (i == 6) {
                        zombies.Add(tempTf.GetChild(q).gameObject);
                    }else if (i == 7) {
                        zombieSpawners.Add(tempTf.GetChild(q).gameObject);
                    } else if (i == 8) {
                        stores.Add(tempTf.GetChild(q).gameObject);

                    }
                }
            }
            if (ZombiePathes.getAllID() != zombiePathes.Count) {
                Debug.Log("faile");
                ZombiePathes.resetID();

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
    public int getLastWeapon() {
        return weapons.Count-1;
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
    public int getClosestStore(Vector3 p, float rad) {
        float distance = float.MaxValue;
        int index = -1;
        for (int i = 0; i < stores.Count; i++) {
            GameObject go = stores[i].GetComponent<Store>().getUsePoint();
            if (go == null) {
                go = stores[i];
            } 
            float tempDistance = Vector3.Distance(go.transform.position,p);
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
    public GameObject getStore(int index) {
        return stores[index];    
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
    public void addZombie(GameObject o) {
        zombies.Add(o);
    }
    public void removeZombie(GameObject o) {
        zombies.Remove(o);
    }
    public List<GameObject> getAllZombies() {
        return zombies;
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
    public Transform getZombieTf() {
        return transform.GetChild(6);
    }
    public Transform getRandomZombieSpawn() {
         System.Random rmd = new System.Random();
        int rand = rmd.Next(0,zombieSpawners.Count-1);
        return zombieSpawners[rand].transform;
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
        if (players != null) {
        bool playersAlive = false;
        for (int i = 0; i < players.Count; i++) {
            if (players[i].GetComponent<PlayerInner>().getHealth() != 0)
            playersAlive = true;
        }
        if (!playersAlive) {
            Cursor.lockState = CursorLockMode.None;
            players[0].GetComponent<PlayerInner>().showDeadMenu();
        }
        }
        if (round == 3) {
            Cursor.lockState = CursorLockMode.None;
            players[0].GetComponent<PlayerInner>().showWinMenu();
        }
        
        System.Random rmd = new System.Random();
        if (zombies.Count == 0 && zombieToSpawnLeft == 0) { // all zombies are dead
            round++;
            zombiesToSpawnPerRound += 2; // number of zombies to spawn
            zombieToSpawnLeft = zombiesToSpawnPerRound;
            if(round > 25)
            {
                timeForEachSpawnerToSpawn = 4f;// time for each spawner to spawn zombie
            }
        }
        if (zombieToSpawnLeft != 0) {
            for (int i = 0; i < zombieSpawners.Count; i++) {                
                bool zombieSpawned = false;
                int rand = rmd.Next(1,3);
                //(float ms, int hea, float rang, float rotatSpeed, int dam, float attSpe, int pts) {
                // weakZombie (1, 100, 2, 0.5, 30, 1.5); (PlayerSpeed is 2, Player health is 150)
                if(round < 5)//weak zombiess
                {
                    zombiePrefab.GetComponent<Zombie>().setup(1, 100, 2, 0.5f, 30, 1.5f,100);
                }
                else if(round < 10) //2/3 weak, 1/3 normal
                {
                    if(rand == 3) //normal
                    {
                        zombiePrefab.GetComponent<Zombie>().setup(1.37f, 150, 2, 0.5f, 50, 1.25f,100); 
                    }
                    else //weak
                    {
                        zombiePrefab.GetComponent<Zombie>().setup(1, 100, 2, 0.5f, 30, 1.5f,100);
                    }
                }
                else if(round < 15)//1/3 weak, 2/3 normal
                {
                    if(rand == 3) //weak
                    {
                        zombiePrefab.GetComponent<Zombie>().setup(1, 100, 2, 0.5f, 30, 1.5f,100);
                    }
                    else //normal
                    {
                        zombiePrefab.GetComponent<Zombie>().setup(1.37f, 150, 2, 0.5f, 50, 1.25f,100);
                    }
                }
                else if(round < 20)//all normal
                {
                    zombiePrefab.GetComponent<Zombie>().setup(1.37f, 150, 2, 0.5f, 50, 1.25f,100);
                }
                else if(round < 25) //2/3 normal, 1/3 hard
                {   
                    if(rand == 3) //hard
                    {
                        zombiePrefab.GetComponent<Zombie>().setup(1.75f, 250, 2, 0.5f, 70, 1,100);
                    }
                    else //normal
                    {
                        zombiePrefab.GetComponent<Zombie>().setup(1.37f, 150, 2, 0.5f, 50, 1.25f,100);
                    }
                }
                else if(round < 30) //1/3 normal, 2/3 hard
                {
                    if(rand == 3) //normal
                    {
                        zombiePrefab.GetComponent<Zombie>().setup(1.37f, 150, 2, 0.5f, 50, 1.25f,100);
                    }
                    else //hard
                    {
                        zombiePrefab.GetComponent<Zombie>().setup(1.75f, 250, 2, 0.5f, 70, 1,100);
                    }
                }
                else //all hard, 
                {
                    zombiePrefab.GetComponent<Zombie>().setup(1.75f, 250, 2, 0.5f, 70, 1,100);
                }
                    if (zombieToSpawnLeft != 0) {
                    zombieSpawned = zombieSpawners[i].GetComponent<Spawner>().spawnZombie(zombiePrefab,timeForEachSpawnerToSpawn);

                    if (zombieSpawned) {
                        zombieToSpawnLeft--;
                    }
                }
            }
        }
    }
}
