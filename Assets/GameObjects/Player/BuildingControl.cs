using System;
using GameObjects.Player;
using JetBrains.Annotations;
using UnityEngine;

[RequireComponent(typeof(PlayerControl))]
public class BuildingControl : MonoBehaviour
{
    [CanBeNull] private PlaceableInfo _handBuildingInfo;
    [SerializeField] private BuildingPreview buildingPreview;
    // [SerializeField] private bool canPlace = true;

    private void Start()
    {
        GetComponent<Inventory>().ActiveSlotContentChanged += OnActiveSlotContentChanged;
    }

    private void OnActiveSlotContentChanged(InventoryActiveSlotContentChangedEventArgs arg)
    {
        if (arg.NewItems?.item is null || arg.NewItems.item.GetType() != typeof(PlaceableInfo))
        {
            _handBuildingInfo = null;
            buildingPreview.isActive = false;
            return;
        }

        _handBuildingInfo = (PlaceableInfo)arg.NewItems.item;
        buildingPreview.isActive = true;
    }

    private void Update()
    {
        if (_handBuildingInfo is null) return;
        buildingPreview.transform.position = Building.GetNewBlockPosition(
                Camera.main.ScreenToWorldPoint(Input.mousePosition),
                _handBuildingInfo.buildingPlaceMode);
        Placing();
    }


    private void Placing()
    {
        // if (!canPlace && Input.GetAxis("Fire1") < 0.5f) canPlace = true;
        // if (!canPlace || Input.GetAxis("Fire1") < 0.5f) return;
        if (Input.GetAxis("Fire1") < 0.5f) return;

        if (_handBuildingInfo is null || !buildingPreview.CanPlace || !UiManager.CanShoot()) return;

        Building.Place(_handBuildingInfo, Camera.main.ScreenToWorldPoint(Input.mousePosition));
        
        // canPlace = false;
    }
}