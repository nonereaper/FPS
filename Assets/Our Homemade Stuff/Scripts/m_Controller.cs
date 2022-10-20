using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class m_Controller : MonoBehaviour
{
    private NetworkManager networkManager;
    [SerializeField] private GameObject playerPrefab;

    private List<GameObject> props; 
    private List<GameObject> weapons;
    private List<GameObject> projectiles;
    private List<GameObject> decay;

    private float savedDistance;
    // Start is called before the first frame update
    void Start()
    {
        networkManager = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();
        if (networkManager.IsServer) {
            IReadOnlyList<NetworkClient> list = networkManager.ConnectedClientsList;
            for (int i = 0; i < list.Count; i++) {
                NetworkObject mpc = networkManager.ConnectedClients[list[i].ClientId].PlayerObject;
                mpc.Despawn();
                GameObject temp = Instantiate(playerPrefab);
                temp.GetComponent<NetworkObject>().SpawnAsPlayerObject(list[i].ClientId);
            }
            props = new List<GameObject>();
            weapons = new List<GameObject>();
            projectiles = new List<GameObject>();
            decay = new List<GameObject>();
            for (int i = 0; i < transform.childCount; i++) {
                Transform tempTf = transform.GetChild(i);
                for (int q = 0; q < tempTf.childCount; q++) {
                    if (i == 1) {
                        weapons.Add(tempTf.GetChild(q).gameObject);
                        tempTf.GetChild(q).gameObject.GetComponent<NetworkObject>().Spawn();
                    } else if (i == 2) {
                        props.Add(tempTf.GetChild(q).gameObject);
                        tempTf.GetChild(q).gameObject.GetComponent<NetworkObject>().Spawn();
                    }
                }
            }
        }
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
    // Update is called once per frame
    void Update()
    {
        
    }
}
