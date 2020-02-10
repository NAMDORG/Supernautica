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
    private float QEPress;
    private Vector3 WASD;
    public static Vector3 firstPoint, nextPoint, movementDirection, closestNormal;
    private Rigidbody player;
    private GameObject nearestClingable;
    public static RaycastHit bodyHit;
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
        QERoll();

        // Functions for testing
        PrintToConsole();   
    }

    private void PrintToConsole()
    {
        // Lines to print some stuff to console for testing
        // Delete this when this script is done

        // Direction player is looking in degrees
        //print(transform.eulerAngles + " forward");

        // Direction of the surface normal in degrees
        //print(Quaternion.LookRotation(-lookHit.normal).eulerAngles + " normal");

        // Print quaternions to console
        // Player rotation
        //print(transform.rotation);

        //print(nearestClingable.name);
    }

    // Process all the functions controlled by mouse buttons and movement
    private void MouseControls()
    {
        // Convert mouse movement into mouselook
        Mouselook();
        // Raycast to see if the player is looking at something
        RaycastFromMouselook();

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
        mouseMovementSum = mouseXY * mouseSensitivity;

        // Move object with the mouse - camera is a child of this object so it moves as well
        transform.Rotate(-mouseMovementSum.y, mouseMovementSum.x, 0f, Space.Self);
    }

    private void RaycastFromMouselook()
    {
        //// TODO: This should maybe be attached to the camera, not the player sphere?
        //if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out lookHit, Mathf.Infinity))
        //{
        //    // Make raycast visible on screen
        //    Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * lookHit.distance, Color.yellow);
        //}
    }

    private void QERoll()
    {
        if (Input.GetKey(KeyCode.Q))
        {
            transform.Rotate(0f, 0f, 1.0f);
        }
        else if (Input.GetKey(KeyCode.E))
        {
            transform.Rotate(0f, 0f, -1.0f);
        }
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
            WASDType = MovementType.velocity;
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
        // TODO: Gravity movement
        // This is the starting point for an eventual feature of artificial gravity. Will need adjusted mouselook too.
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

        //AdjustCameraFOV();

        if (Input.GetKey(KeyCode.LeftShift))
        {
            StopVelocity();
        }
    }

    // Adjust camera field of view to give a little tactile feel to acceleration
    private void AdjustCameraFOV()
    {
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S))
        {
            Camera.main.fieldOfView += WASD.y * 10.0f * Time.deltaTime;
            Camera.main.fieldOfView = Mathf.Clamp(Camera.main.fieldOfView, 55.0f, 65.0f);
        }
        else
        {
            Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, 60.0f, 10 * Time.deltaTime);
        }
        print(Camera.main.fieldOfView);
    }

    private void StopVelocity()
    {
        player.velocity = Vector3.zero;
    }

    private void Cling()
    {
        // Run function to see if object is close enough and capable of being clinged to
        movementDirection = transform.position;
        FindClingable();

        if (objectIsClingable == true && playerIsClinging == false)
        {
            PullToObject();
        }
    }

    private void FindClingable()
    {
        //Cast a sphere out from 'movementDirection' that finds 'Clingable' objects
        Collider[] clingables = Physics.OverlapSphere(movementDirection, 2.0f, 1<<8);
        
        // If there is at least one clingable in range, store the closest one as 'nearestClingable' variable
        float nearestDistance = float.MaxValue;
        float distance;
        
        if (clingables.Length > 0)
        {
            foreach (Collider surface in clingables)
            {
                distance = Vector3.Distance(transform.position, surface.ClosestPoint(transform.position));
                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    firstPoint = surface.ClosestPoint(transform.position);
                    nearestClingable = surface.gameObject;
                }
            }
        
            objectIsClingable = true;
        }
    }

    private void PullToObject()
    {
        transform.position = Vector3.MoveTowards(transform.position, firstPoint, 5.0f * Time.deltaTime);
        movingToCling = true;
    }

    private void OnCollisionStay(UnityEngine.Collision collision)
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
        MovementDirection();

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))
        {
            transform.position = Vector3.MoveTowards(transform.position, nextPoint, 3.0f * Time.deltaTime);
        }
    }

    private void MovementDirection()
    {
        FindClosestNormal();

        if (Input.GetAxisRaw("Horizontal") > 0)
        {
            Vector3 rightProjection = Vector3.ProjectOnPlane(transform.right, bodyHit.normal).normalized;
            movementDirection += rightProjection;
        }
        else if (Input.GetAxisRaw("Horizontal") < 0)
        {
            Vector3 leftProjection = Vector3.ProjectOnPlane(-transform.right, bodyHit.normal).normalized;
            movementDirection += leftProjection;
        }
        
        if (Input.GetAxisRaw("Vertical") > 0)
        {
            Vector3 forwardProjection = Vector3.ProjectOnPlane(transform.forward, bodyHit.normal).normalized;
            movementDirection += forwardProjection;
        }
        else if (Input.GetAxisRaw("Vertical") < 0)
        {
            Vector3 backwardProjection = Vector3.ProjectOnPlane(-transform.forward, bodyHit.normal).normalized;
            movementDirection += backwardProjection;
        }

        nextPoint = bodyHit.collider.ClosestPoint(movementDirection) + closestNormal;
        print(nextPoint);
    }

    private void FindClosestNormal()
    {
        nextPoint = firstPoint;
        Vector3 rayDir = nextPoint - movementDirection;

        if (Physics.Raycast(movementDirection, rayDir, out bodyHit, 1f))
        {
            closestNormal = bodyHit.normal * 0.5f;
            nextPoint += closestNormal;
        }
    }
}
