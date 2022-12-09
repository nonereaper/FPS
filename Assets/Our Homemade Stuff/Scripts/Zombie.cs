using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zombie : MonoBehaviour
{
    [SerializeField] private float movementSpeed;
    [SerializeField] private int health;
    [SerializeField] private float range;
    private Animator animationController; 

    private float savedTime;
    private float timeBeforeNextAnimation;
    private int stateOfAnimation;
    // Start is called before the first frame update
    void Start()
    {
        timeBeforeNextAnimation = 0;
        stateOfAnimation = 0;
        savedTime = UnityEngine.Time.time;
    }

    public void die() {
       // animationController.
        timeBeforeNextAnimation = 2f;
        stateOfAnimation = 1;
       
    }
    public void reduceHealth(int healthReduction) {
        health -= healthReduction;
        if (health <= 0) die();
    }
    // Update is called once per frame
    void Update()
    {
        float differenceInTime = UnityEngine.Time.time-savedTime;
        savedTime = UnityEngine.Time.time;
        timeBeforeNextAnimation -= differenceInTime;
        if (timeBeforeNextAnimation < 0f) timeBeforeNextAnimation = 0f;
        if (stateOfAnimation == 1 && timeBeforeNextAnimation == 0f) {
            Destroy(gameObject);
        }
    }
}
