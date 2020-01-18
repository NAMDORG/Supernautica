using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cling : MonoBehaviour
{
    public bool objectIsClingable, movingToCling, playerIsClinging;

    private Vector3 clingPoint;
    private RaycastHit hit;
    private Vector3 velocity = Vector3.zero;

    public void ClingToObject()
    {
        LookingAtCloseObject();

        if (objectIsClingable == true && playerIsClinging == false)
        {
            PullToObject();
        }
    }

    public void Update()
    {
        if (Input.GetKey(KeyCode.Mouse1) == false)
        {
            objectIsClingable = false;
            movingToCling = false;
            playerIsClinging = false;
        }
    }

    void LookingAtCloseObject()
    {
        //draw a ray stretching out 1.5 units
        //if ray intersects an object, change 'clingable' value to true
        //todo make only certain objects clingable
  
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, 10f))
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
            objectIsClingable = true;
            clingPoint = hit.point;
        }
    }

    private void PullToObject()
    {
        transform.position = Vector3.SmoothDamp(transform.position, clingPoint, ref velocity, 0.3f);
        movingToCling = true;
    }

    private void OnCollisionEnter(UnityEngine.Collision collision)
    {
        if (collision.gameObject.tag == "Clingable" &&  movingToCling)
        {
            //If the GameObject has the same tag as specified, output this message in the console
            playerIsClinging = true;
        }
    }
}
