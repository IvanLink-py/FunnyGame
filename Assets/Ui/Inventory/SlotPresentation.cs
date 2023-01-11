using System;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SlotPresentation : MonoBehaviour, IPointerClickHandler
{
    public InventorySlot mySlot;

    [SerializeField] private bool selfUpdate;
    
    [SerializeField] private Image itemIcon;
    [SerializeField] private Text itemCount;

    public void UpdateItems()
    {
        ShowItems(mySlot.Items);
    }

    private void ShowItems([CanBeNull] Items items)
    {
        // Debug.Log(items.item is null);
        if (items is null)
        {
            itemIcon.sprite = null;
            itemIcon.color = new Color(0, 0, 0, 0);
            itemCount.text = "";
        }
        else
        {
            itemIcon.sprite = items.item.image;
            itemIcon.color = Color.white;
            itemCount.text = items.count != 1 ? items.count.ToString() : "";
        }
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        UiManager.OnSlotClick(mySlot);
    }

    private void Update()
    {
        if (selfUpdate) UpdateItems();
    }
}
