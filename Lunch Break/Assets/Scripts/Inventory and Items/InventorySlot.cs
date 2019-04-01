using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventorySlot
{
    private Stack<InventoryItemBase> mItemStack = new Stack<InventoryItemBase>();
    private int mId = 0;

    public InventorySlot(int id)
    {
        mId = id;
    }

    public int Id
    {
        get { return mId; }
    }

    public void AddItem(InventoryItemBase item)
    {
        item.Slot = this;
        mItemStack.Push(item);
    }

    public InventoryItemBase FirstItem
    {
        get
        {
            if (IsEmpty)
                return null;

            return mItemStack.Peek();
        }
    }

    public bool IsStackable(InventoryItemBase item, int maxStackSize)
    {
        if (IsEmpty)
            return false;

        InventoryItemBase first = mItemStack.Peek();

        if (first.Name == item.Name && mItemStack.Count < maxStackSize)
            return true;

        return false;
    }

    public bool IsEmpty
    {
        get { return Count == 0; }
    }

    public int Count
    {
        get { return mItemStack.Count; }
    }

    public bool Remove(InventoryItemBase item)
    {
        if (IsEmpty)
            return false;

        InventoryItemBase first = mItemStack.Peek();
        if (first.Name == item.Name)
        {
            mItemStack.Pop();
            if (item.transform.parent != null)
            {
                if (!item.transform.parent.CompareTag("Spawner"))
                    GameObject.Destroy(item.gameObject);
            }
            return true;
        }
        return false;
    }
}
