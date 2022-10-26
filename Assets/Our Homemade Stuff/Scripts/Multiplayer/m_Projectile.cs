using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class m_Projectile : NetworkBehaviour
{
    [SerializeField] GameObject hitPoint;
    private m_Controller controller;
    private int damage;
    private GameObject parent;
    private float timeToDespawn, timeSpawned;

    public void setup(int d, GameObject p, float ttD) {
        damage = d;
        parent = p;
        timeToDespawn = ttD;
        controller = GameObject.Find("Controller").GetComponent<m_Controller>();
        timeSpawned = UnityEngine.Time.time;
        Collider[] cs = parent.GetComponentsInChildren<Collider>();
        for (int i = 0; i < cs.Length; i++) {
            Physics.IgnoreCollision(cs[i],GetComponent<Collider>());
        }
        
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }
    // https://docs.unity3d.com/ScriptReference/Collider.OnCollisionEnter.html
    void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground")) {
            ContactPoint point = collision.contacts[0];
            //Debug.DrawRay(point.point, point.normal * 100, Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f), 10f);
            Quaternion rotation = Quaternion.FromToRotation(Vector3.up, point.normal);
            Vector3 position = point.point;
            GameObject o = Instantiate(hitPoint, position, rotation,controller.getDecayTf());
            o.GetComponent<NetworkObject>().Spawn();
            controller.addDecay(o);
            GetComponent<NetworkObject>().Despawn();
            Destroy(gameObject);
        } else if (collision.gameObject.layer == LayerMask.NameToLayer("Movable Objects")) {
            GetComponent<NetworkObject>().Despawn();
            Destroy(gameObject);
        }
    } 
    // Update is called once per frame
    void Update()
    {
        if (UnityEngine.Time.time >= timeToDespawn+timeSpawned) {
            GetComponent<NetworkObject>().Despawn();
            Destroy(gameObject);
        }
    }
}
