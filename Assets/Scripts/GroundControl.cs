using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundControl : MonoBehaviour
{
    private Player parentClass;
    private Transform tf, upperTf, lowerTf;
    [SerializeField] LayerMask ground;
    // Start is called before the first frame update
    void Start()
    {
        tf = GetComponent<Transform>();
        upperTf = tf.GetChild(0).GetComponent<Transform>();
        lowerTf = tf.GetChild(1).GetComponent<Transform>();
        parentClass = GetComponentInParent<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        parentClass.setGrounded(Physics.CheckBox(lowerTf.position,new Vector3(lowerTf.localScale.x/2,lowerTf.localScale.y/2+0.2f,lowerTf.localScale.z/2),lowerTf.rotation,ground));
        if (parentClass.getCrouching()) {
            upperTf.localPosition = new Vector3(0,0.4f,0.15f);
            upperTf.localRotation = Quaternion.Euler(-25,0,0);
            lowerTf.localPosition = new Vector3(0,0.15f,-0.15f);
        } else {
            upperTf.localPosition = new Vector3(0,0.25f,0);
            upperTf.localRotation = Quaternion.Euler(0,0,0);
            lowerTf.localPosition = new Vector3(0,-0.25f,-0);
        }
    }
}
