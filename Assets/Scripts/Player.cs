using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;


/* https://www.youtube.com/watch?v=n0GQL5JgJcY&list=PLrnPJCHvNZuB5ATsJZLKX3AW4V9XaIV9b&index=1
    Video series used to write help write movement code
*/
public class Player : MonoBehaviour
{
    private Rigidbody rb;
    private Transform tf;
    [SerializeField] float movementSpeed;
    [SerializeField] float sprintSpeedMult;
    [SerializeField] float crouchSpeedMult;
    [SerializeField] float jump;
    private bool isGrounded, isCrouching;
    private float cameriaAngle;
    private float angle;
    [SerializeField] GameObject controllerObject;
    private Controller controller;
    private ProjectileCreator projectileCreator;
    private Transform useTf;
    private String highlightedUse;
    private GameObject holdingObstacle;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        tf = GetComponent<Transform>();
        controller = controllerObject.GetComponent<Controller>();
        projectileCreator = tf.GetChild(0).GetChild(0).GetComponent<ProjectileCreator>();
        useTf = tf.GetChild(0).GetChild(0).GetChild(0).GetComponent<Transform>();
        angle = 0.0f;
        cameriaAngle = 0.0f;
        isGrounded = false;
        isCrouching = false;
        holdingObstacle = null;
    }
    public float getAngle() {
        return angle;
    }
    public void rotatePlayer(float an) {        
        rb.MoveRotation(rb.rotation * Quaternion.Euler(new Vector3(0,an,0)));
        angle = rb.rotation.eulerAngles.y-360.0f;
        rb.rotation = Quaternion.Euler(0,rb.rotation.eulerAngles.y,0);
    }
    public void setCameriaAngle(float ca) {
        cameriaAngle = ca;
    }
    public float getCameriaAngle() {
        return cameriaAngle;
    }
    public bool getCrouching() {
        return isCrouching;
    }
    public void setGrounded(bool b) {
        isGrounded = b;
    }
    public String getAllWeaponText() {
        return projectileCreator.getWeaponInfo();
    }
    public String getHighlightedUse() {
        return highlightedUse;
    }
    private void changeWeapon(GameObject w) {
        Weapon tempWeapon = w.GetComponent<Weapon>();
        int index = projectileCreator.indexToSwapWith(tempWeapon);
        if (projectileCreator.getCurrentWeaponSlot()!=-1 || index != -1) {  
            Weapon tempWeapon2 = projectileCreator.swapWeapon(index, tempWeapon);
            if (tempWeapon2 != null) {
                removeWeapon(tempWeapon2);
            }
            Transform tempTransform = w.GetComponent<Transform>();
            tempTransform.SetParent(tf.GetChild(5));
            tempTransform.localPosition = new Vector3(-0.27f,0.22f,0.47f);
            tempTransform.localRotation = Quaternion.Euler(0f,0f,0f);
            w.GetComponent<Rigidbody>().isKinematic = true;
            controller.removeWeapon(w);
            if (index != -1 && projectileCreator.getCurrentWeaponSlot() != index) {
                tempTransform.gameObject.SetActive(false);
            }
        }
    }
    private void removeWeapon(Weapon w) {
        Transform tempTransform = w.GetComponent<Transform>();
        tempTransform.SetParent(controller.GetComponent<Transform>());
        tempTransform.position = useTf.position;
        tempTransform.localRotation = useTf.rotation;
        Rigidbody tempRigidbody = w.GetComponent<Rigidbody>();
        tempRigidbody.GetComponent<Rigidbody>().isKinematic = false;
        tempRigidbody.AddRelativeForce(new Vector3(0,0,500f));
        controller.addWeapon(tempTransform.gameObject);
    }
    public void movePlayer(float x, float y, float z) {
        rb.AddForce(new Vector3(x,y,z),ForceMode.VelocityChange);
    }
    // Update is called once per frame
    void Update()
    {
        float x = rb.velocity.x, y = rb.velocity.y, z = rb.velocity.z;
        double tempAngle = Math.PI*angle/180, tempAngleP = tempAngle+(Math.PI/2);
        if (tempAngleP > Math.PI) {
            tempAngleP -= Math.PI*2;
        }
        float tempMovementSpeed = movementSpeed;
        if (Input.GetButtonDown("Crouch")) {
            tf.position = new Vector3(tf.position.x,tf.position.y-0.4f,tf.position.z);
            isCrouching = true;
        } else if (Input.GetButtonUp("Crouch")) {
            isCrouching = false;
            if (isGrounded) {
                tf.position = new Vector3(tf.position.x,tf.position.y+0.4f,tf.position.z);
            }
        }
        if (isCrouching) {
            tempMovementSpeed *= crouchSpeedMult;
        } else if (Input.GetButton("Sprint")) {
            tempMovementSpeed *= sprintSpeedMult;
        }
        bool canFire = Input.GetButton("Fire1") || Input.GetButtonDown("Fire1");
        if (canFire) {
            projectileCreator.useWeapon(Input.GetButtonDown("Fire1"));
        }
        // use highlight code
        
        if (holdingObstacle == null) {
            float radiusOfUseTool = 2.5f;
            GameObject tempObject = controller.getClosestObject(useTf.position, radiusOfUseTool);
            float closestDistance = controller.getSavedDistance();
            int typeOfHighlight = controller.getSavedtype();
            //Debug.Log(typeOfHighlight);

            if (typeOfHighlight == 0) {
                highlightedUse = "Press E to swap weapons with " + tempObject.name;
            } else if (typeOfHighlight == 1) {
                highlightedUse = "Press E to move " + tempObject.name;
            } else {
                highlightedUse = "";
            }

            if (Input.GetButtonDown("Use")) {
                if (typeOfHighlight == 0) {
                    changeWeapon(tempObject);
                } else if (typeOfHighlight == 1) {
                    holdingObstacle = tempObject;
                    holdingObstacle.GetComponent<Rigidbody>().useGravity = false;
                    Collider[] cs = GetComponentsInChildren<Collider>();
                    for (int i = 0; i < cs.Length; i++) {
                        Physics.IgnoreCollision(cs[i],holdingObstacle.GetComponent<Collider>());
                    }
                    highlightedUse = "";
                }
            }
        }
        if (Input.GetButtonDown("Drop")) {
            if (holdingObstacle == null) {
            Weapon droppingWeapon = projectileCreator.removeCurrentWeapon();
            if (droppingWeapon != null) {
                removeWeapon(droppingWeapon);
            }
            } else {
                holdingObstacle.GetComponent<Rigidbody>().useGravity = true;
                Collider[] cs = GetComponentsInChildren<Collider>();
                for (int i = 0; i < cs.Length; i++) {
                    Physics.IgnoreCollision(cs[i],holdingObstacle.GetComponent<Collider>(),false);
                }
                holdingObstacle = null;
            }
        }
        if (Input.GetButtonDown("Reload")) {
            projectileCreator.reload();
        }
        for (int i = 0; i < projectileCreator.getWeaponSlotLength(); i++) {
            if (Input.GetKeyDown(""+i)) {
                projectileCreator.setCurrentWeaponSlot(i-1);
            }
        }
        if (holdingObstacle != null) {
            Rigidbody tempRb = holdingObstacle.GetComponent<Rigidbody>();
            Transform tempTf = holdingObstacle.GetComponent<Transform>();
            Vector3 tempV = useTf.position - tempTf.position;
            tempV /= 10;
            tempRb.MovePosition(tempTf.position + tempV);
        }
        double increaseZ = Input.GetAxis("Vertical")*Math.Cos(tempAngle)*tempMovementSpeed + Input.GetAxis("Horizontal")*Math.Cos(tempAngleP)*tempMovementSpeed,
        increaseX = Input.GetAxis("Vertical")*Math.Sin(tempAngle)*tempMovementSpeed + Input.GetAxis("Horizontal")*Math.Sin(tempAngleP)*tempMovementSpeed;
        rb.AddForce(new Vector3((float)(increaseX),y,(float)(increaseZ)) - rb.velocity, ForceMode.VelocityChange);

      //  if (Math.Abs(rb.velocity.x) <= 5)
       // rb.AddForce(Input.GetAxis("Horizontal") * movementSpeed,0,0,ForceMode.VelocityChange);
       
       
       //rotatePlayer(Input.GetAxis("Horizontal")*Time.deltaTime);
        
        if (Input.GetButtonDown("Jump") && isGrounded) {
            //rb.velocity = new Vector3(x,jump,z);
            rb.velocity = new Vector3(x,jump,z);
        }
    }
}
