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
    public InventorySlot selectedHotBarSlot;
    public InventorySlot[] slots;

    private void Awake()
    {
        slots = new InventorySlot[defaultSlotCount + hotBarSlotCount];

        for (var i = 0; i < defaultSlotCount; i++)
        {
            slots[i] = new InventorySlot(this, SlotType.Default);
        }
        for (var i = defaultSlotCount; i < defaultSlotCount + hotBarSlotCount; i++)
        {
            slots[i] = new InventorySlot(this, SlotType.Hotbar);
        }

        cursorSlot = new InventorySlot(this, SlotType.Cursor);
        SetSelectedSlot(slots.First(s => s.myType == SlotType.Hotbar));
    }

    public bool IsSelected(InventorySlot slot) => selectedHotBarSlot == slot;

    public void SetSelectedSlot(InventorySlot slot) => selectedHotBarSlot = slot;

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

    public int TryTake(ItemInfo itemType, int count)
    {
        var getted = 0;
        
        foreach (var slot in slots)
        {
            if (slot.Items is null || slot.Items.item != itemType) continue;
            var take = Mathf.Min(slot.Items.count, count - getted);
            getted += take;
            slot.Items.count -= getted;
            if (getted >= count) return getted;
        }

        return getted;
    }
}
