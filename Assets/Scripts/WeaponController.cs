using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{   
    List<GameObject> weaponsList;
    Transform tf;
    private float savedDistance;

	public float getSavedDistance() {
		return this.savedDistance;
	}

	public void setSavedDistance(float savedDistance) {
		this.savedDistance = savedDistance;
	}

    // Start is called before the first frame update
    void Start()
    {
        tf = GetComponent<Transform>();
        weaponsList = new List<GameObject>();
        for (int i = 0; i < tf.childCount; i++) {
            weaponsList.Add(tf.GetChild(i).gameObject);
        }
    }
    public Weapon changeWeapon(Vector3 p, float rad) {
        Weapon tempWeapon = null;
        float distance = float.MaxValue;
        for (int i = 0; i < weaponsList.Count; i++) {
            float tempDistance = Vector3.Distance(weaponsList[i].GetComponent<Transform>().position,p);
            if (tempDistance <= rad && tempDistance < distance) {
                tempWeapon = weaponsList[i].GetComponent<Weapon>();
                distance = tempDistance;
            }
        }
        savedDistance = distance;
        return tempWeapon;
    }
    public void removeWeapon(Weapon w) {
        weaponsList.Remove(w.GetComponent<Transform>().gameObject);
    }
    public void addWeapon(Weapon w) {
        weaponsList.Add(w.GetComponent<Transform>().gameObject);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
