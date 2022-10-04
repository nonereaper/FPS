using System.Collections;
using System.Collections.Generic;
using System;
using Unity.Netcode;
using UnityEngine;

public class m_Player : NetworkBehaviour
{
    // rigidbody for this player
    private Rigidbody rb;
    // velocity for player
    [SerializeField] private float movementSpeed;
    // multiplier for movespeed
    [SerializeField] private float sprintSpeedMult;
    // multiplier for movespeed
    [SerializeField] private float crouchSpeedMult;
    // velocity upward
    [SerializeField] private float jump;
    // mouse sen
    [SerializeField] private float mouseSen;
    // ground layer
    [SerializeField] LayerMask ground;

    // is character crouching
    private bool isCrouching;
    // camera's angle up and down
    private NetworkVariable<float> cameraAngle = new NetworkVariable<float>();
    // character's angle left and right 
    private NetworkVariable<float> characterAngle = new NetworkVariable<float>();

    private Controller controller;

    [SerializeField] private GameObject camera;
    [SerializeField] private GameObject leftArm;
    [SerializeField] private GameObject rightArm;
    [SerializeField] private GameObject emptyWeaponLocation;
    [SerializeField] private GameObject leftLeg;
    [SerializeField] private GameObject rightLeg;
    [SerializeField] private GameObject emptyUse;
    [SerializeField] private GameObject emptyProjectile;

    public override void OnNetworkSpawn() {
        cameraAngle.Value = 0f;
        characterAngle.Value = 0f;
        rb = GetComponent<Rigidbody>();
        isCrouching = false;
        controller = GameObject.Find("Controller").GetComponent<Controller>();
    }
    [ServerRpc]
    private void rotatePlayerServerRpc(float angle) {
        rb.MoveRotation(rb.rotation * Quaternion.Euler(new Vector3(0,angle,0)));
        characterAngle.Value = rb.rotation.eulerAngles.y;
    }
    [ServerRpc]
    private void rotateCameraServerRpc(float angle) {
        float angle2 = cameraAngle.Value - angle;
        if (angle2 > 90f)
        angle2 = 90f;
        else if (angle2 < -90f)
        angle2 = -90f;
        characterAngle.Value = angle2;
        camera.transform.localRotation = Quaternion.Euler(angle2,0f,0f);
    }
    [ServerRpc]
    private void movePlayerServerRpc(Vector3 velocity) {
        rb.AddForce(velocity - rb.velocity, ForceMode.VelocityChange);
    }
    private bool isGrounded() {
        return 
        Physics.CheckBox(leftLeg.transform.position,new Vector3(leftLeg.transform.localScale.x/2,leftLeg.transform.localScale.y/2+0.2f,leftLeg.transform.localScale.z/2),leftLeg.transform.rotation,ground) ||
        Physics.CheckBox(rightLeg.transform.position,new Vector3(rightLeg.transform.localScale.x/2,rightLeg.transform.localScale.y/2+0.2f,rightLeg.transform.localScale.z/2),rightLeg.transform.rotation,ground);
    }
    void FixedUpdate() {
        if (!IsOwner) return;
        rotatePlayerServerRpc(Input.GetAxis("Mouse X") * Time.deltaTime * mouseSen);
        rotateCameraServerRpc(Input.GetAxis("Mouse Y") * Time.deltaTime * mouseSen);

        double tempAngle = characterAngle.Value/180*Math.PI, tempAngleP = tempAngle+(Math.PI/2);
        float tempMovementSpeed = movementSpeed;
        if (isCrouching)
            tempMovementSpeed *= crouchSpeedMult;
        else if (Input.GetButton("Sprint"))
            tempMovementSpeed *= sprintSpeedMult;
        double increaseZ = Input.GetAxis("Vertical")*Math.Cos(tempAngle)*tempMovementSpeed + Input.GetAxis("Horizontal")*Math.Cos(tempAngleP)*tempMovementSpeed,
        increaseX = Input.GetAxis("Vertical")*Math.Sin(tempAngle)*tempMovementSpeed + Input.GetAxis("Horizontal")*Math.Sin(tempAngleP)*tempMovementSpeed;
        float increaseY = rb.velocity.y;
        if (Input.GetButtonDown("Jump") && isGrounded()) {
            increaseY = jump;
        }
        movePlayerServerRpc(new Vector3((float)increaseX,increaseY,(float)increaseZ));
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
