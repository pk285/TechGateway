using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.MagicLeap;

/* 
Start receiving MLInputControllerButton input from the Control
Use a bool _enable variable to activate / deactivate the Cube GameObject
Use _rotationSpeed and _distance consts to define the rotation speed of our Cube and the initial distance from the camera
Place the Cube in front of the user when the Home Button is tapped
Rotate the Cube if the Bumper is tapped
 */

/* 
A declaration of the float _rotationSpeed, the float _distance, the bool _enabled and the bool _bumper consts and variables. 
The latter will be turned to true when the Bumper is tapped.
*/

public class ControlScript : MonoBehaviour
{
    private GameObject _cube;
    private MLInputController _controller;
    private const float _rotationSpeed = 30.0f;
    private const float _distance = 2.0f;
    private const float _moveSpeed = 1.2f;
    private bool _enabled = true;
    private bool _bumper = true;

/* 
In our Awake() method, we link the GameObject to our Cube, we deactivate it and we start receiving some input by the Control. 
A OnButtonDown method handles the MLInput.OnControllerButtonDown events that refer to the Bumper.
*/

    void Awake()
    {
        _cube = GameObject.Find("Cube");
        _cube.SetActive(true);
        MLInput.Start();
        MLInput.OnControllerButtonDown += OnButtonDown;
        MLInput.OnControllerButtonUp += OnButtonUp;
        _controller = MLInput.GetController(MLInput.Hand.Left);
    }

/*
Stops all input.
*/

    void OnDestroy()
    {
        MLInput.OnControllerButtonDown -= OnButtonDown;
        MLInput.OnControllerButtonUp -= OnButtonUp;
        MLInput.Stop();
    }

/*
In our Update() method, we rotate the Cube if the Bumper button was tapped (that is, while our _bumper variable is true). 
We also call the CheckControl method which refers to input received from the Trigger and the Touchpad.
*/

    void Update()
    {
        if (_bumper && _enabled)
        {
            _cube.transform.Rotate(Vector3.up, +_rotationSpeed * Time.deltaTime);
        }
        CheckControl();
    }

    void CheckControl()
    {
        if (_controller.TriggerValue > 0.2f && _enabled)
        {
            _bumper = true;
            _cube.transform.Rotate(Vector3.up, -_rotationSpeed * Time.deltaTime);
        }
        else if (_controller.Touch1PosAndForce.z > 0.0f && _enabled)
        {
            float X = _controller.Touch1PosAndForce.x;
            float Y = _controller.Touch1PosAndForce.y;
            Vector3 forward = Vector3.Normalize(Vector3.ProjectOnPlane(transform.forward, Vector3.up));
            Vector3 right = Vector3.Normalize(Vector3.ProjectOnPlane(transform.right, Vector3.up));
            Vector3 force = Vector3.Normalize((X * right) + (Y * forward));
            _cube.transform.position += force * Time.deltaTime * _moveSpeed;
        }
    }

/*
Our OnButtonDown(byte controller_id, MLInputControllerButton button) method has two parameters (both of them should be used). 
The most important one, the button, will be compared against the button inputs recognized by our application. 
If the Bumper input is recognized, we turn our _bumper variable to true.
*/

    void OnButtonDown(byte controller_id, MLInputControllerButton button)
    {
        if ((button == MLInputControllerButton.Bumper && _enabled))
        {
            _bumper = true;
        }
    }

/* 
Similarly, if a HomeTap is recognized, we activate the Cube, we place it in front of the camera and we set the _enable variable to true.
*/

    void OnButtonUp(byte controller_id, MLInputControllerButton button)
    {
        if (button == MLInputControllerButton.HomeTap)
        {
            _cube.SetActive(true);
            _cube.transform.position = transform.position + transform.forward * _distance;
            _cube.transform.rotation = transform.rotation;
            _enabled = true;
        }
    }

}
 