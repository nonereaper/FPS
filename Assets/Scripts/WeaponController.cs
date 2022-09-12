using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{   
    GameObject[] weaponsList;
    Transform tf;
    // Start is called before the first frame update
    void Start()
    {
        tf = GetComponent<Transform>();
        weaponsList = new GameObject[tf.childCount];
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public GameObject getWeapon(int index) {
        return weaponsList[index];
    }
}
