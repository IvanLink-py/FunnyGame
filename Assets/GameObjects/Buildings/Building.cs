using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Collider2D))]
public class Building : Destructible
{
    public static Vector3 GetNewBlockPosition(Vector3 oldPos, PlaceMode placeMode)
    {
        if (!placeMode.gridSnap)
        {
            oldPos.z = 0;
            return oldPos;
        }

        oldPos.x = Mathf.Floor(oldPos.x + 0.5f);
        oldPos.y = Mathf.Floor(oldPos.y + 0.5f);
        oldPos.z = 0;
        
        return oldPos;
    }

    public static void Place(GameObject prefab, PlaceMode placeMode, Vector3 rawPos)
    {
        var instantiate = Instantiate(prefab);
            
        var transformPosition = GetNewBlockPosition(rawPos, placeMode);
        instantiate.transform.position = transformPosition;
    }
    
    public static void Place(PlaceableInfo itemInfo, Vector3 rawPos)
    {
        Place(itemInfo.buildingPrefab, itemInfo.buildingPlaceMode, rawPos);
    }
}