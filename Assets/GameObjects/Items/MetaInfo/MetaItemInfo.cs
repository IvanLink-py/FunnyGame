using System;

[Serializable]
public class MetaItemInfo
{
    public virtual bool CanStack(MetaItemInfo other) => Equals(other);

    public virtual MetaItemInfo Stack(MetaItemInfo other) => other;
}