using System;
using System.Collections.Generic;


#region Inventory Events args

public class InventoryContentChangedEventArgs : EventArgs
{
    public Inventory Inventory;
    public IEnumerable<InventorySlot> Slots;
    public Items ItemsTaken;
    public Items ItemsLost;
}

public class InventoryItemsTakenEventArgs : InventoryContentChangedEventArgs
{
}

public class InventoryItemsLostEventArgs : InventoryContentChangedEventArgs
{
}

public class InventoryActiveSlotChangedEventArgs : EventArgs
{
    public InventorySlot PrevSlot;
    public InventorySlot CurrentSlot;
}

public class InventoryActiveSlotContentChangedEventArgs : EventArgs
{
    public Items NewItems;
}

#endregion

#region InventorySlot Events args

public class InventorySlotContentChangedEventArgs : EventArgs
{
    public Inventory Inventory;
    public InventorySlot Slot;
    public Items NewItems;
}

#endregion

#region UiManager Events args

public class UiStateChangedEventArgs : EventArgs
{
    public UiState PrevState;
    public UiState NewState;
}

#endregion

#region Entity Events args

public abstract class EntityIndicatorsChangedEventArgs : EventArgs
{
    public Entity Entity;
    public float OldValue;
    public float NewValue;
}

public class EntityHpChangedEventArgs : EntityIndicatorsChangedEventArgs
{
}

public class EntityArmorChangedEventArgs : EntityIndicatorsChangedEventArgs
{
}

public class EntityDeathEventArgs : EventArgs
{
    public Entity Entity;
}

#endregion

#region PlayerControl Events args

public class PlayerStaminaChangedEventArgs : EntityIndicatorsChangedEventArgs
{
}

public class PlayerHungerChangedEventArgs : EntityIndicatorsChangedEventArgs
{
}

public class PlayerThirstChangedEventArgs : EntityIndicatorsChangedEventArgs
{
}

#endregion