using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class ArmControl : MonoBehaviour
{
    private Player parentClass;
    private Transform tf;

    // Start is called before the first frame update
    void Start()
    {
        tf = GetComponent<Transform>();
        parentClass = GetComponentInParent<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        double tempAngle = -(parentClass.getCameriaAngle())/180*Math.PI;
        double changeX = Math.Cos(tempAngle)*0.25, changeY = Math.Sin(tempAngle)*0.25;
        tf.localPosition = new Vector3(tf.localPosition.x,(float)(changeY+0.9f),(float)changeX);
        tf.localRotation = Quaternion.Euler(parentClass.getCameriaAngle(),0,0);
    }
}
