using System;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SlotPresentation : MonoBehaviour, IPointerClickHandler
{
    public InventorySlot mySlot;
    
    [SerializeField] private Image itemIcon;
    [SerializeField] private Text itemCount;
    
    private void Start()
    {
        mySlot.ContentChanged += MySlotOnContentChanged;
        UpdateItems();
    }

    private void MySlotOnContentChanged(InventorySlotContentChangedEventArgs arg0)
    {
        UpdateItems(arg0.NewItems);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        UiManager.OnSlotClick(mySlot, eventData.button);
    }

    public void UpdateItems()
    {
        ShowItems(mySlot.Items, mySlot.inventory.IsSelected(mySlot));
    }
    
    private void UpdateItems(Items items)
    {
        ShowItems(items, mySlot.inventory.IsSelected(mySlot));
    }

    protected virtual void ShowItems([CanBeNull] Items items, bool isSelected)
    {
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
}