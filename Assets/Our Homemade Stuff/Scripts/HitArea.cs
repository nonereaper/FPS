using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitArea : MonoBehaviour
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
            Destroy(gameObject);
        }
    }
}
