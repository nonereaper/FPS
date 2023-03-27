using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Projectile : NetworkBehaviour
{
    [SerializeField] GameObject hitPoint;
    private bool deadShot;
    private Controller controller;
    private int damage;
    private GameObject parent;
    private float timeToDespawn, timeSpawned;

    public void setup(int d, GameObject p, float ttD, bool deadShot) {
        damage = d;
        parent = p;
        timeToDespawn = ttD;
        controller = GameObject.Find("Controller").GetComponent<Controller>();
        timeSpawned = UnityEngine.Time.time;
        Collider[] cs = parent.GetComponentsInChildren<Collider>();
        for (int i = 0; i < cs.Length; i++) {
            Physics.IgnoreCollision(cs[i],GetComponent<Collider>());
        }
        this.deadShot = deadShot;
        
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }
    // https://docs.unity3d.com/ScriptReference/Collider.OnCollisionEnter.html
    void OnCollisionEnter(Collision collision) {
        
        if (collision.gameObject.layer == LayerMask.NameToLayer("EnemyInner")) {
            Zombie zombie = collision.gameObject.GetComponent<Zombie>();
            
            ContactPoint point = collision.contacts[0];

            int type = zombie.collides(point.point);
            int damage2 = damage;
            if (type == 1) {
                if (deadShot) {
                    damage2 *=3;
                } else {
                    damage2 *=2;
                }
            }
            
            zombie.reduceHealth(damage2);
            if (zombie.getHealth() <= 0) {
                parent.GetComponent<PlayerInner>().addPoints(zombie.getPoints());
            }
            selfDestory();
        } else if (collision.gameObject.layer == LayerMask.NameToLayer("Ground")) {
            ContactPoint point = collision.contacts[0];
            //Debug.DrawRay(point.point, point.normal * 100, Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f), 10f);
            Quaternion rotation = Quaternion.FromToRotation(Vector3.up, point.normal);
            Vector3 position = point.point;
            GameObject o = Instantiate(hitPoint, position, rotation,controller.getDecayTf());
            if (controller.isIsMult()) {
                o.GetComponent<NetworkObject>().Spawn();
            }
            controller.addDecay(o);
            selfDestory();
        } else if (collision.gameObject.layer == LayerMask.NameToLayer("Movable Objects")) {
            selfDestory();
        }
    } 
    public void selfDestory() {
        if (controller.isIsMult()) {
            GetComponent<NetworkObject>().Despawn();
        }
        controller.removeProjectile(gameObject);
        Destroy(gameObject);
    }
    // Update is called once per frame
    void Update()
    {
        if (UnityEngine.Time.time >= timeToDespawn+timeSpawned) {
            selfDestory();
        }
    }
}
