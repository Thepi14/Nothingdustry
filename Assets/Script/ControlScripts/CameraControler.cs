using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class CameraControler : MonoBehaviour
{
    public GameObject cameraObj;
    public float size;
    public float speed = 0.005f;

    void Start()
    {
        
    }
    void Update()
    {
        size = cameraObj.GetComponent<Camera>().orthographicSize;
        if (Input.GetAxis("Horizontal") < 0)
        {
            cameraObj.transform.position -= new Vector3(speed * size, 0, 0);
        }
        if (Input.GetAxis("Horizontal") > 0)
        {
            cameraObj.transform.position += new Vector3(speed * size, 0, 0);
        }
        if (Input.GetAxis("Vertical") < 0)
        {
            cameraObj.transform.position -= new Vector3(0, speed * size, 0);
        }
        if (Input.GetAxis("Vertical") > 0)
        {
            cameraObj.transform.position += new Vector3(0, speed * size, 0);
        }
        if (Input.mouseScrollDelta.y < 0)
        {
            cameraObj.GetComponent<Camera>().orthographicSize += 0.05f * size/2;
        }
        if (Input.mouseScrollDelta.y > 0)
        {
            cameraObj.GetComponent<Camera>().orthographicSize -= 0.05f * size/2;
        }
    }
}
