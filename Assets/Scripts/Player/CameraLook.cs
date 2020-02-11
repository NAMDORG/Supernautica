using UnityEngine;

public class CameraLook : MonoBehaviour
{
    private RaycastHit lookHit;
    public Texture2D crosshairImage;
    private GameObject lookHitObject;
    private bool isResource;


    // Update is called once per frame
    void Update()
    {
        LookForObject();
        GrabObject();
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

    private void GrabObject()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (isResource == true && lookHit.distance <= 2.0f)
            {
                Destroy(lookHitObject);
                GetComponent<Inventory>().GiveItem(lookHitObject.name);
                ShowInventory();
            }
        }

        // TODO: Pull object towards camera (maybe not necessary until I have models to work with)
    }

    private void ShowInventory()
    {
        string temp = "";
        foreach(Consumable consumable in Inventory.characterConsumables)
        {
            temp += consumable.title + '\n';
        }
        
        print(temp);
    }

    private void OnGUI()
    {
        float xMin = (Screen.width / 2) - (crosshairImage.width / 2);
        float yMin = (Screen.height / 2) - (crosshairImage.height / 2);
        GUI.DrawTexture(new Rect(xMin, yMin, crosshairImage.width, crosshairImage.height), crosshairImage);
    }
}
