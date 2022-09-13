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
    [SerializeField] GameObject weaponControllerObject;
    private WeaponController weaponController;
    private ProjectileCreator projectileCreator;
    private Transform useTf;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        tf = GetComponent<Transform>();
        weaponController = weaponControllerObject.GetComponent<WeaponController>();
        projectileCreator = tf.GetChild(0).GetChild(0).GetComponent<ProjectileCreator>();
        useTf = tf.GetChild(0).GetChild(1).GetComponent<Transform>();
        angle = 0.0f;
        cameriaAngle = 0.0f;
        isGrounded = false;
        isCrouching = false;
    }
    public float getAngle() {
        return angle;
    }
    public void rotatePlayer(float an) {        
        rb.MoveRotation(rb.rotation * Quaternion.Euler(new Vector3(0,an,0)));
        angle = rb.rotation.eulerAngles.y-360.0f;
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
    private void changeWeapon(Weapon w) {
        int index = projectileCreator.indexToSwapWith(w);
        if (projectileCreator.getCurerntWeaponSlot()!=-1 || index != -1) {  
            Weapon tempWeapon2 = projectileCreator.swapWeapon(index, w);
            if (tempWeapon2 != null) {
                removeWeapon(tempWeapon2);
            }
            Transform tempTransform = w.GetComponent<Transform>();
            tempTransform.SetParent(tf.GetChild(5));
            tempTransform.localPosition = new Vector3(-0.27f,0.22f,0.47f);
            tempTransform.localRotation = Quaternion.Euler(0f,0f,0f);
            w.GetComponent<Rigidbody>().isKinematic = true;
            weaponController.removeWeapon(w);
            if (index != -1 && projectileCreator.getCurrentWeaponSlot() != index) {
                tempTransform.gameObject.SetActive(false);
            }
        }
    }
    private void removeWeapon(Weapon w) {
        Transform tempTransform = w.GetComponent<Transform>();
        tempTransform.SetParent(weaponControllerObject.GetComponent<Transform>());
        tempTransform.position = useTf.position;
        tempTransform.localRotation = useTf.rotation;
        Rigidbody tempRigidbody = w.GetComponent<Rigidbody>();
        tempRigidbody.GetComponent<Rigidbody>().isKinematic = false;
        tempRigidbody.AddRelativeForce(new Vector3(0,0,500f));
        weaponController.addWeapon(w);
    }
    public void movePlayer(float z) {
        double tempAngle = Math.PI*angle/180;
        double increaseZ = Math.Cos(tempAngle)*z,
        increaseX = Math.Sin(tempAngle)*z;
        Debug.Log(increaseX + " " + increaseZ);
        rb.velocity = new Vector3((float)(increaseX)+rb.velocity.x,rb.velocity.y,(float)(increaseZ)+rb.velocity.z);
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
        if (Input.GetButtonDown("Use")) {
            Weapon tempWeapon = weaponController.changeWeapon(useTf.position, 2.5f);
            if (tempWeapon != null) {
                changeWeapon(tempWeapon);
            }
        }
        if (Input.GetButtonDown("Fire2")) {
            removeWeapon(projectileCreator.removeCurrentWeapon());
        }
        if (Input.GetButtonDown("Reload")) {
            projectileCreator.reload();
        }
        for (int i = 0; i < projectileCreator.getWeaponSlotLength(); i++) {
            if (Input.GetKeyDown(""+i)) {
                projectileCreator.setCurrentWeaponSlot(i-1);
            }
        }
        double increaseZ = Input.GetAxis("Vertical")*Math.Cos(tempAngle)*tempMovementSpeed + Input.GetAxis("Horizontal")*Math.Cos(tempAngleP)*tempMovementSpeed,
        increaseX = Input.GetAxis("Vertical")*Math.Sin(tempAngle)*tempMovementSpeed + Input.GetAxis("Horizontal")*Math.Sin(tempAngleP)*tempMovementSpeed;
        rb.velocity = new Vector3((float)(increaseX),y,(float)(increaseZ));

      //  if (Math.Abs(rb.velocity.x) <= 5)
       // rb.AddForce(Input.GetAxis("Horizontal") * movementSpeed,0,0,ForceMode.VelocityChange);
       
       
       //rotatePlayer(Input.GetAxis("Horizontal")*Time.deltaTime);
        
        if (Input.GetButtonDown("Jump") && isGrounded) {
            //rb.velocity = new Vector3(x,jump,z);
            rb.velocity = new Vector3(x,jump,z);
        }
    }
}
