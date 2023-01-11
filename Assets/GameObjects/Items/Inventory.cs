using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

public class Inventory : MonoBehaviour
{
    public int defaultSlotCount;
    public int hotBarSlotCount;
    public InventorySlot cursorSlot;
    public InventorySlot[] slots;

    private void Awake()
    {
        slots = new InventorySlot[defaultSlotCount + hotBarSlotCount];

        for (var i = 0; i < hotBarSlotCount; i++)
        {
            slots[i] = new InventorySlot(this, SlotType.Hotbar);
        }
        for (var i = hotBarSlotCount; i < hotBarSlotCount + defaultSlotCount; i++)
        {
            slots[i] = new InventorySlot(this, SlotType.Default);
        }

        cursorSlot = new InventorySlot(this, SlotType.Cursor);
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

    public int Count(ItemInfo item)
    {
        return slots
            .Where(slot => slot.Items is not null && slot.Items.item == item)
            .Sum(slot => slot.Items.count);
    }
}
