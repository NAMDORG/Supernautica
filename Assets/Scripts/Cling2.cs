using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cling2 : MonoBehaviour
{

    public bool objectIsClingable, movingToCling, playerIsClinging;

    private Vector3 surfaceNormal, closestPoint, movementDirection;
    public static Vector3 newClosestPoint, newClosestPointOffset;
    public RaycastHit lookHit, bodyHit;
    private Vector3 velocity = Vector3.zero;
    private Transform capsule;

    [SerializeField] float speed = 1f;

    public void Update()
    {
        //if not holding the right mouse button, player is no longer clinging. Change variables to reflect.
        if (Input.GetKey(KeyCode.Mouse1) == false)
        {
            objectIsClingable = false;
            movingToCling = false;
            playerIsClinging = false;
            Controls.WASDType = MovementType.velocity;
        }
    }

    public void ClingToObject()
    {
        if (playerIsClinging == false)
        {
            LookingAtCloseObject();
        }

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
    }

    private void PullToObject()
    {
        transform.position = Vector3.SmoothDamp(transform.position, lookHit.point, ref velocity, 0.3f); ;
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
            Controls.WASDType = MovementType.clinging;

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
        movementDirection = transform.position;

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

        newClosestPoint = lookHit.collider.ClosestPoint(movementDirection);
        newClosestPoint += lookHit.normal * .5f;
    }

    public void RaycastTest()
    {
        Vector3 rayDir = newClosestPoint - movementDirection;

        if (Physics.Raycast(movementDirection, rayDir, out bodyHit, 1f))
        {
            //make the raycast visible on the screen
            Debug.DrawRay(newClosestPoint,bodyHit.normal,Color.red);
            newClosestPoint +=  bodyHit.normal;
        }
    }
}
