using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitControls : MonoBehaviour
{

    public float speed = 5.0f;
    public Transform center;
    public Camera cam;
    private float deltaX;
    private float deltaY;


    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Mouse1))
        {

            deltaX = Input.GetAxis("Mouse X");
            deltaY = Input.GetAxis("Mouse Y");
            transform.RotateAround(center.position, new Vector3(0, 1, 0), deltaX * speed);
            transform.RotateAround(center.position, cam.transform.right, -deltaY * speed);

        }
    }
}
