using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GyroControl : MonoBehaviour {

    private bool isGyroEnabled;
    private Gyroscope gyroscope;

    private GameObject cameraHolder;
    private Quaternion gRotarion;

	// Use this for initialization
	void Start ()
    {
        cameraHolder = new GameObject("Camera Holder");

        //assuming this script is attached to the MainCamera
        cameraHolder.transform.position = transform.position;
        transform.SetParent(cameraHolder.transform);

        isGyroEnabled = EnableGyro();
	}
	
	// Update is called once per frame
	void Update ()
    {
		if(isGyroEnabled)
        {
            //changing camera rotation according to gyroscope input.
            transform.localRotation = gyroscope.attitude * gRotarion;
        }
	}

    private bool EnableGyro()
    {
        //checking if the device have built-in gyroscope
        if(SystemInfo.supportsGyroscope)
        {
            gyroscope = Input.gyro;
            gyroscope.enabled = true;

            //pointing forward when start.
            cameraHolder.transform.rotation = Quaternion.Euler(90f, 90f, 0f);
            gRotarion = new Quaternion(0, 0, 1, 0);
            return true;
        }

        return false;
    }
}
