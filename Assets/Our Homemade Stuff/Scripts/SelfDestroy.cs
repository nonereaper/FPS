using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class SelfDestroy : MonoBehaviour
{   
    private float time;
    [SerializeField] float secondsBeforeRemoved;
    private Controller controller;
    // Start is called before the first frame update
    void Start()
    {
        time = Time.time;
        controller = GameObject.Find("Controller").GetComponent<Controller>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time >= secondsBeforeRemoved+time) {
            if (controller.isIsMult()) {
                GetComponent<NetworkObject>().Despawn();
            }
            controller.removeDecay(gameObject);
            controller.removeProjectile(gameObject);
            Destroy(gameObject);
        }
    }
}
