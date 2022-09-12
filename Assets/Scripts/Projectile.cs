using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] GameObject hitPoint;
    private GameObject projectileController;
    private int damage;
    private GameObject parent;
    private float timeToDespawn, timeSpawned;

    public void setup(int d, GameObject p, float ttD, GameObject pc) {
        damage = d;
        parent = p;
        timeToDespawn = ttD;
        projectileController = pc;
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
    // https://learn.unity.com/tutorial/using-c-to-launch-projectiles
    void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.tag == "Structure") {
            ContactPoint point = collision.contacts[0];
            //Debug.DrawRay(point.point, point.normal * 100, Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f), 10f);
            Quaternion rotation = Quaternion.FromToRotation(Vector3.up, point.normal);
            Vector3 position = point.point;
            projectileController.GetComponent<ProjectileController>().addNewDecay(Instantiate(hitPoint, position, rotation,projectileController.GetComponent<Transform>()));
            Destroy(gameObject);
        }
    } 
    // Update is called once per frame
    void Update()
    {
        if (UnityEngine.Time.time >= timeToDespawn+timeSpawned) {
            Destroy(gameObject);
        }
    }
}
