using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class Inventory : MonoBehaviour
{
    public int defaultSlotCount;
    public int hotBarSlotCount;
    public InventorySlot cursorSlot;
    public InventorySlot selectedHotBarSlot;
    public InventorySlot[] slots;

    public event UnityAction<InventoryContentChangedEventArgs> ContentChanged;
    public event UnityAction<InventoryItemsTakenEventArgs> ItemsTaken;
    public event UnityAction<InventoryItemsLostEventArgs> ItemsLost;
    public event UnityAction<InventoryActiveSlotChangedEventArgs> ActiveSlotChanged;
    public event UnityAction<InventoryActiveSlotContentChangedEventArgs> ActiveSlotContentChanged;

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
            slots[i].ContentChanged += OnHotBarSlotContentChanged;
        }

        cursorSlot = new InventorySlot(this, SlotType.Cursor);
        SetSelectedSlot(slots.First(s => s.myType == SlotType.Hotbar));
    }

    private void OnHotBarSlotContentChanged(InventorySlotContentChangedEventArgs args)
    {
        if (args.Slot == selectedHotBarSlot)
            ActiveSlotContentChanged?.Invoke(new InventoryActiveSlotContentChangedEventArgs
            {
                NewItems = args.NewItems
            });
    }

    public bool IsSelected(InventorySlot slot) => selectedHotBarSlot == slot;

    public void SetSelectedSlot(InventorySlot slot)
    {
        if (selectedHotBarSlot == slot) return;
        var args = new InventoryActiveSlotChangedEventArgs { PrevSlot = selectedHotBarSlot };

        selectedHotBarSlot = slot;

        args.CurrentSlot = selectedHotBarSlot;
        ActiveSlotChanged?.Invoke(args);
        ActiveSlotContentChanged?.Invoke(new InventoryActiveSlotContentChangedEventArgs { NewItems = slot.Items });
    }

    public void SetSelectedSlot(int n) =>
        SetSelectedSlot(slots.Where(s => s.myType == SlotType.Hotbar).Skip(n).First());

    public int TryPut(Items items)
    {
        if (items.count == 0) return 0;
        var eventArgs = new InventoryItemsTakenEventArgs
        {
            Inventory = this, Slots = new List<InventorySlot>(),
            ItemsTaken = new Items { item = items.item, metaInfo = items.metaInfo, count = 0 }
        };

        foreach (var slot in slots)
        {
            var amount = slot.TryPut(items);
            if (amount <= 0) continue;
            items.count -= amount;

            ((List<InventorySlot>)eventArgs.Slots).Add(slot);
            eventArgs.ItemsTaken.count += amount;

            if (items.count <= 0) break;
        }

        if (!eventArgs.Slots.Any()) return items.count;

        ContentChanged?.Invoke(eventArgs);
        ItemsTaken?.Invoke(eventArgs);
        return items.count;
    }

    public void PutOrDrop(Items items, Vector3 dropPos)
    {
        TryPut(items);
        if (items.count == 0) return;
        GameManager.ItemDrop(items, dropPos);
    }

    public int Count(ItemInfo item)
    {
        return slots
            .Where(slot => slot.Items is not null && slot.Items.item == item)
            .Sum(slot => slot.Items.count);
    }

    public int TryTake(ItemInfo itemType, int count)
    {
        var got = 0;

        var eventArgs = new InventoryItemsLostEventArgs
        {
            Inventory = this, Slots = new List<InventorySlot>(),
            ItemsLost = new Items { item = itemType, metaInfo = new MetaItemInfo(), count = 0 }
        };

        foreach (var slot in slots)
        {
            var take = slot.TryTake(itemType, count-got);
            got += take;

            ((List<InventorySlot>)eventArgs.Slots).Add(slot);
            eventArgs.ItemsLost.count += take;

            if (got >= count) break;
        }

        if (got == 0) return 0;

        ItemsLost?.Invoke(eventArgs);
        ContentChanged?.Invoke(eventArgs);

        return got;
    }
}