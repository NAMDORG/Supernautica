using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MovementType { position, velocity, clinging };

public class Controls : MonoBehaviour
{
    [SerializeField] float mouseSensitivity = 2f;
    [SerializeField] float movementSpeed = 1f;
    [SerializeField] Vector3 bodyVelocity = Vector3.zero;

    public MovementType WASDType;

    private Vector2 mouseMovementSum;
    private Vector3 WASD;
    private Vector3 pullVelocity = Vector3.zero;
    private Rigidbody player;
    private RaycastHit lookHit;
    public bool objectIsClingable, movingToCling, playerIsClinging;

    private void Start()
    {
        // Make cursor invisible on game start
        Cursor.visible = false;

        // Declare the player meshrigidbody
        player = GetComponent<Rigidbody>();

        WASDType = MovementType.velocity;
    }

    private void Update()
    {
        // Process mouse controls
        MouseControls();

        // Process WASD Controls
        WASDControls();

        // Process other key commands

    }

    // Process all the functions controlled by mouse buttons and movement
    private void MouseControls()
    {
        // Convert mouse movement into mouselook
        Mouselook();

        // What happens when the right mouse button is clicked
        MouseOne();
    }

    // Convert mouse movement in two dimensions into mouselook
    private void Mouselook()
    {
        // How much as the mouse moved in X and Y
        Vector2 mouseXY = new Vector2
            (Input.GetAxisRaw("Mouse X"),
                Input.GetAxisRaw("Mouse Y"));

        // Add new mouse movement to the variable holding camera movement (with mouse sensitivity multiplier)
        mouseMovementSum += mouseXY * mouseSensitivity;

        // Restrict up-down mouse movement to 90 degrees
        //mouseMovementSum.y = Mathf.Clamp(mouseMovementSum.y, -90f, 90f);

        // Move object with the mouse - camera is a child of this object so it moves as well
        this.transform.localRotation = Quaternion.Euler(-mouseMovementSum.y, mouseMovementSum.x, 0f);
    }

    // Process clicking the right mouse button (Cling)
    private void MouseOne()
    {
        if (Input.GetKey(KeyCode.Mouse1))
        {
            Cling();
        }
        else
        {
            objectIsClingable = false;
            movingToCling = false;
            playerIsClinging = false;
        }
    }

    // Turn mouse movement into a Vector2, then decide which Movement Type to process
    private void WASDControls()
    {
        // Pressing a WASD key applies a vector in that direction on a 2d plane
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

    // Movement Type
    // WASD moves player position forward, backward, left, right
    private void WASDPosition()
    {
        // Forward backward left right position for moving under gravity
        this.transform.Translate(WASD.x, 0f, WASD.y);
    }

    // Movement Type
    // WASD applies a force that creates a velocity forward, backward, left, right
    private void WASDVelocity()
    {
        WASD *= movementSpeed;

        //WASD creates a force that adds velocity to the player
        player.AddRelativeForce(WASD.x, 0f, WASD.y);
        bodyVelocity = player.velocity;

        if (Input.GetKey(KeyCode.Space))
        {
            StopVelocity();
        }
    }

    private void StopVelocity()
    {
        player.velocity = Vector3.zero;
    }

    private void Cling()
    {
        if (playerIsClinging == false)
        {
            // Run function to see if object looked at is close enough and capable of being clinged to
            LookingAtClingable();
        }

        if (objectIsClingable == true && playerIsClinging == false)
        {
            PullToObject();
        }
    }

    private void LookingAtClingable()
    {
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out lookHit, 20f))
        {
            // Make the raycast visible on the screen
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * lookHit.distance, Color.yellow);

            // If an object is close enough and eligible to cling, change variable to true
            objectIsClingable = true;

            PullToObject();
        }
    }

    private void PullToObject()
    {
        transform.position = Vector3.SmoothDamp(transform.position, lookHit.point, ref pullVelocity, 0.3f); ;
        movingToCling = true;
    }

    private void OnCollisionEnter(UnityEngine.Collision collision)
    {
        // If the player collides with an object with the right kind of collider while they're 'moving to cling'
        if (collision.gameObject.tag == "Clingable" && movingToCling)
        {
            // Cling to that object
            playerIsClinging = true;
            movingToCling = false;
            WASDType = MovementType.clinging;

            // Zero velocity
            StopVelocity();
        }
    }

    // Movement Type
    // WASD moves player up, down, left, and right relative to an object's surface
    private void WASDClinging()
    {
        // TODO: Cling controls
    }
}
