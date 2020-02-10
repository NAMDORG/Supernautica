using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConsumableDatabase : MonoBehaviour
{
    public List<Consumable> consumables = new List<Consumable>();

    private void Awake()
    {
        BuildDatabase();
    }

    public Consumable GetConsumable(int id)
    {
        return consumables.Find(consumables => consumables.id == id);
    }

    public Consumable GetConsumable(string consumableName)
    {
        return consumables.Find(consumables => consumables.title == consumableName);
    }

    void BuildDatabase()
    {
        // TODO: I need to figure out resources for this game.
        consumables = new List<Consumable>() { (new Consumable(0, "Oxygen Tank", "A tank filled with breathable air.")) };
    }
}
