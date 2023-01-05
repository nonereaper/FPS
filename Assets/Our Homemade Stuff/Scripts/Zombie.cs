using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Zombie : MonoBehaviour
{
    [SerializeField] private float movementSpeed;
    [SerializeField] private int health;
    [SerializeField] private float range;
    [SerializeField] private float rotationSpeed;
    private Animator animationController; 

    private float savedTime;
    private float timeBeforeNextAnimation;
    private int stateOfAnimation;
    private float characterAngle;
    private Rigidbody rb;
    private GameObject playerToChase;

    private Controller controller;
    private int stateOfZombieAI;
    private int targetID;
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
        targetID = -1;
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
        // https://answers.unity.com/questions/306639/rotating-an-object-towards-target-on-a-single-axis.html
        // distance between target and the actual rotating object
        Vector3 D = p - transform.position;  
        
        
        // calculate the Quaternion for the rotation
        Quaternion rot = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(D), rotationSpeed * Time.deltaTime);
        
        //Apply the rotation 
        transform.rotation = rot; 
        
        // put 0 on the axys you do not want for the rotation object to rotate
        transform.eulerAngles = new Vector3(0,transform.eulerAngles.y,0); 
        characterAngle = transform.eulerAngles.y;
    }
    public void moveForward() {
        double tempAngle = characterAngle/180*Math.PI;

        double increaseZ = Math.Cos(tempAngle)*movementSpeed,
        increaseX = Math.Sin(tempAngle)*movementSpeed;
        
        rb.AddForce(new Vector3((float)increaseX,0,(float)increaseZ) - rb.velocity, ForceMode.VelocityChange);
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
        if (targetID == -1) {
            targetID = pathZombie.GetComponent<ZombiePathes>().getID();
        } else if (Vector3.Distance(transform.position,pathZombie.transform.position) < 10f) {
            targetID = pathZombie.GetComponent<ZombiePathes>().search(pathPlayer.GetComponent<ZombiePathes>().getID());
        }
        
        if (pathPlayer.GetComponent<ZombiePathes>().getID() == pathZombie.GetComponent<ZombiePathes>().getID()) {
            rotate(findPlayerPosition());
        } else {
             else {
                rotate(pathZombie.transform.position);
                moveForward();
            }
        }
        
        
    }
}
