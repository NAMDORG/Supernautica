using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerInventory : MonoBehaviour
{
    public static List<Item> characterItems = new List<Item>();
    public ItemDatabase itemDatabase;
    private string inventory;
    private float oxygenMeter = 60.0f;

    private void Start()
    {
        transform.GetChild(1).position = new Vector2((Screen.width / 2), (Screen.height / 2));
    }

    private void Update()
    {
        ShowInventory();
        OxygenMeter();

        // Text of Child 0 (Show Inventory) = temp string
        //transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = inventory;

        // Text of Child 2 (Oxygen) = oxygen meter variable
        transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = Mathf.Round(oxygenMeter).ToString();
    }

    //=========================================/
    // Inventory functions
    //=========================================/

    public void GiveItem(int id)
    {
        Item itemToAdd = itemDatabase.GetItem(id);
        characterItems.Add(itemToAdd);
    }
    
    public void GiveItem(string itemName)
    {
        Item itemToAdd = itemDatabase.GetItem(itemName);
        characterItems.Add(itemToAdd);
    }
    
    public Item CheckForItem(int id)
    {
        return characterItems.Find(Item => Item.id == id);
    }

    public void ShowInventory()
    {
        inventory = "";
        foreach (Item item in characterItems)
        {
            inventory += item.title + '\n';
        }
    }

    //=========================================/
    // Resource meter functions
    //=========================================/

    private void OxygenMeter()
    {
        oxygenMeter -= Time.deltaTime;
    }

}
