using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cling2 : MonoBehaviour
{

    public bool objectIsClingable, movingToCling, playerIsClinging;

    private Vector3 surfaceNormal, closestPoint;
    public RaycastHit lookHit, bodyHit;
    private Vector3 velocity = Vector3.zero;
    private Transform capsule;

    public void Update()
    {
        //if not holding the right mouse button, player is no longer clinging. Change variables to reflect.
        if (Input.GetKey(KeyCode.Mouse1) == false)
        {
            objectIsClingable = false;
            movingToCling = false;
            playerIsClinging = false;
            //Controls.WASDType = MovementType.velocity;
        }
    }

    public void ClingToObject()
    {
        LookingAtCloseObject();

        if (objectIsClingable == true && playerIsClinging == false)
        {
            PullToObject();
        }

        else if (playerIsClinging == true)
        {
            RaycastAtNewPosition();
        }
    }

    void LookingAtCloseObject()
    {
        //draw a ray stretching out x units
        //if ray intersects an object, change 'clingable' value to true
        //todo make only certain objects clingable

        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out lookHit, 20f))
        {
            //make the raycast visible on the screen
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * lookHit.distance, Color.yellow);
            
            //if an object is close enough and eligible to cling, change variable to true
            objectIsClingable = true;
        }
        
        if (lookHit.collider)
        {
            closestPoint = lookHit.collider.ClosestPoint(transform.position);
        }
    }

    private void PullToObject()
    {
        transform.position = Vector3.SmoothDamp(transform.position, closestPoint, ref velocity, 0.3f);
        movingToCling = true;
    }

    private void OnCollisionEnter(UnityEngine.Collision collision)
    {
        //if the player collides with an object with the right kind of collider while they're 'moving to cling'
        if (collision.gameObject.tag == "Clingable" && movingToCling)
        {
            //cling to that object
            playerIsClinging = true;
            movingToCling = false;

            //zero velocity
            GetComponent<Controls>().StopVelocity();
        }
    }

    private void RaycastAtNewPosition()
    {
        MovementDirection();



    }

    private void MovementDirection()
    {
        Vector3 movementDirection = Vector3.zero;

        if (Input.GetAxisRaw("Horizontal") > 0)
        {
            movementDirection += transform.right;
        }
        else if (Input.GetAxisRaw("Horizontal") < 0)
        {
            movementDirection += -transform.right;
        }

        if (Input.GetAxisRaw("Vertical") > 0)
        {
            movementDirection += transform.up;
        }
        else if (Input.GetAxisRaw("Vertical") < 0)
        {
            movementDirection += -transform.up;
        }

        movementDirection.Normalize();
        print(movementDirection);
    }
}
