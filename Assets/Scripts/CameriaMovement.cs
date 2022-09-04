using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameriaMovement : MonoBehaviour
{
    private float angleV;
    private bool firstPerson;
    private Transform tf;
    // Start is called before the first frame update
    void Start()
    {
        tf = GetComponent<Transform>();
        Cursor.lockState = CursorLockMode.Locked;
        angleV = 0;
        firstPerson = true;
    }

    // Update is called once per frame
    void Update()
    {
        
        float difX = Input.GetAxis("Mouse X") * Time.deltaTime * 100f, difY = Input.GetAxis("Mouse Y") * Time.deltaTime * 100f;
        GetComponentInParent<PlayerMovement>().rotatePlayer(difX);
        angleV -= difY;
        if (angleV >= 90.0) {
            angleV = 90.0f;
        }
        if (angleV <= -90.0) {
            angleV = -90.0f;
        }
        tf.localRotation = Quaternion.Euler(angleV,0,0);
        if (Input.GetButtonDown("ChangeMouseView")) {
            if (firstPerson) {
                tf.localPosition = new Vector3(0,2f,-5f);
            } else {
                tf.localPosition = new Vector3(0,0.9f,0);
            }
            firstPerson = !firstPerson;
        }
    }
}
