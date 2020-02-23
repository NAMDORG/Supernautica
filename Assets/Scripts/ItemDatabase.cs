using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDatabase : MonoBehaviour
{
    public static List<Item> items = new List<Item>();

    private void Awake()
    {
        BuildDatabase();
    }

    public Item GetItem(int id)
    {
        return items.Find(Items => Items.id == id);
    }

    public Item GetItem(string ItemName)
    {
        return items.Find(Items => Items.title == ItemName);
    }

    void BuildDatabase()
    {
        // TODO: I need to figure out resources for this game.
        items = new List<Item>()
        {
            (new Item(0, "Oxygen Tank", "A tank filled with breathable air.")),
            (new Item(1, "Health Pack", "A kit that assists with healing."))
        };
    }
}
