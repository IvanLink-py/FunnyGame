using System;

[Serializable]
public class Items
{
    public ItemInfo item;
    public int count;

    public bool IsTypeEquals(Items other)
    {
        return item.Equals(other.item);
    }
}