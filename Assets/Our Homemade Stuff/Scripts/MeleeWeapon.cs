using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeapon : MonoBehaviour
{
    [SerializeField] int damage;

    [SerializeField] float attackSpeed;
    [SerializeField] bool turn90;
	[SerializeField] float swingTime;
	[SerializeField] float startAngleOfSwing;
	[SerializeField] float endAngleOfSwing;

    [SerializeField] GameObject handPosition;

	private float timeLeftInSwing;
	private float angleSwingLeft;
	private Vector3 origPos;
	private Quaternion origRot;


	public Vector3 getOrigPos() {
		return this.origPos;
	}

	public void setOrigPos(Vector3 origPos) {
		this.origPos = origPos;
	}

	public Quaternion getOrigRot() {
		return this.origRot;
	}

	public void setOrigRot(Quaternion origRot) {
		this.origRot = origRot;
	}

	public float getTimeLeftInSwing() {
		return this.timeLeftInSwing;
	}

	public void setTimeLeftInSwing(float timeLeftInSwing) {
		this.timeLeftInSwing = timeLeftInSwing;
	}

	public float getAngleSwingLeft() {
		return this.angleSwingLeft;
	}

	public void setAngleSwingLeft(float angleSwingLeft) {
		this.angleSwingLeft = angleSwingLeft;
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

	public float getStartAngleOfSwing() {
		return this.startAngleOfSwing;
	}

	public void setStartAngleOfSwing(float startAngleOfSwing) {
		this.startAngleOfSwing = startAngleOfSwing;
	}

	public float getEndAngleOfSwing() {
		return this.endAngleOfSwing;
	}

	public void setEndAngleOfSwing(float endAngleOfSwing) {
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
