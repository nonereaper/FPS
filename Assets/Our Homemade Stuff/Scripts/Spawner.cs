using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    // Start is called before the first frame update
    private float savedTime;
    private float timeBeforeNextSpawn;
    private Controller controller;
    void Start() {
        controller = GameObject.Find("Controller").GetComponent<Controller>();
        savedTime = UnityEngine.Time.time;
        timeBeforeNextSpawn = 0f;
    }
    public bool spawnZombie(GameObject zombie, float tbns) {
        /*
        if (timeBeforeNextSpawn == 0f) {
            GameObject z = Instantiate(zombie,transform.position,transform.rotation,controller.getZombieTf());
            controller.addZombie(z);
            timeBeforeNextSpawn = tbns;
            return true;
        }*/
        return false;
    }
    // Update is called once per frame
    void Update()
    {
        float differenceInTime = UnityEngine.Time.time - savedTime;
        timeBeforeNextSpawn -= differenceInTime;
        if (timeBeforeNextSpawn < 0f) timeBeforeNextSpawn = 0f;
    }
}
