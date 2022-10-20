using System.Collections;
using System.Collections.Generic;
using System;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class m_Player : NetworkBehaviour
{
    // rigidbody for this player
    private Rigidbody rb;
    // velocity for player
    [SerializeField] private float movementSpeed;
    // multiplier for movespeed
    [SerializeField] private float sprintSpeedMult;
    // multiplier for movespeed
    [SerializeField] private float crouchSpeedMult;
    // velocity upward
    [SerializeField] private float jump;
    // mouse sen
    [SerializeField] private float mouseSen;
    // ground layer
    [SerializeField] LayerMask ground;

    // is character crouching
    private NetworkVariable<bool> isCrouching = new NetworkVariable<bool>();
    // camera's angle up and down
    private NetworkVariable<float> cameraAngle = new NetworkVariable<float>();
    // character's angle left and right 
    private NetworkVariable<float> characterAngle = new NetworkVariable<float>();
    // the distance from the camera to the use empty
    private NetworkVariable<float> distanceOfUse = new NetworkVariable<float>();
    // the distance from the camera to the projectile empty 
    private NetworkVariable<float> distanceOfProjectile = new NetworkVariable<float>();

    private m_Controller controller;

    private GameObject[] weaponBar;
    private int currentWeaponIndex;
    private NetworkVariable<float> savedTime = new NetworkVariable<float>();

    [SerializeField] private GameObject camera;
    [SerializeField] private GameObject leftArm;
    [SerializeField] private GameObject rightArm;
    [SerializeField] private GameObject emptyWeaponLocation;
    [SerializeField] private GameObject aimCamera;
    [SerializeField] private GameObject leftLeg;
    [SerializeField] private GameObject rightLeg;
    [SerializeField] private GameObject movementHitbox;
    [SerializeField] private GameObject emptyUse;
    [SerializeField] private GameObject emptyProjectile;

    private GameObject heldProp;

    [SerializeField] private TMP_Text useText;

    public override void OnNetworkSpawn() {
        cameraAngle.Value = 0f;
        characterAngle.Value = 0f;
        rb = GetComponent<Rigidbody>();
        isCrouching.Value = false;
        heldProp = null;
        distanceOfProjectile.Value = emptyProjectile.transform.localPosition.z;
        distanceOfUse.Value = emptyUse.transform.localPosition.z;
        controller = GameObject.Find("Controller").GetComponent<m_Controller>();
        weaponBar = new GameObject[5];
        currentWeaponIndex = 0;
        savedTime.Value = UnityEngine.Time.time;
        useText.text = "";
    }
    [ServerRpc]
    private void holdPropServerRpc(int index) {
        heldProp = controller.getProp(index);
        heldProp.GetComponent<Rigidbody>().useGravity = false;
        
    }
    [ServerRpc]
    private void dropPropServerRpc() {
        heldProp = null;
    }
    [ServerRpc]
    private void swapWeaponToServerRpc(int index) {
        GameObject pastWeapon = weaponBar[currentWeaponIndex];
        if (pastWeapon != null) {
            pastWeapon.SetActive(false);
        }
        currentWeaponIndex = index;
        GameObject currentWeapon = weaponBar[currentWeaponIndex];
        if (currentWeapon != null) {
            pastWeapon.SetActive(true);
            if (currentWeaponIndex != 0) {
                float distance = currentWeapon.GetComponent<Weapon>().getMussle().transform.position.z-transform.position.z;
                distanceOfProjectile.Value = distance + 0.2f;
                aimCamera.transform.position = currentWeapon.GetComponent<Weapon>().getAimPosition().transform.position;
            }
        }
    }
    [ServerRpc]
    private void addWeaponToPlayerServerRpc(int weaponTakenIndex) {
        GameObject weapon = controller.getWeapon(weaponTakenIndex);
        
        int index = findWeaponSlot(weapon.GetComponent<MeleeWeapon>() != null);
       
        removeWeaponCurrentServerRpc(index);

        controller.removeWeapon(weapon);
        weaponBar[index] = weapon;
        weapon.transform.SetParent(leftArm.transform);
        if (index == currentWeaponIndex) {
            swapWeaponToServerRpc(index);
        }
    }
    [ServerRpc]
    private void removeWeaponCurrentServerRpc(int index) {
        GameObject outWeapon = weaponBar[index];
        weaponBar[index] = null;
        if (outWeapon != null) {
            outWeapon.transform.SetParent(controller.getWeaponTf());
            controller.addWeapon(outWeapon);
            outWeapon.SetActive(true);
        }
    }
    private int findWeaponSlot(bool isMeleeWeapon) {
        if (isMeleeWeapon)
        return 0;
        for (int i = 1; i < weaponBar.Length; i++) {
            if (weaponBar[i] == null)
            return i;
        }
        return currentWeaponIndex;
    }
    [ServerRpc]
    private void rotatePlayerServerRpc(float angle) {
        rb.MoveRotation(rb.rotation * Quaternion.Euler(new Vector3(0,angle,0)));
        characterAngle.Value = rb.rotation.eulerAngles.y;
    }
    [ServerRpc]
    private void rotateCameraServerRpc(float angle) {
        float angle2 = cameraAngle.Value - angle;
        if (angle2 > 90f)
        angle2 = 90f;
        else if (angle2 < -90f)
        angle2 = -90f;
        characterAngle.Value = angle2;
        rotateArmsServerRpc(angle2/180*Math.PI);
        moveProjectileCreatorAndUseServerRpc(angle2/180*Math.PI);
        camera.transform.localRotation = Quaternion.Euler(angle2,0f,0f);
    }
    [ServerRpc]
    private void rotateArmsServerRpc(double angle) {
        double changeX = Math.Cos(angle)*0.25, changeY = Math.Sin(angle)*0.25;
        leftArm.transform.localPosition = new Vector3(leftArm.transform.localPosition.x,(float)(changeY+0.23f),(float)changeX);
        leftArm.transform.localRotation = Quaternion.Euler((float)(angle*180/Math.PI),0,0);
        rightArm.transform.localPosition = new Vector3(rightArm.transform.localPosition.x,(float)(changeY+0.23f),(float)changeX);
        rightArm.transform.localRotation = Quaternion.Euler((float)(angle*180/Math.PI),0,0);
    }
    [ServerRpc]
    private void moveProjectileCreatorAndUseServerRpc(double angle) {
        emptyProjectile.transform.localRotation = Quaternion.Euler((float)(angle*180/Math.PI),0f,0f);
        emptyProjectile.transform.localPosition = new Vector3(0f,(float)(-Math.Sin(angle)*distanceOfProjectile.Value)+0.7f,(float)(Math.Cos(angle)*distanceOfProjectile.Value));

        emptyUse.transform.localRotation = Quaternion.Euler((float)(angle*180/Math.PI),0f,0f);
        emptyUse.transform.localPosition = new Vector3(0f,(float)(-Math.Sin(angle)*distanceOfUse.Value)+0.7f,(float)(Math.Cos(angle)*distanceOfUse.Value));
    }
    [ServerRpc]
    private void movePlayerServerRpc(Vector3 velocity) {
        rb.AddForce(velocity - rb.velocity, ForceMode.VelocityChange);
    }
    [ServerRpc]
    private void crouchPlayerServerRpc() {
        Transform upperLTf = leftLeg.transform.GetChild(0), lowerLTf = leftLeg.transform.GetChild(1),
        upperRTf = rightLeg.transform.GetChild(0), lowerRTf = rightLeg.transform.GetChild(1);
        if (isCrouching.Value) {
            upperLTf.localPosition = new Vector3(0,0.4f,0.15f);
            upperLTf.localRotation = Quaternion.Euler(-25,0,0);
            lowerLTf.localPosition = new Vector3(0,0.15f,-0.15f);
            upperRTf.localPosition = new Vector3(0,0.4f,0.15f);
            upperRTf.localRotation = Quaternion.Euler(-25,0,0);
            lowerRTf.localPosition = new Vector3(0,0.15f,-0.15f);
            movementHitbox.transform.localPosition = new Vector3(0,0.05f,0);
            movementHitbox.transform.localScale = new Vector3(1.5f,1.15f,1.5f);
        } else {
            upperLTf.localPosition = new Vector3(0,0.25f,0);
            upperLTf.localRotation = Quaternion.Euler(0,0,0);
            lowerLTf.localPosition = new Vector3(0,-0.25f,-0);
            upperRTf.localPosition = new Vector3(0,0.25f,0);
            upperRTf.localRotation = Quaternion.Euler(0,0,0);
            lowerRTf.localPosition = new Vector3(0,-0.25f,-0);
            movementHitbox.transform.localPosition = new Vector3(0,-0.145f,0);
            movementHitbox.transform.localScale = new Vector3(1.5f,1.335f,1.5f);
        }
    }
    private bool isGrounded() {
        return 
        Physics.CheckBox(leftLeg.transform.GetChild(1).position,new Vector3(leftLeg.transform.GetChild(1).localScale.x/2,leftLeg.transform.GetChild(1).localScale.y/2+0.2f,leftLeg.transform.GetChild(1).localScale.z/2),leftLeg.transform.GetChild(1).rotation,ground) ||
        Physics.CheckBox(rightLeg.transform.GetChild(1).position,new Vector3(rightLeg.transform.GetChild(1).localScale.x/2,rightLeg.transform.GetChild(1).localScale.y/2+0.2f,rightLeg.transform.GetChild(1).localScale.z/2),rightLeg.transform.GetChild(1).rotation,ground);
    }
    void FixedUpdate() {
        if (!IsOwner) return;
        rotatePlayerServerRpc(Input.GetAxis("Mouse X") * Time.deltaTime * mouseSen);
        rotateCameraServerRpc(Input.GetAxis("Mouse Y") * Time.deltaTime * mouseSen);

        if (Input.GetButtonDown("Crouch")) {
            isCrouching.Value = true;
            crouchPlayerServerRpc();
        } else if (Input.GetButtonUp("Crouch")) {
            isCrouching.Value = false;
            crouchPlayerServerRpc();
        }
        
        double tempAngle = characterAngle.Value/180*Math.PI, tempAngleP = tempAngle+(Math.PI/2);
        float tempMovementSpeed = movementSpeed;
        if (isCrouching.Value)
            tempMovementSpeed *= crouchSpeedMult;
        else if (Input.GetButton("Sprint"))
            tempMovementSpeed *= sprintSpeedMult;
        double increaseZ = Input.GetAxis("Vertical")/*Math.Cos(tempAngle)*/*tempMovementSpeed + Input.GetAxis("Horizontal")/*Math.Cos(tempAngleP)*/*tempMovementSpeed,
        increaseX = Input.GetAxis("Vertical")/*Math.Sin(tempAngle)*/*tempMovementSpeed + Input.GetAxis("Horizontal")/*Math.Sin(tempAngleP)*/*tempMovementSpeed;
        float increaseY = rb.velocity.y;
        if (Input.GetButtonDown("Jump") && isGrounded()) {
            increaseY = jump;
        }
        movePlayerServerRpc(new Vector3((float)increaseX,increaseY,(float)increaseZ));
    }
    // Update is called once per frame
    void Update()
    {
        if (!IsOwner) return;
        int weaponIndex = controller.getClosestWeapon(emptyUse.transform.position,5f);
        float weaponDistance = controller.getSavedDistance();
        int propIndex = controller.getClosestProp(emptyUse.transform.position,5f);
        float propDistance = controller.getSavedDistance();
        int typeToUse = -1;
        if (weaponIndex != -1 && propIndex != -1) {
            if (weaponDistance > propDistance) {
                typeToUse = 0;
            } else {
                typeToUse = 1;
            }
        } else if (weaponIndex != -1) {
            typeToUse = 0;
        } else if (propIndex != -1) {
            typeToUse = 1;
        }
        if (typeToUse == 0) {
            GameObject weapon = controller.getWeapon(weaponIndex);
            useText.text = "Equip: " + weapon.name;
        } else if (typeToUse == 1) {
            GameObject prop = controller.getProp(propIndex);
            useText.text = "Pick up: " + prop.name;
        } else {
            useText.text = "";
        }
        if (Input.GetButtonDown("Use")) {
            if (typeToUse == 0) {
                addWeaponToPlayerServerRpc(weaponIndex);
            } else if (typeToUse == 1) {

            }
        }
    }
}
