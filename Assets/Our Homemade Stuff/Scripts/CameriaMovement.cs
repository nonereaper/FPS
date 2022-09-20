using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameriaMovement : MonoBehaviour
{
    [SerializeField] float mouseSen;
    private Player parentClass;
    private bool firstPerson;
    private Transform tf;
    private Transform projectileCreatorTransform;
    // Start is called before the first frame update
    void Start()
    {
        tf = GetComponent<Transform>();
        Cursor.lockState = CursorLockMode.Locked;
        firstPerson = true;
        parentClass = GetComponentInParent<Player>();
        projectileCreatorTransform = tf.GetChild(0).GetComponent<Transform>();
    }
    public void updateCameria(float amount) {
        float angleV = parentClass.getCameriaAngle()-amount;
            if (!firstPerson) {
                if (angleV >= 30.0) {
                    angleV = 30.0f;
                }
                if (angleV <= -30.0) {
                    angleV = -30.0f;
                }
            } else {
                if (angleV >= 90.0) {
                    angleV = 90.0f;
                }
                if (angleV <= -90.0) {
                    angleV = -90.0f;
                }
            }
        parentClass.setCameriaAngle(angleV);
        tf.localRotation = Quaternion.Euler(parentClass.getCameriaAngle(),0,0);   
    }
    // Update is called once per frame
    void Update()
    {   
        parentClass.rotatePlayer(Input.GetAxis("Mouse X") * Time.deltaTime * mouseSen);
        updateCameria(Input.GetAxis("Mouse Y") * Time.deltaTime * mouseSen);
        
        
        if (Input.GetButtonDown("ChangeMouseView")) {
            if (firstPerson) {
                tf.localPosition = new Vector3(0,1.5f,-4f);
                parentClass.setCameriaAngle(0);
                tf.localRotation = Quaternion.Euler(parentClass.getCameriaAngle(),0,0);
                projectileCreatorTransform.localPosition = new Vector3(0,-0.6f,9.164f);
            } else {
                tf.localPosition = new Vector3(0,0.9f,0);
                projectileCreatorTransform.localPosition = new Vector3(0,0,2.5f);
            }
            firstPerson = !firstPerson;
        }
    }
}
