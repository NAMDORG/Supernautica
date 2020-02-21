﻿using System;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    private RaycastHit lookHit;
    public Texture2D crosshairImage;
    private GameObject lookHitObject, inventoryUI;
    private bool isResource;
    public static bool lookingAtSame;

    private void Start()
    {
        inventoryUI = GameObject.Find("Inventory");
    }

    // Update is called once per frame
    void Update()
    {
        LookForObject();
        LookingAtClinged();
    }

    private void LookingAtClinged()
    {
        if (lookHitObject == PlayerControls.nearestClingable)
        {
            lookingAtSame = true;
        }
        else
        {
            lookingAtSame = false;
        }
    }

    private void LookForObject()
    {
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out lookHit, Mathf.Infinity))
        {
            lookHitObject = lookHit.collider.gameObject;
            IsLookHitObjectAResource();
        }
    }

    private void IsLookHitObjectAResource()
    {
        if (lookHitObject.GetComponent<Collider>().tag == "Resource")
        {
            isResource = true;
        }
        else
        {
            isResource = false;
        }
    }

    public void GrabObject()
    {
        print("test");
        if (isResource == true && lookHit.distance <= 2.0f)
        {
            Destroy(lookHitObject);
            inventoryUI.GetComponent<PlayerInventory>().GiveItem(lookHitObject.name);
        }

        // TODO: Pull object towards camera (maybe not necessary until I have models to work with)
    }

    private void OnGUI()
    {
        float xMin = (Screen.width / 2) - (crosshairImage.width / 2);
        float yMin = (Screen.height / 2) - (crosshairImage.height / 2);
        GUI.DrawTexture(new Rect(xMin, yMin, crosshairImage.width, crosshairImage.height), crosshairImage);
    }
}
