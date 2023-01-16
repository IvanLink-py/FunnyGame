using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlaceableInfo", menuName = "Item/Placeable item", order = 5)]
public class PlaceableInfo : ItemInfo
{
    public Type PlaceableType;
}
