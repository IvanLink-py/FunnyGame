using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "PlaceableInfo", menuName = "Item/Placeable item", order = 5)]
public class PlaceableInfo : ItemInfo
{
    public GameObject buildingPrefab;
    public PlaceMode buildingPlaceMode;

    public float buildDelay;
    
}
