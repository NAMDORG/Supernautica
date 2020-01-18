﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controls : MonoBehaviour
{

    //Allow control tweaking while testing
    [SerializeField] float mouseSensitivity = 2f;
    [SerializeField] float keySensitivity = 10f;
    //Show the body's velocity in the inspector for bug testing
    [SerializeField] Vector3 bodyVelocity;

    private Vector2 mouseMovementSum;
    private Vector3 WASD;
    private Rigidbody player;

    // Start is called before the first frame update
    void Start()
    {
        player = GetComponent<Rigidbody>();
    }


    // Update is called once per frame
    void Update()
    {
        MouseMovement();
        //WASDPosition();
        WASDVelocity();
    }

    void MouseMovement()
    {
        //How much as the mouse moved in X and Y
        Vector2 mouseXY = new Vector2
            (Input.GetAxisRaw("Mouse X"),
                Input.GetAxisRaw("Mouse Y"));

        //add new mouse movement to the variable holding camera movement (with mouse sensitivity multiplier)
        mouseMovementSum += mouseXY * mouseSensitivity;
        //restrict up-down movement to 90 degrees
        mouseMovementSum.y = Mathf.Clamp(mouseMovementSum.y, -90f, 90f);

        //move camera with mouse
        this.transform.localRotation = Quaternion.Euler(-mouseMovementSum.y, mouseMovementSum.x, 0f);
        //todo figure out euler vs angleaxis and which one I should be using
    }

//    void WASDPosition()
//    {
//        //Apply vector to WASD keys
//        WASD = new Vector3
//            (Input.GetAxisRaw("Horizontal"),
//                Input.GetAxisRaw("Vertical"),
//                    0f);
//
//        WASDMovementSpeed();
//
//        //Apply WASD input to player location
//        this.transform.Translate(WASD.x, 0f, WASD.y);
//    }
//
//    void WASDMovementSpeed()
//    {
//        //speed up movement by holding left shift
//        if (Input.GetKey(KeyCode.LeftShift))
//        {
//            WASD = WASD / keySensitivity * 2f;
//        }
//        else
//        {
//            WASD /= keySensitivity;
//        }
//    }

    private void WASDVelocity()
    {
        //Apply vector to WASD keys
        WASD = new Vector3
            (Input.GetAxisRaw("Horizontal"),
                Input.GetAxisRaw("Vertical"),
                    0f);

        WASD *= keySensitivity;

        //WASD creates a force that adds velocity to the player
        player.AddForce(WASD.x, 0f, WASD.y);
        bodyVelocity = player.velocity;

        StopVelocity();
    }

    private void StopVelocity()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            player.velocity = Vector3.zero;
        }
    }
}