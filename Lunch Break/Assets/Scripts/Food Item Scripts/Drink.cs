using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drink : InventoryItemBase
{
    public override string Name
    {
        get
        {
            return "Drink";
        }
    }

    public override void OnUse()
    {
        // TODO: Do something with the object
        base.OnUse();
    }
}
