using System;

[Serializable]
public class Items
{
    public ItemInfo item;
    public int count;
    public MetaItemInfo metaInfo = new ();

    public bool CanStack(Items other)
    {
        return item.Equals(other.item) && metaInfo.CanStack(other.metaInfo);
    }
}