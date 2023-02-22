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
    [SerializeField] private int damage;
    [SerializeField] private float attackSpeed;
    [SerializeField] private int points;
    [SerializeField] private GameObject colliderObject;
    [SerializeField] private GameObject headObject;
    [SerializeField] private GameObject bodyObject;
    private Animator animationController; 

    private float savedTime;
    private float timeBeforeNextAnimation;
    private float timeBeforeNextAttack;
    private int stateOfAnimation;
    private float characterAngle;
    private Rigidbody rb;
    private GameObject playerToChase;

    private Controller controller;
    private GameObject targetZombiePath;
    private GameObject playerZombiePath;
    private int stateOfAI;

    private float savedTime2;
    private Vector3 savedPlace;
    private float savedTime3;

    // Start is called before the first frame update
    void Start()
    {
        controller = GameObject.Find("Controller").GetComponent<Controller>();
        timeBeforeNextAnimation = 0;
        timeBeforeNextAttack = 0;
        stateOfAnimation = 0;
        savedTime = UnityEngine.Time.time;
        savedTime2 = UnityEngine.Time.time;
        savedTime3 = 0;
        animationController = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        characterAngle = rb.rotation.eulerAngles.y;
        targetZombiePath = null;
        playerZombiePath = null;
        stateOfAI = 0;
    }
    public int collides(Vector3 p) {
        Collider m_Collider = headObject.GetComponent<Collider>();
        if (m_Collider.bounds.Contains(p)) {
            return 1;
        }
     return 0;
    }
    public void die() {
       // animationController.
        timeBeforeNextAnimation = 2f;
        stateOfAnimation = 1;
       animationController.SetTrigger("die");
       gameObject.layer = LayerMask.NameToLayer("IgnoreCollisions");
       Transform[] oTemp = GetComponentsInChildren<Transform>();
            for (int i = 0; i < oTemp.Length; i++) {
                oTemp[i].gameObject.layer = LayerMask.NameToLayer("IgnoreCollisions");
            }
    }
    public void reduceHealth(int healthReduction) {
       
        health -= healthReduction;
        if (health <= 0) die();
    }
    public int getHealth() {
        return health;
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
        
       rb.AddForce(new Vector3((float)increaseX,rb.velocity.y,(float)increaseZ) - rb.velocity, ForceMode.VelocityChange);
    }
    public Vector3 findPlayerPosition() {
        return playerToChase.transform.position;
    }
    public void setup(float ms, int hea, float rang, float rotatSpeed, int dam, float attSpe, int pts) {
        movementSpeed = ms;
        health = hea;
        rang = range;
        rotationSpeed = rotatSpeed;
        damage = dam;
        attackSpeed = attSpe;
        points = pts;
    }
    public int getPoints() {
        return points;
    }
    void FixedUpdate() {
        playerToChase = controller.getClosestPlayer(transform.position);
        GameObject playerPath = controller.getPathes(controller.findClosestPath(playerToChase.transform.position));
        GameObject zombiePath = controller.getPathes(controller.findClosestPath(transform.position));

        if (playerZombiePath == null || (playerZombiePath != null && playerPath.GetComponent<ZombiePathes>().getID() != playerZombiePath.GetComponent<ZombiePathes>().getID())) {
            playerZombiePath = playerPath;
            stateOfAI = 0;
            //targetZombiePath = zombiePath;
            //if (targetZombiePath.GetComponent<ZombiePathes>().getID() != zombiePath.GetComponent<ZombiePathes>().getID()) {

            //}
        }
        
        if (targetZombiePath == null) {
            targetZombiePath = zombiePath;
        }

        if (Vector3.Distance(transform.position,playerToChase.transform.position) < Vector3.Distance(transform.position,targetZombiePath.transform.position)) {
            stateOfAI = 1;
        }

        if (Vector3.Distance(transform.position,targetZombiePath.transform.position) < 4f) {
            if (stateOfAI == 0 && playerZombiePath.GetComponent<ZombiePathes>().getID() != targetZombiePath.GetComponent<ZombiePathes>().getID()) {
                int targetID = targetZombiePath.GetComponent<ZombiePathes>().search(playerPath.GetComponent<ZombiePathes>().getID());
                targetZombiePath = targetZombiePath.GetComponent<ZombiePathes>().searchAdj(targetID);
                
            } else {
                stateOfAI = 1;
            }
        }

        if (stateOfAI == 1) {
            rotate(findPlayerPosition());
            moveForward();
        } else {
            rotate(targetZombiePath.transform.position);
            moveForward();
        }
    }
    public void respawn() {
        transform.position = controller.getRandomZombieSpawn().position;
    }
    public void ignoringZombies() {
        colliderObject.layer = LayerMask.NameToLayer("EnemyIgnoreCollisions");
    }
    // Update is called once per frame
    void Update()
    {
        float differenceInTime = UnityEngine.Time.time-savedTime;
        savedTime = UnityEngine.Time.time;

        timeBeforeNextAnimation -= differenceInTime;
        if (timeBeforeNextAnimation < 0f) timeBeforeNextAnimation = 0f;
        if (stateOfAnimation == 1 && timeBeforeNextAnimation == 0f) {
            controller.removeZombie(gameObject);
            Destroy(gameObject);
        }
        timeBeforeNextAttack -= differenceInTime;
        if (timeBeforeNextAttack < 0f) timeBeforeNextAttack = 0f;

        if (savedTime3 != 0f) {
            savedTime3 -= differenceInTime;
            if (savedTime3 <= 0f) {
                savedTime3 = 0f;
                colliderObject.layer = LayerMask.NameToLayer("EnemyCollisions");
            }
        }

        differenceInTime = UnityEngine.Time.time-savedTime2;
        if (differenceInTime > 5f) {
            
            savedTime2 = UnityEngine.Time.time;
            if (stateOfAI != 1 && Vector3.Distance(transform.position,savedPlace) < 0.5f) {
                if (savedTime3 == 0f) {
                    savedTime3 = 6f;
                    colliderObject.layer = LayerMask.NameToLayer("EnemyIgnoreCollisions");
                } else {
                    respawn();
                    savedTime3 = 0;
                }
            }
            savedPlace = transform.position;
        }
        

        if (timeBeforeNextAttack == 0 && stateOfAnimation != 1) {
            if (playerToChase != null && Vector3.Distance(transform.position,playerToChase.transform.position) <= range) {
                Vector3 D = playerToChase.transform.position - transform.position;  
                Quaternion rot = Quaternion.LookRotation(D,Vector3.up);
                
                //transform.rotation = Quaternion.Euler(0, ,0);
            
                if (Math.Abs(rot.eulerAngles.y-transform.rotation.eulerAngles.y)< Math.PI/8) {
                    playerToChase.GetComponent<PlayerInner>().reduceHealth(damage);
                    timeBeforeNextAttack = attackSpeed;
                    animationController.SetTrigger("attack");
                }
                
            }
        }
    }
}
