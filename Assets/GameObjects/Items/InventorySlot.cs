using System;
using JetBrains.Annotations;
using UnityEngine;

[Serializable]
public class InventorySlot
{
    [CanBeNull]
    public Items Items
    {
        get => items is null || items.count == 0 ? null : items;
        set
        {
            if (value is null || value.count == 0) items = null;
            else items = value;
        }
    }

    public SlotType myType;
    private Inventory _myInventory;

    [SerializeField] [CanBeNull] private Items items;

    public InventorySlot(Inventory myInventory, SlotType myType = SlotType.Default)
    {
        this.myType = myType;
        _myInventory = myInventory;
    }
    
    public int CanPut(Items put)
    {
        if (Items is null) return Mathf.Min(put.count, put.item.stackSize);
        if (!put.IsTypeEquals(Items)) return 0;
        if (put.item.stackSize >= put.count + Items.count) return put.count;
        return put.item.stackSize - Items.count;
    }

    public int TryPut(Items put)
    {
        if (Items is null)
        {
            Items = new Items { item = put.item, count = Mathf.Min(put.count, put.item.stackSize) };
            return Mathf.Min(put.count, put.item.stackSize);
        }

        var amount = CanPut(put);
        Items.count += amount;
        return amount;
    }


    public int TransferTo(InventorySlot receiver, TakeMode mode)
    {
        if (Items is null) return 0;
        int transfer;
        switch (mode)
        {
            case TakeMode.Full:
                transfer = receiver.TryPut(Items);
                items = new Items { item = Items.item, count = Items.count - transfer };
                break;
            case TakeMode.Half:
                transfer = receiver.TryPut(new Items { item = Items.item, count = (int)Mathf.Ceil(Items.count / 2f) });
                items = new Items { item = Items.item, count = Items.count - transfer };
                break;
            case TakeMode.One:
                transfer = receiver.TryPut(new Items { item = Items.item, count = 1 });
                items = new Items { item = Items.item, count = Items.count - transfer };
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
        }

        return transfer;
    }
}

public enum SlotType
{
    Default,
    Hotbar,
    Cursor
}

public enum TakeMode
{
    Full,
    Half,
    One
}