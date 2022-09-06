using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundControl : MonoBehaviour
{
    private PlayerMovement parentClass;
    private Transform tf;
    [SerializeField] LayerMask ground;
    // Start is called before the first frame update
    void Start()
    {
        tf = GetComponent<Transform>();
        parentClass = GetComponentInParent<PlayerMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        parentClass.setGrounded(Physics.CheckBox(tf.position,new Vector3(tf.localScale.x/2,tf.localScale.y/2+0.2f,tf.localScale.z/2),tf.rotation,ground));
    }
}
