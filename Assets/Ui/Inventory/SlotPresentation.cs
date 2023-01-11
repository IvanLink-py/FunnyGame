using System;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

public class SlotPresentation : MonoBehaviour
{
    public InventorySlot mySlot;
    
    [SerializeField] private Image itemIcon;
    [SerializeField] private Text itemCount;

    public void FixedUpdate()
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
}
