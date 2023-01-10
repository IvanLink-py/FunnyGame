using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemDB", menuName = "Item/Item db", order = 0)]
public class ItemDB : ScriptableObject
{
    public List<ItemInfo> infoList = new ();

    [CanBeNull]
    public ItemInfo GetItemInfo(string id)
    {
        var l = infoList.Where(i => i.id.Equals(id)).ToList();
        switch (l.Count)
        {
            case > 1:
                Debug.Log($"В базе более 1 предмета с id \"{id}\"");
                break;
            case 0:
                return null;
        }

        return l[0];
    }
}
