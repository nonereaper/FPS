using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;


/* https://www.youtube.com/watch?v=n0GQL5JgJcY&list=PLrnPJCHvNZuB5ATsJZLKX3AW4V9XaIV9b&index=1
    Video series used to write help write movement code
*/
public class PlayerMovement : MonoBehaviour
{
    private Rigidbody rb;
    [SerializeField] float movementSpeed;
    [SerializeField] float jump;
    private double angle;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        angle = 0.0;
    }

    // Update is called once per frame
    void Update()
    {
       float x = rb.velocity.x, y = rb.velocity.y, z = rb.velocity.z;
       double increaseZ = Math.Cos(Math.PI*angle/180)*movementSpeed, increaseX = Math.Sin(Math.PI*angle/180)*movementSpeed;
            /*
        //if (Math.Abs(z) <= 5) {
            rb.AddForce(0,0,),ForceMode.VelocityChange);
        //}
        //if (Math.Abs(x) <= 5) {
            rb.AddForce(,0,0,ForceMode.VelocityChange);
        //}*/
            rb.velocity = new Vector3((float)(Input.GetAxis("Vertical")*increaseX),y,(float)(Input.GetAxis("Vertical") * increaseZ));
      //  if (Math.Abs(rb.velocity.x) <= 5)
       // rb.AddForce(Input.GetAxis("Horizontal") * movementSpeed,0,0,ForceMode.VelocityChange);
       
       rb.MoveRotation(rb.rotation * Quaternion.Euler(new Vector3(0,Input.GetAxis("Horizontal"),0)));
       angle+=Input.GetAxis("Horizontal");
        if (angle > 180.0) {
            angle -= 360.0;
        }
        if (angle < -180.0) {
            angle +=360.0;
        }
        if (Input.GetButtonDown("Jump")) {
            //rb.velocity = new Vector3(x,jump,z);
            rb.velocity = new Vector3(x,jump,z);
        }
    }
}
