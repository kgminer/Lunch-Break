using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public const int SLOTS = 3;
    public int maxStackSize = 3;
    public IList<InventorySlot> mSlots = new List<InventorySlot>();

    public event EventHandler<InventoryEventArgs> ItemAdded;
    public event EventHandler<InventoryEventArgs> ItemRemoved;
    public event EventHandler<InventoryEventArgs> ItemUsed;

    public Inventory()
    {
        for (int i = 0; i < SLOTS; i++)
        {
            mSlots.Add(new InventorySlot(i));
        }
    }

    private InventorySlot FindStackableSlot(InventoryItemBase item)
    {
        foreach (InventorySlot slot in mSlots)
        {
            if (slot.IsStackable(item, maxStackSize))
                return slot;
        }
        return null;
    }

    // Getter function to determine if there is a space for the item in the inventory
    public Boolean IsFull(InventoryItemBase item)
    {
        // Cover weirdness case of faulty interaction and say the inventory is full
        if (item == null)
            return true;

        // Inventory slots could be full but there is a stack available for this item, so the inventory can hold this item
        if (FindStackableSlot(item) != null)
            return false;

        // If there are no available stacks and no free slots, the inventory can not hold this item
        else if (FindNextEmptySlot() == null)
            return true;

        // If there is no available stack but there is a free slot, the inventory can hold the item
        else
            return false;
    }
    
    private InventorySlot FindNextEmptySlot()
    {
        foreach (InventorySlot slot in mSlots)
        {
            if (slot.IsEmpty)
                return slot;
        }
        return null;
    }

    public void AddItem(InventoryItemBase item)
    {
        InventorySlot freeSlot = FindStackableSlot(item);
        
        if (freeSlot == null)
        {
            freeSlot = FindNextEmptySlot();
        }

        if (freeSlot != null)
        {
            freeSlot.AddItem(item);

            if (ItemAdded != null)
            {
                ItemAdded(this, new InventoryEventArgs(item));
            }
        }
    }

    internal void UseItem(InventoryItemBase item)
    {
        if (ItemUsed != null)
        {
            ItemUsed(this, new InventoryEventArgs(item));
        }
    }

    public void RemoveItem(InventoryItemBase item)
    {
        foreach (InventorySlot slot in mSlots)
        {
            if (slot.Remove(item))
            {
                if (ItemRemoved != null)
                {
                    ItemRemoved(this, new InventoryEventArgs(item));
                }
                break;
            }
        }
    }

    public void RemoveItem(InventoryItemBase item, int itemToRemove)
    {
        // Remove the item from the inventory stack, returns true when successful 
        if (this.mSlots[itemToRemove].Remove(item))
        {
            if (ItemRemoved != null)
            {
                // Fire off the removal event to the HUD controller
                ItemRemoved(this, new InventoryEventArgs(item, itemToRemove));
            }
        }
    }
    
    public void Empty()
    {
        foreach (InventorySlot slot in mSlots)
        {
            slot.RemoveAllInSlot();
        }
    }
}
