using System;

[Serializable]
public class MetaItemInfo
{
    public virtual bool CanStack(MetaItemInfo other) => true;

    public virtual MetaItemInfo Stack(MetaItemInfo other) => other;
}