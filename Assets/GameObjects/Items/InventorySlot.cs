// #nullable enable

using System;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Serialization;

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

    public void Clear() => Items = null;

    public int CanPut(Items put)
    {
        if (Items is null) return put.count;
        if (!put.IsTypeEquals(Items)) return 0;
        if (put.item.stackSize >= put.count + Items.count) return put.count;
        return put.item.stackSize - Items.count;
    }

    public int TryPut(Items put)
    {
        if (Items is null)
        {
            Items = new Items { item = put.item, count = put.count};
            return put.count;
        }

        var amount = CanPut(put);
        Items.count += amount;
        return amount;
    }

    [CanBeNull]
    public Items Take(TakeMode mode = TakeMode.Full)
    {
        switch (mode)
        {
            case TakeMode.Full:
                var taken = Items;
                Clear();
                return taken;

            case TakeMode.Half:
                if (Items is null) return null;
                var take = Items.count / 2;
                Items.count -= take;
                return new Items { count = take, item = Items.item };

            case TakeMode.One:
                if (Items is null) return null;
                Items.count -= 1;
                return new Items { count = 1, item = Items.item };

            default:
                throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
        }
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