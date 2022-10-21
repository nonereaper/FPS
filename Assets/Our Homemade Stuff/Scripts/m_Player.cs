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
    private NetworkVariable<int> characterMovementState = new NetworkVariable<int>();
    // camera's angle up and down
    private NetworkVariable<float> cameraAngle = new NetworkVariable<float>();
    // character's angle left and right 
    private NetworkVariable<float> characterAngle = new NetworkVariable<float>();
    float distanceOfProjSpawn;
    float distanceOfUseSelector;

    private m_Controller controller;

    private GameObject[] weaponBar;
    private int currentWeaponIndex;
    private float time;
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
    [SerializeField] private TMP_Text itemInfoText;

    private bool lockCursor;

    public override void OnNetworkSpawn() {
        cameraAngle.Value = 0f;
        characterAngle.Value = 0f;
        characterMovementState.Value = 0;
        controller = GameObject.Find("Controller").GetComponent<m_Controller>();

        savedTime.Value = UnityEngine.Time.time;
        useText.text = "";
        lockCursor = true;
        Cursor.lockState = CursorLockMode.Locked;
        if (IsServer) {
            rb = GetComponent<Rigidbody>();
            heldProp = null;
            time = UnityEngine.Time.time;
            distanceOfProjSpawn = emptyProjectile.transform.localPosition.z;
            distanceOfUseSelector = emptyUse.transform.localPosition.z;
            
            weaponBar = new GameObject[5];
            currentWeaponIndex = 0;
        }
    }
    [ServerRpc]
    private void setTextItemTextServerRpc() {
        string temp = "" + isGrounded() + "\n";
        for (int i = 0; i < weaponBar.Length; i++) {
            temp+="Weapon Slot " +(i+1) + " is: ";
            if (weaponBar[i] == null) {
            temp+= "empty.\n" ;
            } else {
            temp+= weaponBar[i].name + "\n";
            }
        }
        if (weaponBar[currentWeaponIndex] != null) {
            Weapon currentWeapon = weaponBar[currentWeaponIndex].GetComponent<Weapon>();
            if (currentWeapon != null) {
                temp += "Magazine: (" + currentWeapon.getCurrentMagazine() + "/" + currentWeapon.getMagazine() + ")\n";
                temp += "Stored ammo Left: (" + currentWeapon.getCurrentTotalAmmo() + "/" + currentWeapon.getTotalAmmo() + ")";
            }
        }
        setTextItemTextClientRpc(temp);
    }
    [ClientRpc]
    private void setTextItemTextClientRpc(string t) {
        if (IsOwner && IsClient) {
            itemInfoText.text = t;
        }
    }
    [ServerRpc]
    private void movePropServerRpc() {
        if (heldProp != null) {
            Vector3 tempV = emptyUse.transform.position - heldProp.transform.position;
            tempV /= 10;
            heldProp.GetComponent<Rigidbody>().MovePosition(heldProp.transform.position + tempV);
        }
    }
    [ServerRpc]
    private void holdPropServerRpc(int index) {
        heldProp = controller.getProp(index);
        heldProp.GetComponent<Rigidbody>().useGravity = false;
        heldProp.layer = LayerMask.NameToLayer("IgnoreCollisions");
        Transform[] oTemp = heldProp.GetComponentsInChildren<Transform>();
        for (int i = 0; i < oTemp.Length; i++) {
            oTemp[i].gameObject.layer = LayerMask.NameToLayer("IgnoreCollisions");
        }
    }
    [ServerRpc]
    private void dropPropServerRpc() {
        heldProp.GetComponent<Rigidbody>().useGravity = true;
        heldProp.layer = LayerMask.NameToLayer("Moveable Objects");
        Transform[] oTemp = heldProp.GetComponentsInChildren<Transform>();
        for (int i = 0; i < oTemp.Length; i++) {
            oTemp[i].gameObject.layer = LayerMask.NameToLayer("Moveable Objects");
        }
        heldProp = null;
    }
    [ServerRpc]
    private void reloadServerRpc() {

    }
    [ServerRpc]
    private void useWeaponServerRpc(bool buttonDown) {
        GameObject weapon = weaponBar[currentWeaponIndex];
        if (weapon == null) {
            return;
        }
        Weapon gun = weapon.GetComponent<Weapon>();
        if (gun != null) {
            if (heldProp != null) {
                return;
            }
            if (gun.getReloadTimeLeft() != 0f || UnityEngine.Time.time < time+gun.getAttackSpeed()) {
                return;
            } else if (gun.getCurrentMagazine() == 0) { // no ammo to shoot
                reloadServerRpc();
                return;
            }
            if (!((buttonDown && gun.isSemiAuto()) || (!buttonDown && gun.isAuto()))) {
                return;
            }
            gun.setCurrentMagazine(gun.getCurrentMagazine()-1);
            time = UnityEngine.Time.time;
            
            double spreadMult = 1.0;
            if (characterMovementState.Value == 1) {
                spreadMult = gun.getCrouchSpread();
            } else if (characterMovementState.Value == 2) {
                spreadMult = gun.getSprintSpread();
            }
            rotateCameraServerRpc(gun.getRecoil());
            float spread = (float)(gun.getSpread()*spreadMult);
                for (int i = 0; i < gun.getNumber(); i++) {
                    Transform tf2 = gun.getMussle().transform;
                    float rotationTempX = tf2.rotation.eulerAngles.x + UnityEngine.Random.Range(-spread,spread),
                    rotationTempY = tf2.rotation.eulerAngles.y + UnityEngine.Random.Range(-spread,spread);
                    Quaternion q = Quaternion.Euler(rotationTempX, rotationTempY, tf2.rotation.eulerAngles.z);
                    GameObject o = Instantiate(gun.getProj(),tf2.position,q,controller.getProjectileTf());
                    o.GetComponent<NetworkObject>().Spawn();
                    o.GetComponent<Rigidbody>().AddRelativeForce(new Vector3(0,0,gun.getVelocity()));
                    //o.GetComponent<Projectile>().setup(gun.getDamage(),transform,10);
                    controller.addProjectile(o);
                }
            double angleOfCamera = cameraAngle.Value/180*Math.PI;
            double increaseY = Math.Sin(angleOfCamera)*gun.getBackBlast(), increaseZ = Math.Cos(angleOfCamera)*gun.getBackBlast();
            movePlayerServerRpc(false,true,true,0f,(float)increaseY,-(float)increaseZ);
            GameObject o2 = Instantiate(gun.getFireExplosion(),gun.getMussle().transform.position,gun.getMussle().transform.rotation,controller.getDecayTf());
            o2.GetComponent<NetworkObject>().Spawn();
            if (gun.isLaunchShells()) {
                GameObject o3 = Instantiate(gun.getShells(),gun.getShellPosition().transform.position,gun.getShellPosition().transform.rotation,controller.getDecayTf());
                o3.GetComponent<Rigidbody>().AddRelativeForce(new Vector3(gun.getShellForce(),0,0));
                o3.GetComponent<NetworkObject>().Spawn();
            }    
        }
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
                distanceOfProjSpawn = distance + 0.2f;
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
        weapon.transform.SetParent(rightArm.transform);
        weapon.transform.position = emptyWeaponLocation.transform.position;
        weapon.transform.rotation = emptyWeaponLocation.transform.rotation;
        weapon.GetComponent<Rigidbody>().isKinematic = true;
        weapon.layer = LayerMask.NameToLayer("IgnoreCollisions");
        Transform[] oTemp = weapon.GetComponentsInChildren<Transform>();
        for (int i = 0; i < oTemp.Length; i++) {
            oTemp[i].gameObject.layer = LayerMask.NameToLayer("IgnoreCollisions");
        }

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
            outWeapon.GetComponent<Rigidbody>().isKinematic = true;
            outWeapon.layer = LayerMask.NameToLayer("Moveable Objects");
            Transform[] oTemp = outWeapon.GetComponentsInChildren<Transform>();
            for (int i = 0; i < oTemp.Length; i++) {
                oTemp[i].gameObject.layer = LayerMask.NameToLayer("Moveable Objects");
            }
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
        cameraAngle.Value = angle2;
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
        emptyProjectile.transform.localPosition = new Vector3(0f,(float)(-Math.Sin(angle)*distanceOfProjSpawn)+0.7f,(float)(Math.Cos(angle)*distanceOfProjSpawn));

        emptyUse.transform.localRotation = Quaternion.Euler((float)(angle*180/Math.PI),0f,0f);
        emptyUse.transform.localPosition = new Vector3(0f,(float)(-Math.Sin(angle)*distanceOfUseSelector)+0.7f,(float)(Math.Cos(angle)*distanceOfUseSelector));
    }
    [ServerRpc]
    private void movePlayerServerRpc(bool changeX, bool changeY, bool changeZ, float x, float y, float z) {
        Vector3 temp = new Vector3(rb.velocity.x,rb.velocity.y,rb.velocity.z);
        if (changeX)
        temp.x = x;
        if (changeY)
        temp.y = y;
        if (changeZ)
        temp.z = z;
        rb.AddForce(temp - rb.velocity, ForceMode.VelocityChange);
    }
    [ServerRpc]
    private void crouchPlayerServerRpc() {
        Transform upperLTf = leftLeg.transform.GetChild(0), lowerLTf = leftLeg.transform.GetChild(1),
        upperRTf = rightLeg.transform.GetChild(0), lowerRTf = rightLeg.transform.GetChild(1);
        if (characterMovementState.Value == 1) {
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
        Physics.CheckBox(leftLeg.transform.GetChild(1).position,new Vector3(leftLeg.transform.GetChild(1).localScale.x/4,leftLeg.transform.GetChild(1).localScale.y/2+0.2f,leftLeg.transform.GetChild(1).localScale.z/4),leftLeg.transform.GetChild(1).rotation,LayerMask.NameToLayer("Ground")) ||
        Physics.CheckBox(rightLeg.transform.GetChild(1).position,new Vector3(rightLeg.transform.GetChild(1).localScale.x/4,rightLeg.transform.GetChild(1).localScale.y/2+0.2f,rightLeg.transform.GetChild(1).localScale.z/4),rightLeg.transform.GetChild(1).rotation,LayerMask.NameToLayer("Ground"));
    }
    void FixedUpdate() {
        if (!IsOwner) return;
        rotatePlayerServerRpc(Input.GetAxis("Mouse X") * Time.fixedDeltaTime * mouseSen);
        rotateCameraServerRpc(Input.GetAxis("Mouse Y") * Time.fixedDeltaTime * mouseSen);

        if (Input.GetButtonDown("Sprint")) {
            if (characterMovementState.Value == 1) {
                characterMovementState.Value = 0;
                crouchPlayerServerRpc();
            } 
            characterMovementState.Value = 2;
        } else {
            bool changeInState = false;
            if (Input.GetButtonDown("Crouch")) {
                characterMovementState.Value = 1;
                changeInState = true;
            } 
            if (Input.GetButtonUp("Crouch")) {
                characterMovementState.Value = 0;
                changeInState = true;
            }
            if (changeInState) {
                crouchPlayerServerRpc();
            }
        }
        
        double tempAngle = characterAngle.Value/180*Math.PI, tempAngleP = tempAngle+(Math.PI/2);
        float tempMovementSpeed = movementSpeed;
        if (characterMovementState.Value == 1)
            tempMovementSpeed *= crouchSpeedMult;
        else if (characterMovementState.Value == 2)
            tempMovementSpeed *= sprintSpeedMult;
        double increaseZ = Input.GetAxis("Vertical")*Math.Cos(tempAngle)*tempMovementSpeed + Input.GetAxis("Horizontal")*Math.Cos(tempAngleP)*tempMovementSpeed,
        increaseX = Input.GetAxis("Vertical")*Math.Sin(tempAngle)*tempMovementSpeed + Input.GetAxis("Horizontal")*Math.Sin(tempAngleP)*tempMovementSpeed;
        if (Input.GetButtonDown("Jump") && isGrounded()) {
            movePlayerServerRpc(true,true,true,(float)increaseX,jump,(float)increaseZ);
        } else {
            movePlayerServerRpc(true,false,true,(float)increaseX,0f,(float)increaseZ);
        }
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
        bool canFire = Input.GetButton("Fire1") || Input.GetButtonDown("Fire1");
        if (canFire) {
            useWeaponServerRpc(Input.GetButtonDown("Fire1"));
        }
        if (Input.GetButtonDown("LockMouse")) {
            lockCursor = !lockCursor;
            if (lockCursor) {
                Cursor.lockState = CursorLockMode.Locked;
            } else {
                Cursor.lockState = CursorLockMode.None;
            }
        }
        setTextItemTextServerRpc();
    }
}
