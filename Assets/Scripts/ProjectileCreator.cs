using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileCreator : MonoBehaviour
{
    // projectile Controller
    [SerializeField] GameObject projectileController;

    private Weapon[] weaponSlot;
    // -1= melee, 0=weaponSlot1, 1= weaponSlot2,
    private int currentWeaponSlot; 

    private float time, time2;
    private Player parentClass;
    private CameriaMovement cameriaMovement;
    private Transform tf;
    // Start is called before the first frame update
    void Start()
    {
        tf = GetComponent<Transform>();
        parentClass = GetComponentInParent<Player>();
        cameriaMovement = tf.parent.GetComponentInChildren<CameriaMovement>();
        time = UnityEngine.Time.time;
        time2 = UnityEngine.Time.time;
        weaponSlot = new Weapon[9];
        currentWeaponSlot = -1;
    }
    public String getWeaponInfo() {
        String temp = "";
        for (int i = 0; i < weaponSlot.Length; i++) {
            temp+="Weapon Slot " +(i+1) + " is: ";
            if (weaponSlot[i] == null) {
            temp+= "empty.\n" ;
            } else {
            temp+= weaponSlot[i].GetComponent<Transform>().gameObject.name + "\n";
            }
        }
        temp += "Melee Weapon is: " + "\n";
        if (currentWeaponSlot != -1) {
            Weapon currentWeapon = weaponSlot[currentWeaponSlot];
            if (currentWeapon != null) {
                temp += "Magazine: (" + currentWeapon.getCurrentMagazine() + "/" + currentWeapon.getMagazine() + ")\n";
                temp += "Stored ammo Left: (" + currentWeapon.getCurrentTotalAmmo() + "/" + currentWeapon.getTotalAmmo() + ")";
            }
        }
        return temp;
    }
    public void setCurrentWeaponSlot(int i) {
        if (currentWeaponSlot != -1  && weaponSlot[currentWeaponSlot] != null)
        weaponSlot[currentWeaponSlot].GetComponent<Transform>().gameObject.SetActive(false);
        if (i == -1)
            currentWeaponSlot = -1;
        else {
            currentWeaponSlot = i;
            if (weaponSlot[i] != null) {
                weaponSlot[i].GetComponent<Transform>().gameObject.SetActive(true);
            }
        }
    }
    public int getWeaponSlotLength() {
        return weaponSlot.Length + 1;   
    }
    public void reload() {
        if (currentWeaponSlot != -1) {
            Weapon currentWeapon = weaponSlot[currentWeaponSlot];
            if (currentWeapon == null) {
                return;
            }
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
    public int indexToSwapWith(Weapon w) {
        if (currentWeaponSlot != -1 && weaponSlot[currentWeaponSlot] == null)
        return currentWeaponSlot;
        for (int i = 0; i < weaponSlot.Length; i++) {
            if (weaponSlot[i] == null) {
                return i;
            }
        }
        return -1;
    }
    public int getCurrentWeaponSlot() {
        return currentWeaponSlot;
    }
    public Weapon swapWeapon(int index, Weapon w) {
        if (index != -1) {
            weaponSlot[index] = w;
            return null;
        } else {
            Weapon temp = weaponSlot[currentWeaponSlot];
            weaponSlot[currentWeaponSlot] = w;
            return temp;
        }
    }
    public Weapon removeCurrentWeapon() {
        if (currentWeaponSlot != -1) {
            Weapon temp = weaponSlot[currentWeaponSlot];
            weaponSlot[currentWeaponSlot] = null;
            return temp;
        }
        return null;
    }
    public void useWeapon(bool buttonDown) {
        if (currentWeaponSlot == -1) {

        } else { // do weapon
            Weapon currentWeapon = weaponSlot[currentWeaponSlot];
            if (currentWeapon == null) { // no weapon: stop
                return;
            }
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
                    float rotationTempX = tf.rotation.eulerAngles.x + UnityEngine.Random.Range(-spread,spread),
                    rotationTempY = tf.rotation.eulerAngles.y + UnityEngine.Random.Range(-spread,spread);
                    Quaternion q = Quaternion.Euler(rotationTempX, rotationTempY, tf.rotation.eulerAngles.z);
                    GameObject o = Instantiate(currentWeapon.getProj(),tf.position,q,projectileController.GetComponent<Transform>());
                    o.GetComponent<Rigidbody>().AddRelativeForce(new Vector3(0,0,currentWeapon.getVelocity()));
                    o.GetComponent<Projectile>().setup(currentWeapon.getDamage(),tf.parent.gameObject.transform.parent.gameObject,10,projectileController);
                    o.GetComponent<Transform>().localScale = new Vector3(currentWeapon.getRadius(),currentWeapon.getRadius(),currentWeapon.getRadius());
                    projectileController.GetComponent<ProjectileController>().addNewProjectile(o);
                }
                parentClass.movePlayer(-currentWeapon.getBackBlast());
            }
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (currentWeaponSlot != -1) {
            Weapon currentWeapon = weaponSlot[currentWeaponSlot];
            if (currentWeapon != null &&  currentWeapon.getReloadTimeLeft() != 0f) {
                currentWeapon.setReloadTimeLeft(currentWeapon.getReloadTimeLeft()-(UnityEngine.Time.time-time2));
                if (currentWeapon.getReloadTimeLeft() < 0f){
                    currentWeapon.setReloadTimeLeft(0f);
                }
            }
        }
        time2 = UnityEngine.Time.time;
    }
}
