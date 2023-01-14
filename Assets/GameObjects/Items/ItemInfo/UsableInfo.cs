using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Usable item", menuName = "Item/Usable item", order = 4)]
public class UsableInfo : ItemInfo
{
    public UseAction useAction;
    public float useActionProp;

    public void OnUse(Entity user)
    {
        switch (useAction)
        {
            case UseAction.Heal:
                user.Heal(useActionProp);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}

public enum UseAction
{
    Heal
}
