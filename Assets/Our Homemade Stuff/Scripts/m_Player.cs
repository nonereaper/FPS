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
    private NetworkVariable<bool> isCrouching = new NetworkVariable<bool>();
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
        isCrouching.value = false;
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
        rotateArmsServerRpc(angle2);
        camera.transform.localRotation = Quaternion.Euler(angle2,0f,0f);
    }
    [ServerRpc]
    private void rotateArmsServerRpc(float angle) {
        double changeX = Math.Cos(angle)*0.25, changeY = Math.Sin(angle)*0.25;
        leftArm.transform.localPosition = new Vector3(leftArm.transform.localPosition.x,(float)(changeY+0.23f),(float)changeX);
        leftArm.transform.localRotation = Quaternion.Euler(angle,0,0);
        rightArm.transform.localPosition = new Vector3(rightArm.transform.localPosition.x,(float)(changeY+0.23f),(float)changeX);
        rightArm.transform.localRotation = Quaternion.Euler(angle,0,0);
    }
    [ServerRpc]
    private void movePlayerServerRpc(Vector3 velocity) {
        rb.AddForce(velocity - rb.velocity, ForceMode.VelocityChange);
    }
    [ServerRpc]
    private void crouchPlayerServerRpc() {
        Transform upperLTf = leftLeg.transform.GetChild(0), lowerLTf = leftLeg.transform.GetChild(1),
        upperRTf = rightLeg.transform.GetChild(0), lowerRTf = rightLeg.transform.GetChild(1);
        if (isCrouching.value) {
            upperLTf.localPosition = new Vector3(0,0.4f,0.15f);
            upperLTf.localRotation = Quaternion.Euler(-25,0,0);
            lowerLTf.localPosition = new Vector3(0,0.15f,-0.15f);
            upperRTf.localPosition = new Vector3(0,0.4f,0.15f);
            upperRTf.localRotation = Quaternion.Euler(-25,0,0);
            lowerRTf.localPosition = new Vector3(0,0.15f,-0.15f);
        } else {
            upperLTf.localPosition = new Vector3(0,0.25f,0);
            upperLTf.localRotation = Quaternion.Euler(0,0,0);
            lowerLTf.localPosition = new Vector3(0,-0.25f,-0);
            upperRTf.localPosition = new Vector3(0,0.25f,0);
            upperRTf.localRotation = Quaternion.Euler(0,0,0);
            lowerRTf.localPosition = new Vector3(0,-0.25f,-0);
        }
    }
    private bool isGrounded() {
        return 
        Physics.CheckBox(leftLeg.transform.GetChild(1).position,new Vector3(leftLeg.transform.GetChild(1).localScale.x/2,leftLeg.transform.GetChild(1).localScale.y/2+0.2f,leftLeg.transform.GetChild(1).localScale.z/2),leftLeg.transform.GetChild(1).rotation,ground) ||
        Physics.CheckBox(rightLeg.transform.GetChild(1).position,new Vector3(rightLeg.transform.GetChild(1).localScale.x/2,rightLeg.transform.GetChild(1).localScale.y/2+0.2f,rightLeg.transform.GetChild(1).localScale.z/2),rightLeg.transform.GetChild(1).rotation,ground);
    }
    void FixedUpdate() {
        if (!IsOwner) return;
        rotatePlayerServerRpc(Input.GetAxis("Mouse X") * Time.deltaTime * mouseSen);
        rotateCameraServerRpc(Input.GetAxis("Mouse Y") * Time.deltaTime * mouseSen);

        if (Input.GetButtonDown("Crouch")) {
            isCrouching.value = true;
            crouchPlayerServerRpc();
        }
        if (Input.GetButtonUp("Crouch")) {
            isCrouching.value = false;
            crouchPlayerServerRpc();
        }
        double tempAngle = characterAngle.Value/180*Math.PI, tempAngleP = tempAngle+(Math.PI/2);
        float tempMovementSpeed = movementSpeed;
        if (isCrouching.value)
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
