using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileCreator : MonoBehaviour
{
    // projectile Controller
    private GameObject controllerObject;
    private Controller controller;

    private GameObject[] weaponSlot;
    // 0= melee, 1=weaponSlot1, 2= weaponSlot2,
    private int currentWeaponSlot; 

    private float time, time2, meleeTime;
    private Player parentClass;
    private CameriaMovement cameriaMovement;
    private Transform tf;
    // Start is called before the first frame update
    void Start()
    {
        tf = GetComponent<Transform>();
        parentClass = GetComponentInParent<Player>();
        cameriaMovement = tf.parent.GetComponentInChildren<CameriaMovement>();
        controllerObject = GameObject.Find("Controller");
        controller = controllerObject.GetComponent<Controller>();
        time = UnityEngine.Time.time;
        time2 = UnityEngine.Time.time;
        meleeTime = 0;
        weaponSlot = new GameObject[9];
        currentWeaponSlot = 0;
    }
    public void moveTo(float z) {
        tf.localPosition = new Vector3(tf.localPosition.x,tf.localPosition.y,z);
    }
    public String getWeaponInfo() {
        String temp = "";
        for (int i = 0; i < weaponSlot.Length; i++) {
            temp+="Weapon Slot " +(i+1) + " is: ";
            if (weaponSlot[i] == null) {
            temp+= "empty.\n" ;
            } else {
            temp+= weaponSlot[i].name + "\n";
            }
        }
        if (currentWeaponSlot != 0  && weaponSlot[currentWeaponSlot] != null) {
            Weapon currentWeapon = weaponSlot[currentWeaponSlot].GetComponent<Weapon>();
            if (currentWeapon != null) {
                temp += "Magazine: (" + currentWeapon.getCurrentMagazine() + "/" + currentWeapon.getMagazine() + ")\n";
                temp += "Stored ammo Left: (" + currentWeapon.getCurrentTotalAmmo() + "/" + currentWeapon.getTotalAmmo() + ")";
            }
        }
        return temp;
    }
    public void setCurrentWeaponSlot(int i) {
        if (weaponSlot[currentWeaponSlot] != null)
        weaponSlot[currentWeaponSlot].SetActive(false);
        
        currentWeaponSlot = i;
        if (weaponSlot[i] != null) {
            weaponSlot[i].SetActive(true);
            if (i != 0) {
                parentClass.setProjectileCreatorDistance(weaponSlot[i].GetComponent<Weapon>());
            }
        }
  
    }
    public int getWeaponSlotLength() {
        return weaponSlot.Length;   
    }
    public void reload() {
        if (currentWeaponSlot != 0) {
            if (weaponSlot[currentWeaponSlot] == null) {
                return;
            }
            Weapon currentWeapon = weaponSlot[currentWeaponSlot].GetComponent<Weapon>();
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
    public int indexToSwapWith(GameObject w, bool isMeleeWeapon) {
        if (isMeleeWeapon) {
            return 0;
        } else {
            for (int i = 1; i < weaponSlot.Length; i++) {
                if (weaponSlot[i] == null) {
                    return i;
                }
            }       
        }
        if (isMeleeWeapon && currentWeaponSlot == 0)
        return currentWeaponSlot;

        return -1;
    }
    public int getCurrentWeaponSlot() {
        return currentWeaponSlot;
    }
    public GameObject getCurrentWeapon() {
        return weaponSlot[currentWeaponSlot];
    }
    public GameObject swapWeapon(int index, GameObject w) {
        GameObject temp = weaponSlot[index];
        weaponSlot[index] = w;
        return temp;
    }
    public GameObject removeCurrentWeapon() {
        GameObject temp = weaponSlot[currentWeaponSlot];
        weaponSlot[currentWeaponSlot] = null;
        return temp;
    }
    public void useWeapon(bool buttonDown) {
        if (weaponSlot[currentWeaponSlot] == null) { // no weapon: stop
                return;
            }
        if (currentWeaponSlot == 0) {
            MeleeWeapon currentWeapon = weaponSlot[currentWeaponSlot].GetComponent<MeleeWeapon>();
            if (UnityEngine.Time.time < time+currentWeapon.getAttackSpeed()) { // cannot attack let: stop
                return;
            }
            if (meleeTime != 0) {
                return;
            }
            time = UnityEngine.Time.time;
            
            currentWeapon.setTimeLeftInSwing(currentWeapon.getSwingTime());
            Transform tempTf = currentWeapon.GetComponent<Transform>();
            currentWeapon.setOrigPos(tempTf.position);
            currentWeapon.setOrigRot(tempTf.rotation);
            if (currentWeapon.isTurn90()) {
                tempTf.RotateAround(parentClass.getWeaponLocationTransform().position,parentClass.getWeaponLocationTransform().forward,90f);
            }
            tempTf.RotateAround(parentClass.getWeaponLocationTransform().position,parentClass.getWeaponLocationTransform().up,currentWeapon.getStartAngleOfSwing());
                
        } else { // do weapon
            Weapon currentWeapon = weaponSlot[currentWeaponSlot].GetComponent<Weapon>();
            
            if (currentWeapon.getReloadTimeLeft() != 0f) { // in the middle of reload
                return;
            }
            if (UnityEngine.Time.time < time+currentWeapon.getAttackSpeed()) { // cannot attack let: stop
                return;
            }
            if (currentWeapon.getCurrentMagazine() == 0) { // no ammo to shoot
                reload();
                return;
            }
            bool canFire = false;
            if ((buttonDown && currentWeapon.isSemiAuto()) || (!buttonDown && currentWeapon.isAuto())) {
                canFire = true;
            }
            if (canFire) {
                currentWeapon.setCurrentMagazine(currentWeapon.getCurrentMagazine()-1);
                time = UnityEngine.Time.time;
                double crouchSpreadReduction = 1.0;
                if (parentClass.getCrouching()) {
                    crouchSpreadReduction = currentWeapon.getCrouchSpread();
                }
                cameriaMovement.updateCameria(currentWeapon.getRecoil());
                float spread = (float)(currentWeapon.getSpread()*crouchSpreadReduction);
                for (int i = 0; i < currentWeapon.getNumber(); i++) {
                    Transform tf2 = currentWeapon.getMussle().GetComponent<Transform>();
                    float rotationTempX = tf2.rotation.eulerAngles.x + UnityEngine.Random.Range(-spread,spread),
                    rotationTempY = tf2.rotation.eulerAngles.y + UnityEngine.Random.Range(-spread,spread);
                    Quaternion q = Quaternion.Euler(rotationTempX, rotationTempY, tf2.rotation.eulerAngles.z);
                    GameObject o = Instantiate(currentWeapon.getProj(),tf2.position,q,controller.getProjectileTransformation());
                    o.GetComponent<Rigidbody>().AddRelativeForce(new Vector3(0,0,currentWeapon.getVelocity()));
                    o.GetComponent<Projectile>().setup(currentWeapon.getDamage(),tf.transform.parent.gameObject,10,controllerObject);
                    controller.addProjectile(o);
                }
                double angleOfCamera = parentClass.getCameriaAngle()/180*Math.PI;
                double increaseY = Math.Sin(angleOfCamera)*currentWeapon.getBackBlast(), increaseZ = Math.Cos(angleOfCamera)*currentWeapon.getBackBlast();
                parentClass.movePlayer(0,(float)increaseY,-(float)increaseZ);
                Instantiate(currentWeapon.getFireExplosion(),currentWeapon.getMussle().GetComponent<Transform>().position,currentWeapon.getMussle().GetComponent<Transform>().rotation,controller.getDecayTransformation());
                if (currentWeapon.isLaunchShells()) {
                    GameObject o = Instantiate(currentWeapon.getShells(),currentWeapon.getShellPosition().GetComponent<Transform>().position,currentWeapon.getShellPosition().GetComponent<Transform>().rotation,controller.getDecayTransformation());
                    o.GetComponent<Rigidbody>().AddRelativeForce(new Vector3(currentWeapon.getShellForce(),0,0));
                }
            }
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (currentWeaponSlot != 0 && weaponSlot[currentWeaponSlot] != null) {
            Weapon currentWeapon = weaponSlot[currentWeaponSlot].GetComponent<Weapon>();
            if (currentWeapon.getReloadTimeLeft() != 0f) {
                currentWeapon.setReloadTimeLeft(currentWeapon.getReloadTimeLeft()-(UnityEngine.Time.time-time2));
                if (currentWeapon.getReloadTimeLeft() < 0f){
                    currentWeapon.setReloadTimeLeft(0f);
                }
            }
        } else if (currentWeaponSlot == 0 && meleeTime != 0) {
            MeleeWeapon currentWeapon = weaponSlot[currentWeaponSlot].GetComponent<MeleeWeapon>();
            Transform tempTf = currentWeapon.GetComponent<Transform>();
            float changeInAngle = currentWeapon.getEndAngleOfSwing() - currentWeapon.getStartAngleOfSwing();
            tempTf.RotateAround(parentClass.getWeaponLocationTransform().position,parentClass.getWeaponLocationTransform().up,changeInAngle);
            meleeTime=meleeTime-(UnityEngine.Time.time-time2);
        }
        time2 = UnityEngine.Time.time;
    }
}
