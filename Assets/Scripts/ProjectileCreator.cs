using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileCreator : MonoBehaviour
{
    // actual proj object
    private GameObject proj;
    // projectile Controller
    [SerializeField] GameObject projectileController;

    private Weapon[] weaponSlot;
    // 0=weaponSlot1, 1= weaponSlot2, 3 = melee
    private int currentWeaponSlot; 

    private float time, time2;
    private Player parentClass;
    private CameriaMovement cameriaMovement;
    private Transform tf;
    private float reloadTimeLeft;
    // Start is called before the first frame update
    void Start()
    {
        tf = GetComponent<Transform>();
        parentClass = GetComponentInParent<Player>();
        cameriaMovement = tf.parent.GetComponentInChildren<CameriaMovement>();
        time = UnityEngine.Time.time;
        time2 = UnityEngine.Time.time;
        weaponSlot = new Weapon[2];
        currentWeaponSlot = 0;
    }
    public void reload() {
        if (currentWeaponSlot != 2) {
            Weapon currentWeapon = weaponSlot[currentWeaponSlot];
            if (currentWeapon == null) {
                return;
            }
            if (reloadTimeLeft != 0f) {
                return;
            }
            if (currentWeapon.getCurrentTotalAmmo() == 0) {
                return;
            }
            int amountToAdd = currentWeapon.getCurrentTotalAmmo()%currentWeapon.getMagazine();
            currentWeapon.setCurrentMagazine(amountToAdd);
            currentWeapon.setCurrentTotalAmmo(currentWeapon.getCurrentTotalAmmo()-amountToAdd);
            reloadTimeLeft = currentWeapon.getReloadTime();
        }
    }
    public void useWeapon(bool buttonDown) {
        if (currentWeaponSlot == 2) {

        } else { // do weapon
            Weapon currentWeapon = weaponSlot[currentWeaponSlot];
            if (currentWeapon == null) { // no weapon: stop
                return;
            }
            if (UnityEngine.Time.time < time+currentWeapon.getAttackSpeed()) { // cannot attack let: stop
                return;
            }
            if (currentWeapon.getCurrentMagazine() == 0) { // no ammo to shoot
                return;
            }
            if (reloadTimeLeft != 0f) { // in the middle of reload
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
                float spread = currentWeapon.getSpread()/180;
                for (int i = 0; i < currentWeapon.getNumber(); i++) {
                    float rotationTempX = tf.rotation.eulerAngles.x + UnityEngine.Random.Range(-spread,spread),
                    rotationTempY = tf.rotation.eulerAngles.y + UnityEngine.Random.Range(-spread,spread);
                    
                    Quaternion q = Quaternion.Euler(rotationTempX, rotationTempY, tf.rotation.eulerAngles.z);
                    GameObject o = Instantiate(proj,tf.position,q,projectileController.GetComponent<Transform>());
                    o.GetComponent<Rigidbody>().AddRelativeForce(new Vector3(0,0,-currentWeapon.getVelocity()));
                    o.GetComponent<Projectile>().setup(currentWeapon.getDamage(),tf.parent.gameObject.transform.parent.gameObject,10,projectileController);
                    o.GetComponent<Transform>().localScale = new Vector3(currentWeapon.getRadius(),currentWeapon.getRadius(),currentWeapon.getRadius());
                    projectileController.GetComponent<ProjectileController>().addNewProjectile(o);
                }
            }
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (reloadTimeLeft != 0f) {
            reloadTimeLeft -= (UnityEngine.Time.time-time2);
            time2 = UnityEngine.Time.time;
            if (reloadTimeLeft < 0f){
                reloadTimeLeft =0f;
            }
        }
    }
}
