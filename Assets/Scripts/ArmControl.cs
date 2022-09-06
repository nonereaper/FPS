using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class ArmControl : MonoBehaviour
{
    [SerializeField] GameObject proj;
    private float projVelocity;
    private PlayerMovement parentClass;
    private Transform tf;

    // Start is called before the first frame update
    void Start()
    {
        tf = GetComponent<Transform>();
        parentClass = GetComponentInParent<PlayerMovement>();
        projVelocity = 700f;
    }

    // Update is called once per frame
    void Update()
    {
        double tempAngle = -(parentClass.getCameriaAngle())/180*Math.PI;
        double changeX = Math.Cos(tempAngle)*0.25, changeY = Math.Sin(tempAngle)*0.25;
        tf.localPosition = new Vector3(tf.localPosition.x,(float)(changeY+0.25f),(float)changeX);
        tf.localRotation = Quaternion.Euler(parentClass.getCameriaAngle()-90,tf.localRotation.y,tf.localRotation.z);
        if (Input.GetButtonDown("Fire1")) {
            GameObject o = Instantiate(proj,new Vector3(tf.position.x+10f,tf.position.y,tf.position.z),
            Quaternion.Euler(parentClass.getCameriaAngle()-90,tf.rotation.y,tf.rotation.z));
            o.GetComponent<Rigidbody>().AddRelativeForce(new Vector3(0, projVelocity,0));
        }
    }
}
