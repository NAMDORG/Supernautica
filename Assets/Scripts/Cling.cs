using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cling : MonoBehaviour
{
    public bool objectIsClingable, movingToCling, playerIsClinging;

    private Vector3 clingPoint;
    public RaycastHit hit;
    private Vector3 velocity = Vector3.zero;
    private Transform capsule;

    public void ClingToObject()
    {
        LookingAtCloseObject();

        if (objectIsClingable == true && playerIsClinging == false)
        {
            PullToObject();
        }
        else if (playerIsClinging == true)
        {
            //run cling movement function
            ClingMovement();
        }
    }

    public void Update()
    {
        //if not holding the right mouse button, player is no longer clinging. Change variables to reflect.
        if (Input.GetKey(KeyCode.Mouse1) == false)
        {
            objectIsClingable = false;
            movingToCling = false;
            playerIsClinging = false;
            this.gameObject.transform.parent = null;
            Controls.WASDType = MovementType.velocity;
        }
    }

    void LookingAtCloseObject()
    {
        //draw a ray stretching out 1.5 units
        //if ray intersects an object, change 'clingable' value to true
        //todo make only certain objects clingable
  
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, 1.5f))
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
            objectIsClingable = true;
            clingPoint = hit.point;
        }
    }

    private void PullToObject()
    {
        if (playerIsClinging == false)
        {
            transform.position = Vector3.SmoothDamp(transform.position, clingPoint, ref velocity, 0.3f);
        }
        movingToCling = true;
    }

    private void OnCollisionEnter(UnityEngine.Collision collision)
    {
        //if the player collides with an object with the right kind of collider while they're 'moving to cling'
        if (collision.gameObject.tag == "Clingable" &&  movingToCling)
        {
            //cling to that object
            playerIsClinging = true;
            //zero velocity
            GetComponent<Controls>().StopVelocity();
            //set an object variable as the parent of that clingable object
            capsule = hit.collider.gameObject.transform.parent;
        }

        //todo collision only happens once. make collisions constantly happening?
    }

    private void ClingMovement()
    {
        //set the player's parent as the parent of the object they're clinging to
        this.gameObject.transform.SetParent(capsule);
        Controls.WASDType = MovementType.clinging;
    }
}
