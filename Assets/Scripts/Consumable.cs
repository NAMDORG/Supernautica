using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Consumable
{
    public int id;
    public string title;
    public string description;
    public Sprite icon;
    // TODO: What other things are important about consumabels?

    public Consumable(int id, string title, string description)
    {
        this.id = id;
        this.title = title;
        this.description = description;
        this.icon = Resources.Load<Sprite>("Sprites/Consumable Icons/" + title);
    }
}
