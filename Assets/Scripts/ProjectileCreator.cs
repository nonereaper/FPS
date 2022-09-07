using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileCreator : MonoBehaviour
{
        [SerializeField] GameObject proj;
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
            double changeZ = Math.Cos(tempAngle)*projVelocity, changeX = Math.Sin(tempAngle)*projVelocity;

            //float changeCZ = (float)(Math.Cos(tempCameriaAngle)*distance), changeCY = (float)(Math.Sin(tempCameriaAngle)*distance);
            GameObject o = Instantiate(proj,tf.position,tf.rotation);
            //(float)changeX,0,(float)changeZ)
            o.GetComponent<Rigidbody>().AddRelativeForce(new Vector3(0,0,-projVelocity));
            o.GetComponent<Projectile>().setup(0,tf.parent.gameObject.transform.parent.gameObject);
        }
    }
}
