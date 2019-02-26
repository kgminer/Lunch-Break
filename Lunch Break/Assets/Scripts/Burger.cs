using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Burger : InventoryItemBase
{
    public override string Name
    {
        get
        {
            return "Burger";
        }
    }

    public override void OnUse()
    {
        // TODO: Do something with the object
        base.OnUse();
    }
}
