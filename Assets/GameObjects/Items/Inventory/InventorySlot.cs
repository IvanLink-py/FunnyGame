using System;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Events;
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
    public Inventory inventory;

    public event UnityAction<InventorySlotContentChangedEventArgs> ContentChanged;

    [SerializeField] [CanBeNull] private Items items;

    public InventorySlot(Inventory myInventory, SlotType myType = SlotType.Default)
    {
        this.myType = myType;
        inventory = myInventory;
    }

    public int CanPut(Items put)
    {
        if (Items is null) return Mathf.Min(put.count, put.item.stackSize);
        if (!put.CanStack(Items)) return 0;
        if (put.item.stackSize >= put.count + Items.count) return put.count;
        return put.item.stackSize - Items.count;
    }

    public int TryPut(Items put)
    {
        if (put.count <= 0) return 0;

        if (Items is null)
        {
            Items = new Items
                { item = put.item, count = Mathf.Min(put.count, put.item.stackSize), metaInfo = put.metaInfo };
            ContentChanged?.Invoke(new InventorySlotContentChangedEventArgs
            {
                Inventory = inventory,
                NewItems = Items,
                Slot = this
            });
            return Mathf.Min(put.count, put.item.stackSize);
        }

        var amount = CanPut(put);
        Items.count += amount;
        if (amount > 0)
            ContentChanged?.Invoke(new InventorySlotContentChangedEventArgs
            {
                Inventory = inventory,
                NewItems = Items,
                Slot = this
            });
        ;
        return amount;
    }

    public void Drop(Vector3 pos)
    {
        if (Items is null) return;
        GameManager.ItemDrop(Items, pos);
        Items = null;
        ContentChanged?.Invoke(new InventorySlotContentChangedEventArgs
        {
            Inventory = inventory, Slot = this, NewItems = Items
        });
    }
    
    public int TransferTo(InventorySlot receiver, TakeMode mode)
    {
        if (Items is null) return 0;

        var transfer = mode switch
        {
            TakeMode.Full => receiver.TryPut(Items),
            TakeMode.Half => receiver.TryPut(new Items
            {
                item = Items.item, count = (int)Mathf.Ceil(Items.count / 2f), metaInfo = Items.metaInfo
            }),
            TakeMode.One => receiver.TryPut(new Items
            {
                item = Items.item, count = 1, metaInfo = Items.metaInfo
            }),
            _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, null)
        };

        if (transfer == 0) return 0;

        Items = new Items { item = Items.item, count = Items.count - transfer, metaInfo = Items.metaInfo };

        ContentChanged?.Invoke(new InventorySlotContentChangedEventArgs
        {
            Inventory = inventory,
            NewItems = Items,
            Slot = this
        });

        return transfer;
    }

    public void Swap(InventorySlot other)
    {
        if (other.Items is null) return;
        var temp = new Items
            { item = other.Items.item, count = other.Items.count, metaInfo = other.Items.metaInfo };
        other.Items = Items;
        Items = temp;
        ContentChanged?.Invoke(new InventorySlotContentChangedEventArgs
        {
            Inventory = inventory, NewItems = Items, Slot = this
        });
        other.ContentChanged?.Invoke(new InventorySlotContentChangedEventArgs
        {
            Inventory = other.inventory, NewItems = other.Items, Slot = other
        });
    }

    public int TryTake(ItemInfo itemInfo, int count)
    {
        if (Items is null || !Items.item.Equals(itemInfo)) return 0;
        var take = Mathf.Min(Items.count, count);
        Items.count -= take;
        ContentChanged?.Invoke(new InventorySlotContentChangedEventArgs
        {
            Inventory = inventory, NewItems = Items, Slot = this
        });
        return take;
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