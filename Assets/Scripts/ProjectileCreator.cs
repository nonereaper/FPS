using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileCreator : MonoBehaviour
{
    [SerializeField] GameObject proj;
    // the spread of the projectile that will based off a radian
    [SerializeField] float distanceOfSpread;
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
            double tempAngle = Math.PI*parentClass.getAngle()/180, spreadAngleDiff = Math.Atan(distanceOfSpread/projVelocity);
            for (int i = 0; i < numberOfProjectiles; i++) {
                double tempAngle2 = tempAngle + Math.PI*UnityEngine.Random.Range(-(float)spreadAngleDiff,(float)spreadAngleDiff);
                //Debug.Log((Math.Cos(tempAngle)*projVelocity-changeZ) + " " + (Math.Sin(tempAngle)*projVelocity-changeX));
                //Debug.Log(tf.rotation.eulerAngles.x + ", " + tf.rotation.eulerAngles.y + ", " + tf.rotation.eulerAngles.z);
                double rotationTempX = tf.rotation.eulerAngles.x, rotationTempY = tf.rotation.eulerAngles.y;
                rotationTempX += 180*Math.PI*UnityEngine.Random.Range(-(float)spreadAngleDiff,(float)spreadAngleDiff);
                rotationTempY += 180*Math.PI*UnityEngine.Random.Range(-(float)spreadAngleDiff,(float)spreadAngleDiff);
                Quaternion q = Quaternion.Euler((float)rotationTempX, (float)rotationTempY, tf.rotation.eulerAngles.z);
                GameObject o = Instantiate(proj,tf.position,q);
                o.GetComponent<Rigidbody>().AddRelativeForce(new Vector3(0,0,-projVelocity));
                o.GetComponent<Projectile>().setup(damageOfProjectiles,tf.parent.gameObject.transform.parent.gameObject,10);
            }
        }
    }
}
