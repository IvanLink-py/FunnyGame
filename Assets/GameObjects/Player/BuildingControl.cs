using System;
using GameObjects.Player;
using JetBrains.Annotations;
using UnityEngine;

[RequireComponent(typeof(PlayerControl))]
public class BuildingControl : MonoBehaviour
{
    [CanBeNull] private PlaceableInfo _handBuildingInfo;
    [SerializeField] private Transform buildingPreview;
    [SerializeField] private bool canPlace;

    private void Start()
    {
        GetComponent<Inventory>().ActiveSlotContentChanged += OnActiveSlotContentChanged;
    }

    private void OnActiveSlotContentChanged(InventoryActiveSlotContentChangedEventArgs arg)
    {
        if (arg.NewItems?.item is null || arg.NewItems.item.GetType() != typeof(PlaceableInfo))
        {
            _handBuildingInfo = null;
            UiManager.IsAimVisible = true;
            return;
        }

        _handBuildingInfo = (PlaceableInfo)arg.NewItems.item;
        UiManager.IsAimVisible = false;
    }

    private void Update()
    {
        if (_handBuildingInfo is null) return;
        buildingPreview.position = Building.GetNewBlockPosition(
                Camera.main.ScreenToWorldPoint(Input.mousePosition),
                _handBuildingInfo.buildingPlaceMode);
        Placing();
    }


    private void Placing()
    {
        if (!canPlace && Input.GetAxis("Fire1") < 0.5f) canPlace = true;
        if (!canPlace || Input.GetAxis("Fire1") < 0.5f) return;

        if (_handBuildingInfo is null) return;

        Building.Place(_handBuildingInfo, Camera.main.ScreenToWorldPoint(Input.mousePosition));
        
        canPlace = false;
    }
}