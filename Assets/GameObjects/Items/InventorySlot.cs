#nullable enable
using System;

public class InventorySlot
{
    public Items? Items;
    public SlotType myType = SlotType.Default;

    public void Clear() => Items = null;

    public int CanPut(Items put)
    {
        if (Items is null) return put.count;
        if (put.IsTypeEquals(Items)) return 0;
        if (put.item.stackSize >= put.count + Items.count) return put.count;
        return put.item.stackSize - Items.count;
    }

    public void Put(Items put)
    {
        if (Items is null) Items = put;
        else Items.count += CanPut(put);
    }

    public Items? Take(TakeMode mode = TakeMode.Full)
    {
        switch (mode)
        {
            case TakeMode.Full:
                var items = Items;
                Clear();
                return items;
            
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
    Default, Hotbar, Cursor
}

public enum TakeMode
{
    Full, Half, One
}