using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] GameObject hitPoint;
    private GameObject controllerObject;
    private Controller controller;
    private int damage;
    private GameObject parent;
    private float timeToDespawn, timeSpawned;

    public void setup(int d, GameObject p, float ttD, GameObject pc) {
        damage = d;
        parent = p;
        timeToDespawn = ttD;
        controllerObject = pc;
        controller = controllerObject.GetComponent<Controller>();
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
        if (collision.gameObject.tag == "Structure") {
            ContactPoint point = collision.contacts[0];
            //Debug.DrawRay(point.point, point.normal * 100, Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f), 10f);
            Quaternion rotation = Quaternion.FromToRotation(Vector3.up, point.normal);
            Vector3 position = point.point;
            Transform parentTransform = collision.gameObject.GetComponent<Transform>();
            Transform childTransform = hitPoint.GetComponent<Transform>();
            GameObject o = Instantiate(hitPoint, position, rotation,parentTransform);
            // 10
            double x = (double)childTransform.localScale.x / parentTransform.localScale.x,
            y = (double)childTransform.localScale.y / parentTransform.localScale.y,
            z = (double)childTransform.localScale.z / parentTransform.localScale.z;
            o.GetComponent<Transform>().localScale = new Vector3((float)x,(float)y,(float)z);
            controller.addDecay(o);
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
