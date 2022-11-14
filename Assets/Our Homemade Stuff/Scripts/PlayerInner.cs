using System.Collections;
using System.Collections.Generic;
using System;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class PlayerInner : MonoBehaviour
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
        public float getMouseSen() {
            return this.mouseSen;
        }
    [SerializeField] private float sprintMaxTime;

    private float currentSprintTime;
    private float currentTimeBeforeRestore;
    [SerializeField] private double restoreSprintTimeMult;
    [SerializeField] private float timeBeforeStartRestoreSprint;
    
    // is character crouching
    private int characterMovementState;
    // camera's angle up and down
    private float cameraAngle;
    // character's angle left and right 
    private float characterAngle;
    float distanceOfProjSpawn;
    float distanceOfUseSelector;

    private Controller controller;

    private GameObject[] weaponBar;
        public int getWeaponBarSize() {
            return weaponBar.Length;
        }
    private int currentWeaponIndex;
    private float swapWeaponTime;
    private float fireWeapontime;
    private float savedTime;

    [SerializeField] private GameObject mainCamera;
    [SerializeField] private GameObject feetHitbox;
    [SerializeField] private GameObject leftArm;
    [SerializeField] private GameObject rightArm;
    [SerializeField] private GameObject head;
    [SerializeField] private GameObject emptyWeaponLocation;
    [SerializeField] private GameObject aimCamera;
    [SerializeField] private GameObject leftLeg;
    [SerializeField] private GameObject rightLeg;
    [SerializeField] private GameObject movementHitbox;
    [SerializeField] private GameObject emptyUse;
    [SerializeField] private GameObject emptyProjectile;

    private GameObject heldProp;

    [SerializeField] private TMP_Text useText;
        public string getUseText() {
            return useText.text;
        }
    [SerializeField] private TMP_Text itemInfoText;

    private bool lockCursor;
    // Start is called before the first frame update
    
    
    void Start()
    {
        cameraAngle = 0f;
        characterAngle = 0f;
        characterMovementState = 0;
        controller = GameObject.Find("Controller").GetComponent<Controller>();

        currentSprintTime = sprintMaxTime;
        currentTimeBeforeRestore = 0f;

        useText.text = "";
        lockCursor = true;
        Cursor.lockState = CursorLockMode.Locked;

        rb = GetComponent<Rigidbody>();
        heldProp = null;

        savedTime = UnityEngine.Time.time;
        fireWeapontime = 0f;
        swapWeaponTime = 0f;
        distanceOfProjSpawn = emptyProjectile.transform.localPosition.z;
        distanceOfUseSelector = emptyUse.transform.localPosition.z;

        
        weaponBar = new GameObject[5];
        currentWeaponIndex = 0;
    }
    public void updateItemInfo() {
        string temp = "" + isGrounded() + "\n";
        temp += "State of Character: " + characterMovementState + "\n";
        temp += "Sprint time left: " + currentSprintTime + "\n";
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
                String fireType = "Automatic";
                if (currentWeapon.getFireType() == 0) {
                    fireType = "Semi-automatic";
                }
                temp += "Fire type: " + fireType + "\n";
                temp += "Magazine: (" + currentWeapon.getCurrentMagazine() + "/" + currentWeapon.getMagazine() + ")\n";
                temp += "Stored ammo Left: (" + currentWeapon.getCurrentTotalAmmo() + "/" + currentWeapon.getTotalAmmo() + ")";
            }
        }
        itemInfoText.text = temp;
    }
    public void updateUseInfo(string s) {
        useText.text = s;
    }
    public void moveProp() {
        if (heldProp != null) {
            Vector3 tempV = emptyUse.transform.position - heldProp.transform.position;
            tempV /= 10;
            heldProp.GetComponent<Rigidbody>().MovePosition(heldProp.transform.position + tempV);
        }
    }
    public void holdProp(int index) {
        if (heldProp != null) return;
        heldProp = controller.getProp(index);
        heldProp.GetComponent<Rigidbody>().useGravity = false;
        heldProp.layer = LayerMask.NameToLayer("IgnoreCollisions");
        Transform[] oTemp = heldProp.GetComponentsInChildren<Transform>();
        for (int i = 0; i < oTemp.Length; i++) {
            oTemp[i].gameObject.layer = LayerMask.NameToLayer("IgnoreCollisions");
        }
    }
    public void dropProp() {
        if (heldProp == null) return;
        heldProp.GetComponent<Rigidbody>().useGravity = true;
        heldProp.layer = LayerMask.NameToLayer("Movable Objects");
        Transform[] oTemp = heldProp.GetComponentsInChildren<Transform>();
        for (int i = 0; i < oTemp.Length; i++) {
            oTemp[i].gameObject.layer = LayerMask.NameToLayer("Movable Objects");
        }
        heldProp = null;
    }
    public void reload() {
        if (currentWeaponIndex != 0) {
            if (weaponBar[currentWeaponIndex] == null) {
                return;
            }
            if (swapWeaponTime != 0f) {
                return;
            }
            Weapon currentWeapon = weaponBar[currentWeaponIndex].GetComponent<Weapon>();
            if (currentWeapon.getReloadTimeLeft() != 0f) {
                return;
            }
            if (currentWeapon.getCurrentTotalAmmo() == 0) {
                return;
            }
            int amountToAdd = currentWeapon.getMagazine()-currentWeapon.getCurrentMagazine();
            if (currentWeapon.getCurrentTotalAmmo() < amountToAdd) {
                amountToAdd = currentWeapon.getCurrentTotalAmmo();
            }
            currentWeapon.setCurrentMagazine(currentWeapon.getCurrentMagazine()+amountToAdd);
            currentWeapon.setCurrentTotalAmmo(currentWeapon.getCurrentTotalAmmo()-amountToAdd);
            currentWeapon.setReloadTimeLeft(currentWeapon.getReloadTime());
        }
    }
    public void changeGunFireType() {
        GameObject weapon = weaponBar[currentWeaponIndex];
        if (weapon == null) {
            return;
        }
        Weapon gun = weapon.GetComponent<Weapon>();
        if (gun.getFireType() == 0) {
            if (gun.isAuto()) {
                gun.setFireType(1);
            }
        } else if (gun.getFireType() == 1) {
            if (gun.isSemiAuto()) {
                gun.setFireType(0);
            }
        }
    }
    public void useWeapon(bool notHoldDownButton) {
        GameObject weapon = weaponBar[currentWeaponIndex];
        if (weapon == null) {
            return;
        }
        Weapon gun = weapon.GetComponent<Weapon>();
        if (gun != null) {
            if (heldProp != null) {
                return;
            }
            if (gun.getReloadTimeLeft() != 0f || fireWeapontime != 0f || swapWeaponTime != 0f) {
                return;
            }
            if (gun.getCurrentMagazine() == 0) {
                reload();
                return;
            }
            if (!((notHoldDownButton && gun.getFireType() == 0) || (!notHoldDownButton && gun.getFireType() == 1))) {
                return;
            }
            gun.setCurrentMagazine(gun.getCurrentMagazine()-1);
            fireWeapontime = gun.getAttackSpeed();
            
            double spreadMult = 1.0;
            if (characterMovementState == 1) {
                spreadMult = gun.getCrouchSpread();
            } else if (characterMovementState == 2) {
                spreadMult = gun.getSprintSpread();
            }
            float spread = (float)(gun.getSpread()*spreadMult);
            for (int i = 0; i < gun.getNumber(); i++) {
                Transform tf2 = gun.getMussle().transform;
                float rotationTempX = tf2.rotation.eulerAngles.x + UnityEngine.Random.Range(-spread,spread),
                rotationTempY = tf2.rotation.eulerAngles.y + UnityEngine.Random.Range(-spread,spread);
                Quaternion q = Quaternion.Euler(rotationTempX, rotationTempY, tf2.rotation.eulerAngles.z);
                GameObject o = Instantiate(gun.getProj(),tf2.position,q,controller.getProjectileTf());
                if (controller.isIsMult()) {
                    o.GetComponent<NetworkObject>().Spawn();
                } else {
                    o.GetComponent<Rigidbody>().isKinematic = false;
                }
                o.GetComponent<Rigidbody>().AddRelativeForce(new Vector3(0,0,gun.getVelocity()));
                o.GetComponent<Projectile>().setup(gun.getDamage(),transform.gameObject,10);
                controller.addProjectile(o);
            }
            rotateCamera(gun.getRecoil());
            GameObject o2 = Instantiate(gun.getFireExplosion(),gun.getMussle().transform.position,gun.getMussle().transform.rotation,controller.getDecayTf());
            if (controller.isIsMult()) {
                o2.GetComponent<NetworkObject>().Spawn();
            }
            if (gun.isLaunchShells()) {
                GameObject o3 = Instantiate(gun.getShells(),gun.getShellPosition().transform.position,gun.getShellPosition().transform.rotation,controller.getDecayTf());
                o3.GetComponent<Rigidbody>().AddRelativeForce(new Vector3(gun.getShellForce(),0,0));
                if (controller.isIsMult()) {
                    o3.GetComponent<NetworkObject>().Spawn();
                } else {
                    o3.GetComponent<Rigidbody>().isKinematic = false;
                }
            }    
        }
    }
    public void swapWeaponTo(int index) {
        GameObject pastWeapon = weaponBar[currentWeaponIndex];
        if (pastWeapon != null) {
            pastWeapon.SetActive(false);
        }
        currentWeaponIndex = index;
        GameObject currentWeapon = weaponBar[currentWeaponIndex];
        if (currentWeapon != null) {
            currentWeapon.SetActive(true);
            /* add swap weapon time )TODO(*/
            swapWeaponTime = 1;
            if (currentWeaponIndex != 0) {
                float distance = currentWeapon.GetComponent<Weapon>().getMussle().transform.position.z-transform.position.z;
                distanceOfProjSpawn = distance + 0.2f;
                aimCamera.transform.position = currentWeapon.GetComponent<Weapon>().getAimPosition().transform.position;
            } else {
                switchWeaponSights(false);
            }
        }
    }
    public void addWeaponToPlayer(int weaponTakenIndex) {
        GameObject weapon = controller.getWeapon(weaponTakenIndex);
        
        int index = findWeaponSlot(weapon.GetComponent<MeleeWeapon>() != null);
       
        removeWeapon(index);

        controller.removeWeapon(weapon);
        weaponBar[index] = weapon;
        //weapon.transform.SetParent(rightArm.transform);
        weapon.SetActive(false);
        weapon.transform.position = emptyWeaponLocation.transform.position;
        weapon.transform.rotation = emptyWeaponLocation.transform.rotation;
        weapon.GetComponent<Rigidbody>().isKinematic = true;
        weapon.layer = LayerMask.NameToLayer("IgnoreCollisions");
        Transform[] oTemp = weapon.GetComponentsInChildren<Transform>();
        for (int i = 0; i < oTemp.Length; i++) {
            oTemp[i].gameObject.layer = LayerMask.NameToLayer("IgnoreCollisions");
        }

        if (index == currentWeaponIndex) {
            swapWeaponTo(index);
        }
    }
    public void moveAllWeapon() {
        for (int i = 0; i < weaponBar.Length; i++) {
            if (weaponBar[i] != null) {
                weaponBar[i].transform.position = emptyWeaponLocation.transform.position;
                weaponBar[i].transform.localRotation = emptyWeaponLocation.transform.rotation;
                weaponBar[i].transform.Rotate(-90f,90f,0f);
                //weaponBar[i].transform.localRotation = Quaternion.Euler(new Vector3(weaponBar[i].transform.rotation.x,weaponBar[i].transform.rotation.y,weaponBar[i].transform.rotation.z));
            }
        }
    }
    public void removeWeapon(int index) {
        GameObject outWeapon = weaponBar[index];
        weaponBar[index] = null;
        if (outWeapon != null) {
            outWeapon.GetComponent<Rigidbody>().isKinematic = false;
            outWeapon.layer = LayerMask.NameToLayer("Movable Objects");
            Transform[] oTemp = outWeapon.GetComponentsInChildren<Transform>();
            for (int i = 0; i < oTemp.Length; i++) {
                oTemp[i].gameObject.layer = LayerMask.NameToLayer("Movable Objects");
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
    public void rotatePlayer(float angle) {
        rb.MoveRotation(rb.rotation * Quaternion.Euler(new Vector3(0,angle,0)));
        characterAngle = rb.rotation.eulerAngles.y;
    }
    public void rotateCamera(float angle) {
        float angle2 = cameraAngle - angle;
        if (angle2 > 90f)
        angle2 = 90f;
        else if (angle2 < -90f)
        angle2 = -90f;
        cameraAngle = angle2;
        //rotateArms(angle2);
        mainCamera.transform.position = head.transform.position;
        moveProjectileCreatorAndUse(angle2);
        mainCamera.transform.localRotation = Quaternion.Euler(angle2,0f,0f);
    }
    public void rotateArms(float angle) {
        double dAngle = -angle/180*Math.PI;
        double changeX = Math.Cos(dAngle)*0.25, changeY = Math.Sin(dAngle)*0.25;
        leftArm.transform.localPosition = new Vector3(leftArm.transform.localPosition.x,(float)(changeY+0.23f),(float)changeX);
        leftArm.transform.localRotation = Quaternion.Euler((float)(angle),0,0);
        rightArm.transform.localPosition = new Vector3(rightArm.transform.localPosition.x,(float)(changeY+0.23f),(float)changeX);
        rightArm.transform.localRotation = Quaternion.Euler((float)(angle),0,0);
    }
    public void moveProjectileCreatorAndUse(float angle) {
        double dAngle = angle/180*Math.PI;
        emptyProjectile.transform.localRotation = Quaternion.Euler(angle,0f,0f);
        emptyProjectile.transform.localPosition = new Vector3(0f,(float)(-Math.Sin(dAngle)*distanceOfProjSpawn)+0.7f,(float)(Math.Cos(dAngle)*distanceOfProjSpawn));
        emptyUse.transform.localRotation = Quaternion.Euler(angle,0f,0f);
        emptyUse.transform.localPosition = new Vector3(0f,(float)(-Math.Sin(dAngle)*distanceOfUseSelector)+0.7f,(float)(Math.Cos(dAngle)*distanceOfUseSelector));
    }
    public void movePlayer(float x, float y, float z) {
        rb.AddForce(new Vector3(x,y,z) - rb.velocity, ForceMode.VelocityChange);
    }
    public void addForceToPlayer(float x, float y, float z) {
        rb.AddForce(new Vector3(x,y,z), ForceMode.VelocityChange);
    }
    public void crouchPlayer() {
        /*
        Transform upperLTf = leftLeg.transform.GetChild(0), lowerLTf = leftLeg.transform.GetChild(1),
        upperRTf = rightLeg.transform.GetChild(0), lowerRTf = rightLeg.transform.GetChild(1);
        if (characterMovementState == 1) {
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
        */
    }
    public bool isGrounded() {
        LayerMask ground = LayerMask.GetMask("Ground"), props = LayerMask.GetMask("Movable Objects");

        return Physics.CheckBox(feetHitbox.transform.position,new Vector3(feetHitbox.transform.localScale.x/2,feetHitbox.transform.localScale.y/2+0.2f,feetHitbox.transform.localScale.z/2),feetHitbox.transform.rotation,ground) || 
        Physics.CheckBox(feetHitbox.transform.position,new Vector3(feetHitbox.transform.localScale.x/2,feetHitbox.transform.localScale.y/2+0.2f,feetHitbox.transform.localScale.z/2),feetHitbox.transform.rotation,props);
        /*Transform tL = leftLeg.transform.GetChild(1), tR = rightLeg.transform.GetChild(1);
        return //Physics.CheckBox(movementHitbox.transform.position,new Vector3(movementHitbox.transform.localScale.x/4,movementHitbox.transform.localScale.y/2,movementHitbox.transform.localScale.y/4),movementHitbox.transform.rotation,ground);
            Physics.CheckBox(tL.position,new Vector3(tL.localScale.x/4,tL.localScale.y/2+0.2f,tL.localScale.z/4),tL.rotation,ground) ||
            Physics.CheckBox(tR.position,new Vector3(tR.localScale.x/4,tR.localScale.y/2+0.2f,tR.localScale.z/4),tR.rotation,ground) ||
            Physics.CheckBox(tL.position,new Vector3(tL.localScale.x/4,tL.localScale.y/2+0.2f,tL.localScale.z/4),tL.rotation,props) ||
            Physics.CheckBox(tR.position,new Vector3(tR.localScale.x/4,tR.localScale.y/2+0.2f,tR.localScale.z/4),tR.rotation,props); */
    }
    public void switchView() {
        lockCursor = !lockCursor;
        if (lockCursor) {
            Cursor.lockState = CursorLockMode.Locked;
        } else {
            Cursor.lockState = CursorLockMode.None;
        }
    }
    public void switchWeaponSights(bool aimDownSights) {
        if (currentWeaponIndex != 0 && aimDownSights) {
            mainCamera.GetComponent<Camera>().enabled = false;
            aimCamera.GetComponent<Camera>().enabled = true;
        } else {
            mainCamera.GetComponent<Camera>().enabled = true;
            aimCamera.GetComponent<Camera>().enabled = false;
        }
    }
    public void setUseTool(bool usePressed) {
        if (heldProp != null) return;
        int weaponIndex = controller.getClosestWeapon(emptyUse.transform.position,1f);
        float weaponDistance = controller.getSavedDistance();
        int propIndex = controller.getClosestProp(emptyUse.transform.position,1f);
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
        if (usePressed) {
            if (typeToUse == 0) {
                addWeaponToPlayer(weaponIndex);
            } else if (typeToUse == 1) {
                holdProp(propIndex);
            }
        }
    }
    public void changeStateOfCharacter(bool sprint, bool enterC, bool exitC) {
        if (sprint && currentSprintTime > 0f) { // sprint button is held down
            if (characterMovementState == 1) { // if crouched
                characterMovementState = 0; // uncrouched on
                crouchPlayer(); // fix character
            } 
            characterMovementState = 2; // sprint on
            currentTimeBeforeRestore = 0f;
        } else {
            bool changeInState = false;
            if (enterC) { // pressed crouch button
                characterMovementState = 1; // enter crouch
                changeInState = true;
            } 
            if (exitC) { // let go of coruch button
                characterMovementState = 0; // exit crouch
                changeInState = true;
            }
            if (changeInState) {
                crouchPlayer();
            } else if (characterMovementState != 1) {
                characterMovementState = 0;
            }
        }
    }
    public void move(float vertical, float horizontal, bool ju) {
        double tempAngle = characterAngle/180*Math.PI, tempAngleP = tempAngle+(Math.PI/2);
        float tempMovementSpeed = movementSpeed;
        if (characterMovementState == 1)
            tempMovementSpeed *= crouchSpeedMult;
        else if (characterMovementState == 2)
            tempMovementSpeed *= sprintSpeedMult;
        double increaseZ = vertical*Math.Cos(tempAngle)*tempMovementSpeed + horizontal*Math.Cos(tempAngleP)*tempMovementSpeed,
        increaseX = vertical*Math.Sin(tempAngle)*tempMovementSpeed + horizontal*Math.Sin(tempAngleP)*tempMovementSpeed;
        float up = rb.velocity.y;
        if (ju && isGrounded()) {
            up =jump;
        }
        movePlayer((float)increaseX,up,(float)increaseZ);
    }
    public void drop() {
        if (heldProp != null) {
            dropProp();
        } else {
            removeWeapon(currentWeaponIndex);
        }
    }
    public void updateTime() {
        float differenceInTime = UnityEngine.Time.time-savedTime;
        savedTime = UnityEngine.Time.time;
        if (characterMovementState == 2) {
            currentSprintTime -= differenceInTime;
            if (differenceInTime < 0f) {
                currentSprintTime = 0f;
                characterMovementState = 0;
            }
        } else {
            if (currentTimeBeforeRestore < timeBeforeStartRestoreSprint) {
                currentTimeBeforeRestore += differenceInTime;
            } else {
                currentSprintTime += (float)(differenceInTime*restoreSprintTimeMult);
                if (currentSprintTime > sprintMaxTime) {
                    currentSprintTime = sprintMaxTime;
                }
            }
        }
        if (weaponBar[currentWeaponIndex] != null) {
            fireWeapontime -= differenceInTime;
            if (fireWeapontime < 0f) {
                fireWeapontime = 0f;
            }
            Weapon currentWeapon = weaponBar[currentWeaponIndex].GetComponent<Weapon>();
            if (swapWeaponTime != 0f && differenceInTime != 0f) {
                swapWeaponTime -= differenceInTime;
                if (swapWeaponTime < 0f) {
                    differenceInTime = -swapWeaponTime;
                    swapWeaponTime = 0f;
                } else {
                    differenceInTime = 0f;
                }
            }
            if (currentWeapon.getReloadTimeLeft() != 0f && differenceInTime != 0f) {
                currentWeapon.setReloadTimeLeft(currentWeapon.getReloadTimeLeft()-differenceInTime);
                if (currentWeapon.getReloadTimeLeft() < 0f){
                    differenceInTime = -currentWeapon.getReloadTimeLeft();
                    currentWeapon.setReloadTimeLeft(0f);
                } else {
                    differenceInTime = 0f;
                }
            }
        }
    }
    public void FixedUpdate() {
        
    }
    public void Update()
    {
        moveAllWeapon();
        moveProp();
        updateItemInfo();
        updateTime();
    }
}
