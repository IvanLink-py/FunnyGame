using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Inventory : MonoBehaviour
{
    public int slotCount;
    public InventorySlot[] slots;

    private void Awake()
    {
        slots = new InventorySlot[slotCount];

        for (var i = 0; i < slotCount; i++)
        {
            slots[i] = new InventorySlot(this, SlotType.Hotbar);
        }
    }

    public int TryPut(Items items)
    {
        foreach (var slot in slots)
        {
            var amount = slot.TryPut(items);
            if (amount <= 0) continue;
            items.count -= amount;
            if (items.count <= 0) return items.count;
        }
        return items.count;
    }
}
