using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{   
    // projectile object
    [SerializeField] GameObject proj;
    // radius of spread of proj, using radians
    [SerializeField] float spread;
    // number of projs
    [SerializeField] int number;
    // damage of projs
    [SerializeField] int damage;
    // seconds before shooting again
    [SerializeField] float attackSpeed;
    // time for single shot
    [SerializeField] float singleShotAttackSpeed;
    // force in which the projectile shoots
    [SerializeField] float velocity;
    // radius of projectile's size
    [SerializeField] float radius;
    // can change fire type 
    [SerializeField] bool singleShot, semiAuto, auto;
    // fire type (0 - single shot, 1 - semi auto, 2 - auto)
    [SerializeField] int fireType;
    // crouch's spread reduction (0.0(no spread) -> 1.0(same spread))
    [SerializeField] double crouchSpread;
    // recoil after shooting a round, using degree
    [SerializeField] float recoil;
    // how much pushed back from firing
    [SerializeField] float backBlast;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
