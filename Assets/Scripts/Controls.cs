using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MovementType { position, velocity, clinging };

public class Controls : MonoBehaviour
{
    //Game state
    public static MovementType WASDType;

    //Allow control tweaking while testing
    [SerializeField] float mouseSensitivity = 2f;
    [SerializeField] float keySensitivity = 10f;

    //Show the body's velocity in the inspector for bug testing
    [SerializeField] Vector3 bodyVelocity;

    private Vector2 mouseMovementSum;
    public static Vector3 WASD;
    private Rigidbody player;



    // Start is called before the first frame update
    void Start()
    {
        //declare the player meshrigidbody
        player = GetComponent<Rigidbody>();

        //make cursor invisible when starting the game
        Cursor.visible = false;

        //there are 3 ways WASD keys can be processed. I want the 'velocity' type.
        WASDType = MovementType.clinging;

    }


    // Update is called once per frame
    void Update()
    {
        //process mouselook
        MouseMovement();
        //process what left and right mouseclicks do
        MouseClick();
        //process WASD controls - decide which WASD option should be used
        WASDOptions();
    }

    //****************************
    //Mouse Controls
    //****************************

    void MouseMovement()
    {
        //How much as the mouse moved in X and Y
        Vector2 mouseXY = new Vector2
            (Input.GetAxisRaw("Mouse X"),
                Input.GetAxisRaw("Mouse Y"));

        //add new mouse movement to the variable holding camera movement (with mouse sensitivity multiplier)
        mouseMovementSum += mouseXY * mouseSensitivity;
        //restrict up-down mouse movement to 90 degrees
        mouseMovementSum.y = Mathf.Clamp(mouseMovementSum.y, -90f, 90f);

        //move camera with mouse
        this.transform.localRotation = Quaternion.Euler(-mouseMovementSum.y, mouseMovementSum.x, 0f);
        //todo figure out euler vs angleaxis and which one I should be using
    }

    void MouseClick()
    {
        if (Input.GetKey(KeyCode.Mouse1))
        {
            GetComponent<Cling2>().ClingToObject();
        }
    }

    //*************************
    //WASD Controls
    //*************************

    private void WASDOptions()
    {
        //Pressing a WASD key applies a vector in that direction on a 2d plane
        WASD = new Vector3
            (Input.GetAxisRaw("Horizontal"),
                Input.GetAxisRaw("Vertical"),
                    0f);

        switch (WASDType)
        {
            case MovementType.position:
                WASDPosition();
                break;
            case MovementType.velocity:
                WASDVelocity();
                break;
            case MovementType.clinging:
                WASDClinging();
                break;
        }
    }

    void WASDPosition()
    {
        WASDMovementSpeed();

        //Apply WASD input to player position
        this.transform.Translate(WASD.x, 0f, WASD.y);
    }

    void WASDMovementSpeed()
    {
        //speed up movement by holding left shift
        if (Input.GetKey(KeyCode.LeftShift))
        {
            WASD = WASD / keySensitivity * 2f;
        }
        else
        {
            WASD /= keySensitivity;
        }
    }

    void WASDVelocity()
    {
        WASD *= keySensitivity;

        //WASD creates a force that adds velocity to the player
        player.AddRelativeForce(WASD.x, 0f, WASD.y);
        bodyVelocity = player.velocity;

        if (Input.GetKey(KeyCode.Space))
        {
            StopVelocity();
        }
    }

    void WASDClinging()
    {
        //Apply WASD input to player location
        this.transform.Translate(WASD.x, WASD.y, 0f);
        print(WASD);
    }

    //*****************************
    //Other Controls
    //*****************************

    //Space zeroes velocity, nested in WASDVelocity function

    //*****************************
    //Non-key Functions
    //*****************************

    public void StopVelocity()
    {
        player.velocity = Vector3.zero;
    }

}