using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameriaMovement : MonoBehaviour
{
    private double angleH, angleV;
    private float oriX, oriY;
    private Transform tf;
    // Start is called before the first frame update
    void Start()
    {
        tf = GetComponent<Transform>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = true;
        angleH = 0;
        angleV = 0;
        oriX = Screen.width/2;
        oriY = Screen.height/2;
    }

    // Update is called once per frame
    void Update()
    {
        float difX = Input.GetAxis("Mouse X") * Time.deltaTime * 100, difY = Input.GetAxis("Mouse Y") * Time.deltaTime * 100;
        
        tf.Rotate(tf.rotation * Quaternion.Euler(new Vector3(difX,difY,0)),Space.Self);
    }
}
