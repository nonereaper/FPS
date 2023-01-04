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
    private GameObject playerToChase;

    private Controller controller;
    private int stateOfZombieAI;
    // Start is called before the first frame update
    void Start()
    {
        controller = GameObject.Find("Controller").GetComponent<Controller>();
        timeBeforeNextAnimation = 0;
        stateOfAnimation = 0;
        savedTime = UnityEngine.Time.time;
        animationController = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        characterAngle = rb.rotation.eulerAngles.y;
        stateOfZombieAI = 0;
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
    public void rotate(Vector3 p) {
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
        return playerToChase.transform.position;
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
        playerToChase = controller.getClosestPlayer(transform.position);
        GameObject pathPlayer = controller.getPathes(controller.findClosestPath(playerToChase.transform.position));
        GameObject pathZombie = controller.getPathes(controller.findClosestPath(transform.position));
        if (pathPlayer.GetComponent<ZombiePathes>().getID() == pathZombie.GetComponent<ZombiePathes>().getID()) {
            rotate(findPlayerPosition());
        } else {
            rotate(pathZombie.transform.position);
        }
        
        
    }
}
