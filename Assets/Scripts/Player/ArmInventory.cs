using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArmInventory : MonoBehaviour
{
    public static List<Item> characterItems = new List<Item>();
    public ItemDatabase itemDatabase;
    private string inventory;

    GameObject armScreen;
    bool inventoryEnabled;

    private void Start()
    {
        armScreen = GameObject.Find("ArmScreen");
        armScreen.transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = false;
        armScreen.transform.GetChild(1).GetComponent<Canvas>().enabled = false;
    }

    private void Update()
    {
        ShowInventory();

        inventoryKey();
    }

    private void inventoryKey()
    {
        if ((Input.GetKeyDown(KeyCode.I)) && (!inventoryEnabled))
        {
            inventoryEnabled = true;
        }
        else if ((Input.GetKeyDown(KeyCode.I)) && (inventoryEnabled))
        {
            inventoryEnabled = false;
        }


        armScreen.transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = inventoryEnabled;
        armScreen.transform.GetChild(1).GetComponent<Canvas>().enabled = inventoryEnabled;
    }

    public void GiveItem(string itemName)
    {
        Item itemToAdd = itemDatabase.GetItem(itemName);
        characterItems.Add(itemToAdd);
    }

    public void ShowInventory()
    {
        InsertSprite();

    }

    private void InsertSprite()
    {
        Image spriteCell;
        int index = 0;

        foreach (Item item in characterItems)
        {
            spriteCell = transform.GetChild(index).GetChild(0).GetComponentInChildren<Image>();
            spriteCell.enabled = true;
            spriteCell.sprite = item.icon;

            index += 1;
        }
    }
}
