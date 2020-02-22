using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerInventory : MonoBehaviour
{
    public static List<Consumable> characterConsumables = new List<Consumable>();
    public ConsumableDatabase consumableDatabase;
    private string inventory;
    private float oxygenMeter = 60.0f;
    Stack<Consumable> newStack = new Stack<Consumable> { };

    private void Start()
    {
        transform.GetChild(1).position = new Vector2((Screen.width / 2), (Screen.height / 2));
    }

    private void Update()
    {
        ShowInventory();
        OxygenMeter();

        // Text of Child 0 (Show Inventory) = temp string
        transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = inventory;

        // Text of Child 2 (Oxygen) = oxygen meter variable
        transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = Mathf.Round(oxygenMeter).ToString();
    }

    //=========================================/
    // Inventory functions
    //=========================================/

    public void GiveItem(int id)
    {
        Consumable consumableToAdd = consumableDatabase.GetConsumable(id);
        characterConsumables.Add(consumableToAdd);
    }

    public void GiveItem(string consumableName)
    {
        Consumable consumableToAdd = consumableDatabase.GetConsumable(consumableName);
        characterConsumables.Add(consumableToAdd);
    }

    public void CreateStack(string consumableName)
    {
        Consumable consumableToAdd = consumableDatabase.GetConsumable(consumableName);
        newStack.Push(consumableToAdd);
    }

    public Consumable CheckForConsumable(int id)
    {
        return characterConsumables.Find(consumable => consumable.id == id);
    }

    public void ShowInventory()
    {
        inventory = "";
        foreach (Consumable consumable in characterConsumables)
        {
            inventory += consumable.title + '\n';
        }
    }

    //=========================================/
    // Resource meter functions
    //=========================================/

    private void OxygenMeter()
    {
        oxygenMeter -= Time.deltaTime;
    }

    //=========================================/
    // Inventory v2
    //=========================================/


}
