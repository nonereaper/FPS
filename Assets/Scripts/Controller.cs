using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
    private List<GameObject> obstacles; 
    private List<GameObject> weapons;
    private List<GameObject> projectiles;
    private List<GameObject> decay;
    private Transform tf;
    private float savedDistance;
    private int savedType;
    // Start is called before the first frame update
    void Start()
    {
        obstacles = new List<GameObject>();
        weapons = new List<GameObject>();
        projectiles = new List<GameObject>();
        decay = new List<GameObject>();
        tf = GetComponent<Transform>();
        for (int i = 0; i < tf.childCount; i++) {
            Transform tempTf = tf.GetChild(i);
            for (int q = 0; q < tempTf.childCount; q++) {
                if (i == 1) {
                    weapons.Add(tempTf.GetChild(q).gameObject);
                } else if (i == 2) {
                    obstacles.Add(tempTf.GetChild(q).gameObject);
                }
            }
        }
        
    }
    public GameObject getClosestObject(Vector3 p, float rad) {
        GameObject tempObject = null;
        float distance = float.MaxValue;
        savedType = -1;
        for (int i = 0; i < weapons.Count; i++) {
            float tempDistance = Vector3.Distance(weapons[i].GetComponent<Transform>().position,p);
            if (tempDistance <= rad && tempDistance < distance) {
                tempObject = weapons[i];
                distance = tempDistance;
                savedType = 0;
            }
        }
        for (int i = 0; i < obstacles.Count; i++) {
            float tempDistance = Vector3.Distance(obstacles[i].GetComponent<Transform>().position,p);
            if (tempDistance <= rad && tempDistance < distance) {
                tempObject = obstacles[i];
                distance = tempDistance;
                savedType = 1;
            }
        }
        savedDistance = distance;
        return tempObject;
    }
    // saved distance stuff for use
    public float getSavedDistance() {
		return this.savedDistance;
	}
	public void setSavedDistance(float savedDistance) {
		this.savedDistance = savedDistance;
	}
    public int getSavedtype() {
		return this.savedType;
	}
	public void setSavedtype(int savedType) {
		this.savedType = savedType;
	}
    // add weapon
    public void removeWeapon(GameObject w) {
        weapons.Remove(w);
    }
    public void addWeapon(GameObject w) {
        weapons.Add(w);
    }
    public Transform getWeaponTransformation() {
        return tf.GetChild(1);
    }
    public Transform getProjectileTransformation() {
        return tf.GetChild(0);
    }
    public Transform getDecayTransformation() {
        return tf.GetChild(3);
    }
    // add proj and decay
    public void addProjectile(GameObject go) {
        projectiles.Add(go);
    } 
    public void addDecay(GameObject go) {
        decay.Add(go);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
