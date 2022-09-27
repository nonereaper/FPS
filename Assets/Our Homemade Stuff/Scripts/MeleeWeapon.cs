using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeapon : MonoBehaviour
{
    [SerializeField] int damage;

    [SerializeField] float attackSpeed;
    [SerializeField] bool turn90;
	[SerializeField] float swingTime;
	[SerializeField] double startAngleOfSwing;
	[SerializeField] double endAngleOfSwing;

    [SerializeField] GameObject handPosition;

	private float reloadTimeLeft;

	public float getReloadTimeLeft() {
		return this.reloadTimeLeft;
	}

	public void setReloadTimeLeft(float reloadTimeLeft) {
		this.reloadTimeLeft = reloadTimeLeft;
	}

	public bool isTurn90() {
		return this.turn90;
	}

	public void setTurn90(bool turn90) {
		this.turn90 = turn90;
	}

	public float getSwingTime() {
		return this.swingTime;
	}

	public void setSwingTime(float swingTime) {
		this.swingTime = swingTime;
	}

	public double getStartAngleOfSwing() {
		return this.startAngleOfSwing;
	}

	public void setStartAngleOfSwing(double startAngleOfSwing) {
		this.startAngleOfSwing = startAngleOfSwing;
	}

	public double getEndAngleOfSwing() {
		return this.endAngleOfSwing;
	}

	public void setEndAngleOfSwing(double endAngleOfSwing) {
		this.endAngleOfSwing = endAngleOfSwing;
	}

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
