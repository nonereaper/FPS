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
    [SerializeField] float sprintSpeedMult;
    [SerializeField] float jump;
    private float cameriaAngle;
    private float angle;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        angle = 0.0f;
        cameriaAngle = 0.0f;
    }
    public void rotatePlayer(float an) {
        angle += an;
        angle += fixAngle(angle);
        rb.MoveRotation(rb.rotation * Quaternion.Euler(new Vector3(0,an,0)));
    }
    public void setCameriaAngle(float ca) {
        cameriaAngle = ca;
    }
    public float getCameriaAngle() {
        return cameriaAngle;
    }
    public void addCameriaAngle(float ca) {
        cameriaAngle += ca;
    }
    private float fixAngle(float an) {
        if (an > 180.0f)
            return -360.0f;
        else if (an < -180.0f)
            return 360.0f;
        else 
            return 0.0f;
    }
    // Update is called once per frame
    void Update()
    {
        float x = rb.velocity.x, y = rb.velocity.y, z = rb.velocity.z;
        double tempAngle = Math.PI*angle/180, tempAngleP = tempAngle+(Math.PI/2);
        if (tempAngleP > Math.PI) {
            tempAngleP -= Math.PI*2;
        }
        float tempMovementSpeed = movementSpeed;
        if (Input.GetButton("Crotch")) {
            tempMovementSpeed *= 0.7f;
        } else if (Input.GetButton("Sprint")) {
            tempMovementSpeed *= sprintSpeedMult;
        }
        double increaseZ = Input.GetAxis("Vertical")*Math.Cos(tempAngle)*tempMovementSpeed + Input.GetAxis("Horizontal")*Math.Cos(tempAngleP)*tempMovementSpeed,
        increaseX = Input.GetAxis("Vertical")*Math.Sin(tempAngle)*tempMovementSpeed + Input.GetAxis("Horizontal")*Math.Sin(tempAngleP)*tempMovementSpeed;
        rb.velocity = new Vector3((float)(increaseX),y,(float)(increaseZ));

      //  if (Math.Abs(rb.velocity.x) <= 5)
       // rb.AddForce(Input.GetAxis("Horizontal") * movementSpeed,0,0,ForceMode.VelocityChange);
       
       
       //rotatePlayer(Input.GetAxis("Horizontal")*Time.deltaTime);
        
        if (Input.GetButtonDown("Jump")) {
            //rb.velocity = new Vector3(x,jump,z);
            rb.velocity = new Vector3(x,jump,z);
        }
    }
}
