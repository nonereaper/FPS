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
    private float projVelocity;
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
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetButtonDown("Fire1")) {
            double tempAngle = Math.PI*parentClass.getAngle()/180;
         //   for (int i = 0; i < numberOfProjectiles; i++) {
                double tempAngle2 = tempAngle*UnityEngine.Random.Range(-angleSpread,angleSpread);

                double changeZ = Math.Cos(tempAngle2)*projVelocity, changeX = Math.Sin(tempAngle2)*projVelocity;

                GameObject o = Instantiate(proj,tf.position,tf.rotation);
                o.GetComponent<Rigidbody>().AddRelativeForce(new Vector3(0,0,-projVelocity));
                o.GetComponent<Projectile>().setup(damageOfProjectiles,tf.parent.gameObject.transform.parent.gameObject,10);
          //  }
        }
    }
}
