using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    public static List<Consumable> characterConsumables = new List<Consumable>();
    public ConsumableDatabase consumableDatabase;

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
}
