using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controls : MonoBehaviour
{

    //Allow control tweaking while testing
    [SerializeField] float mouseSensitivity = 1f;

    private Vector2 mouseMovementSum;
    private Transform body;

    // Start is called before the first frame update
    void Start()
    {
        body = this.transform.parent.transform;
    }

    // Update is called once per frame
    void Update()
    {
        MouseMovement();
        WASDMovement();
    }

    void MouseMovement()
    {
        //How much as the mouse moved in X and Y
        Vector2 mouseXY = new Vector2
            (Input.GetAxisRaw("Mouse X"),
                Input.GetAxisRaw("Mouse Y"));

        //add new mouse movement to the variable holding camera movement
        mouseMovementSum += mouseXY * mouseSensitivity;

        //up-down camera movement
        this.transform.localRotation = Quaternion.AngleAxis(-mouseMovementSum.y, Vector3.right);

        //rotate parent body object left-right, along with camera
        body.localRotation = Quaternion.AngleAxis(mouseMovementSum.x, Vector3.up);
    }

    void WASDMovement()
    {
        //todo wasd movement of body
    }

}
