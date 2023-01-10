using System;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public int slotCount;
    private InventorySlot[] _slots;

    private void Awake()
    {
        _slots = new InventorySlot[slotCount];

        for (var i = 0; i < slotCount; i++)
        {
            _slots[i] = new InventorySlot();
        }
    }
}
