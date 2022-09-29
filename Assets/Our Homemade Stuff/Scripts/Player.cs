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
    private GameObject controllerObject;
    private Controller controller;
    [SerializeField] GameObject projectileCreatorGameObject;
    private ProjectileCreator projectileCreator;
    private float distanceOfProjectileCreator;
    [SerializeField] GameObject UseGameObject;
    private Transform useTf;
    private float distanceOfUse;
    private Transform movementHitboxTf;
    private Transform weaponLocationTf;

    [SerializeField] GameObject aimCameraObject;
    private Transform aimCameraTf;
    
    private String highlightedUse;
    private GameObject holdingObstacle;

    

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        tf = GetComponent<Transform>();
        controllerObject = GameObject.Find("Controller");
        controller = controllerObject.GetComponent<Controller>();
        projectileCreator = projectileCreatorGameObject.GetComponent<ProjectileCreator>();
        useTf = UseGameObject.GetComponent<Transform>();
        movementHitboxTf = tf.GetChild(7);
        weaponLocationTf =tf.GetChild(5).GetChild(1).GetComponent<Transform>();
        distanceOfProjectileCreator = projectileCreatorGameObject.GetComponent<Transform>().localPosition.z;
        distanceOfUse = UseGameObject.GetComponent<Transform>().localPosition.z;
        aimCameraTf = aimCameraObject.GetComponent<Transform>();
        aimCameraObject.GetComponent<Camera>().enabled = false;
        angle = 0.0f;
        cameriaAngle = 0.0f;
        isGrounded = false;
        isCrouching = false;
        holdingObstacle = null;
        
    }
    public GameObject getController() {
        return controllerObject;
    }
    public float getAngle() {
        return angle;
    }
    public Transform getWeaponLocationTransform() {
        return weaponLocationTf;
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
    public void setProjectileCreatorDistance(Weapon w) {
        float distance = w.getMussle().GetComponent<Transform>().position.z-tf.position.z;
        distanceOfProjectileCreator = distance + 0.2f;
    }
    private void moveProjectileCreatorAndUse() {
        double tempAngle = Math.PI*cameriaAngle/180;
        Transform tempTf = projectileCreatorGameObject.GetComponent<Transform>();
        tempTf.localRotation = Quaternion.Euler(cameriaAngle,0f,0f);
        tempTf.localPosition = new Vector3(0f,(float)(-Math.Sin(tempAngle)*distanceOfProjectileCreator)+0.7f,(float)(Math.Cos(tempAngle)*distanceOfProjectileCreator));

        useTf.localRotation = Quaternion.Euler(cameriaAngle,0f,0f);
        useTf.localPosition = new Vector3(0f,(float)(-Math.Sin(tempAngle)*distanceOfUse)+0.7f,(float)(Math.Cos(tempAngle)*distanceOfUse));
    }
    private void changeWeapon(GameObject w) {
        Weapon tempWp = w.GetComponent<Weapon>();
        MeleeWeapon tempMeWp = w.GetComponent<MeleeWeapon>();
        int index = projectileCreator.indexToSwapWith(w,tempMeWp != null);
        if (index == -1)
        return;
        GameObject outWeapon = projectileCreator.swapWeapon(index, w);
        if (outWeapon != null)
        removeWeapon(outWeapon);
        Transform inWpTf = w.GetComponent<Transform>();
        inWpTf.SetParent(tf.GetChild(5));
        if (tempWp != null)
        inWpTf.localPosition = weaponLocationTf.localPosition - tempWp.getHandPosition().GetComponent<Transform>().localPosition;
        else if (tempMeWp != null)
        inWpTf.localPosition = weaponLocationTf.localPosition - tempMeWp.getHandPosition().GetComponent<Transform>().localPosition;
        
        inWpTf.localRotation = Quaternion.Euler(0f,0f,0f);
        w.layer = LayerMask.NameToLayer("EquippedDrops");
        Transform[] oTemp = w.GetComponentsInChildren<Transform>();
        for (int i = 0; i < oTemp.Length; i++) {
            oTemp[i].gameObject.layer = LayerMask.NameToLayer("EquippedDrops");
        }
        w.GetComponent<Rigidbody>().isKinematic = true;
        controller.removeWeapon(w);
        controller.removeMeleeWeapon(w);
        if (projectileCreator.getCurrentWeaponSlot() != index) {
            w.SetActive(false);
        }
        if (projectileCreator.getCurrentWeaponSlot() == index && index != 0) {
            setProjectileCreatorDistance(tempWp);
            aimCameraTf.position = tempWp.getAimPosition().GetComponent<Transform>().position;
        }
            
    }
    private void removeWeapon(GameObject w) {
        Transform tempTransform = w.GetComponent<Transform>();
        tempTransform.SetParent(controller.GetComponent<Transform>());
        tempTransform.position = useTf.position;
        tempTransform.localRotation = useTf.rotation;
        Rigidbody tempRigidbody = w.GetComponent<Rigidbody>();
        tempRigidbody.GetComponent<Rigidbody>().isKinematic = false;
        tempRigidbody.AddRelativeForce(new Vector3(0,0,500f));
        w.layer = LayerMask.NameToLayer("Drops");
        Transform[] oTemp = tempTransform.gameObject.GetComponentsInChildren<Transform>();
            for (int i = 0; i < oTemp.Length; i++) {
                oTemp[i].gameObject.layer = LayerMask.NameToLayer("Drops");
            }
        controller.addWeapon(w);
    }
    public void movePlayer(float x, float y, float z) {
        rb.AddForce(new Vector3(x,y,z),ForceMode.VelocityChange);
    }
    void FixedUpdate() {
        double tempAngle = Math.PI*angle/180, tempAngleP = tempAngle+(Math.PI/2);
        if (tempAngleP > Math.PI) {
            tempAngleP -= Math.PI*2;
        }
        float tempMovementSpeed = movementSpeed;
        if (isCrouching) {
            tempMovementSpeed *= crouchSpeedMult;
        } else if (Input.GetButton("Sprint")) {
            tempMovementSpeed *= sprintSpeedMult;
        }
        if (holdingObstacle != null) {
            Rigidbody tempRb = holdingObstacle.GetComponent<Rigidbody>();
            Transform tempTf = holdingObstacle.GetComponent<Transform>();
            if (!tempRb.isKinematic) {
                Vector3 tempV = useTf.position - tempTf.position;
                tempV /= 10;
                tempRb.MovePosition(tempTf.position + tempV);
            }
        }
        double increaseZ = Input.GetAxis("Vertical")*Math.Cos(tempAngle)*tempMovementSpeed + Input.GetAxis("Horizontal")*Math.Cos(tempAngleP)*tempMovementSpeed,
        increaseX = Input.GetAxis("Vertical")*Math.Sin(tempAngle)*tempMovementSpeed + Input.GetAxis("Horizontal")*Math.Sin(tempAngleP)*tempMovementSpeed;
        rb.AddForce(new Vector3((float)(increaseX),rb.velocity.y,(float)(increaseZ)) - rb.velocity, ForceMode.VelocityChange);

        if (Input.GetButtonDown("Jump") && isGrounded) {
            //rb.velocity = new Vector3(x,jump,z);
            rb.AddForce(new Vector3(0,jump,0),ForceMode.VelocityChange);
        }
    }
    // Update is called once per frame
    void Update()
    {
        
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

            if (typeOfHighlight == 0 || typeOfHighlight == 2) {
                highlightedUse = "Press E to swap weapons with " + tempObject.name;
            } else if (typeOfHighlight == 1) {
                highlightedUse = "Press E to move " + tempObject.name;
            } else {
                highlightedUse = "";
            }

            if (Input.GetButtonDown("Use")) {
                if (typeOfHighlight == 0 || typeOfHighlight == 2) {
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
            GameObject droppingWeapon = projectileCreator.removeCurrentWeapon();
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
        if (projectileCreator.getCurrentWeapon() != null) {
            if (Input.GetButtonDown("Fire2")) {
                aimCameraObject.GetComponent<Camera>().enabled = true;
                tf.GetChild(0).gameObject.GetComponent<Camera>().enabled = false;
            } else if (Input.GetButtonUp("Fire2")) {
                aimCameraObject.GetComponent<Camera>().enabled = false;
                tf.GetChild(0).gameObject.GetComponent<Camera>().enabled = true;
            }
        }
        if (Input.GetButtonDown("Reload")) {
            projectileCreator.reload();
        }
        for (int i = 0; i < projectileCreator.getWeaponSlotLength(); i++) {
            if (Input.GetKeyDown(""+i)) {
                projectileCreator.setCurrentWeaponSlot(i-1);
                if (i-1 != 0 && projectileCreator.getCurrentWeapon() != null) {
                    setProjectileCreatorDistance(projectileCreator.getCurrentWeapon().GetComponent<Weapon>());
                    aimCameraTf.position = projectileCreator.getCurrentWeapon().GetComponent<Weapon>().getAimPosition().GetComponent<Transform>().position;
                }
            }
        }
        if (Input.GetButtonDown("Crouch")) {
            tf.position = new Vector3(tf.position.x,tf.position.y-0.4f,tf.position.z);
            movementHitboxTf.localPosition = new Vector3(0,0.05f,0);
            movementHitboxTf.localScale = new Vector3(1.5f,1.15f,1.5f);
            isCrouching = true;
        } else if (Input.GetButtonUp("Crouch")) {
            isCrouching = false;
            movementHitboxTf.localPosition = new Vector3(0,-0.145f,0);
            movementHitboxTf.localScale = new Vector3(1.5f,1.335f,1.5f);
            if (isGrounded) {
                tf.position = new Vector3(tf.position.x,tf.position.y+0.4f,tf.position.z);
            }
        }
        moveProjectileCreatorAndUse();
    }
    
}
