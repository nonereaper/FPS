using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{   
    [SerializeField] GameObject proj;
    [SerializeField] int magazine;
    [SerializeField] int totalAmmo;
    [SerializeField] int currentMagazine;
    [SerializeField] int currentTotalAmmo;
    // spread of weapon's projectile from center point by degree
    [SerializeField] float spread;
    // number of projectiles
    [SerializeField] int number;
    [SerializeField] int damage;
    // seconds before each attack
    [SerializeField] float attackSpeed;
    // seconds for reload
    [SerializeField] float reloadTime;
    // force added to projectile
    [SerializeField] float velocity;
    // projectile radius
    [SerializeField] float radius;
    [SerializeField] bool semiAuto, auto;
    // 0 - semiAuto, 1 - auto
    [SerializeField] int fireType;
    // reduced spread of weapon's projectiles (0.0 -> 1.0)
    [SerializeField] double crouchSpread;
    // recoil of firing weapon by degree amount upward
    [SerializeField] float recoil;
    // force backward from firing weapon
    [SerializeField] float backBlast;

	public GameObject getProj() {
		return this.proj;
	}

	public void setProj(GameObject proj) {
		this.proj = proj;
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

	public float getRadius() {
		return this.radius;
	}

	public void setRadius(float radius) {
		this.radius = radius;
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

	public float getBackBlast() {
		return this.backBlast;
	}

	public void setBackBlast(float backBlast) {
		this.backBlast = backBlast;
	}

	public float getReloadTime() {
		return this.reloadTime;
	}

	public void setReloadTime(float reloadTime) {
		this.reloadTime = reloadTime;
	}

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
