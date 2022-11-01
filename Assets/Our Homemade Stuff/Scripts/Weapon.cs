using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{   
    [SerializeField] private GameObject proj;
	[SerializeField] private GameObject fireExplosion;
	[SerializeField] private GameObject shells;


    [SerializeField] private int magazine;
    [SerializeField] private int totalAmmo;
    [SerializeField] private int currentMagazine;
    [SerializeField] private int currentTotalAmmo;
    // spread of weapon's projectile from center point by degree
    [SerializeField] private float spread;
    // number of projectiles
    [SerializeField] private int number;
    [SerializeField] private int damage;
    // seconds before each attack
    [SerializeField] private float attackSpeed;
    // seconds for reload
    [SerializeField] private float reloadTime;
    // force added to projectile
    [SerializeField] private float velocity;
    [SerializeField] private bool semiAuto, auto;
    // 0 - semiAuto, 1 - auto
    [SerializeField] private int fireType;
    // reduced spread of weapon's projectiles (0.0 -> 1.0)
    [SerializeField] private double crouchSpread;
	// increased spread of weapon's projectiles (# >= 0)
	[SerializeField] private double sprintSpread;

    // recoil of firing weapon by degree amount upward
    [SerializeField] private float recoil;
    
	[SerializeField] private bool launchShells;
	[SerializeField] private float shellForce;

	[SerializeField] private GameObject mussle;
	[SerializeField] private GameObject handPosition;

	
	[SerializeField] private GameObject aimPosition;
	[SerializeField] private GameObject shellPosition;
	

	public GameObject getShellPosition() {
		return this.shellPosition;
	}


	// 0.3,0,0.5,
	// 0, 40, 0
	private float reloadTimeLeft;

	public float getReloadTimeLeft() {
		return this.reloadTimeLeft;
	}

	public void setReloadTimeLeft(float reloadTimeLeft) {
		this.reloadTimeLeft = reloadTimeLeft;
	}
	
	public void setShellPosition(GameObject shellPosition) {
		this.shellPosition = shellPosition;
	}

	public float getShellForce() {
		return this.shellForce;
	}

	public void setShellForce(float shellForce) {
		this.shellForce = shellForce;
	}

	public GameObject getShells() {
		return this.shells;
	}

	public void setShells(GameObject shells) {
		this.shells = shells;
	}

	public GameObject getHandPosition() {
		return this.handPosition;
	}

	public void setHandPosition(GameObject handPosition) {
		this.handPosition = handPosition;
	}

	public GameObject getAimPosition() {
		return this.aimPosition;
	}

	public void setAimPosition(GameObject aimPosition) {
		this.aimPosition = aimPosition;
	}

	public GameObject getFireExplosion() {
		return this.fireExplosion;
	}

	public void setFireExplosion(GameObject fireExplosion) {
		this.fireExplosion = fireExplosion;
	}

	public GameObject getMussle() {
		return this.mussle;
	}

	public void setMussle(GameObject mussle) {
		this.mussle = mussle;
	}

	public GameObject getProj() {
		return this.proj;
	}

	public void setProj(GameObject proj) {
		this.proj = proj;
	}
	
	public bool isLaunchShells() {
		return this.launchShells;
	}

	public void setLaunchShells(bool launchShells) {
		this.launchShells = launchShells;
	}

	public int getMagazine() {
		return this.magazine;
	}

	public void setMagazine(int magazine) {
		this.magazine = magazine;
	}

	public int getTotalAmmo() {
		return this.totalAmmo;
	}

	public void setTotalAmmo(int totalAmmo) {
		this.totalAmmo = totalAmmo;
	}

	public int getCurrentMagazine() {
		return this.currentMagazine;
	}

	public void setCurrentMagazine(int currentMagazine) {
		this.currentMagazine = currentMagazine;
	}

	public int getCurrentTotalAmmo() {
		return this.currentTotalAmmo;
	}

	public void setCurrentTotalAmmo(int currentTotalAmmo) {
		this.currentTotalAmmo = currentTotalAmmo;
	}

	public float getSpread() {
		return this.spread;
	}

	public void setSpread(float spread) {
		this.spread = spread;
	}

	public double getSprintSpread() {
		return this.sprintSpread;
	}

	public void setSprintSpread(double sprintSpread) {
		this.sprintSpread = sprintSpread;
	}
	
	public int getNumber() {
		return this.number;
	}

	public void setNumber(int number) {
		this.number = number;
	}

	public int getDamage() {
		return this.damage;
	}

	public void setDamage(int damage) {
		this.damage = damage;
	}

	public float getAttackSpeed() {
		return this.attackSpeed;
	}

	public void setAttackSpeed(float attackSpeed) {
		this.attackSpeed = attackSpeed;
	}

	public float getVelocity() {
		return this.velocity;
	}

	public void setVelocity(float velocity) {
		this.velocity = velocity;
	}

	public bool isSemiAuto() {
		return this.semiAuto;
	}

	public bool isAuto() {
		return this.auto;
	}

	public int getFireType() {
		return this.fireType;
	}

	public void setFireType(int fireType) {
		this.fireType = fireType;
	}

	public double getCrouchSpread() {
		return this.crouchSpread;
	}

	public void setCrouchSpread(double crouchSpread) {
		this.crouchSpread = crouchSpread;
	}

	public float getRecoil() {
		return this.recoil;
	}

	public void setRecoil(float recoil) {
		this.recoil = recoil;
	}

	public float getReloadTime() {
		return this.reloadTime;
	}

	public void setReloadTime(float reloadTime) {
		this.reloadTime = reloadTime;
	}

    void Start()
    {
       reloadTimeLeft = 0f; 
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
