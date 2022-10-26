using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class m_SelfDestroy : MonoBehaviour
{   
    private float time;
    [SerializeField] float secondsBeforeRemoved;
    // Start is called before the first frame update
    void Start()
    {
        time = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time >= secondsBeforeRemoved+time) {
            if (GetComponent<NetworkObject>() != null) {
                GetComponent<NetworkObject>().Despawn();
            }
            Destroy(gameObject);
        }
    }
}
