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
    private string temp;

    private void Update()
    {
        ShowInventory();

        transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = temp;
    }

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

    public Consumable CheckForConsumable(int id)
    {
        return characterConsumables.Find(consumable => consumable.id == id);
    }

    public void ShowInventory()
    {
        temp = "";
        foreach (Consumable consumable in characterConsumables)
        {
            temp += consumable.title + '\n';
        }
    }
}
