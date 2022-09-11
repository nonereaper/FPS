using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileCreator : MonoBehaviour
{
    // actual proj object
    [SerializeField] GameObject proj;
    // radius of spread of proj, using radians
    [SerializeField] float spread;
    // number of projs
    [SerializeField] int number;
    // damage of projs
    [SerializeField] int damage;
    // seconds before shooting again
    [SerializeField] float attackSpeed;
    // force in which the projectile shoots
    [SerializeField] float velocity;
    // radius of projectile's size
    [SerializeField] float radius;
    // is semi-auto
    [SerializeField] bool semiAuto;
    // crouch's spread reduction (0.0(no spread) -> 1.0(same spread))
    [SerializeField] double crouchSpread;
    // recoil after shooting a round, using degree
    [SerializeField] float recoil;

    private float time;
    private Player parentClass;
    private CameriaMovement cameriaMovement;
    private Transform tf;
    // Start is called before the first frame update
    void Start()
    {
        tf = GetComponent<Transform>();
        parentClass = GetComponentInParent<Player>();
        cameriaMovement = tf.parent.GetComponentInChildren<CameriaMovement>();
        time = UnityEngine.Time.time;
    }

    // Update is called once per frame
    void Update()
    {

        if (((!semiAuto && Input.GetButton("Fire1"))||(semiAuto && Input.GetButtonDown("Fire1"))) && UnityEngine.Time.time >= time+attackSpeed) {
            time = UnityEngine.Time.time;
            double crouchSpreadReduction = 1.0;
            if (parentClass.getCrouching()) {
                crouchSpreadReduction = crouchSpread;
            }
            cameriaMovement.updateCameria(recoil);
            for (int i = 0; i < number; i++) {
                double rotationTempX = tf.rotation.eulerAngles.x, rotationTempY = tf.rotation.eulerAngles.y;
                rotationTempX += 180*Math.PI*UnityEngine.Random.Range(-spread,spread);
                rotationTempY += 180*Math.PI*UnityEngine.Random.Range(-spread,spread);
                Quaternion q = Quaternion.Euler((float)rotationTempX, (float)rotationTempY, tf.rotation.eulerAngles.z);
                GameObject o = Instantiate(proj,tf.position,q);
                o.GetComponent<Rigidbody>().AddRelativeForce(new Vector3(0,0,-velocity));
                o.GetComponent<Projectile>().setup(damage,tf.parent.gameObject.transform.parent.gameObject,10);
                o.GetComponent<Transform>().localScale = new Vector3(radius,radius,radius);
            }
        }
    }
}
