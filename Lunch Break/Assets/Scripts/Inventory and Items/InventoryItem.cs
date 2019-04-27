using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInventoryItem
{
    string Name { get; }

    Sprite Image { get;  }

    void OnPickup();

    void OnDrop();

    InventorySlot Slot { get; set; }
}

public class InventoryEventArgs : EventArgs
{
    public InventoryEventArgs(InventoryItemBase item)
    {
        Item = item;
    }

    public InventoryEventArgs(InventoryItemBase item, int slotNum)
    {
        Item = item;
        SlotNum = slotNum;
    }

    public InventoryItemBase Item;
    public int SlotNum;
}
