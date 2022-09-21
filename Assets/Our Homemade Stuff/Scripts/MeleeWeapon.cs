using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeapon : MonoBehaviour
{
    [SerializeField] int damage;

    [SerializeField] float attackSpeed;
    [SerializeField] bool turn90;

    [SerializeField] GameObject handPosition;


	public GameObject getHandPosition() {
		return this.handPosition;
	}

	public void setHandPosition(GameObject handPosition) {
		this.handPosition = handPosition;
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
