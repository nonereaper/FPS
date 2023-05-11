using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeapon : MonoBehaviour
{
    [SerializeField] int damage;

    [SerializeField] float attackSpeed;
	[SerializeField] private float swapTime;
    [SerializeField] private GameObject proj;

	public float getSwapTime() {
		return this.swapTime;
	}
	public void setSwapTime(float st) {
		swapTime = st;
	}

	
	public GameObject getProj() {
		return this.proj;
	}

	public void setProj(GameObject proj) {
		this.proj = proj;
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

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
