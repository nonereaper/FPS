using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileCreator : MonoBehaviour
{
    [SerializeField] GameObject proj;
    // the spread of the projectile that will based off a radian
    [SerializeField] float angleSpread;
    [SerializeField] int numberOfProjectiles;
    [SerializeField] int damageOfProjectiles;
    [SerializeField] float timeBeforeShootAgain;
    
    private float projVelocity, time;
    private PlayerMovement parentClass;
    private Transform tf;
    private float startY, startZ;
    // Start is called before the first frame update
    void Start()
    {
        tf = GetComponent<Transform>();
        parentClass = GetComponentInParent<PlayerMovement>();
        projVelocity = 5000f;
        startY = tf.localPosition.y;
        startZ = tf.localPosition.z;
        time = UnityEngine.Time.time;
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetButton("Fire1") && UnityEngine.Time.time >= time+timeBeforeShootAgain) {
            time = UnityEngine.Time.time;
            double tempAngle = Math.PI*parentClass.getAngle()/180;
            for (int i = 0; i < numberOfProjectiles; i++) {
                double tempAngle2 = tempAngle + Math.PI*UnityEngine.Random.Range(-angleSpread,angleSpread);
                Debug.Log(tempAngle + " " + tempAngle2);
                double changeZ = Math.Cos(tempAngle2)*projVelocity, changeX = Math.Sin(tempAngle2)*projVelocity;

                GameObject o = Instantiate(proj,tf.position,tf.rotation);
                o.GetComponent<Rigidbody>().AddRelativeForce(new Vector3(0,0,-projVelocity));
                o.GetComponent<Projectile>().setup(damageOfProjectiles,tf.parent.gameObject.transform.parent.gameObject,10);
            }
        }
    }
}
