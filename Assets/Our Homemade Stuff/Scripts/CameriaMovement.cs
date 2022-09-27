using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameriaMovement : MonoBehaviour
{
    [SerializeField] float mouseSen;
    private Player parentClass;
    private bool firstPerson, cursorIn;
    private Transform tf;
    // Start is called before the first frame update
    void Start()
    {
        tf = GetComponent<Transform>();
        Cursor.lockState = CursorLockMode.Locked;
        firstPerson = true;
        cursorIn = true;
        parentClass = GetComponentInParent<Player>();
    }
    public void updateCameria(float amount) {
        float angleV = parentClass.getCameriaAngle()-amount;
            
           if (angleV >= 90.0) {
               angleV = 90.0f;
           }
                if (angleV <= -90.0) {
               angleV = -90.0f;
           }
            
        parentClass.setCameriaAngle(angleV);
        tf.localRotation = Quaternion.Euler(parentClass.getCameriaAngle(),0,0);   
    }
    // Update is called once per frame
    void Update()
    {   
        if (cursorIn) {
            parentClass.rotatePlayer(Input.GetAxis("Mouse X") * Time.deltaTime * mouseSen);
            updateCameria(Input.GetAxis("Mouse Y") * Time.deltaTime * mouseSen);
        }
        
        if (Input.GetButtonDown("ChangeMouseView")) {
            if (firstPerson) {
                tf.localPosition = new Vector3(0,1.5f,-4f);
                tf.localRotation = Quaternion.Euler(parentClass.getCameriaAngle(),0,0);
            } else {
                tf.localPosition = new Vector3(0,0.9f,0);
            }
            firstPerson = !firstPerson;
        }
        if (Input.GetButtonDown("LockMouse")) {
            cursorIn = !cursorIn;
            if (cursorIn) {
                Cursor.lockState = CursorLockMode.Locked;
            } else {
                Cursor.lockState = CursorLockMode.None;
            }
        }
    }
}
