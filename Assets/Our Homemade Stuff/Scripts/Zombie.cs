using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Zombie : MonoBehaviour
{
    [SerializeField] private float movementSpeed;
    [SerializeField] private int health;
    [SerializeField] private float range;
    [SerializeField] private float rotationAmount;
    private Animator animationController; 

    private float savedTime;
    private float timeBeforeNextAnimation;
    private int stateOfAnimation;
    private float characterAngle;
    private Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
        
        timeBeforeNextAnimation = 0;
        stateOfAnimation = 0;
        savedTime = UnityEngine.Time.time;
        animationController = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        characterAngle = rb.rotation.eulerAngles.y;
    }

    public void die() {
       // animationController.
        timeBeforeNextAnimation = 2f;
        stateOfAnimation = 1;
       animationController.SetTrigger("die");
    }
    public void reduceHealth(int healthReduction) {
       
        health -= healthReduction;
        if (health <= 0) die();
    }
    public void rotate() {
        Vector3 p = findPlayerPosition();
        transform.LookAt(p);
       // Vector3 newDirection = Vector3.RotateTowards(transform.up, p, 1f, 0.0f);
        //Debug.Log(newDirection.x + " " + newDirection.y + " " + newDirection.z);
        //transform.rotation = Quaternion.Euler(0f,transform.rotation.y+newDirection.y,0f);
        //transform.rotation = Quaternion.LookRotation(new Vector3(0,0,0),newDirection);

        //Debug.Log(angleChange);
        //transform.rotation = Quaternion.Euler(0f,transform.rotation.y-(float)angleChange,0f);
        //rb.MoveRotation(rb.rotation * Quaternion.Euler(new Vector3(0,(float)angleChange,0)));
        //characterAngle = rb.rotation.eulerAngles.y;
        //Debug.Log(characterAngle);
    }
    public Vector3 findPlayerPosition() {
        return GameObject.Find("Player").transform.position;
    }
    // Update is called once per frame
    void Update()
    {
        float differenceInTime = UnityEngine.Time.time-savedTime;
        savedTime = UnityEngine.Time.time;
        timeBeforeNextAnimation -= differenceInTime;
        if (timeBeforeNextAnimation < 0f) timeBeforeNextAnimation = 0f;
        if (stateOfAnimation == 1 && timeBeforeNextAnimation == 0f) {
           // Destroy(gameObject);
        }
        rotate();
    }
}
